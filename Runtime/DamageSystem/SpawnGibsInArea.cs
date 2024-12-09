using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GiantSword
{
    public class SpawnGibsInArea : MonoBehaviour
    {
        [SerializeField] private bool _disableSelf = false;
        [SerializeField] private int _gibCount = 10;
        [SerializeField] private float _gibForce = 1;
        [SerializeField]  private BoxCollider _volume;
        [SerializeField] private List<Rigidbody> _gibPrefabs;

        private void OnValidate()
        {
        }

        [Button]
        public void Trigger()
        {
            if (_disableSelf)
            {
                gameObject.SetActive(false);
            }

           // spawn gibs inside volume
              for (int i = 0; i < _gibCount; i++)
              {
                  Vector3 randomPoint = _volume.RandomWorldPoint();

                Rigidbody gib = Instantiate(_gibPrefabs.GetRandom(), randomPoint, Random.rotation);
                gib.AddForce(Random.insideUnitSphere * _gibForce, ForceMode.Impulse);
              }
           
        }
    }
}
