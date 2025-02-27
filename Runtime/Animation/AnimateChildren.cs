using System;
using System.Collections;
using System.Collections.Generic;
using GiantSword;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class AnimateChildren : MonoBehaviour {

    [Min(1)]
    public int fps = 12;

    private bool _isPlaying = true;
    public bool loop = true;
    public bool randomOffset = false;
    private bool _isComplete = false;

    [SerializeField] private UnityEvent _onLoopComplete;
    float timer = 0;
    int frame = 0;

    public bool isPlaying => _isPlaying;


    [Button]
    public void Reset()
    {
        frame = 0;
        timer = 0;
        _isComplete = false;
        SetFrame();
        _isPlaying = true;
    }

    private void OnEnable()
    {
        if(randomOffset)
            timer = fps * Random.value;

        Reset();
    }

    // Update is called once per frame
    void Update () {

        if (transform.childCount == 0)
            return;

        timer += Time.deltaTime;

        while (timer >= 1f/fps )
        {
            timer -= 1f / fps;
            frame++;

            
            if(frame > transform.childCount - 1)
            {
                if (loop)
                {
                    frame = 0;
                }
                else
                {
                    frame = transform.childCount - 1;
                    _isComplete = true;
                    _isPlaying = false;
                }
                _onLoopComplete.Invoke();
            }

            SetFrame();
        }

	}

    void SetFrame()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(i == frame);
        }
    }
}
