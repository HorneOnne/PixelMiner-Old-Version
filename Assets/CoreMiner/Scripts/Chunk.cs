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
        private float _unloadChunkDistance = 200;
        private float _updateFrequency = 1.0f;
        private float _updateTimer = 0.0f;

        [Header("Tilemap visualization")]
        public Tilemap IsometricTileMap;

        public bool ChunkLoaded;



        private void Awake()
        {
            ChunkLoaded = false;
        }


        private void Update()
        {
            if (Time.time - _updateTimer > _updateFrequency)
            {
                _updateTimer = Time.time;
                if (Vector2.Distance(Camera.main.transform.position, transform.position) > _unloadChunkDistance && ChunkLoaded 
                    && WorldGeneration.Instance.AutoUnloadChunk)
                {
                    WorldGeneration.Instance.ActiveChunks.Remove(this);
                    gameObject.SetActive(false);
                }
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

                    if (heightValue < WorldGeneration.Instance.DeepWater)
                    {
                        IsometricTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.DeepWater));
                    }                  
                    else if (heightValue < WorldGeneration.Instance.Water)
                    {
                        IsometricTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Water));
                    }
                    else if (heightValue < WorldGeneration.Instance.Sand)
                    {
                        IsometricTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Sand));
                    }
                    else if (heightValue < WorldGeneration.Instance.Grass)
                    {
                        IsometricTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.DirtGrass));
                    }
                    else if (heightValue < WorldGeneration.Instance.Forest)
                    {
                        IsometricTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.ForestGrass));
                    }
                    else if (heightValue < WorldGeneration.Instance.Rock)
                    {
                        IsometricTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Rock));
                    }
                    else if (heightValue < WorldGeneration.Instance.Snow)
                    {
                        IsometricTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Snow));
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

                    if (heightValue < WorldGeneration.Instance.DeepWater)
                    {
                        IsometricTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.DeepWater));
                    }
                    else if (heightValue < WorldGeneration.Instance.Water)
                    {
                        IsometricTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Water));
                    }
                    else if (heightValue < WorldGeneration.Instance.Sand)
                    {
                        IsometricTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Sand));
                    }
                    else if (heightValue < WorldGeneration.Instance.Grass)
                    {
                        IsometricTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.DirtGrass));
                    }
                    else if (heightValue < WorldGeneration.Instance.Forest)
                    {
                        IsometricTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.ForestGrass));
                    }
                    else if (heightValue < WorldGeneration.Instance.Rock)
                    {
                        IsometricTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Rock));
                    }
                    else if (heightValue < WorldGeneration.Instance.Snow)
                    {
                        IsometricTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Snow));
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
            IsometricTileMap.ClearAllTiles();
        }

        public void UpdateNeighbors()
        {
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    Tile t = ChunkData.GetValue(x,y);

                    t.Top = GetTop(t);
                    t.Bottom = GetBottom(t);
                    t.Left = GetLeft(t);
                    t.Right = GetRight(t);
                }
            }
        }


        private Tile GetTop(Tile t)
        {
            return ChunkData.GetValue(t.FrameX, MathHelper.Mod(t.FrameY - 1, _height));
        }
        private Tile GetBottom(Tile t)
        {
            return ChunkData.GetValue(t.FrameX, MathHelper.Mod(t.FrameY + 1, _height));
        }
        private Tile GetLeft(Tile t)
        {
            return ChunkData.GetValue(MathHelper.Mod(t.FrameX - 1, _width), t.FrameY);
        }
        private Tile GetRight(Tile t)
        {
            return ChunkData.GetValue(MathHelper.Mod(t.FrameX + 1, _width), t.FrameY);
        }

        #region Utilities
        // Function to convert world coordinates to tile coordinates
        public Vector3Int WorldToTilePosition(Vector3 worldPosition)
        {
            Vector3Int tilePosition = IsometricTileMap.WorldToCell(worldPosition);
            return tilePosition;
        }

        // Function to convert tile coordinates to world coordinates
        public Vector3 TileToWorldPosition(Vector3Int tilePosition)
        {
            Vector3 worldPosition = IsometricTileMap.GetCellCenterWorld(tilePosition);
            return worldPosition;
        }
        #endregion
    }
}

