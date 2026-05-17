using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Asset Auditor
/// ─────────────────────────────────────────────────────────────────────────────
/// Two entry points:
///
///   • Right-click assets/folders in Project window → "Audit For Issues"
///     Reimports each asset (ForceUpdate), detects missing scripts in prefabs
///     and ScriptableObjects, calls OnValidate on all MonoBehaviours.
///
///   • GameObject menu → "Audit Scene For Issues"
///     Scans the active scene for missing scripts and calls OnValidate on
///     every MonoBehaviour in the scene.
///
/// Results appear in the same floating window for both audit types.
/// Place this file in any Editor folder, e.g. Assets/Editor/AssetAuditor.cs
/// ─────────────────────────────────────────────────────────────────────────────
/// </summary>
public class AssetAuditorWindow : EditorWindow
{
    // =========================================================================
    // Types
    // =========================================================================

    private enum ResultSource { Asset, Scene }

    private struct AuditResult
    {
        public ResultSource   Source;
        public string         Label;   // asset path OR "Scene: <name>"
        public List<LogEntry> Logs;
    }

    private struct LogEntry
    {
        public string  Message;
        public string  StackTrace;
        public LogType Type;
        public bool    IsMissingScript;
        public bool    IsOnValidateMutation;  // dirty after OnValidate — info only, no counter
        public bool    IsOnValidateException;
    }

    // =========================================================================
    // Constants
    // =========================================================================

