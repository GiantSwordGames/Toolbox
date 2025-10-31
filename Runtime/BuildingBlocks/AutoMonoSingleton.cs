using JamKit;
using UnityEngine;

/// <summary>
/// A singleton behaviour that will create itself if it doesn't already exist
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class AutoMonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance = null;

    /// <summary>
    /// The singleton instance
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_instance == null && (!Application.isEditor || Application.isPlaying))
            {
                CreateInstance();
            }
            return _instance;
        }
    }

    /// <summary>
    /// Whether or not the singleton instance exists
    /// </summary>
    public static bool HasInstance
    {
        get { return (_instance != null); }
    }

    /// <summary>
    /// Creates the singleton instance if it does not already exist.
    /// </summary>
    public static void EnsureInstanceExists()
    {
        if (_instance == null && (!Application.isEditor || Application.isPlaying))
        {
            CreateInstance();
        }
    }

    static void CreateInstance()
    {
        GameObject newGo = new GameObject((typeof(T).ToString()).ToUpperCamelCase());
        _instance = newGo.AddComponent<T>();
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this as T)
        {
            Destroy(_instance.gameObject);
            _instance = null;
        }
    }
}