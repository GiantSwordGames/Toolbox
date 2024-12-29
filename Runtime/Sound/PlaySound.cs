using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace GiantSword
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
            
        }        
        
       
        [SerializeField] SoundAsset _soundAsset;
        [ShowNonSerializedField] private SoundInstance _soundInstance;
 
        [SerializeField]  Behaviour _behaviour = Behaviour.Manual;
        [SerializeField] Parenting _parenting = Parenting.NoParent;
        [SerializeField] bool _playOnEnable = false;
        [SerializeField] bool _stopOnDisable = false;
        [SerializeField] float _delay;

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
                GiantSword.SafeCoroutineRunner.StartCoroutine(IEPlay());
            }
        }

        [Button]
        public void Stop()
        {
         
             if(_soundInstance)
            {
                _soundInstance.Stop();
            }

        }

        private IEnumerator IEPlay()
        {
            if(_delay > 0)
                yield return new WaitForSeconds(_delay); 
            
        
            {
                if (_parenting == Parenting.ParentToThisTransform)
                {
                    _soundInstance = _soundAsset.Play( transform, transform.position);
                }
                else if (_parenting == Parenting.ParentToListenerAtCurrentPosition)
                {
                    _soundInstance = _soundAsset.Play( Camera.main.transform, transform.position);
                }
                else if (_parenting == Parenting.ParentToListenerAtPositionZero)
                {
                    _soundInstance = _soundAsset.Play( Camera.main.transform,Camera.main.transform.position);
                }
                else if (_parenting == Parenting.NoParent)
                {
                    _soundInstance = _soundAsset.Play( null, transform.position);
                }

            }
            
        }
    }
}