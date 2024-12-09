using UnityEngine;

using UnityEditor;

using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using GiantSword;
using RichardPieterse;

namespace RichardPieterse.AssetRules
{
    class AssetRuleWindow : EditorWindow
    {
        private enum ViewMode
        {
            AssetSize,
            RuleResults
        }
        
        private List<RuleResult> _ruleResults = new List<RuleResult>();
        private List<RuleResult> _sessionIgnoredRuleResults = new List<RuleResult>();
        private List<WarningRule.WarningResult> _warningResults = new List<WarningRule.WarningResult>();
        private static int _fixCount;
        private Vector2 _ruleScrollPos;
        private Vector2 _warningScrollPos;
        private bool _showFullPaths = true;
        private ViewMode _currentViewMode = ViewMode.RuleResults;

        private GUIStyle _kLeftAligned;

        public static void Get(List<RuleResult> ruleResults, List<WarningRule.WarningResult> warningResults)
        {
            AssetRuleWindow window = EditorWindow.GetWindow<AssetRuleWindow>("Asset Rule Window");
            if (window == null)
            {
                window = ScriptableObject.CreateInstance<AssetRuleWindow>();
                window.position = new Rect(Screen.width / 2, Screen.height / 2, 1900, 1200);
                window.ShowUtility();
            }
            
            foreach (RuleResult result in ruleResults)
            {
                if (window._ruleResults.Contains(result) == false) 
                    window._ruleResults.Add(result);
            }
            
            foreach (WarningRule.WarningResult result in warningResults)
            {
                if (window._warningResults.Contains(result) == false) 
                    window._warningResults.Add(result);
            }

            window._ruleResults.Sort((A, B) => A.rule.order.CompareTo(B.rule.order));
            window._warningResults.Sort((A, B) => B.fileSizeMB.CompareTo(A.fileSizeMB));
        }
        
        private void OnDestroy()
        {
            _ruleResults.Clear();
            _warningResults.Clear();
            _sessionIgnoredRuleResults.Clear();
        }

        private void OnGUI()
        {
            _kLeftAligned = new GUIStyle(GUI.skin.label);
            _kLeftAligned.alignment = TextAnchor.MiddleLeft;
            GUILayout.BeginVertical();
            {
                GUILayout.Space(20);
                DrawViewDropdown();
                GUILayout.Space(20);

                if (_currentViewMode == ViewMode.AssetSize)
                {
                    if (_warningResults.Count == 0)
                    {
                        GUI.color = Color.green;
                        GUILayout.Label("No issues found!");
                        GUI.color = Color.white;
                    }
                    else
                    {
                        DrawWarnings();
                    }
                }

                if (_currentViewMode == ViewMode.RuleResults)
                {
                    if (_ruleResults.Count == 0)
                    {
                        GUI.color = Color.green;
                        GUILayout.Label("No issues found!");
                        GUI.color = Color.white;
                    }
                    else
                    {
                        DrawSelectionUtilityBar();
                        DrawRuleResults();
                        GUILayout.FlexibleSpace();
                        GUILayout.Space(20);
                        DrawFooterButtons();
                    }
                }
            }
            GUILayout.EndVertical();
        }

