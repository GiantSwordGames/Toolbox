using System;
using NaughtyAttributes;
using UnityEngine;

namespace JamKit
{
    public class MusicZone : MonoBehaviour
    {
        private static SoundInstance _activeGlobalTrack;
        [ShowNonSerializedField] private SoundInstance _myActiveTrack;
        [SerializeField] private SoundAsset _track;
        [SerializeField] private float _fadeIntime = 1;
        [SerializeField] private float _fadeOutTime = 1;
        [SerializeField] private bool _persistAcrossScenes = true;
        private bool _hasTriggered = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            Player player = other.GetComponentInParent<Player>();
            if (player)
            {
                TriggerFadeIn();
            }                
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            Player player = other.GetComponentInParent<Player>();
            if (player)
            {
                TriggerFadeOut();
            }                
        }


        [Button]
        public void TriggerFadeIn()
        {
            SoundInstance previousTrack = _activeGlobalTrack;


            if (previousTrack)
            {
                if (previousTrack && previousTrack.soundAsset == _track)
                {
                    return;
                }
            }

            if (_track)
            {
                if (_myActiveTrack == null)
                {
                    _myActiveTrack  = _track.Play( transform,transform.position);
                }
                _myActiveTrack.FadeIn(_fadeIntime);
                _activeGlobalTrack = _myActiveTrack;
            }
            else
            {
                _activeGlobalTrack = null;
            }
           
            if (previousTrack)
            {
                previousTrack.FadeOut(_fadeOutTime);
            }

            if (_persistAcrossScenes && _activeGlobalTrack)
            {
                _activeGlobalTrack.transform.parent = null;
                DontDestroyOnLoad(_activeGlobalTrack.gameObject);
            }
        }
        
        [Button]
        public void TriggerFadeOut()
        {
            if (_myActiveTrack)
            {
                _myActiveTrack.FadeOut(_fadeOutTime);
            }
        }
    }
}