using UnityEngine;
using PixelMiner.WorldGen;
using PixelMiner.Enums;

namespace PixelMiner
{
    public class Tool : MonoBehaviour
    {
        [SerializeField] private GameObject _cursorPrefab;
        private Main _main;

        private Transform _cursor;

        private void Start()
        {
            _main = Main.Instance;

            // Cursor
            _cursor = Instantiate(_cursorPrefab).transform;
            _cursor.gameObject.hideFlags = HideFlags.HideInHierarchy;
        }

       
        private void Update()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Tile tile = Main.Instance.GetTile(mousePosition, out Chunk chunk);
            Vector2 tileWorldPos = _main.GetTileWorldPosition(mousePosition);
            _cursor.position = tileWorldPos;


            if (Input.GetMouseButton(0))
            {
                if (chunk != null)
                {
                    chunk.SetTile(tile.FrameX, tile.FrameY, _main.GetTileBase(TileType.DirtGrass));
                }

            }

            if(Input.GetMouseButton(1))
            {
                if (chunk != null)
                {
                    chunk.SetTile(tile.FrameX, tile.FrameY, null);
                }
            }

         
        }

        public void SetChunkColor(Chunk chunk, Color color)
        {
            chunk.LandTilemap.color = color;
        }
    }
}

