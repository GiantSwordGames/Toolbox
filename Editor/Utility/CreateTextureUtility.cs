using UnityEngine;
using UnityEditor;


namespace JamKit
{
    public  class CreateTextureUtility
    {
        public static int textureWidth = 1024;
        public static int textureHeight = 1024;
        

        [MenuItem( MenuPaths.QUICK_CREATE + "Create Texture", false, MenuPaths.QUICK_CREATE_PRIORITY)]
        public static void CreateWhiteTexture(MenuCommand menuCommand)
        {
            // Get the path of the selected folder in the Project window
            string folderPath = GetSelectedFolderPath();

            // Create the texture
            Texture2D whiteTexture = new Texture2D(textureWidth, textureHeight);

            // Set all pixels to white
            Color32[] pixels = new Color32[textureWidth * textureHeight];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new Color32(255, 255, 255, 255);
            }

            whiteTexture.SetPixels32(pixels);
            whiteTexture.Apply();

            // Encode texture to PNG
            byte[] bytes = whiteTexture.EncodeToPNG();

            // Create a unique asset path in the selected folder
            string path = AssetDatabase.GenerateUniqueAssetPath(folderPath + "/NewWhiteTexture.png");

            // Write the PNG file
            System.IO.File.WriteAllBytes(path, bytes);

            // Refresh the asset database to recognize the new file
            AssetDatabase.Refresh();

            // Clean up
            Object.DestroyImmediate(whiteTexture);
        }

        private static string GetSelectedFolderPath()
        {
            string path = "Assets";

            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (System.IO.File.Exists(path))
                {
                    path = System.IO.Path.GetDirectoryName(path);
                }

                break;
            }

            return path;
        }
    }
}
