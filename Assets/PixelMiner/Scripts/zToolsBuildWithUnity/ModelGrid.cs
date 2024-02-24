using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ToolBuildWithUnity
{
    public class ModelGrid : MonoBehaviour
    {
        public int Width;
        public int Height;

        [SerializeField] private GameObject _gridPrefab;
        [SerializeField] private Tilemap _gridmap;
        [SerializeField] private TileBase _tile;

        private void Awake()
        {
            _gridmap = GetComponentInChildren<Tilemap>();

            for(int y = 0; y < Height; y++)
            {
                for(int x = 0; x < Width; x++)
                {
                    _gridmap.SetTile(new Vector3Int(x, y, 0), _tile);
                }
            }
        }
    }
}
