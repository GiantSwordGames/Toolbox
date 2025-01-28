using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    public class PlayMusic : MonoBehaviour
    {
        private static SoundInstance _currentTrack;
        [SerializeField] private SoundAsset _track;
        [SerializeField] private float _crossfadeTime = 0;
        [SerializeField] private bool _persistAcrossScenes = true;

        private bool _hasTriggered = false;
        [Button]
        public void Trigger()
        {
            if (_hasTriggered)
            {
                return;
            }
            
            _hasTriggered = true;
            SoundInstance previousTrack = _currentTrack;


            if (previousTrack)
            {
                if (previousTrack && previousTrack.soundAsset == _track)
                {
                    return;
                }
            }

            _currentTrack = _track.Play( transform,transform.position);
            _currentTrack.FadeIn(_crossfadeTime);
            if (previousTrack)
            {
                previousTrack.FadeOut(_crossfadeTime);
            }

            if (_persistAcrossScenes)
            {
                _currentTrack.transform.parent = null;
                DontDestroyOnLoad(_currentTrack);
            }
        }
        
    }
}
