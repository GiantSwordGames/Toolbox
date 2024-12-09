using UnityEngine;

namespace GiantSword
{
    public static class SoundSystemAlt
    {
        private static AudioListener _audioListener;
        public static SoundInstance Play(this SoundAsset soundAsset) => PlaySound(soundAsset);
        public static SoundInstance Play(this SoundAsset soundAsset, Vector3 position) => PlaySound(soundAsset, position);
        public static SoundInstance Play(this SoundAsset soundAsset, Transform parent, Vector3 position) => PlaySound(soundAsset, parent, position);

        public static SoundInstance PlaySound(SoundAsset soundAsset, Transform parent, Vector3 position)
        {
            SoundInstance soundInstance = SoundInstance.Create(soundAsset, parent, position);
            return soundInstance;   
        }

        public static SoundInstance PlaySound(SoundAsset soundAsset, Vector3 position)
        {
            return PlaySound(soundAsset, null, position);
        }

        public static SoundInstance PlaySound(SoundAsset soundAsset)
        {
            if(_audioListener == null)
                _audioListener = GameObject.FindObjectOfType<AudioListener>();
            
            Transform t = _audioListener.transform;
            SoundInstance soundInstance = SoundInstance.Create(soundAsset, t, t.position);
            return soundInstance;
        }
    }
}