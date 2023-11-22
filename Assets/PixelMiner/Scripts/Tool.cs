using UnityEngine;
using PixelMiner.WorldGen;
using PixelMiner.Enums;
using PixelMiner.Utilities;

namespace PixelMiner
{
    public class Tool : MonoBehaviour
    {
        private Main _main;


        // Cached
        private TextMesh _isoCoordText;


        private void Start()
        {
            _main = Main.Instance;

            _isoCoordText = Utilities.Utilities.CreateWorldText("", null,fontSize: 10, textAnchor: TextAnchor.MiddleCenter);
            _isoCoordText.hideFlags = HideFlags.HideInInspector;
        }

        public Vector2 Offset;
        private void Update()
        {
            if(_isoCoordText != null)
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition += Offset;
                Tile tile = Main.Instance.GetTile(mousePosition);
                Vector2 worldTilePos = Main.Instance.GetTileWorldPosition(mousePosition);
                Main.Instance.SetTileColor(mousePosition, Color.magenta);
                if(tile != null)
                {
                    SetIsoCoordText($"GridTile: {worldTilePos} " +
                        $"\n Mouse: {mousePosition} " +
                        $"\n Frame: {tile.FrameX} \t {tile.FrameY}", mousePosition);
                }
                
            }

            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition += Offset;
                Chunk chunk = _main.GetChunk(mousePosition, WorldGeneration.Instance.ChunkWidth, WorldGeneration.Instance.ChunkHeight);
     
                if (chunk != null)
                {
                    //SetChunkColor(chunk, Color.red);
                    chunk.SetTile(mousePosition, Main.Instance.GetTileBase(TileType.DirtGrass));
                }

            }

            if(Input.GetMouseButtonDown(1))
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition += new Vector2(0f, -0.75f);
                Chunk chunk = _main.GetChunk(mousePosition, WorldGeneration.Instance.ChunkWidth, WorldGeneration.Instance.ChunkHeight);
                if (chunk != null)
                {
                    //SetChunkColor(chunk, Color.white);
                    chunk.SetTile(mousePosition, null);
                }
            }
        }

        public void SetChunkColor(Chunk chunk, Color color)
        {
            chunk.LandTilemap.color = color;
        }


        #region Utilities
        public void SetIsoCoordText(string text, Vector2 position)
        {
            _isoCoordText.text = text;
            _isoCoordText.transform.position = position;
        }
        #endregion
    }
}

