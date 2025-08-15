namespace JamKit
{
    using UnityEngine;
    
    public static class GizmoUtility
    {
    
        private static Transform _gizmoParent;
    
        private static Transform gizmoParent
        {
            get
            {
                if (_gizmoParent == null)
                {
                    _gizmoParent = new GameObject("Gizmos").transform;
                    _gizmoParent.transform.position = Vector3.zero;
                }
                
                return _gizmoParent;
            }
        }
        
       
    
      
        
        /// <summary>
        /// Draws an arrow gizmo that billboards towards the scene camera.
        /// </summary>
        /// <param name="position">The starting position of the arrow.</param>
        /// <param name="direction">The direction of the arrow.</param>
        /// <param name="color">The color of the arrow.</param>
        /// <param name="arrowHeadLength">The length of the arrowhead.</param>
        /// <param name="arrowHeadAngle">The angle of the arrowhead.</param>
        public static void DrawArrowToPosition(Vector3 position, Vector3 direction,  float arrowHeadLength = 0.25f,
            float arrowHeadAngle = 20.0f)
        {
    
            position -= direction;
            
            Gizmos.DrawRay(position, direction);
    
            Camera sceneCamera = RuntimeEditorHelper.GetSceneCamera();
            if (sceneCamera != null)
            {
                Vector3 cameraPosition = sceneCamera.transform.position;
                Vector3 toCamera = (cameraPosition - (position + direction)).normalized;
    
                Vector3 right = Quaternion.LookRotation(toCamera) * Quaternion.Euler(0, arrowHeadAngle, 0) *
                                Vector3.forward;
                Vector3 left = Quaternion.LookRotation(toCamera) * Quaternion.Euler(0, -arrowHeadAngle, 0) *
                               Vector3.forward;
    
                Gizmos.DrawRay(position + direction, right * arrowHeadLength);
                Gizmos.DrawRay(position + direction, left * arrowHeadLength);
            }
        }
    
        /// <summary>
        /// Draws an arrow gizmo that billboards towards the scene camera.
        /// </summary>
        /// <param name="position">The starting position of the arrow.</param>
        /// <param name="direction">The direction of the arrow.</param>
        /// <param name="color">The color of the arrow.</param>
        /// <param name="arrowHeadLength">The length of the arrowhead.</param>
        /// <param name="arrowHeadAngle">The angle of the arrowhead.</param>
        public static void DrawArrowFromPosition(Vector3 position, Vector3 direction, float arrowHeadLength = 0.25f)
        {
    
            // Draw the main line of the arrow
            Gizmos.DrawLine(position, position + direction);
    
            // Calculate the arrowhead points
            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + 20, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - 20, 0) * new Vector3(0, 0, 1);
    
            // Draw the arrowhead lines
            Gizmos.DrawLine(position + direction, position + direction + right * arrowHeadLength);
            Gizmos.DrawLine(position + direction, position + direction + left * arrowHeadLength);
    
            // Restore the previous Gizmos color
        }
    
        public static void DrawArrow(Vector3 startPosition, Vector3 endPosition, float arrowHeadLength = 0.25f,  float arrowPositionPercent = 1.0f)
        {
            float arrowHeadAngle = 45.0f;
            arrowPositionPercent = Mathf.Clamp01(arrowPositionPercent);

            Vector3 direction = endPosition - startPosition;
            Vector3 arrowPosition = Vector3.Lerp(startPosition, endPosition, arrowPositionPercent);

            // Draw main line from start to arrow position
            Gizmos.DrawLine(startPosition, arrowPosition);

            // Draw remaining line (optional, if you want the arrowhead to be "on top" of the line)
            if (arrowPositionPercent < 1.0f)
                Gizmos.DrawLine(arrowPosition, endPosition);

            // Draw Arrowhead
            Camera sceneCamera = RuntimeEditorHelper.GetSceneCamera();
            if (sceneCamera == null)
                return;

            Vector3 cameraForward = (sceneCamera.transform.position - arrowPosition).normalized;
            Vector3 sideAxis = Vector3.Cross(direction.normalized, cameraForward).normalized;

            Vector3 right = Quaternion.AngleAxis(arrowHeadAngle, sideAxis) * -direction.normalized;
            Vector3 left = Quaternion.AngleAxis(-arrowHeadAngle, sideAxis) * -direction.normalized;

            Gizmos.DrawRay(arrowPosition, right * arrowHeadLength);
            Gizmos.DrawRay(arrowPosition, left * arrowHeadLength);
        }
        public static void DrawCircle(Vector3 center, float radius,  Vector3 axis, int resolution = 32)
        {
            float angle = 0f;
            Vector3[] points = new Vector3[resolution + 1];
    
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, axis.normalized);
    
            for (int i = 0; i <= resolution; i++)
            {
                float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                float z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
    
                Vector3 point = new Vector3(x, 0, z);
                points[i] = rotation * point + center;
    
                angle += 360f / resolution;
            }
    
            for (int i = 0; i < resolution; i++)
            {
                Gizmos.DrawLine(points[i], points[i + 1]);
            }
        }
    }
}
