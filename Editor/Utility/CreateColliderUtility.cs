using UnityEditor;
using UnityEngine;

namespace JamKit
{
    
    public class CreateColliderUtility
    {

        private const string PATH = "GameObject/Collider";

        [MenuItem(PATH + "/Box Collider 2D", false, 10)]
        public static void CreateGameObjectWithBoxCollider2D(MenuCommand menuCommand)
        {
            // Create a new GameObject
            GameObject newGameObject = new GameObject("BoxCollider2D");

            GameObject parent = Selection.activeGameObject;
            if (parent)
            {
                newGameObject.transform.parent = parent.transform;
                newGameObject.transform.localPosition = Vector3.zero;
            }

            // Add a BoxCollider component to the GameObject
            newGameObject.AddComponent<BoxCollider2D>();

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newGameObject, "Create GameObject with Box Collider");

            // Select the newly created GameObject
            Selection.activeObject = newGameObject;

            // Focus the Scene View camera on the new GameObject
            SceneView.lastActiveSceneView.FrameSelected();
            
        }
        
        [MenuItem(PATH + "/Circle Collider 2D", false, 10)]
        public static void CreateGameObjectWithCircleCollider2D(MenuCommand menuCommand)
        {
            // Create a new GameObject
            GameObject newGameObject = new GameObject("CircleCollider2D");

            GameObject parent = Selection.activeGameObject;
            if (parent)
            {
                newGameObject.transform.parent = parent.transform;
                newGameObject.transform.localPosition = Vector3.zero;
            }

            // Add a BoxCollider component to the GameObject
            newGameObject.AddComponent<CircleCollider2D>();

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newGameObject, "Create GameObject with Box Collider");

            // Select the newly created GameObject
            Selection.activeObject = newGameObject;

            // Focus the Scene View camera on the new GameObject
            SceneView.lastActiveSceneView.FrameSelected();
        }
        
        [MenuItem(PATH + "/Polygon Collider 2D", false, 10)]
        public static void CreateGameObjectWithPolygonCollider2D(MenuCommand menuCommand)
        {
            // Create a new GameObject
            GameObject newGameObject = new GameObject("PolygonCollider2D");

            GameObject parent = Selection.activeGameObject;
            if (parent)
            {
                newGameObject.transform.parent = parent.transform;
                newGameObject.transform.localPosition = Vector3.zero;
            }

            // Add a BoxCollider component to the GameObject
            newGameObject.AddComponent<PolygonCollider2D>();

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newGameObject, "Create GameObject with Box Collider");

            // Select the newly created GameObject
            Selection.activeObject = newGameObject;

            // Focus the Scene View camera on the new GameObject
            SceneView.lastActiveSceneView.FrameSelected();
        }
        
        
        

        [MenuItem(PATH+ "/Box Collider", false, 10)]
        public static void CreateGameObjectWithBoxCollider(MenuCommand menuCommand)
        {

            // Create a new GameObject
            GameObject newGameObject = new GameObject("BoxCollider");
            
            GameObject parent = Selection.activeGameObject;
            if (parent)
            {
                newGameObject.transform.parent = parent.transform;
                newGameObject.transform.localPosition = Vector3.zero;
            }

            // Add a BoxCollider component to the GameObject
            newGameObject.AddComponent<BoxCollider>();

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newGameObject, "Create GameObject with Box Collider");

            // Select the newly created GameObject
            Selection.activeObject = newGameObject;
            
            // Focus the Scene View camera on the new GameObject
            SceneView.lastActiveSceneView.FrameSelected();
        }
        
        [MenuItem(PATH+ "/Sphere Collider", false, 10)]
        public static void CreateGameObjectWitSphereCollider(MenuCommand menuCommand)
        {
            // Create a new GameObject
            GameObject newGameObject = new GameObject("SphereCollider");

            GameObject parent = Selection.activeGameObject;
            if (parent)
            {
                newGameObject.transform.parent = parent.transform;
                newGameObject.transform.localPosition = Vector3.zero;
            }
            
            // Add a BoxCollider component to the GameObject
            newGameObject.AddComponent<SphereCollider>();
            
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newGameObject, "Create GameObject with Box Collider");

            // Select the newly created GameObject
            Selection.activeObject = newGameObject;
            
            // Focus the Scene View camera on the new GameObject
            SceneView.lastActiveSceneView.FrameSelected();
        }
        
        [MenuItem(PATH+ "/Capsule Collider", false, 10)]
        public static void CreateGameObjectWithCapsuleCollider(MenuCommand menuCommand)
        {
            // Create a new GameObject
            GameObject newGameObject = new GameObject("CapsuleCollider");

            GameObject parent = Selection.activeGameObject;
            if (parent)
            {
                newGameObject.transform.parent = parent.transform;
                newGameObject.transform.localPosition = Vector3.zero;
            }
            
            // Add a BoxCollider component to the GameObject
            newGameObject.AddComponent<CapsuleCollider>();
            
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(newGameObject, "Create GameObject with Box Collider");

            // Select the newly created GameObject
            Selection.activeObject = newGameObject;
            
            // Focus the Scene View camera on the new GameObject
            SceneView.lastActiveSceneView.FrameSelected();
        }
    }
}