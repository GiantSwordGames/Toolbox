using System.Collections;
using System.Collections.Generic;
using JamKit;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class VideoTrigger : MonoBehaviour
{
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private UnityEvent _onTrigger;
[SerializeField] private float _delay;
     
    bool _hasStarted = false;
    bool _hasTriggered = false;
    void Reset()
    {
            _videoPlayer = _videoPlayer ?? GetComponent<VideoPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_hasTriggered == false)
        {
            if (_videoPlayer.isPlaying)
            {
                _hasStarted = true;
            }
            else
            {
                if (_hasStarted)
                {
                    _hasTriggered = true;
                    AsyncHelper.Delay(_delay, Trigger);
                }
            }
            
        }
        
    }

    private void Trigger()
    {
        _onTrigger?.Invoke();
    }
}
