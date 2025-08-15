using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using Debug = UnityEngine.Debug;

public class TakeScreenshot : MonoBehaviour
{
    [SerializeField] private bool _enableHotKey = false;

    public void TriggerDefault()
    {
        Trigger(Screen.height);
    }
    public void TriggerHD()
    {
        Trigger(1080);        
    }
    public void Trigger4K()
    {
        Trigger(1080*2);        
    }
    public void Trigger(int screenHeight)
    {
        Camera camera = GetComponent<Camera>();
        if (camera == null)
        {
            Debug.LogError("PixelPerfectScreenshot: No camera found on this GameObject.");
            return;
        }

        int screenWidth = Mathf.CeilToInt(screenHeight * camera.aspect);

        // Create RenderTexture
        RenderTexture rt = new RenderTexture(screenWidth, screenHeight, 24);
        camera.targetTexture = rt;

        // Render to the texture
        Texture2D screenshot = new Texture2D(screenWidth, screenHeight, TextureFormat.RGB24, false);
        camera.Render();
        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, screenWidth, screenHeight), 0, 0);
        screenshot.Apply();

        // Save to Desktop
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string filePath = Path.Combine(desktopPath, $"Screenshot_{timestamp}.png");
        File.WriteAllBytes(filePath, screenshot.EncodeToPNG());
        Debug.Log("Screenshot saved to: " + filePath);
#if UNITY_EDITOR

        // Open Finder/Explorer to the file location
        EditorUtility.RevealInFinder(filePath);
#endif

        // Cleanup
        camera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);
        DestroyImmediate(screenshot);
    }

    private void Update()
    {
        if (_enableHotKey && Input.GetKeyDown(KeyCode.P))
        {
            TriggerDefault();
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TakeScreenshot))]
public class PixelPerfectScreenshotEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TakeScreenshot script = (TakeScreenshot)target;
        if (GUILayout.Button("Take Screenshot of Game View"))
        {
            script.TriggerDefault();
        }
        
        if (GUILayout.Button("Take Screenshot HD"))
        {
            script.TriggerHD();
        }
        
        if (GUILayout.Button("Take Screenshot 4K"))
        {
            script.Trigger4K();
        }
    }
    
    //  camera context menu items
    
    
    
}
#endif
