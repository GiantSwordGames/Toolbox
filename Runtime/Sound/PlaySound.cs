using System;
using System.Collections;
using NaughtyAttributes;
using SoundManager;
using UnityEngine;
using UnityEngine.Serialization;

namespace JamKit
{
    public class PlaySound : MonoBehaviour
    {
        private enum Parenting
        {
            ParentToThisTransform,
            ParentToListenerAtCurrentPosition,
            ParentToListenerAtPositionZero,
            NoParent,
        }

        private enum Behaviour
        {
            Manual,
            PlayOnEnable,
            PlayOnSubsequentEnables,
            PlayOnDisable,
        }


        [InlineScriptableObject]
        [SerializeField]
        EffectSoundBank _soundAsset;

        [ShowNonSerializedField] private EffectSoundInstance _soundInstance;

        [SerializeField] Behaviour _behaviour = Behaviour.Manual;
        [SerializeField] Parenting _parenting = Parenting.NoParent;
        [SerializeField] bool _playOnEnable = false;
        [SerializeField] bool _stopOnDisable = false;
        [SerializeField] float _delay;
        [SerializeField] private bool _looping;

        private int _enableCount;

        private void OnValidate()
        {
            if (_playOnEnable)
            {
                _behaviour = Behaviour.PlayOnEnable;
            }
        }

        void OnEnable()
        {
            _enableCount++;
            if (_behaviour == Behaviour.PlayOnEnable)
            {
                Trigger();
            }
            else if (_behaviour == Behaviour.PlayOnSubsequentEnables)
            {
                if (_enableCount > 1)
                {
                    Trigger();
                }
            }
        }

        private void OnDisable()
        {
            if (_stopOnDisable)
            {
                Stop();
            }

            if (_behaviour == Behaviour.PlayOnDisable)
            {
                Trigger();
            }
        }


        [Button]
        public void Trigger()
        {
            if (enabled == false || gameObject.activeInHierarchy == false)
            {
                return;
            }

            if (Application.isPlaying)
            {
                SafeCoroutineRunner.StartCoroutine(IEPlay());
            }
        }

        [Button]
        public void Stop()
        {

            if (_soundInstance)
            {
                // _soundInstance.Stop();
            }

        }

        private IEnumerator IEPlay()
        {
            if (_delay > 0)
                yield return new WaitForSeconds(_delay);


            {
                if (_parenting == Parenting.ParentToThisTransform)
                {
                    _soundInstance = _soundAsset.Play(transform);
                }
                else if (_parenting == Parenting.ParentToListenerAtCurrentPosition)
                {
                    _soundInstance = _soundAsset.Play(Camera.main.transform);
                }
                else if (_parenting == Parenting.ParentToListenerAtPositionZero)
                {
                    _soundInstance = _soundAsset.Play(Camera.main.transform);
                }
                else if (_parenting == Parenting.NoParent)
                {
                    _soundInstance = _soundAsset.Play(transform.position);
                }

                if (_soundInstance != null) _soundInstance.IsLooping = _looping;

            }

        }


    }
}