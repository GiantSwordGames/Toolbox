namespace Meat
{
    using UnityEngine;

    public class TileUtility : MonoBehaviour
    {
        [SerializeField] private GameObject _floor;
        [SerializeField] private GameObject _wallNorth;
        [SerializeField] private GameObject _wallWest;
        [SerializeField] private GameObject _wallSouth;
        [SerializeField] private GameObject _wallEast;
        [SerializeField] private GameObject _ceiling;

        public GameObject floor => _floor;
        public GameObject wallNorth => _wallNorth;
        public GameObject wallWest => _wallWest;
        public GameObject wallSouth => _wallSouth;
        public GameObject wallEast => _wallEast;
        public GameObject ceiling => _ceiling;
    }
}
