using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GiantSword
{
    public class Preference<T>
    {
        public enum Mode
        {
            Project,
            Global
        }
        
        private static List<string> __keyRegistry = new List<string>();
        private string _key;
        private string _rawKey;
        private bool _initialized = false;
        private T _value;
        private T _default;
        public Action<T> _onChanged;

        public Preference(string key, T defaultValue, Mode mode = Mode.Project)
        {
            _rawKey = key;
            if(mode == Mode.Global)
                _key = key;
            else
                _key =  Application.dataPath +"_"+ key;
            
            _value = defaultValue;
            _default = defaultValue;
            _onChanged = null;

            if (__keyRegistry.Contains(key) == false)
            {
                __keyRegistry.Add(_key);
            }
            else
            {
                Debug.LogError("Duplicate Preference Key " + key);
            }
        }

        private void LazyInitialize()
        {
            if(_initialized)
                return;

            _initialized = true;
#if UNITY_EDITOR
            if (EditorPrefs.HasKey(_key) == false)
            {
                Set(_default, true);
            }
            else
            {
                Set(LoadFromEditorPrefs());
            }
#endif
        }

        public static implicit operator bool(Preference<T> pref)
        {
#if UNITY_EDITOR
            pref.LazyInitialize();
            return EditorPrefs.GetBool(pref._key);
#else
            return false;
#endif
        }

        public static implicit operator string(Preference<T> pref)
        {

#if UNITY_EDITOR
            pref.LazyInitialize();
            return EditorPrefs.GetString(pref._key);
#else
            return "";
#endif
        }

        public static implicit operator int(Preference<T> pref)
        {
#if UNITY_EDITOR
            pref.LazyInitialize();
            return EditorPrefs.GetInt(pref._key);
#else
            return 0;
#endif
        }

        public static implicit operator float(Preference<T> pref)
        {
#if UNITY_EDITOR
            pref.LazyInitialize();
            return EditorPrefs.GetFloat(pref._key);
#else
            return 0;
#endif
        }

        public static implicit operator Object(Preference<T> pref)
        {
#if UNITY_EDITOR
            pref.LazyInitialize();
            var assetPath = EditorPrefs.GetString(pref._key);
            var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
            return asset;
#else
            return null;
#endif
        }

        public T value
        {
            set => Set(value);
            get
            {
                LazyInitialize();
                return _value;
            }
        }

        public Action<T> onChanged
        {
            get => _onChanged;
            set => _onChanged = value;
        }

        public string key => _key;

        private void SetObject(Object newValue, bool force = false)
        {
#if UNITY_EDITOR
            var assetPath = AssetDatabase.GetAssetPath(newValue );
            EditorPrefs.SetString(_key, assetPath);
            LoadFromEditorPrefs();
            #endif
        }
        private void Set(T newValue, bool force = false)
        {

            if (force == false && _value != null && _value.Equals(newValue))
            {
                return;
            }

            _value = newValue;

#if UNITY_EDITOR
            if (typeof(T) == typeof(string))
                EditorPrefs.SetString(_key, Convert.ToString(newValue));

            else if (typeof(T) == typeof(float))
            {
                EditorPrefs.SetFloat(_key, Convert.ToSingle(newValue));

            }
            else if (typeof(T) == typeof(int))
            {
                EditorPrefs.SetInt(_key, Convert.ToInt32(newValue));
            }

            else if (typeof(T) == typeof(bool))
                EditorPrefs.SetBool(_key, Convert.ToBoolean(newValue));

            else if (newValue == null)
                EditorPrefs.SetString(_key, string.Empty);

            else if (newValue is UnityEngine.Object)
            {
                var assetPath = AssetDatabase.GetAssetPath(newValue as UnityEngine.Object);
                EditorPrefs.SetString(_key, assetPath);
            }
#endif
            _onChanged?.Invoke(_value);
        }

        private T LoadFromEditorPrefs()
        {
#if UNITY_EDITOR
            LazyInitialize();

            if (typeof(T) == typeof(float))
                return (T) Convert.ChangeType(EditorPrefs.GetFloat(this._key), typeof(T));

            if (typeof(T) == typeof(int))
                return (T) Convert.ChangeType(EditorPrefs.GetInt(this._key), typeof(T));

            if (typeof(T) == typeof(bool))
                return (T) Convert.ChangeType(EditorPrefs.GetBool(this._key), typeof(T));

            if (typeof(T) == typeof(string))
                return (T) Convert.ChangeType(EditorPrefs.GetString(this._key), typeof(T));

            if (typeof(T).IsSubclassOf(typeof(Object)))
            {
                string str = EditorPrefs.GetString(this._key);

                Object asset = AssetDatabase.LoadAssetAtPath(str, typeof(T));

                return (T) Convert.ChangeType(asset, typeof(T));
            }

            return (T) Convert.ChangeType((bool) this, typeof(T));
#else
            return default;
#endif
        }

        public void DrawDefaultGUI(string label = "")
        {
#if UNITY_EDITOR
            
            if (label == "")
                label = _rawKey.ToTitleCase();
            
            
            if (this is Preference<bool> boolPref)
            {
                EditorGUI.BeginChangeCheck();
                // var newValue =  EditorGUILayout.Toggle(boolPref.value, label);
                var newValue =  GUILayout.Toggle(boolPref.value,  label.ToTitleCase());
                if (EditorGUI.EndChangeCheck())
                {
                    boolPref.value = newValue;
                }
                return;
            }
            
            if (this is Preference<int> intPref)
            {
                EditorGUI.BeginChangeCheck();
                var newValue =  EditorGUILayout.IntField(intPref.value, label);
                if (EditorGUI.EndChangeCheck())
                {
                    intPref.value = newValue;
                }
                return;
            }
            
            
            if (this is Preference<float> floatPref)
            {
                EditorGUI.BeginChangeCheck();
                var newValue =  EditorGUILayout.FloatField(floatPref.value, label);
                if (EditorGUI.EndChangeCheck())
                {
                    floatPref.value = newValue;
                }
                return;
            }
            
            
            if (this is Preference<string> stringPref)
            {
                EditorGUI.BeginChangeCheck();
                var newValue =  EditorGUILayout.TextField(stringPref.value, label);
                if (EditorGUI.EndChangeCheck())
                {
                    stringPref.value = newValue;
                }
                return;
            }
            
            if (typeof(T).IsSubclassOf(typeof(Object)))
            {
                
                // draw object field
                EditorGUI.BeginChangeCheck();
                var newValue = EditorGUILayout.ObjectField(label, (Object) this, typeof(T), false);
                if (EditorGUI.EndChangeCheck())
                {
                    SetObject(newValue);
                }
                
                return;
            }
            
            throw new NotImplementedException();
#endif
        }

        public void DrawSlider(float min, float max)
        {
#if UNITY_EDITOR
            using (var changeCheck = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.LabelField("Spatial Blend:");
                float newValue = EditorGUILayout.Slider((float) this, min, max);
                if (changeCheck.changed)
                {
                    Set((T) Convert.ChangeType(newValue, typeof(T)));
                }
            }
#endif
        }

    }

    public static class PreferenceExtensions
    {
        public static void Toggle(this Preference<bool> pref)
        {
#if UNITY_EDITOR
            pref.value = !pref.value;
#endif
        }
    }
}
