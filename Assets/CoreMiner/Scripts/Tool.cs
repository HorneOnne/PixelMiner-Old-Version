using UnityEngine;

namespace CoreMiner
{
    public class Tool : MonoBehaviour
    {
        private Main _main;

        private void Start()
        {
            _main = Main.Instance;
        }


        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition += new Vector2(0.0f, -0.75f);
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




        
    }
}

