using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace GiantSword
{
   
    
    public static class Extensions
    {
        public static void SafeInvoke(this UnityEvent unityEvent)
        {
            try
            {
                unityEvent?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        public static void SafeInvoke<T>(this UnityEvent<T> unityEvent, T arg)
        {
            try
            {
                unityEvent?.Invoke(arg);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        public static Vector3 RandomLocalPoint(this BoxCollider boxCollider)
        {
            Vector3 localPoint = new Vector3(
                Random.Range(-boxCollider.size.x / 2, boxCollider.size.x / 2),
                Random.Range(-boxCollider.size.y / 2, boxCollider.size.y / 2),
                Random.Range(-boxCollider.size.z / 2, boxCollider.size.z / 2)
            );
            
            localPoint += boxCollider.center;

            return localPoint;
        }
        
        public static Vector3 RandomWorldPoint(this BoxCollider boxCollider)
        {
            Vector3 localPoint = boxCollider.RandomLocalPoint();
            return boxCollider.transform.TransformPoint(localPoint);
        }

        public static List<Transform> GetDirectChildren(this Transform transform)
        {
            List<Transform> children = new List<Transform>();
            foreach (Transform child in transform)
            {
                children.Add(child);
            }

            return children;
        }

        public static Vector3 MultiplyComponentWise(this Vector3 vector, Vector3 operand)
        {
            return new Vector3(vector.x * operand.x, vector.y * operand.y, vector.z * operand.z);
        }

        public static Vector3 MultiplyComponentWise(this Vector3 vector, float x, float y, float z)
        {
            return new Vector3(vector.x * x, vector.y * y, vector.z * z);
        }

        public static Vector2 MultiplyComponentWise(this Vector2 vector, Vector2 operand)
        {
            return new Vector2(vector.x * operand.x, vector.y * operand.y);
        }

        public static Vector2 MultiplyComponentWise(this Vector2 vector, float x, float y)
        {
            return new Vector2(vector.x * x, vector.y * y);
        }

        public static Vector3 DivideComponentWise(this Vector3 vector, Vector3 operand)
        {
            return new Vector3(vector.x / operand.x, vector.y / operand.y, vector.z / operand.z);
        }

        public static Vector2 DivideComponentWise(this Vector2 vector, Vector2 operand)
        {
            return new Vector2(vector.x / operand.x, vector.y / operand.y);
        }

        public static Vector3 Rounded(this Vector3 vector)
        {
            return new Vector3(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), Mathf.RoundToInt(vector.z));
        }

        public static Vector3 To(this Vector3 from, Vector3 to)
        {
            return to - from;
        }

        public static float GetLengthInSeconds(this AnimationCurve curve)
        {
            if (curve == null || curve.length == 0)
            {
                return 0;
            }
         
            return curve[curve.length -1].time;
        }
        
        public static bool IsEmpty(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
        
        public static bool IsNotEmpty(this string str)
        {
            return IsEmpty(str) == false;
        }

        
        public static float Dot(this Vector2 lhs, Vector2 rhs)
        {
            return Vector2.Dot(lhs, rhs);
        }
        
        public static float Dot(this Vector3 lhs, Vector3 rhs)
        {
            return Vector3.Dot(lhs, rhs);
        }
        
        public static Vector3 Cross(this Vector3 lhs, Vector3 rhs)
        {
            return Vector3.Cross(lhs, rhs);
        }

        public static Vector3 MidPoint(this Vector3 from, Vector3 to)
        {
            return (to + from) / 2;
        }
        public static Vector2 Rotate(this Vector2 from, float min, float max)
        {
            float degrees = Random.Range(min, max);
            return from.Rotate(degrees);
        }
        
        public static Vector3 Rotate(this Vector3 from, float min, float max)
        {
            return Quaternion.Euler( Random.Range(min, max), Random.Range(min, max), Random.Range(min, max))*from;
        }
        public static Vector2 Round(this Vector2 from)
        {
            return new Vector2(Mathf.Round(from.x), Mathf.Round(from.y));
        }
        public static Vector3 Round(this Vector3 from)
        {
            return new Vector3(Mathf.Round(from.x), Mathf.Round(from.y));
        }
        public static Vector2 Rotate(this Vector2 from, float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);
            return new Vector2(cos * from.x - sin * from.y, sin * from.x + cos * from.y);
        }
        public static T GetRandomElement<T>(this T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }
        
        public static T GetElementLooping<T>(this T[] array, int i)
        {
            return array[(i+array.Length) % array.Length];
        }
        
        
        public static T GetElementLooping<T>(this List<T> array, int i)
        {
            return array[(i+array.Count) % array.Count];
        }

        public static void Encapsulate(this BoxCollider2D boxCollider2D, BoxCollider2D other)
        {
            // Calculate bounds for the current box collider
            Bounds bounds = boxCollider2D.bounds;

            // Encapsulate the bounds of the other collider
            bounds.Encapsulate(other.bounds);

            // Convert world bounds back into local collider space
            Vector2 localCenter = boxCollider2D.transform.InverseTransformPoint(bounds.center);

            // Adjust for scale
            Vector3 lossyScale = boxCollider2D.transform.lossyScale;

            // Calculate new size considering lossyScale
            Vector2 newSize = new Vector2(bounds.size.x / Mathf.Abs(lossyScale.x), bounds.size.y / Mathf.Abs(lossyScale.y));

            // Assign new values to the collider
            boxCollider2D.offset = localCenter;
            boxCollider2D.size = newSize;
        }


        public static void IgnoreCollider(this Collider2D colliderA, Collider2D colliderB, bool ignore = true)
        {
            Physics2D.IgnoreCollision(colliderA, colliderB, ignore);
        }
        
        public static void TemporarilyIgnoreCollider(this Collider2D colliderA, Collider2D colliderB, float duration, bool ignore = true)
        {
            Physics2D.IgnoreCollision(colliderA, colliderB, ignore);
            AsyncHelper.Delay(duration, () => 
            {
                Physics2D.IgnoreCollision(colliderA, colliderB, ignore == false);
            });
        }



        public static Vector2 GetContactPosition(this Collision2D collision2D)
        {
            return collision2D.contacts[0].point;
        }
        
        public static Vector2 GetContactNormal(this Collision2D collision2D)
        {
            return collision2D.contacts[0].normal;
        }
        
        public static Vector3 GetContactPosition(this Collision collision)
        {
            return collision.contacts[0].point;
        }
        
        public static Vector3 GetContactNormal(this Collision collision)
        {
            return collision.contacts[0].normal;
        }
       
        public static List<T> ExtractElementsOfType<T, T2>(this IEnumerable<T2> collection)
        {
            List<T> clips = new List<T>();
            foreach (var item in collection)
            {
                if (item is T casted)
                {
                    clips.Add(casted);
                }
            }
            return clips;
        }
        
        public static T GetOrAddComponent<T>(this GameObject uo) where T : Component
        {
            var component = uo.GetComponent<T>();
            if (component != null)
            {
                return component;
            }

            return uo.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this Component uo) where T : Component
        {
            var component = uo.GetComponent<T>();
            if (component != null)
            {
                return component;
            }

            return uo.gameObject.AddComponent<T>();
        }


        public static Vector3 To(this Transform from, Transform to)
        {
            return from.position.To(to.position);
        }
        public static float DistanceTo(this Vector3 from, Vector3 to)
        {
            return (to - from).magnitude;
        }
        
        public static Vector2 XY(this Vector3 vector3)
        {
            return (Vector2)vector3;
        }

        public static Color WithAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }
        
        public static Color LerpTo(this Color from, Color to, float lerp)
        {
            return Color.Lerp(from, to, lerp);
        }

        public static Vector3 WithXZ(this Vector3 vector, Vector3 value)
        {
            vector.x = value.x;
            vector.z = value.z;
            return vector;
        }
        
        public static Vector3 WithXY(this Vector3 vector, Vector3 value)
        {
            vector.x = value.x;
            vector.y = value.y;
            return vector;
        }

        public static Vector3 WithX(this Vector3 vector, float value)
        {
            vector.x = value;
            return vector;
        }

        public static Vector3 WithY(this Vector3 vector, float value)
        {
            vector.y = value;
            return vector;
        }
        
        public static Vector2 WithX(this Vector2 vector, float value)
        {
            vector.x = value;
            return vector;
        }

        public static Vector2 WithY(this Vector2 vector, float value)
        {
            vector.y = value;
            return vector;
        }

        public static Vector3 WithZ(this Vector3 vector, float value)
        {
            vector.z = value;
            return vector;
        }


        public static JointDrive WithPositionSpring(this JointDrive drive, float spring)
        {
            drive.positionSpring = spring;
            return drive;
        }


        public static JointDrive WithPositionDamper(this JointDrive drive, float damper)
        {
            drive.positionDamper = damper;
            return drive;
        }

        public static void SetAsActiveScene(this Scene scene)
        {
            SceneManager.SetActiveScene(scene);
        }

        public static Coroutine DoScaleTween(this Transform transform, float start, float to, float duration)
        {
            return DoScaleTween( transform, new Vector3(start, start, start), new Vector3(to, to, to), duration);
            
        }
        public static Coroutine DoScaleTween( this Transform transform, Vector3 start, Vector3 to, float duration)
        {
            return AsyncHelper.StartCoroutine(IEDoScaleTween( transform, start, to, duration));
        }

        public static void OnComplete(this Coroutine coroutine, Action action)
        {
            AsyncHelper.InvokeOnCoroutineComplete(coroutine, action);
        }

        public static T Instantate<T>(this T prefab) where T : Object
        {
            return Object.Instantiate(prefab);
        }
        
        
        public static T Instantate<T>(this T prefab, Vector3 position) where T : Object
        {
            return Object.Instantiate(prefab, position, Quaternion.identity);
        }
        
        public static T Instantate<T>(this T prefab, Transform parent) where T : Object
        {
            return Object.Instantiate(prefab, parent);
        }

        public static T Instantate<T>(this T prefab, Transform parent, Vector3 position) where T : Object
        {
            return Object.Instantiate(prefab, position, Quaternion.identity, parent);
        }
        private static IEnumerator IEDoScaleTween(Transform transform, Vector3 start, Vector3 to, float duration)
        {
            float lerp = 0;
            while (lerp < 1)
            {
                lerp += Time.deltaTime / duration;
                lerp = Mathf.Clamp01(lerp);
                transform.localScale = Vector3.Lerp(start, to, lerp);
                yield return null;
            }   
            yield break;
        }
        public static void SetProperty(this Renderer meshRenderer, string name, float value)
        {
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            meshRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat(name, value);
            meshRenderer.SetPropertyBlock(propertyBlock);
        }

        public static void SetProperty(this Renderer meshRenderer, string name, Vector4 value)
        {
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            meshRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetVector(name, value);
            meshRenderer.SetPropertyBlock(propertyBlock);
        }

        public static void DestroyChildren(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                RuntimeEditorHelper.SmartDestroy(transform.GetChild(i).gameObject);
            }
        }

        public static string GetHierarchyPath(this Component component)
        {
            string path = component.name;
            Transform transform = component.transform;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }

            return path;
        }

        public static List<T> GetComponentsInChildren<T>(this GameObject[] gameObjects) where T : Component
        {
            List<T> results = new List<T>();
            for (int i = 0; i < gameObjects.Length; i++)
            {
                T[] components = gameObjects[i].GetComponentsInChildren<T>();
                results.AddRange(components);
            }

            return results;
        }

        public static Collider[] GetOverlappingColliders(this Collider _collider)
        {
            if (_collider is BoxCollider boxCollider)
            {
                Vector3 worldCenter = boxCollider.transform.TransformPoint(boxCollider.center);
                Vector3 worldHalfExtents = Vector3.Scale(boxCollider.size, boxCollider.transform.lossyScale) * 0.5f;
                return Physics.OverlapBox(worldCenter, worldHalfExtents, boxCollider.transform.rotation);
            }

            if (_collider is SphereCollider sphereCollider)
            {
                return Physics.OverlapSphere(sphereCollider.transform.position + sphereCollider.center, sphereCollider.radius);
            }
            
            if (_collider is CapsuleCollider capsuleCollider)
            {
                Vector3 point0 = capsuleCollider.transform.TransformPoint(capsuleCollider.center + Vector3.up * capsuleCollider.height / 2);
                Vector3 point1 = capsuleCollider.transform.TransformPoint(capsuleCollider.center - Vector3.up * capsuleCollider.height / 2);
                return Physics.OverlapCapsule(point0, point1, capsuleCollider.radius);
            }
            
            return new Collider[0];
        }
        
        public static Collider2D[] GetOverlappingColliders(this Collider2D _collider)
        {
            if (_collider is BoxCollider2D boxCollider)
            {
                Vector3 worldCenter = boxCollider.transform.TransformPoint(boxCollider.offset);
                Vector3 size = boxCollider.size.MultiplyComponentWise(boxCollider.transform.lossyScale);
                return Physics2D.OverlapBoxAll(worldCenter, size, boxCollider.transform.eulerAngles.z);
            }

            if (_collider is CircleCollider2D circleCollider)
            {
                return Physics2D.OverlapCircleAll((Vector2)circleCollider.transform.position + circleCollider.offset, circleCollider.radius);
            }
            
            if (_collider is CapsuleCollider2D capsuleCollider)
            {
                Vector3 point0 = capsuleCollider.transform.TransformPoint(capsuleCollider.offset + Vector2.up * capsuleCollider.size.y / 2);
                Vector3 point1 = capsuleCollider.transform.TransformPoint(capsuleCollider.offset - Vector2.up * capsuleCollider.size.y / 2);
                return Physics2D.OverlapCapsuleAll(point0, point1, capsuleCollider.direction, capsuleCollider.transform.eulerAngles.z);
            }
            
            return Array.Empty<Collider2D>();
            
        }
        
        public static void DrawWireGizmos(this Collider collider)
        {
            var cache =Gizmos.matrix;
            Gizmos.matrix = collider.transform.localToWorldMatrix;
            if (collider is BoxCollider boxCollider)
            {
                Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
            }
            if (collider is SphereCollider sphereCollider)
            {
                Gizmos.DrawWireSphere(sphereCollider.center, sphereCollider.radius);
            }
            if (collider is CapsuleCollider capsuleCollider)
            {
                Gizmos.DrawWireSphere(capsuleCollider.center + Vector3.up * capsuleCollider.height / 2, capsuleCollider.radius);
                Gizmos.DrawWireSphere(capsuleCollider.center - Vector3.up * capsuleCollider.height / 2, capsuleCollider.radius);
                Gizmos.DrawWireCube(capsuleCollider.center, new Vector3(capsuleCollider.radius * 2, capsuleCollider.height, capsuleCollider.radius * 2));
            }
            
            Gizmos.matrix = cache;
        }

        public static string BitMaskToString(this LayerMask layerMask)
        {
            return BitMaskToString(layerMask.value);
        }
        
        public static string BitMaskToString(this int integer)
        {
            string bitmask = Convert.ToString(integer, 2).PadLeft(32, '0'); // 32-bit binary
            return bitmask;
        }
        
        public static string GetFullPath(this Component component)
        {
            return component.transform.GetHierarchyPath() + "/" + component.GetType().Name;
        }
   
        public static bool IsOutOfFrame(this Camera camera, Vector3 point, float marginInViewSpace = 0)
        {
            return IsVisible(camera, point, false, 0, marginInViewSpace) == false;
        }

        public static bool IsVisible(this Camera camera, Vector3 point, bool raycast = false,
            int layerMaskOfBlockingObjects = 0, float marginInViewSpace = 0)
        {
            Vector3 viewPoint = camera.WorldToViewportPoint(point);

            if (viewPoint.x > 1 + marginInViewSpace || viewPoint.y > 1 + marginInViewSpace || viewPoint.z > camera.farClipPlane)
                return false;

            if (viewPoint.x < 0 - marginInViewSpace || viewPoint.y < 0 - marginInViewSpace || viewPoint.z < camera.nearClipPlane)
                return false;

            Vector3 vector3 = camera.transform.position.To(point);
            if (raycast && Physics.Raycast(camera.transform.position, vector3.normalized, out RaycastHit hit,
                    vector3.magnitude, layerMaskOfBlockingObjects))
            {
                return false;
            }

            return true;
        }

        
        public static void SetEnabled(this Object obj, bool enabled)
        {
            if (obj is MonoBehaviour comp)
            {
                comp.enabled = enabled;
            }
            else if (obj is GameObject go)
            {
                go.SetActive(enabled);

            }
        }

        public static void SetDirection(this Transform transform, int direction)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * direction;
            transform.localScale = scale;
        }
        
        public static void SetDirection(this Transform transform, float direction)
        {
            SetDirection(transform, direction>=0?1:-1);
        }

        public static void Toggle(this GameObject gameObject)
        {
            gameObject.SetActive(gameObject.activeSelf == false);
        }
        public static Transform GetFirstActiveChild(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf)
                {
                    return child;
                }
            }

            return null;
        }

        public static void SnapLocalPosition(this Transform transform)
        {
            Vector3 localPosition = transform.localPosition;
            localPosition.x = Mathf.Round(localPosition.x);
            localPosition.y = Mathf.Round(localPosition.y);
            localPosition.z = Mathf.Round(localPosition.z);
            transform.localPosition = localPosition;
        }
        public static Transform GetLastActiveChild(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                if (child.gameObject.activeSelf)
                {
                    return child;
                }
            }

            return null;
        }
        
        public static Transform GetNextSibling(this Transform child, int direction, bool loop = true)
        {
            if (child.parent == null)
            {
                return null;
            }
            
            int index = child.GetSiblingIndex();
            
            index += direction;

            if (loop)
            {
                index += child.parent.childCount;
                index %= child.parent.childCount;
            }
            else
            {
                index = Mathf.Clamp(index, 0, child.parent.childCount - 1);
            }
            
            
            return child.parent.GetChild(index);
        }

     

        public static void Solo(this Transform transform, bool enableParentChain = false)
        {
            foreach (Transform sibling in transform.parent)
            {
                sibling.gameObject.SetActive(sibling == transform);
            }
            
            if (enableParentChain)
            {
                Transform parent = transform.parent;
                while (parent != null)
                {
                    parent.gameObject.SetActive(true);
                    parent = parent.parent;
                }
            }
        }

        public static void Deactivate(this Transform transform)
        {
            transform.gameObject.SetActive(false);
        }
        
        public static void Solo(this GameObject gameObject)
        {
            Solo(gameObject.transform);
        }

        public static void Deactivate(this GameObject gameObject)
        {
            gameObject.SetActive(false);
        }

        public static Vector3 GetBottom(this Bounds bounds)
        {
            return bounds.center - Vector3.up * bounds.extents.y;
        }
        
        public static Vector3 GetTop(this Bounds bounds)
        {
            return bounds.center + Vector3.up * bounds.extents.y;
        }
        

        public static List<T> GetDirectChildren<T>(this Component component, bool includeInactive = false) where T : Component
        {
            return component.gameObject.GetDirectChildren<T>(includeInactive);
        }

        public static void SetLayer(this GameObject gameObject, string layer, bool applyToChildren = false)
        {
            SetLayer(gameObject, LayerMask.NameToLayer(layer), applyToChildren);
        }
        
        public static void SetLayer(this GameObject gameObject, int layer, bool applyToChildren =false)
        {
            gameObject.layer = layer;
            if (applyToChildren)
            {
                foreach (Transform child in gameObject.transform)
                {
                    child.gameObject.SetLayer(layer, true);
                }
            }
        }


        

        public static List<T> GetDirectChildren<T>(this GameObject gameObject, bool includeInactive =false ) where T : Component
        {
            List<Transform> directChildren = gameObject.transform.GetDirectChildren();
            List<T> children = new List<T>();
            foreach (Transform child in directChildren)
            {
                var component = child.gameObject.GetComponent<T>();
                if (component)
                {
                    children.Add(component);
                }
            }

            return children;
        }

        public static bool IsInViewport(this Camera camera, Vector3 position)
        {
            Vector3 viewportPoint = camera.WorldToViewportPoint(position);
            return viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1;
        }


        public static Coroutine TweenFloat(this TextMeshProUGUI text, float start, float end,float duration,
            Func<float, string> formattingFunction, Action onComplete = null)
        {
            return AsyncHelper.StartCoroutine(text.IETweenFloat(start, end, duration, formattingFunction, onComplete));
        }

        private static IEnumerator IETweenFloat(this TextMeshProUGUI text, float start, float end,float duration,
            Func<float, string> formatingFunction, Action onComplete)
        {
            float lerp = 0;
            while (lerp < 1)
            {
                lerp += Time.deltaTime/duration;
                lerp = Mathf.Clamp01(lerp);
                text.text = formatingFunction(Mathf.Lerp(start, end, lerp));
                yield return null;
            }
            onComplete?.Invoke();
        }
        
        public static Coroutine TweenCurrentText(this TextMeshProUGUI text, float durationPerLetter =0.02f, Action onShowLetter = null)
        {
            return AsyncHelper.StartCoroutine(text.IETweenCurrentText(durationPerLetter,onShowLetter));
        }

        private static IEnumerator IETweenCurrentText(this TextMeshProUGUI text, float durationPerLetter, Action onShowLetter)
        {
            string str = text.text;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                builder.Append(str[i]);
                text.text = builder.ToString();
                onShowLetter?.Invoke();
                yield return new WaitForSeconds(durationPerLetter);
            }
        }
        
        public static Coroutine AnimateTextAmount(this TextMeshProUGUI text, string prefix, float value,
            string postfix, float duration, int digits = 0)
        {
            return AsyncHelper.StartCoroutine(text.IEAnimateTextAmount(prefix, value, postfix, duration, digits));
        }

        private static IEnumerator IEAnimateTextAmount(this TextMeshProUGUI text, string prefix, float value, string postfix, float duration, int digits = 0)
        {
            float lerp = 0;
            while (lerp < 1)
            {
                lerp += Time.deltaTime/duration;
                lerp = Mathf.Clamp01(lerp);
                
                text.text = prefix + (value * lerp).RoundToString(digits) + postfix;
                yield return null;
            }
        }

        public static void SoloNextChild(this Transform transform)
        {
            int current = -1;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.activeSelf)
                {
                    current = i;
                    break;
                }
            }

            current++;
            current+= transform.childCount;
            current %= transform.childCount;
            
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(i == current);
            }
        }
        
        
        
        public static void SoloPreviousChild(this Transform transform)
        {
            if(transform.childCount == 0)
            {
                return;
            }
            int current = -1;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.activeSelf)
                {
                    current = i;
                    break;
                }
            }

            current--;
            current+= transform.childCount;
            current %= transform.childCount;
            
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(i == current);
            }
        }

        /// <summary>
        /// checks if the list has been initialized for the key. If not it initializes the list. Then adds the value.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        public static void AddToListAtKey<T1, T2>(this Dictionary<T1, List<T2>> dictionary, T1 key, T2 value)
        {
            try
            {
                if (key == null)
                {
                    Debug.LogWarning("key is null for value: " + value);
                    return;
                }

                if (dictionary.ContainsKey(key) == false)
                {
                    dictionary.Add(key, new List<T2>());
                }

                dictionary[key].Add(value);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// checks if the list has been initialized for the key and removes the value. If the list is empty it is removed from the dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        public static void RemoveFromListAtKey<T1, T2>(this Dictionary<T1, List<T2>> dictionary, T1 key, T2 value)
        {
            try
            {
                if (key == null)
                {
                    Debug.LogWarning("key is null for value: " + value);
                    return;
                }

                if (dictionary.ContainsKey(key))
                {
                    dictionary[key].Remove(value);
                    if (dictionary[key].Count == 0)
                    {
                        dictionary.Remove(key);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

        }
        
        public static bool ContainsExactly<T>(this IList<T> list, IList<T> values )
        {
            if (list.Count != values.Count)
            {
                return false;
            }
            
            for (int i = 0; i < values.Count; i++)
            {
                if (list[i].Equals( values[i]) ==false)
                {
                    return false;
                }
            }

            return true;
        }
        public static bool Contains<T>(this IList<T> list, IList<T> values )
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (list.Contains(values[i]) == false)
                {
                    return false;
                }
            }

            return true;
        }


        public static bool ContainsAny<T>(this IList<T> list, IList<T> values)
        {
            return AnyOverlap(list, values);
        }
        public static bool AnyOverlap<T>(this IList<T> listA, IList<T> listB )
        {
            if(listB.IsNullOrEmpty())
                return false;
        
            for (int i = 0; i < listB.Count; i++)
            {
                if (listA.Contains(listB[i]))
                {
                    return true;
                }
            }

            return false;
        }
        
        public static T GetRandom<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0)
            {
                throw new System.ArgumentException("The list is empty or null.");
            }

            int randomIndex = Random.Range(0, list.Count);
            return list[randomIndex];
        }
        
        
        public static T Last<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0)
            {
                return default;
            }

            return list[^1];
        }

        public static int GetRandomIndex<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0)
            {
                throw new System.ArgumentException("The list is empty or null.");
            }

            return Random.Range(0, list.Count);
        }

        public static T GetElement<T>(this IList<T> list, float lerp)
        {
            
            if (list == null || list.Count == 0)
            {
                throw new System.ArgumentException("The list is empty or null.");
            }

            if (list.Count == 1)
            {
                return list[0];
            }
            
            int index = Mathf.RoundToInt(lerp * (list.Count - 1));
            return list[index];
        }        
        
        public static T GetElementClamped<T>(this IList<T> list, int index)
        {
            return list[Mathf.Clamp(index, 0, list.Count - 1)];
        }       
        
        public static T GetElement<T>(this T[] list, float lerp)
        {
            if (list == null || list.Length == 0)
            {
                throw new System.ArgumentException("The list is empty or null.");
            }

            if (list.Length == 1)
            {
                return list[0];
            }
            
            int index = Mathf.CeilToInt(lerp * (list.Length - 1));
            return list[index];
        }        

        public static int GetNextWrappedIndex<T>(this IList<T> list, int index)
        {
            if (list == null || list.Count == 0)
            {
                throw new System.ArgumentException("The list is empty or null.");
            }

            index++;
            index += list.Count;
            index %= list.Count;
            return index;
        }
        
        public static int GetPreviousWrappedIndex<T>(this IList<T> list, int index)
        {
            if (list == null || list.Count == 0)
            {
                throw new System.ArgumentException("The list is empty or null.");
            }

            index--;
            index += list.Count;
            index %= list.Count;
            return index;
        }
        
        public static bool IsNullOrEmpty<T>(this IList<T> list, bool throwError = false)
        {
            return list == null || list.Count == 0;
        }
        
        public static Rect[] SplitWidthPercent(this Rect rect, float percent, float spacing = 0)
        {
            float rw = rect.width * percent - spacing / 2f;
            Rect lr = Rect.MinMaxRect(rect.min.x, rect.min.y, rect.min.x + rw, rect.max.y);
            Rect rr = Rect.MinMaxRect(lr.max.x + spacing / 2f, lr.min.y, rect.max.x, rect.max.y);
            return new[] {lr, rr};
        }

        public static Rect[] SplitFromLeft(this Rect rect, float dst, float spacing = 0)
        {
            Rect lr = Rect.MinMaxRect(rect.min.x, rect.min.y, rect.min.x + dst - spacing/2f, rect.max.y);
            Rect rr = Rect.MinMaxRect(lr.max.x + spacing/2f, lr.min.y, rect.max.x, rect.max.y);
            return new[] {lr, rr};
        }
        
        
        public static string RoundToString(this float value, int digits = 1)
        {
            var formatted = Math.Round(value, digits).ToString();
            if (digits > 0)
            {
                if (formatted.Contains(".") == false)
                {
                    formatted += ".";
                    for (int i = 0; i < digits; i++)
                    {
                        formatted += "0";
                    }
                }
            }

            return formatted;
        }

        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                // Swap elements
                (list[i], list[j]) = (list[j], list[i]);
            }

            return list;
        }

        public static int CountRegisteredListeners(this Action list)
        {
            if (list == null)
            {
                return 0;
            }

            return list.GetInvocationList().Length;
        }
        
        public static int CountRegisteredListeners<T>(this Action<T> list)
        {
            if (list == null)
            {
                return 0;
            }

            return list.GetInvocationList().Length;
        }

        public static void SortBySiblingIndex<T>(this List<T> list) where T : Component
        {
            list.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
        }
        
        public static void SortBySiblingIndex<T>(this T[] list) where T : Component
        {
            System.Array.Sort(list, (a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
        }
        
        public static Vector3 Snap(this Vector3 vector, float snapValue)
        {
            return new Vector3(
                Mathf.Round(vector.x / snapValue) * snapValue,
                Mathf.Round(vector.y / snapValue) * snapValue,
                Mathf.Round(vector.z / snapValue) * snapValue
            );
        }
        
        public static float Round(this float value, int digits =0)
        {
            return (float)Math.Round(value, digits);
        }
        
        public static float RoundToNearest(this float value, float increment)
        {
            return Mathf.Round(value / increment) * increment;
        }


        public static void DoTween(this AnimationCurve curve, Action<float> tweenFunctions)
        {
           AsyncHelper.StartCoroutine(IEDoTween(curve, tweenFunctions));
        }
        

        private static IEnumerator IEDoTween(this AnimationCurve curve, Action<float> tweenFunctions)
        {
            float duration = curve.GetLengthInSeconds();
            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp(time ,0, duration);
                float value = curve.Evaluate(t);
                tweenFunctions?.Invoke(value);
                yield return null;
            }
        }
        
        public static int ToInt(this float value)
        {
            return (int)value;
        }

        public static bool IsInside(this Vector3 point, Vector2 center, Vector2 size)
        {
            return point.x > center.x - size.x / 2 && point.x < center.x + size.x / 2 &&
                   point.y > center.y - size.y / 2 && point.y < center.y + size.y / 2;
        }
        
        public static bool HasComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() != null;
        }
        
        public static bool HasComponent<T>(this Component component) where T : Component
        {
            return component.GetComponent<T>() != null;
        }

        public static float AsPercent(this float value)
        {
            var percent = (int)(value * 100);
            return percent;
        }
        public static string ToTitleCase(this object obj)
        {
            return obj.ToString().ToTitleCase();
        }
        
        public static string ToTitleCase(this string text)
        {
            text=text.Replace("_", " ").Trim();

            string result = "";
            
            for (int i = 0; i < text.Length; i++)
            {
                if (i == 0)
                {
                    result += char.ToUpper(text[i]);
                }
                else if (char.IsUpper(text[i]))
                {
                    result += " " + text[i];
                }
                else
                {
                    result += text[i];
                }
            }

            return result;
        }

        public static string ToTitleCase(this string text, string remove)
        {

            string result = text.Replace(remove,"");
            return  result.ToTitleCase();
        }

        public static string ToUpperCamelCase(this string input)
        {
            input=input.Replace("_", " ").Trim();
            string result = Regex.Replace(input, @"(?:^|_|\s)(.)", match => match.Groups[1].Value.ToUpper());
            return result;
        }

        public static StringBuilder AppendNewline(this StringBuilder sb, string text)
        {
            if (sb == null) throw new ArgumentNullException(nameof(sb));

            return sb.Append(text).Append(Environment.NewLine);
        }

        public static List<Rigidbody> GetRigidbodies(this List<Collider> colliders)
        {
            return GetUniqueComponentsInParents<Rigidbody>(colliders);
        }
        
        public static List<T> GetUniqueComponentsInParents<T>(this List<Collider> colliders) where T : Component
        {
            List<T> uniqueComponents = new List<T>();

            foreach (Collider col in colliders)
            {
                T componentInParent = col.GetComponentInParent<T>();

                if (componentInParent != null)
                {
                    uniqueComponents.AddIfNotContained(componentInParent);
                }
            }

            return uniqueComponents;
        } 
        public static List<T> GetComponentsInChildren<T>(this List<T> components) where T : Component
        {
            List<T> uniqueComponents = new List<T>();

            foreach (T col in components)
            {
                T[] componentsInChildren = col.GetComponentsInChildren<T>();

                uniqueComponents.AddRange(componentsInChildren);
            }

            return uniqueComponents;
        }
        
        public static bool IsDescendentOfTransform(this Transform transform, Transform parent)
        {
            while (transform != null)
            {
                if (transform == parent)
                {
                    return true;
                }

                transform = transform.parent;
            }

            return false;
        }
        
        public static void RemoveIfContained<T1, T2>(this Dictionary<T1, List<T2>> dictionary, T1 key, T2 value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key].Remove(value);
            }
        }

        public static void AddIfNotContained<T1, T2>(this Dictionary<T1, List<T2>> dictionary, T1 key, T2 value)
        {
            if (dictionary.ContainsKey(key) == false)
            {
                dictionary[key].Add(value);
            }
        }

        public static void RemoveIfContained<T1>(this List<T1> list, T1 value)
        {
            if (list.Contains(value))
            {
                list.Remove(value);
            }
        }
        
        public static void AddIfNotContained<T1>(this List<T1> list, T1 value)
        {
            if (list.Contains(value) == false)
            {
                list.Add(value);
            }
        }
      

        public static Transform SetLossyScale(this Transform transform, Vector3 scale)
        {
            if (transform.parent)
            {
                transform.localScale = scale.DivideComponentWise(transform.parent.lossyScale);
            }
            else
            {
                transform.localScale = scale;
            }
            return transform;
        }

 		public static Transform SetLocalScaleX(this Transform transform, float x)
        {
            Vector3 scale = transform.localScale;
            scale.x = x;
            transform.localScale = scale;
            return transform;
        }

        public static Transform SetLocalScaleY(this Transform transform, float y)
        {
            Vector3 scale = transform.localScale;
            scale.y = y;
            transform.localScale = scale;
            return transform;
        }

        public static Transform SetLocalScaleZ(this Transform transform, float z)
        {
            Vector3 scale = transform.localScale;
            scale.z = z;
            transform.localScale = scale;
            return transform;
        }

        public static void Scale(this Transform transform, float scale)
        {
            transform.localScale *= scale;
    }
        public static void Scale(this Transform transform, Vector3 scale)
        {
            transform.localScale = Vector3.Scale(transform.localScale, scale);
        }
        
        public static void SetLocalX(this Transform transform, float value)
        {
            transform.localPosition = transform.localPosition.WithX(value);
        }
        
        public static void SetLocalY(this Transform transform, float value)
        {
            transform.localPosition = transform.localPosition.WithY(value);
        }
        
        public static void SetLocalZ(this Transform transform, float value)
        {
            transform.localPosition = transform.localPosition.WithZ(value);
        }
        
        public static void SetXY(this Transform transform, Vector3 value)
        {
            transform.position = value.WithZ(transform.position.z);
        }
        
        public static void SetLocalXY(this Transform transform, Vector3 value)
        {
            transform.localPosition = value.WithZ(transform.localPosition.z);
        }

        
        public static bool IsEqualXY(this Transform transform, Vector3 value, float tolerance = 0.000f)
        {
            Vector2 difference = transform.position - value;
            difference.x = Mathf.Abs(difference.x);
            difference.y = Mathf.Abs(difference.y);
            return difference.x <= tolerance && difference.y <= tolerance;
        }

        
        public static string AppendNewLine(this string text, string newText)
        {
            return   text + Environment.NewLine + newText;
        }

        public static void SetElementsEnabled<T>(this T[] components, bool state) where T : Behaviour
        {
            for (int i = 0; i < components.Length; i++)
            {
                components[i].enabled = state;
            }
        }
        
        public static void SetElementsEnabled<T>(this List<T> components, bool state) where T : Behaviour
        {
            for (int i = 0; i < components.Count; i++)
            {
                components[i].enabled = state;
            }
        }
        
        public static void SetX(this Transform transform, float value)
        {
            transform.position = transform.position.WithX(value);
        }
        
        public static void SetY(this Transform transform, float value)
        {
            transform.position = transform.position.WithY(value);
        }
        
        public static void SetZ(this Transform transform, float value)
        {
            transform.position = transform.position.WithZ(value);
        }
        
        public static Transform FlipX(this Transform transform, bool flip)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (flip ? -1 : 1);
            transform.localScale = scale;
            return transform;
        }

        public static bool Contains(this List<SceneReference> sceneReferences, Scene scene)
        {
            for (int i = 0; i < sceneReferences.Count; i++)
            {
                if (sceneReferences[i].SceneName == scene.name)
                {
                    return true;
                }
            }

            return false;
        }



        public static Transform GetPreviousSibling(this Transform transform)
        {
            if (transform.parent == null)
            {
                return null;
            }

            int index = transform.GetSiblingIndex();
            if (index == 0)
            {
                return null;
            }

            return transform.parent.GetChild(index - 1);
        }

        public static string ElementsToString<T>(this T[] _array)
        {
            string s = "";
            if (_array != null)
            {
                for (int i = 0; i < _array.Length; i++)
                {
                    s += _array[i].ToString() + ", ";
                }
            }

            return s;
        }


