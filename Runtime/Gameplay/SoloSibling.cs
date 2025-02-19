using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace GiantSword
{
    public class SoloSibling : MonoBehaviour
    {
        [FormerlySerializedAs("_sibling")] [SerializeField] private Transform _target;

        [SerializeField] private bool _chooseRandom = false;    
        
        [Button]
        public void Trigger()
        {
            if (enabled == false)
            {
                return;
            }
            if (_target)
            {
                if (_chooseRandom)
                {
                    int index = Random.Range(0, _target.childCount);
                    Transform child = _target.GetChild(index);
                    Solo solo = child.GetComponent<Solo>();
                    if (solo)
                    {
                        solo.Trigger();
                    }
                    else
                    {
                        child.Solo();
                    }
                }
                else
                {
                    Solo solo = _target.GetComponent<Solo>();
                    if (solo)
                    {
                        solo.Trigger();
                    }
                    else
                    {
                        _target.Solo();
                    }  
                }
            }
        }
    }
}
