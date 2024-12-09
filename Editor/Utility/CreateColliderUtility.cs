using UnityEditor;
using UnityEngine;

namespace GiantSword
{
    
    public class CreateColliderUtility
    {

        private const string PATH = "GameObject/Collider";
        
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