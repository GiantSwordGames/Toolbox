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

    public bool loop = true;
    public bool randomOffset = false;
    private bool _isComplete = false;

    [SerializeField] private UnityEvent _onLoopComplete;
    float timer = 0;
    int frame = 0;

    List<Transform> children = new List<Transform>();

    [Button]
    public void Reset()
    {
        frame = 0;
        timer = 0;
        _isComplete = false;
        SetFrame();
    }

    private void OnEnable()
    {
        children = transform.GetDirectChildren();

        if(randomOffset)
            timer = fps * Random.value;

        Reset();
    }

    // Update is called once per frame
    void Update () {

        if (children.Count == 0)
            return;

        timer += Time.deltaTime;

        while (timer >= 1f/fps )
        {
            timer -= 1f / fps;
            frame++;

            
            if(frame > children.Count - 1)
            {
                if (loop)
                {
                    frame = 0;
                }
                else
                {
                    frame = children.Count - 1;
                    _isComplete = true;
                }
                _onLoopComplete.Invoke();
            }

            SetFrame();
        }

	}

    void SetFrame()
    {
        for (int i = 0; i < children.Count; i++)
        {
            children[i].gameObject.SetActive(i == frame);
        }
    }
}
