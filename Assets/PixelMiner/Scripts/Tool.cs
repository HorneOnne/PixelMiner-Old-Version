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

            Debug.Log($"FloorToInt(5.7): {Mathf.FloorToInt(5.7f)}");
            Debug.Log($"RountToInt(5.7): {Mathf.RoundToInt(5.7f)}");
        }

       
        private void Update()
        {
            if(_isoCoordText != null)
            {
                //Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //Tile tile = Main.Instance.GetTile(mousePosition, out Chunk chunk);
                //if (tile != null)
                //{
                //    Vector3 pos = IsometricUtilities.GlobalToLocal(mousePosition.x, mousePosition.y, isoX: chunk.transform.position.x, isoY: chunk.transform.position.y, isoW: 2f, isoH: 1f);
                //    pos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
                //    Main.Instance.SetTileColor(chunk, (byte)pos.x, (byte)pos.y, Color.magenta);
                //}

                //Vector2 worldTilePos = Main.Instance.GetTileWorldPosition(mousePosition);
                //SetIsoCoordText($"GridTile: {worldTilePos} " +
                //       $"\n Mouse: {mousePosition} " +
                //       $"\n Frame: {tile?.FrameX} \t {tile?.FrameY}" +
                //       $"\n {IsometricUtilities.GlobalToLocal(mousePosition.x, mousePosition.y, isoX: 32, isoY: -16, isoW: 2f, isoH: 1f)}"
                //       , mousePosition);

                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Main.Instance.SetTileColor(mousePosition, Color.magenta);
                Tile tile = Main.Instance.GetTile(mousePosition, out Chunk chunk);
                if(tile != null)
                {
                    SetIsoCoordText($"Mouse: {mousePosition} " +
                       $"\n {Main.Instance.GetTileWorldPosition(mousePosition)}" +
                       $"\n {IsometricUtilities.LocalToGlobal(tile.FrameX, tile.FrameY, chunk.transform.position.x, chunk.transform.position.y)}"
                       , mousePosition);
                }
                
            }

            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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