#if Editor
        public static T GetComponent<T>(this Scene scene) where T : Component
        {
            GameObject[] rootGameObjects = scene.GetRootGameObjects();
            for (int i = 0; i < rootGameObjects.Length; i++)
            {
                T result = rootGameObjects[i].GetComponentInChildren<T>();

                if (result)
                    return result;
            }

            return null;
        }

        public static List<T> GetComponents<T>(this Scene scene) where T : Component
        {
            GameObject[] rootGameObjects = scene.GetRootGameObjects();
            List<T> list = new List<T>();
            for (int i = 0; i < rootGameObjects.Length; i++)
            {
                T[] result = rootGameObjects[i].GetComponentsInChildren<T>();
                list.AddRange(result);
            }

            return list;
        }
        
        public static GameObject FindGameObjectByName(this Scene scene, string name)
        {
            GameObject[] rootGameObjects = scene.GetRootGameObjects();
            for (int i = 0; i < rootGameObjects.Length; i++)
            {

                GameObject result = FindGameObjectByName(rootGameObjects[i], name);
                if (result)
                    return result;
            }

            return null;
        }
#endif


        public static GameObject FindGameObjectByName(this GameObject parent, string name)
        {
            if (parent.name.Equals(name))
            {
                return parent;
            }

            foreach (Transform child in parent.transform)
            {
                GameObject result = child.gameObject.FindGameObjectByName(name);
                if (result)
                    return result;
            }

            return null;
        }
    }
}