    private static readonly HashSet<string> AuditedExtensions =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".fbx", ".obj", ".dae", ".3ds", ".blend", ".ma", ".mb", ".max",
            ".prefab",
            ".asset",
            ".mat",
            ".shader", ".shadergraph", ".shadersubgraph", ".hlsl", ".cginc",
        };

    private static readonly string[] NoisePatterns =
    {
        "generated inconsistent result for asset",
    };

    // Colours — defined once, reused throughout GUI
    private static readonly Color ColError    = new Color(1f,    0.35f, 0.35f);
    private static readonly Color ColWarning  = new Color(1f,    0.80f, 0.20f);
    private static readonly Color ColMissing  = new Color(1f,    0.50f, 0.90f);
    private static readonly Color ColInfo     = new Color(0.75f, 0.75f, 0.75f);
    private static readonly Color ColMutation = new Color(0.55f, 0.80f, 1.00f);
    private static readonly Color ColClean    = new Color(0.35f, 0.85f, 0.50f);

    // =========================================================================
    // State
    // =========================================================================

    // Asset audit queue
    private List<string>      _queue        = new List<string>();
    private int               _totalAssets;
    private int               _currentIndex;

    // Shared run state
    private bool              _running;
    private bool              _cancelRequested;
    private bool              _finished;
    private bool              _sceneAuditDone;
    private string            _statusLabel  = "";

    // Log capture — populated by Application.logMessageReceived
    private string            _captureContext = "";
    private List<LogEntry>    _pendingLogs    = new List<LogEntry>();

    // Results
    private List<AuditResult> _results = new List<AuditResult>();

    // Summary counters
    private int _errorCount;
    private int _warningCount;
    private int _missingScriptCount;
    private int _cleanCount;

    // GUI
    private Vector2 _scrollPos;

    // =========================================================================
    // Menu Items
    // =========================================================================

    // ── Project window right-click ────────────────────────────────────────────

    [MenuItem("Assets/Audit For Issues", false, 9999)]
    private static void LaunchAssetAudit()
    {
        var paths = new List<string>();

        foreach (var obj in Selection.objects)
        {
            var p = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(p)) continue;

            if (AssetDatabase.IsValidFolder(p))
            {
                foreach (var guid in AssetDatabase.FindAssets("", new[] { p }))
                {
                    var fp = AssetDatabase.GUIDToAssetPath(guid);
                    if (!AssetDatabase.IsValidFolder(fp) &&
                        AuditedExtensions.Contains(System.IO.Path.GetExtension(fp)) &&
                        !paths.Contains(fp))
                        paths.Add(fp);
                }
            }
            else if (AuditedExtensions.Contains(System.IO.Path.GetExtension(p)) &&
                     !paths.Contains(p))
            {
                paths.Add(p);
            }
        }

        if (paths.Count == 0)
        {
            EditorUtility.DisplayDialog(
                "Asset Auditor",
                "No supported assets selected.\n\nSupported types:\n" +
                "Models, Prefabs, ScriptableObjects (.asset), Materials, Shaders.",
                "OK");
            return;
        }

        var window = GetWindow<AssetAuditorWindow>(true, "Asset Auditor", true);
        window.minSize = new Vector2(560, 420);
        window.StartAssetAudit(paths);
    }

    [MenuItem("Assets/Audit For Issues", true)]
    private static bool LaunchAssetAuditValidate() =>
        Selection.objects != null && Selection.objects.Length > 0;

    // ── GameObject menu ───────────────────────────────────────────────────────

    [MenuItem("GameObject/Audit Scene For Issues", false, 49)]
    private static void LaunchSceneAudit()
    {
        var scene = SceneManager.GetActiveScene();
        if (!scene.IsValid() || !scene.isLoaded)
        {
            EditorUtility.DisplayDialog("Asset Auditor",
                "No active scene is loaded.", "OK");
            return;
        }

        var window = GetWindow<AssetAuditorWindow>(true, "Asset Auditor", true);
        window.minSize = new Vector2(560, 420);
        window.StartSceneAudit(scene);
    }

    // =========================================================================
    // Window Lifecycle
    // =========================================================================

    private void OnEnable()  => Application.logMessageReceived += OnLogMessage;

    private void OnDisable()
    {
        Application.logMessageReceived -= OnLogMessage;
        if (_running) FinalizeAudit(cancelled: true);
    }

    // =========================================================================
    // Asset Audit
    // =========================================================================

    public void StartAssetAudit(List<string> paths)
    {
        ResetState();
        _queue       = paths;
        _totalAssets = paths.Count;
        EditorApplication.update += AssetAuditStep;
    }

    private void AssetAuditStep()
    {
        if (!_running) return;

        if (_cancelRequested || _currentIndex >= _queue.Count)
        {
            FinalizeAudit(_cancelRequested);
            return;
        }

        var path = _queue[_currentIndex];
        _statusLabel = $"Asset {_currentIndex + 1}/{_totalAssets}  —  {System.IO.Path.GetFileName(path)}";

        var logs = new List<LogEntry>();
        string ext = System.IO.Path.GetExtension(path).ToLowerInvariant();

        // ── 1. Force reimport ─────────────────────────────────────────────────
        BeginCapture(path);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        logs.AddRange(EndCapture());

        // ── 2. Prefab: missing scripts + OnValidate ───────────────────────────
        if (ext == ".prefab")
        {
            var prefabRoot = PrefabUtility.LoadPrefabContents(path);
            if (prefabRoot != null)
            {
                try
                {
                    AuditGameObjectHierarchy(prefabRoot.transform, "", logs);
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(prefabRoot);
                }
            }
        }
        // ── 3. ScriptableObject: missing script + OnValidate ──────────────────
        else if (ext == ".asset")
        {
            var loaded = AssetDatabase.LoadMainAssetAtPath(path);
            if (loaded is ScriptableObject so)
            {
                var mono = MonoScript.FromScriptableObject(so);
                if (mono == null || mono.GetClass() == null)
                    logs.Add(MakeMissingScriptLog("(ScriptableObject root)", 0));
                else
                    RunOnValidate(so, logs);
            }
        }

        CommitResult(ResultSource.Asset, path, logs);

        _currentIndex++;
        Repaint();
    }

    // =========================================================================
    // Scene Audit
    // =========================================================================

    public void StartSceneAudit(Scene scene)
    {
        ResetState();
        _totalAssets = 1;
        EditorApplication.update += () => SceneAuditStep(scene);
    }

    private void SceneAuditStep(Scene scene)
    {
        if (!_running || _sceneAuditDone) return;

        if (_cancelRequested)
        {
            FinalizeAudit(true);
            return;
        }

        _sceneAuditDone = true;
        _statusLabel    = $"Scene: {scene.name}";

        var logs = new List<LogEntry>();

        foreach (var root in scene.GetRootGameObjects())
        {
            if (_cancelRequested) break;
            AuditGameObjectHierarchy(root.transform, "", logs);
        }

        _currentIndex = 1;
        CommitResult(ResultSource.Scene, $"Scene: {scene.name}", logs);
        FinalizeAudit(false);
    }

    // =========================================================================
    // Shared Hierarchy Audit
    // =========================================================================

    /// <summary>
    /// Recursively walks a transform hierarchy.
    /// Checks each node for missing script slots and calls OnValidate on all
    /// valid MonoBehaviours. Used by both the asset and scene audit paths.
    /// </summary>
    private void AuditGameObjectHierarchy(Transform t, string parentPath, List<LogEntry> logs)
    {
        string path = string.IsNullOrEmpty(parentPath) ? t.name : $"{parentPath}/{t.name}";

        var components = t.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] == null)
            {
                logs.Add(MakeMissingScriptLog(path, i));
            }
            else if (components[i] is MonoBehaviour mb)
            {
                RunOnValidate(mb, logs);
            }
        }

        foreach (Transform child in t)
            AuditGameObjectHierarchy(child, path, logs);
    }

    // =========================================================================
    // OnValidate via Reflection
    // =========================================================================

    /// <summary>
    /// Invokes OnValidate on a UnityEngine.Object via reflection.
    /// - Wraps in Undo.RecordObject before calling.
    /// - Logs any exception as [E] (counts as error).
    /// - Logs mutation (IsDirty after call) as [~] info (does NOT count as error).
    /// - All Debug.Log/Warning/Error calls from within OnValidate are captured
    ///   via Application.logMessageReceived and logged normally.
    /// </summary>
    private void RunOnValidate(UnityEngine.Object target, List<LogEntry> logs)
    {
        if (target == null) return;

        var method = target.GetType().GetMethod(
            "OnValidate",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (method == null) return;

        Undo.RecordObject(target, "AssetAuditor OnValidate");

        BeginCapture($"OnValidate:{target.GetType().Name}");
        try
        {
            method.Invoke(target, null);
        }
        catch (TargetInvocationException tie)
        {
            var inner = tie.InnerException ?? tie;
            logs.Add(new LogEntry
            {
                Message               = $"OnValidate exception on {target.GetType().Name}: {inner.Message}",
                StackTrace            = inner.StackTrace ?? "",
                Type                  = LogType.Error,
                IsOnValidateException = true
            });
        }
        catch (Exception ex)
        {
            logs.Add(new LogEntry
            {
                Message               = $"OnValidate exception on {target.GetType().Name}: {ex.Message}",
                StackTrace            = ex.StackTrace ?? "",
                Type                  = LogType.Error,
                IsOnValidateException = true
            });
        }

        // Capture any Debug.Log calls made from within OnValidate
        logs.AddRange(EndCapture());

        // Mutation check — logged as info, does NOT increment any counter
        if (EditorUtility.IsDirty(target))
        {
            logs.Add(new LogEntry
            {
                Message               = $"OnValidate mutated {target.GetType().Name} (object marked dirty)",
                StackTrace            = "",
                Type                  = LogType.Log,
                IsOnValidateMutation  = true
            });
        }
    }

    // =========================================================================
    // Log Capture Helpers
    // =========================================================================

    private void BeginCapture(string context)
    {
        _captureContext = context;
        _pendingLogs.Clear();
    }

    private List<LogEntry> EndCapture()
    {
        _captureContext = "";
        var result = new List<LogEntry>(_pendingLogs);
        _pendingLogs.Clear();
        return result;
    }

    private void OnLogMessage(string message, string stackTrace, LogType type)
    {
        if (string.IsNullOrEmpty(_captureContext)) return;
        if (NoisePatterns.Any(p => message.Contains(p))) return;

        _pendingLogs.Add(new LogEntry
        {
            Message    = message,
            StackTrace = stackTrace,
            Type       = type
        });
    }

    // =========================================================================
    // Result Commit
    // =========================================================================

    private void CommitResult(ResultSource source, string label, List<LogEntry> logs)
    {
        _results.Add(new AuditResult { Source = source, Label = label, Logs = logs });

        // Count mutations as neither error nor warning nor clean — they're purely informational
        bool hasError   = logs.Any(l => IsCountableError(l));
        bool hasWarning = logs.Any(l => l.Type == LogType.Warning && !l.IsOnValidateMutation);

        if (hasError)        _errorCount++;
        else if (hasWarning) _warningCount++;
        else                 _cleanCount++;

        _missingScriptCount += logs.Count(l => l.IsMissingScript);

        // Echo actionable items to the Unity Console with context object for clickability
        var contextObj = source == ResultSource.Asset
            ? AssetDatabase.LoadMainAssetAtPath(label) : null;
        string fileName = source == ResultSource.Asset
            ? System.IO.Path.GetFileName(label) : label;

        foreach (var log in logs)
        {
            // Mutations and plain info logs are not echoed to the console
            if (log.IsOnValidateMutation || log.Type == LogType.Log) continue;

            string prefix = log.IsMissingScript      ? "◈ MISSING SCRIPT"
                          : log.IsOnValidateException ? "✖ VALIDATE ERROR"
                          : log.Type == LogType.Warning ? "⚠ AUDIT WARNING"
                          :                              "✖ AUDIT ERROR";

            string msg = $"{prefix} [{fileName}]: {log.Message}";

            if (log.Type == LogType.Warning)
                Debug.LogWarning(msg, contextObj);
            else
                Debug.LogError(msg, contextObj);
        }
    }

    // =========================================================================
    // Finalize / Reset
    // =========================================================================

    private void FinalizeAudit(bool cancelled)
    {
        EditorApplication.update -= AssetAuditStep;
        _running        = false;
        _finished       = true;
        _captureContext = "";

        string status = cancelled ? "CANCELLED" : "COMPLETE";
        Debug.Log(
            $"[Asset Auditor] {status} — " +
            $"Audited: {_currentIndex}/{_totalAssets}  |  " +
            $"Errors: {_errorCount}  |  " +
            $"Warnings: {_warningCount}  |  " +
            $"Missing Scripts: {_missingScriptCount}  |  " +
            $"Clean: {_cleanCount}");

        Repaint();
    }

    private void ResetState()
    {
        _queue              = new List<string>();
        _results            = new List<AuditResult>();
        _totalAssets        = 0;
        _currentIndex       = 0;
        _running            = true;
        _cancelRequested    = false;
        _finished           = false;
        _sceneAuditDone     = false;
        _errorCount         = 0;
        _warningCount       = 0;
        _missingScriptCount = 0;
        _cleanCount         = 0;
        _scrollPos          = Vector2.zero;
        _statusLabel        = "";
        _pendingLogs.Clear();
    }

    // =========================================================================
    // Convenience
    // =========================================================================

    private static LogEntry MakeMissingScriptLog(string goPath, int slotIndex) =>
        new LogEntry
        {
            Message         = $"Missing Script (slot {slotIndex}) on: \"{goPath}\"",
            StackTrace      = "",
            Type            = LogType.Error,
            IsMissingScript = true
        };

    /// <summary>
    /// Returns true for log entries that should increment the error counter.
    /// Mutations are excluded even though they carry LogType.Log.
    /// </summary>
    private static bool IsCountableError(LogEntry l) =>
        (l.Type == LogType.Error || l.Type == LogType.Exception || l.Type == LogType.Assert)
        && !l.IsOnValidateMutation;

    // =========================================================================
    // GUI
    // =========================================================================

    private void OnGUI()
    {
        // ── Header row ────────────────────────────────────────────────────────
        EditorGUILayout.Space(8);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(6);
        GUILayout.Label("Asset Auditor", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();

        if (!_running)
        {
            GUI.backgroundColor = new Color(0.4f, 0.4f, 0.4f);
            if (GUILayout.Button("Clear", GUILayout.Height(20), GUILayout.Width(52)))
            {
                _results.Clear();
                _errorCount = _warningCount = _missingScriptCount = _cleanCount = 0;
                _finished   = false;
                Repaint();
            }
            GUI.backgroundColor = Color.white;
        }

        GUILayout.Space(6);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(4);

        // ── Progress bar ──────────────────────────────────────────────────────
        float progress = _totalAssets > 0 ? (float)_currentIndex / _totalAssets : 0f;
        Rect barRect = GUILayoutUtility.GetRect(18, 22, GUILayout.ExpandWidth(true));
        barRect.x     += 6;
        barRect.width -= 12;

        string barLabel = _finished
            ? (_cancelRequested
                ? $"Cancelled — {_currentIndex}/{_totalAssets}"
                : $"Complete — {_currentIndex} item(s)")
            : _running ? _statusLabel : "Ready";

        EditorGUI.ProgressBar(barRect, progress, barLabel);
        EditorGUILayout.Space(6);

        // ── Summary chips ─────────────────────────────────────────────────────
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(6);
        DrawChip($"✖  Errors: {_errorCount}",                  ColError);
        GUILayout.Space(4);
        DrawChip($"⚠  Warnings: {_warningCount}",              ColWarning);
        GUILayout.Space(4);
        DrawChip($"◈  Missing Scripts: {_missingScriptCount}", ColMissing);
        GUILayout.Space(4);
        DrawChip($"✔  Clean: {_cleanCount}",                   ColClean);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(6);

        // ── Results scroll list ───────────────────────────────────────────────
        _scrollPos = EditorGUILayout.BeginScrollView(
            _scrollPos,
            new GUIStyle(GUI.skin.scrollView) { padding = new RectOffset(4, 4, 4, 4) },
            GUILayout.ExpandHeight(true));

        if (_results.Count == 0)
        {
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(
                _running ? "Auditing…" : "No results yet.",
                new GUIStyle(EditorStyles.centeredGreyMiniLabel) { fontSize = 11 });
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
        }
        else
        {
            var sorted = _results
                .OrderByDescending(r => r.Logs.Any(IsCountableError) ? 2
                    : r.Logs.Any(l => l.Type == LogType.Warning) ? 1 : 0)
                .ToList();

            foreach (var result in sorted)
                DrawResultRow(result);
        }

        EditorGUILayout.EndScrollView();

        // ── Footer buttons ────────────────────────────────────────────────────
        EditorGUILayout.Space(6);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(6);

        if (_running)
        {
            GUI.backgroundColor = new Color(0.9f, 0.3f, 0.3f);
            if (GUILayout.Button("  Cancel  ", GUILayout.Height(28), GUILayout.Width(90)))
                _cancelRequested = true;
            GUI.backgroundColor = Color.white;
        }
        else if (_finished)
        {
            GUI.backgroundColor = new Color(0.3f, 0.55f, 0.9f);
            if (GUILayout.Button("  Close  ", GUILayout.Height(28), GUILayout.Width(90)))
                Close();
            GUI.backgroundColor = Color.white;
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(6);
    }

    private void DrawResultRow(AuditResult result)
    {
        bool hasError   = result.Logs.Any(IsCountableError);
        bool hasWarning = result.Logs.Any(l => l.Type == LogType.Warning && !l.IsOnValidateMutation);
        bool hasMissing = result.Logs.Any(l => l.IsMissingScript);

        Color rowColor = hasError   ? ColError   :
                         hasWarning ? ColWarning :
                         hasMissing ? ColMissing :
                                      ColClean;

        string icon  = hasError ? "✖" : hasWarning ? "⚠" : hasMissing ? "◈" : "✔";
        string badge = result.Source == ResultSource.Scene ? " [SCENE]" : "";

        EditorGUILayout.BeginVertical(GUI.skin.box);

        // Header — click to ping asset or focus scene object
        var headerStyle = new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Bold,
            wordWrap  = false,
            normal    = { textColor = rowColor },
            hover     = { textColor = Color.white }
        };

        if (GUILayout.Button($"{icon}{badge}  {result.Label}", headerStyle,
                             GUILayout.ExpandWidth(true)))
        {
            if (result.Source == ResultSource.Asset)
            {
                var asset = AssetDatabase.LoadMainAssetAtPath(result.Label);
                if (asset != null)
                {
                    EditorGUIUtility.PingObject(asset);
                    Selection.activeObject = asset;
                }
            }
        }

        // Log entries
        if (result.Logs.Count > 0)
        {
            foreach (var log in result.Logs)
            {
                Color  logColor;
                string tag;

                if (log.IsMissingScript)
                {
                    logColor = ColMissing;   tag = "[◈]";
                }
                else if (log.IsOnValidateMutation)
                {
                    logColor = ColMutation;  tag = "[~]";
                }
                else if (log.IsOnValidateException)
                {
                    logColor = ColError;     tag = "[!]";
                }
                else
                {
                    logColor = log.Type == LogType.Warning ? ColWarning
                             : log.Type == LogType.Log     ? ColInfo
                             :                               ColError;
                    tag      = log.Type == LogType.Warning ? "[W]"
                             : log.Type == LogType.Log     ? "[I]"
                             :                               "[E]";
                }

                GUILayout.Label($"{tag} {log.Message}",
                    new GUIStyle(EditorStyles.miniLabel)
                    {
                        wordWrap = true,
                        normal   = { textColor = logColor },
                        padding  = new RectOffset(16, 4, 1, 1)
                    });
            }
        }
        else
        {
            GUILayout.Label("No issues detected.",
                new GUIStyle(EditorStyles.miniLabel)
                {
                    normal  = { textColor = ColClean },
                    padding = new RectOffset(16, 4, 1, 1)
                });
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(2);
    }

    private void DrawChip(string text, Color color)
    {
        GUILayout.Label(text, new GUIStyle(EditorStyles.miniLabel)
        {
            fontStyle = FontStyle.Bold,
            normal    = { textColor = color },
            padding   = new RectOffset(8, 8, 3, 3),
        });
    }
}