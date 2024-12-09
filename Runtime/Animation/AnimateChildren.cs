using System;
using System.Collections;
using System.Collections.Generic;
using GiantSword;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnimateChildren : MonoBehaviour {

    [Min(1)]
    public int fps = 12;

    public bool loop = true;
    public bool randomOffset = false;


    float timer = 0;
    int frame = 0;

    List<Transform> children = new List<Transform>();

    [Button]
    public void Reset()
    {
        frame = 0;
        timer = 0;
        SetFrame();
    }

    // Use this for initialization
	void Start () {
        children = transform.GetDirectChildren();

        if(randomOffset)
            timer = fps * Random.value;

        SetFrame();
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

            if(loop)
                frame %= children.Count;
            else
            {
                frame = Mathf.Clamp(frame, 0, children.Count);
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
