using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace JamKit
{
    public class FlashingLights : MonoBehaviour
    {
        [SerializeField] private GameObject light1;
        [SerializeField] private GameObject light2;

        [SerializeField] private float _interval = 0.5f;
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(IETrigger());
        }
        
        [Button]
        public void Trigger()
        {
            StopAllCoroutines();
        }

        private IEnumerator IETrigger()
        {
            while (true)
            {
                light1.SetActive(true);
                light2.SetActive(false);
                yield return new WaitForSeconds(_interval);
                light1.SetActive(false);
                light2.SetActive(true);
                yield return new WaitForSeconds(_interval);
                
            }
        }
    }
}