        private void DrawViewDropdown()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Asset Size Validation"), 
                _currentViewMode == ViewMode.AssetSize, () => _currentViewMode = ViewMode.AssetSize);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Name & Path Validation"),
                _currentViewMode == ViewMode.RuleResults, () => _currentViewMode = ViewMode.RuleResults);
            
            Rect rect = EditorGUILayout.BeginHorizontal(GUILayout.Width(200));
            {
                GUILayout.Space(30);
                GUILayout.Label("View Mode:");
                if (GUILayout.Button(
                        _currentViewMode == ViewMode.AssetSize ? "Asset Size" : "Name & Path Validation"))
                {
                    menu.DropDown(new Rect(rect.x + 250, rect.y - 33, 100, 50));
                }
                EditorGUILayout.EndHorizontal();
            }
        }


        private void DrawFooterButtons()
        {
            GUILayout.BeginHorizontal();
            {
                // if (GUILayout.Button("Reset Ignore List"))
                // {
                //     AssetNamingPrefs.instance._ignoreList.Clear();
                //     Reevaluate();
                // }

                // if (GUILayout.Button("Ignore Selected"))
                // {
                //     IgnoreAllSelected();
                // }

                if (GUILayout.Button("Fix Selected"))
                {
                    bool ok = EditorUtility.DisplayDialog("Confirm",
                        "You will only be able to undo this using version control",
                        "Fix", "Cancel");

                    if (ok)
                    {
                        FixAllSelected();
                    }
                }

                if (GUILayout.Button("Refresh"))
                {
                    Reevaluate();
                }

                if (GUILayout.Button("Close"))
                {
                    Close();
                }
            }
            GUILayout.EndHorizontal();
        }
        
        private void DrawRuleResults()
        { 
            // Start scroll view
            _ruleScrollPos = GUILayout.BeginScrollView(_ruleScrollPos);

            // Start row
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical();
                {
                    int removeIndex = -1;
                    for (var i = 0; i < _ruleResults.Count; i++)
                    {
                        if (i % 2 == 0)
                        {
                            GUILayout.BeginHorizontal("Box");
                        }
                        else
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(6);
                        }
                        {
                            _ruleResults[i].selected = GUILayout.Toggle(_ruleResults[i].selected, $" ",GUILayout.Width(10));
                            GUILayout.Space(5);
                            GUILayout.Label($"{i}.", GUILayout.Width(40));
                            
                            GUILayout.Space(5);
         
                            string path = _ruleResults[i].oldPath;
                            if (_showFullPaths == false)
                            {
                                path = _ruleResults[i].oldPath.Replace(_ruleResults[i].commonPath, "");
                            }
                            
                            // Old path and New path labels
                            GUILayout.BeginVertical();
                            {
                                if (DrawLinkButton(path))
                                {
                                    EditorGUIUtility.PingObject(
                                        AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(_ruleResults[i].oldPath));
                                }

                                Color color = GUI.color;
                                GUI.color = Color.green;
                                string outcome = _ruleResults[i].customMessage;
                                if (string.IsNullOrWhiteSpace(outcome))
                                {
                                    if (_showFullPaths)
                                    {
                                        outcome = _ruleResults[i].newPath;
                                    }
                                    else
                                    {
                                        outcome = _ruleResults[i].newPath.Replace(_ruleResults[i].commonPath, "");
                                    }
                                }

                                GUILayout.Label($"{outcome}", _kLeftAligned, GUILayout.Width(600));
                                GUI.color = color;
                            }
                            GUILayout.EndVertical();
                            
                            // Rule label and Fix Action label
                            GUILayout.BeginVertical();
                            {
                                // Rule label
                                Color color = GUI.color;
                                GUI.color = new Color(129 / 255f, 180 / 255f, 255 / 255f, 1);
                                if (GUILayout.Button($"{_ruleResults[i].rule.name}",
                                        _kLeftAligned,GUILayout.Width(250)))
                                {
                                    RuntimeEditorHelper.Select(_ruleResults[i].rule);
                                }
                                GUI.color = color;
                                
                                // Fix Actions label(s)
                                DrawFixActionLabels(_ruleResults[i]);
                            }
                            GUILayout.EndVertical();
                            
                            GUILayout.Space(40);

                            if (GUILayout.Button("Fix",   GUILayout.Width(40)))
                            {
                                if (Fix(_ruleResults[i]))
                                {
                                    removeIndex = i;
                                }
                                else
                                {
                                    EditorUtility.DisplayDialog(
                                        "Fix Failed",
                                        $"Is there already an asset at '{_ruleResults[i].newPath}'?",
                                        "Ok");
                                }
                            }
                            GUILayout.FlexibleSpace();

                            // if (GUILayout.Button("Ignore",  GUILayout.Width(60)))
                            // {
                            //     Ignore(_ruleResults[i]);
                            //     removeIndex = i;
                            // }
                        }
                        
                        if (i % 2 == 0)
                        {
                            GUILayout.EndHorizontal();
                        }
                        else
                        {
                            GUILayout.EndHorizontal();
                        }
                    }

                    // This handles removing from list after individual fixes and ignores.
                    if (removeIndex != -1)
                    {
                        _ruleResults.RemoveAt(removeIndex);
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
            
            GUILayout.EndScrollView();
        }

        private static void DrawFixActionLabels(RuleResult ruleResult, float width = 100)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(width));
            {
                Color color = GUI.color;
                GUI.color = Color.white;
                GUILayout.Label("Fix Action:");
                GUILayout.Space(5);

                StringBuilder stringBuilder = new StringBuilder();
                if (ruleResult.fixActionTags.HasFlag(RuleResult.FixActionTags.CreateFolder))
                {
                    stringBuilder.Append("[Create Folder] ");
                }
                if (ruleResult.fixActionTags.HasFlag(RuleResult.FixActionTags.MoveAsset))
                {
                    stringBuilder.Append("[Move to folder] ");
                }
                if (ruleResult.fixActionTags.HasFlag(RuleResult.FixActionTags.RenameAsset))
                {
                    stringBuilder.Append("[Rename Asset]");
                }
                
                GUILayout.Label(stringBuilder.ToString(), GUILayout.Width(400));
                GUI.color = color;
            }
            GUILayout.EndHorizontal();
        }

        private void DrawSelectionUtilityBar()
        {
            GUILayout.BeginHorizontal(GUILayout.Width(100));
            {
                _showFullPaths = GUILayout.Toggle(_showFullPaths, "full paths ");

                GUILayout.FlexibleSpace();
                
                GUILayout.Label($"{_ruleResults.Count} results");

                GUI.color = Color.green;
                GUILayout.Label($"{_fixCount} files fixed");
                GUI.color = Color.white;
            }
            GUILayout.EndHorizontal();
        }

        private void DrawWarnings()
        {
            GUILayout.BeginHorizontal("Box");
            {
                GUILayout.BeginVertical(GUILayout.MaxHeight(250));
                {
                    EditorGUILayout.HelpBox("Large Assets - Can they be in a smaller format?", MessageType.Error);

                    _warningScrollPos = GUILayout.BeginScrollView(_warningScrollPos);
                    {
                        foreach (WarningRule.WarningResult warningResult in _warningResults)
                        {
                            if (DrawLinkButton(warningResult.message, 800))
                            {
                                EditorGUIUtility.PingObject(warningResult.asset);
                            }
                        }
                    }
                    GUILayout.EndScrollView();

                    // if (GUILayout.Button("Clear Warnings"))
                    // {
                    //     _warningResults.Clear();
                    // }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        private bool DrawLinkButton(string path, int width = 550)
        {
            Color color = GUI.color;
            GUI.color = new Color(129 / 255f, 180 / 255f, 255 / 255f, 1);

            bool result = GUILayout.Button($"{path}", _kLeftAligned, GUILayout.Width(width));

            GUI.color = color;
            return result;
        }

        private void Ignore(RuleResult ruleResult)
        {
            AssetNamingPrefs.instance._ignoreList.Add(ruleResult.oldPath);
            _sessionIgnoredRuleResults.AddIfNotContained(ruleResult);
        }

        private bool Fix(RuleResult ruleResult)
        {
            bool success = ruleResult.ApplyFix();
            if (success)
            {
                _fixCount++;
            }
            else
            {
                Debug.LogWarning($"ASSET VALIDATION: Could not apply fix. " +
                                 $"Is there already an asset with the same name as '{ruleResult.newPath}'?");
            }
            
            if (ruleResult.triggerReevaluation)
            {
                Reevaluate();
            }

            return success;
        }
        
        private void IgnoreAllSelected()
        {
            List<RuleResult> selectedResults = _ruleResults.Where(result => result.selected).ToList();
            foreach (var selectedResult in selectedResults)
            {
                Ignore(selectedResult);
                _ruleResults.Remove(selectedResult);
            }
        }
        
        private void FixAllSelected()
        {
            List<RuleResult> selectedResults = _ruleResults.Where(result => result.selected).ToList();
            
            
            for (int i = 0; i < selectedResults.Count; i++)
            {
                var ruleResult = selectedResults[i];
                EditorUtility.DisplayProgressBar("Renaming",
                    ruleResult.oldPath + " -> " + ruleResult.newPath,
                    (selectedResults.Count - i) / (float)selectedResults.Count);

                if (Fix(ruleResult))
                {
                    _ruleResults.Remove(ruleResult);
                }
            }
            EditorUtility.ClearProgressBar();
        }

        void Reevaluate()
        {
            AssetDatabase.Refresh();
            
            List<RuleResult> newList = new List<RuleResult>();
            for (int i = 0; i < _ruleResults.Count; i++)
            {
                AssetRuleUtility.CheckRule(AssetDatabase.GetAssetPath(_ruleResults[i].asset), newList, null);
            }
            _ruleResults = newList;

            // Handle case where a RuleResult was ignored, the ignore list was reset, and we are refreshing
            for (int i = _sessionIgnoredRuleResults.Count - 1; i >= 0; i--)
            {
                if (!AssetNamingPrefs.instance._ignoreList.Contains(_sessionIgnoredRuleResults[i].oldPath))
                {
                    _ruleResults.Add(_sessionIgnoredRuleResults[i]);
                    _sessionIgnoredRuleResults.RemoveAt(i);
                }
            }
        }
    }
}