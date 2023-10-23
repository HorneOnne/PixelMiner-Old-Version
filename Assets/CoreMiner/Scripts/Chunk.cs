using UnityEngine;
using CoreMiner.Utilities;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;

namespace CoreMiner
{
    public class Chunk : MonoBehaviour
    {
        [Header("Chunk Settings")]
        public CoreMiner.Utilities.Grid<Tile> ChunkData;
        public float FrameX;
        public float FrameY;
        public int IsometricFrameX;
        public int IsometricFrameY;
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private int _cellSize;
        private float _updateFrequency = 0.2f;
        private float _updateTimer = 0.0f;

        [Header("Tilemap visualization")]
        public Tilemap TileMap;

        public bool ChunkLoaded;

        private void Start()
        {
            ChunkLoaded = false;
        }


        private void Update()
        {
            if (Vector2.Distance(Camera.main.transform.position, transform.position) > 200f && ChunkLoaded)
            {
                WorldGeneration.Instance.ActiveChunks.Remove(this);
                gameObject.SetActive(false);
            }
        }

        public void Init(float frameX, float frameY, int isometricFrameX, int isometricFrameY, int _width, int height, float cellSize)
        {
            this.FrameX = frameX;
            this.FrameY = frameY;
            this.IsometricFrameX = isometricFrameX;
            this.IsometricFrameY = isometricFrameY;
            this._width = _width;
            this._height = height;
            ChunkData = new Grid<Tile>(_width, height, cellSize);
        }

        public void LoadHeightMap(float[,] heightValues, float minHeight, float maxHeight)
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    float value = heightValues[x, y];
                    Tile tile = new Tile(x, y);
                    tile.HeightValue = value;
                    ChunkData.SetValue(x, y, tile);
                }
            }

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    float value = ChunkData.GetValue(x, y).HeightValue;
                    //normalize our value between 0 and 1
                    value = (value - minHeight) / (maxHeight - minHeight);

                    Tile t = new Tile(x, y);
                    t.HeightValue = value;
                    ChunkData.SetValue(x, y, t);
                }
            }
        }



        public async Task LoadHeightMapAsync(float[,] heightValues, float minHeight, float maxHeight)
        {
            await Task.Run(() =>
            {
                Parallel.For(0, _width, x =>
                {
                    for (int y = 0; y < _height; y++)
                    {
                        float value = heightValues[x, y];
                        Tile tile = new Tile(x, y);
                        tile.HeightValue = value;
                        ChunkData.SetValue(x, y, tile);
                    }
                });

            });


            // Normalize the height values
            await Task.Run(() =>
            {
                Parallel.For(0, _width, x =>
                {
                    for (int y = 0; y < _height; y++)
                    {
                        float value = ChunkData.GetValue(x, y).HeightValue;
                        //normalize our value between 0 and 1
                        value = (value - minHeight) / (maxHeight - minHeight);

                        Tile t = new Tile(x, y);
                        t.HeightValue = value;
                        ChunkData.SetValue(x, y, t);
                    }
                });

            });
        }

        public void DrawChunk()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    float heightValue = ChunkData.GetValue(x, y).HeightValue;

                    if (heightValue < 0.5f)
                    {
                        TileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.DirtGrass));
                    }
                }
            }
            ChunkLoaded = true;
        }



        public void DrawChunkPerformance()
        {
            StartCoroutine(DrawChunkCoroutine());
        }
        private IEnumerator DrawChunkCoroutine()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    float heightValue = ChunkData.GetValue(x, y).HeightValue;

                    if (heightValue < 0.5f)
                    {
                        TileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.DirtGrass));
                    }
                }

                if (PerformanceManager.Instance.HitFrameLimit())
                {
                    yield return null;
                }
            }
            ChunkLoaded = true;
        }

        public void ClearChunkDraw()
        {
            TileMap.ClearAllTiles();
        }
    }
}

