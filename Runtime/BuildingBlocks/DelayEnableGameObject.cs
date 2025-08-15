using System.Collections;
using System.Collections.Generic;
using JamKit;
using UnityEngine;

public class DelayEnableGameObject : MonoBehaviour
{
    [SerializeField] private float _delay = 0.5f;
    
    void Start()
    {
        gameObject.SetActive(false);
        AsyncHelper.Delay(_delay, () =>
        {
            gameObject.SetActive(true);
        });
    }

}
