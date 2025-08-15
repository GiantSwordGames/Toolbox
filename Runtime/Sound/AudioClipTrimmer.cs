using NaughtyAttributes;
using UnityEngine;

namespace JamKit
{
    public class AudioClipTrimmer : MonoBehaviour
    {
        // Original AudioClip
        public AudioClip originalClip;

        // Start and end time (in seconds) for trimming
        public float startTime = 1.0f;
        public float endTime = 3.0f;


        private void OnValidate()
        {
            endTime = Mathf.Min( endTime, originalClip.length );
        }

        [Button]
        [ContextMenu("Trim")]
        void Trim()
        {
            // Trim the clip
            AudioClip trimmedClip = TrimAudioClip(originalClip, startTime, endTime);
        
            // Play the trimmed clip (Optional)
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = trimmedClip;
            audioSource.Play();
        }

        AudioClip TrimAudioClip(AudioClip clip, float startTime, float endTime)
        {
            // Calculate sample positions
            int frequency = clip.frequency;
            int channels = clip.channels;

            int startSample = Mathf.FloorToInt(startTime * frequency * channels);
            int endSample = Mathf.FloorToInt(endTime * frequency * channels);

            // Get original audio data
            float[] originalData = new float[clip.samples * channels];
            clip.GetData(originalData, 0);

            // Calculate length of the new clip
            int trimmedLength = endSample - startSample;

            // Create a new AudioClip with the trimmed length
            AudioClip trimmedClip = AudioClip.Create(clip.name + "_trimmed", trimmedLength / channels, channels, frequency, false);

            // Extract the relevant audio data
            float[] trimmedData = new float[trimmedLength];
            System.Array.Copy(originalData, startSample, trimmedData, 0, trimmedLength);

            // Set the data for the new clip
            trimmedClip.SetData(trimmedData, 0);

            return trimmedClip;
        }
    }
}