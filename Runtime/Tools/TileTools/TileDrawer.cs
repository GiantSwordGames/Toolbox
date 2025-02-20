using UnityEngine;

    public class TileDrawer : MonoBehaviour
    {
        [SerializeField] private float _tileSize = 2;
        [SerializeField] private float _snapOfset = 0;

        public float tileSize => _tileSize;

        public float SnapOffset => _snapOfset;
    }
