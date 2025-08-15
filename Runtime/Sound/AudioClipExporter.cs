using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace JamKit
{
    
    using UnityEngine;

    [ExecuteInEditMode] // This attribute allows the script to run in Edit Mode
    public class AudioClipExporter : MonoBehaviour
    {
        public List<AudioPlayableAsset> GetAudioPlayableAssets()
        {
            PlayableDirector playableDirector = GetComponent<PlayableDirector>();

            if (playableDirector == null)
            {
                Debug.LogError("PlayableDirector is not assigned.");
                return new List<AudioPlayableAsset>();
            }

            var timelineAsset = playableDirector.playableAsset as TimelineAsset;
            if (timelineAsset == null)
            {
                Debug.LogError("PlayableDirector does not have a TimelineAsset.");
                return new List<AudioPlayableAsset>();
            }

            IEnumerable<TrackAsset> trackAssets = timelineAsset.GetOutputTracks();
            List<AudioPlayableAsset> audioPlayableAssets = new List<AudioPlayableAsset>();

            // Iterate through all output tracks in the Timeline
            foreach (var track in trackAssets)
            {
                if (track is AudioTrack audioTrack)
                {
                    foreach (var clip in audioTrack.GetClips())
                    {
                        var audioPlayableAsset = clip.asset as AudioPlayableAsset;
                        if (audioPlayableAsset != null)
                        {
                            audioPlayableAssets.Add(audioPlayableAsset);
                        }
                    }
                }
            }

            return audioPlayableAssets;
        }

    }
}
