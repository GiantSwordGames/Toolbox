using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace GiantSword
{
    public class SoundInstance : MonoBehaviour
    {
        [SerializeField] AudioSource _audioSource;
        [FormerlySerializedAs("_soundBank")] [SerializeField] SoundAsset _soundAsset;
        private float _pitch;
        private float _time;
        private float _fade = 1;
        private float _volume;

        public float pitch {
           get
           {
               return _pitch;
           }
           set
           {
               _pitch = value;
               ApplyPitchScaling();
           }
       }

        public float audioSourceVolume
        {
            get
            {
                return _audioSource.volume;
            }
            set
            {
                _audioSource.volume = value;
            }
        }
        
        public void FadeOut(float duration)
        {
            StartCoroutine(IEFadeOut(duration));
        }

        private IEnumerator IEFadeOut(float duration)
        {
            float timer = 0;
            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                float lerp = timer / duration;
                lerp = Mathf.Clamp01(lerp);
                _audioSource.volume = (1 - lerp)*_volume;
                yield return null;
            }
            Destroy(gameObject);
        }

        
        public void FadeIn(float duration)
        {
            StartCoroutine(IEFadeIn(duration));
        }
        private IEnumerator IEFadeIn(float duration)
        {
            float timer = 0;
            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                float lerp = timer / duration;
                lerp = Mathf.Clamp01(lerp);
                _audioSource.volume = lerp*_volume;
                yield return null;
            }
        }
        
                
        

        private void ApplyPitchScaling()
        {
            if (_soundAsset.useScaledTime)
            {
                _audioSource.pitch = _pitch*Time.timeScale;
            }
            else
            {
                   
                _audioSource.pitch = _pitch;
            }
        }

        public static SoundInstance Create(SoundAsset soundAsset, Transform parent, Vector3 position)
        {
            if (soundAsset == null)
                return null;
            GameObject go = new GameObject("SoundInstance_"+soundAsset.name);
            SoundInstance soundInstance = go.AddComponent<SoundInstance>();
            soundInstance.Setup(soundAsset, parent, position);
            return soundInstance;
        }

        private void Setup(SoundAsset soundAsset, Transform parent, Vector3 position)
        {
            transform.parent = parent;
            transform.position = position;
            _soundAsset = soundAsset;
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.loop = _soundAsset.isLooping;
            _audioSource.outputAudioMixerGroup = _soundAsset.mixerGroup;
            _audioSource.spatialBlend = _soundAsset.spacialBlend;
            _audioSource.rolloffMode = _soundAsset.rolloffMode;
            _audioSource.minDistance = _soundAsset.rolloffDistance.min;
            _audioSource.maxDistance = _soundAsset.rolloffDistance.max;
            _audioSource.clip = _soundAsset.NextClip();
            _volume = _soundAsset.volume.GetRandom();
            _audioSource.volume = _volume;
            _pitch = _soundAsset.pitch.GetRandom();
            ApplyPitchScaling();
            _audioSource.Play();
        }

        private void Update()
        {
            ApplyPitchScaling();
            _time += Time.deltaTime;
            float audioLength = 0;
            if (_audioSource.clip)
            {
                audioLength = _audioSource.clip.length;
            }

            if (_audioSource.loop == false &&  _time > audioLength + 20f)
            {
                Destroy(gameObject);
            }
        }

        [Button]
        public void Play()
        {
            _audioSource.Play();
        }

        [Button]
        public void Stop()
        {
            _audioSource.Stop();
        }
    }
}