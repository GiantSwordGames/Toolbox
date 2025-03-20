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
       [SerializeField] private bool _autoDestroy = true;
        
        private float _randomizedPitch;
        private float _time;
        private float _fade = 1;
        private float _randomizedVolume;
        private float _pitchIncrement;
        private float _velocityAttenuation =1f;

        public float randomizedPitch {
           get
           {
               return _randomizedPitch;
           }
           set
           {
               _randomizedPitch = value;
               ApplyPitch();
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

        public SoundAsset soundAsset => _soundAsset;

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
                _audioSource.volume = (1 - lerp)*_randomizedVolume;
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
                _audioSource.volume = lerp*_randomizedVolume;
                yield return null;
            }
        }

        private void ApplyVolume()
        {
            _audioSource.volume = _randomizedVolume*_velocityAttenuation;
        }
        private void ApplyPitch()
        {
            float basePitch = _randomizedPitch + _pitchIncrement*_soundAsset.pitchIncrementRange; 
            if (_soundAsset.useScaledTime)
            {
                _audioSource.pitch = basePitch *Time.timeScale;
            }
            else
            {
                   
                _audioSource.pitch = basePitch;
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
            _audioSource.dopplerLevel = _soundAsset.dopplerLevel;
            _audioSource.clip = _soundAsset.NextClip();
            _randomizedVolume = _soundAsset.volume.GetRandom();
            _audioSource.volume = _randomizedVolume;
            ApplyVolume();
            _randomizedPitch = _soundAsset.pitch.GetRandom() + _soundAsset.GetIncrementedPitch();
            ApplyPitch();
            _audioSource.Play();
            _soundAsset.IncrementPitch();
        }

        private void Update()
        {
            ApplyPitch();
            _time += Time.deltaTime;
            float audioLength = 0;
            if (_audioSource.clip)
            {
                audioLength = _audioSource.clip.length;
            }

            if (_autoDestroy &&  _audioSource.loop == false &&  _time > audioLength + 2f)
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

        public void SetIncrementalPitch(float pitchIncrement)
        {
            _pitchIncrement = pitchIncrement;
            ApplyPitch();
        }

        public void SetVelocityAttenuation(float velocity)
        {
            _velocityAttenuation = _soundAsset.velocityAttenuation.GetNormalized(velocity);
            ApplyVolume();
        }
    }
    
}