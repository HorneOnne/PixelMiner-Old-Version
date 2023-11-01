using UnityEngine;
using CoreMiner.Utilities;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace CoreMiner
{
    public class Chunk : MonoBehaviour
    {
        [Header("Chunk Settings")]
        public CoreMiner.Utilities.Grid<Tile> ChunkData;
        public float FrameX;    // Used for calculate world position
        public float FrameY;    // Used for calculate world position
        public int IsometricFrameX; // Used for calculate world generation
        public int IsometricFrameY; // Used for calculate world generation
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        private float _unloadChunkDistance = 200;
        private float _updateFrequency = 1.0f;
        private float _updateTimer = 0.0f;
        private Vector2 _offsetTileFromCenter = new Vector2(0.0f, -0.25f);

        // Neighbors
        public Chunk Left;
        public Chunk Right;
        public Chunk Top;
        public Chunk Bottom;


        [Header("Tilemap visualization")]
        public Tilemap LandTileMap;
        public Tilemap WaterTileMap;
        public Tilemap HeatMap;
        public Tilemap TilegroupMap;
        public Grid Grid;

        public bool HasFourNeighbors;
        public bool ChunkHasDrawn;


        public List<TileGroup> Waters = new List<TileGroup>();
        public List<TileGroup> Lands = new List<TileGroup>();

        private void Awake()
        {
            ChunkHasDrawn = false;
        }



        private void Update()
        {
            if (Time.time - _updateTimer > _updateFrequency)
            {
                _updateTimer = Time.time;
                if (Vector2.Distance(Camera.main.transform.position, transform.position) > _unloadChunkDistance
                    && ChunkHasDrawn
                    && WorldGeneration.Instance.AutoUnloadChunk)
                {
                    WorldGeneration.Instance.ActiveChunks.Remove(this);
                    gameObject.SetActive(false);
                }
            }
        }

        public void Init(float frameX, float frameY, int isometricFrameX, int isometricFrameY, int _width, int height)
        {
            this.FrameX = frameX;
            this.FrameY = frameY;
            this.IsometricFrameX = isometricFrameX;
            this.IsometricFrameY = isometricFrameY;
            this._width = _width;
            this._height = height;
            ChunkData = new Grid<Tile>(_width, height, 1.0f);
        }



        public void LoadChunk()
        {
            gameObject.SetActive(true);
            if (WorldGeneration.Instance.ShowTilegroupMaps)
            {
                TilegroupMap.GetComponent<TilemapRenderer>().enabled = true;
            }
            else
            {
                TilegroupMap.GetComponent<TilemapRenderer>().enabled = false;
            }
        }

        public void UnloadChunk()
        {
            gameObject.SetActive(false);
        }


        public void LoadHeightMap(float[,] heightValues)
        {
            float minHeightNoise = WorldGeneration.Instance.MinHeightNoise;
            float maxHeightNoise = WorldGeneration.Instance.MaxHeightNoise;

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    float value = heightValues[x, y];
                    float normalizeValue = (value - minHeightNoise) / (maxHeightNoise - minHeightNoise);
                    Tile tile = new Tile(x, y);
                    tile.HeightValue = normalizeValue;
                    ChunkData.SetValue(x, y, tile);                
                }
            }
        }




        public async Task LoadHeightMapAsync(float[,] heightValues)
        {
            float minHeightNoise = WorldGeneration.Instance.MinHeightNoise;
            float maxHeightNoise = WorldGeneration.Instance.MaxHeightNoise;

            await Task.Run(() =>
            {
                Parallel.For(0, _width, x =>
                {
                    for (int y = 0; y < _height; y++)
                    {
                        float value = heightValues[x, y];

                        //normalize our value between 0 and 1
                        float normalizeValue = (value - minHeightNoise) / (maxHeightNoise - minHeightNoise);
                        Tile tile = new Tile(x, y);
                        tile.HeightValue = normalizeValue;

                        ChunkData.SetValue(x, y, tile);
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
                        LandTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.DeepWater));
                        WaterTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Water));
                    }
                    else if (heightValue < WorldGeneration.Instance.Water)
                    {
                        //LandTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Water));
                        WaterTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Water));
                    }
                    else if (heightValue < WorldGeneration.Instance.Sand)
                    {
                        LandTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Sand));
                    }
                    else if (heightValue < WorldGeneration.Instance.Grass)
                    {
                        LandTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.DirtGrass));
                    }
                    else if (heightValue < WorldGeneration.Instance.Forest)
                    {
                        //LandTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Water));
                        WaterTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.ForestGrass));
                    }
                    else if (heightValue < WorldGeneration.Instance.Rock)
                    {
                        LandTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Rock));
                    }
                    else if (heightValue < WorldGeneration.Instance.Snow)
                    {
                        LandTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Snow));
                    }
                    else
                    {
                        LandTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Snow));
                    }
                }
            }
            ChunkHasDrawn = true;
        }

        public async Task DrawChunkAsync()
        {
            TileBase[] landTiles = new TileBase[_width * _height];
            TileBase[] waterTiles = new TileBase[_width * _height];
            TileBase[] tilegroupTiles = new TileBase[_width * _height];
            Vector3Int[] positionArray = new Vector3Int[_width * _height];

            await Task.Run(() =>
            {
                Parallel.For(0, _width, x =>
                {
                    for (int y = 0; y < _height; y++)
                    {
                        positionArray[x + _width * y] = new Vector3Int(x, y, 0);

                        float heightValue = ChunkData.GetValue(x, y).HeightValue;
                        if (heightValue < WorldGeneration.Instance.DeepWater)
                        {
                            landTiles[x + _width * y] = Main.Instance.GetTileBase(TileType.Water);
                            waterTiles[x + _width * y] = Main.Instance.GetTileBase(TileType.Water);
                            tilegroupTiles[x + _width * y] = Main.Instance.GetTileBase(TileType.Heat);

                            ChunkData.GetValue(x, y).Collidable = false;
                        }
                        else if (heightValue < WorldGeneration.Instance.Water)
                        {
                            landTiles[x + _width * y] = Main.Instance.GetTileBase(TileType.Water);
                            waterTiles[x + _width * y] = Main.Instance.GetTileBase(TileType.Water);
                            tilegroupTiles[x + _width * y] = Main.Instance.GetTileBase(TileType.Heat);

                            ChunkData.GetValue(x, y).Collidable = false;
                        }
                        else if (heightValue < WorldGeneration.Instance.Sand)
                        {
                            landTiles[x + _width * y] = Main.Instance.GetTileBase(TileType.Sand);
                            waterTiles[x + _width * y] = null;
                            tilegroupTiles[x + _width * y] = Main.Instance.GetTileBase(TileType.Heat);

                            ChunkData.GetValue(x, y).Collidable = true;
                        }
                        else if (heightValue < WorldGeneration.Instance.Grass)
                        {
                            landTiles[x + _width * y] = Main.Instance.GetTileBase(TileType.DirtGrass);
                            waterTiles[x + _width * y] = null;
                            tilegroupTiles[x + _width * y] = Main.Instance.GetTileBase(TileType.Heat);

                            ChunkData.GetValue(x, y).Collidable = true;
                        }
                        else if (heightValue < WorldGeneration.Instance.Forest)
                        {
                            landTiles[x + _width * y] = Main.Instance.GetTileBase(TileType.ForestGrass);
                            waterTiles[x + _width * y] = null;
                            tilegroupTiles[x + _width * y] = Main.Instance.GetTileBase(TileType.Heat);

                            ChunkData.GetValue(x, y).Collidable = true;
                        }
                        else if (heightValue < WorldGeneration.Instance.Rock)
                        {
                            landTiles[x + _width * y] = Main.Instance.GetTileBase(TileType.Rock);
                            waterTiles[x + _width * y] = null;
                            tilegroupTiles[x + _width * y] = Main.Instance.GetTileBase(TileType.Heat);

                            ChunkData.GetValue(x, y).Collidable = true;
                        }
                        else if (heightValue < WorldGeneration.Instance.Snow)
                        {
                            landTiles[x + _width * y] = Main.Instance.GetTileBase(TileType.Snow);
                            waterTiles[x + _width * y] = null;
                            tilegroupTiles[x + _width * y] = Main.Instance.GetTileBase(TileType.Heat);

                            ChunkData.GetValue(x, y).Collidable = true;
                        }
                        else
                        {
                            landTiles[x + _width * y] = Main.Instance.GetTileBase(TileType.Snow);
                            waterTiles[x + _width * y] = null;
                            tilegroupTiles[x + _width * y] = Main.Instance.GetTileBase(TileType.Heat);

                            ChunkData.GetValue(x, y).Collidable = true;
                        }
                    }
                });
            });


            LandTileMap.SetTiles(positionArray, landTiles);
            //WaterTileMap.SetTiles(positionArray, waterTiles);
            TilegroupMap.SetTiles(positionArray, tilegroupTiles);
            ChunkHasDrawn = true;
        }


        public void PaintNeighborsColor()
        {
            //return;
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    Tile t = ChunkData.GetValue(x, y);
                    if (t.HasNeighbors())
                    {
                        LandTileMap.SetColor(new Vector3Int(t.FrameX, t.FrameY), Color.red);
                    }
                }
            }
        }

        public void PaintTilegroupMap()
        {
            foreach (var group in Waters)
            {
                foreach (var tile in group.Tiles)
                {
                    Vector3Int tileFrame = new Vector3Int(tile.FrameX, tile.FrameY, 0);
                    TilegroupMap.SetColor(tileFrame, Color.blue);
                }
            }

            foreach (var group in Lands)
            {
                foreach (var tile in group.Tiles)
                {
                    Vector3Int tileFrame = new Vector3Int(tile.FrameX, tile.FrameY, 0);
                    TilegroupMap.SetColor(tileFrame, Color.green);
                }
            }
        }

        public void DrawChunkPerformance(System.Action onFinished = null)
        {
            StartCoroutine(DrawChunkCoroutine(onFinished));
        }
        private IEnumerator DrawChunkCoroutine(System.Action onFinished)
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    float heightValue = ChunkData.GetValue(x, y).HeightValue;

                    if (heightValue < WorldGeneration.Instance.DeepWater)
                    {
                        LandTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.DeepWater));
                    }
                    else if (heightValue < WorldGeneration.Instance.Water)
                    {
                        LandTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Water));
                    }
                    else if (heightValue < WorldGeneration.Instance.Sand)
                    {
                        LandTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Sand));
                    }
                    else if (heightValue < WorldGeneration.Instance.Grass)
                    {
                        LandTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.DirtGrass));
                    }
                    else if (heightValue < WorldGeneration.Instance.Forest)
                    {
                        LandTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.ForestGrass));
                    }
                    else if (heightValue < WorldGeneration.Instance.Rock)
                    {
                        LandTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Rock));
                    }
                    else if (heightValue < WorldGeneration.Instance.Snow)
                    {
                        LandTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Snow));
                    }

                }

                if (PerformanceManager.Instance.HitFrameLimit())
                {
                    yield return null;
                }
            }
            ChunkHasDrawn = true;
            onFinished?.Invoke();
        }

        public void ClearChunkDraw()
        {
            LandTileMap.ClearAllTiles();
        }


        public void SetTwoSidesChunkNeighbors(Chunk left, Chunk right, Chunk top, Chunk bottom)
        {
            this.Left = left;
            this.Right = right;
            this.Top = top;
            this.Bottom = bottom;

            if (left != null)
            {
                left.Right = this;
            }
            if (right != null)
            {
                right.Left = this;
            }
            if (top != null)
            {
                top.Bottom = this;
            }
            if (bottom != null)
            {
                bottom.Top = this;
            }
            HasFourNeighbors = Left != null && Right != null && Top != null && Bottom != null;
        }
        public bool HasNeighbors()
        {
            HasFourNeighbors = Left != null && Right != null && Top != null && Bottom != null;
            return Left != null && Right != null && Top != null && Bottom != null;
        }


        public void UpdateAllTileNeighbors()
        {
            Debug.Log("UpdateAllTileNeighbors");
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    Tile t = ChunkData.GetValue(x, y);

                    t.Top = GetTop(t);
                    t.Bottom = GetBottom(t);
                    t.Left = GetLeft(t);
                    t.Right = GetRight(t);
                }
            }
        }
        public void UpdateEdgeOfChunkTileNeighbors()
        {
            Debug.Log("UpdateEdgeTileNeighbors");

            // Loop through the first and last rows
            for (int col = 0; col < _width; col++)
            {
                Tile t1 = ChunkData.GetValue(0, col);
                t1.Top = GetTop(t1);
                t1.Bottom = GetBottom(t1);
                t1.Left = GetLeft(t1);
                t1.Right = GetRight(t1);

                Tile t2 = ChunkData.GetValue(_height - 1, col);
                t2.Top = GetTop(t2);
                t2.Bottom = GetBottom(t2);
                t2.Left = GetLeft(t2);
                t2.Right = GetRight(t2);
            }

            // Loop through the first and last columns (excluding corners)
            for (int row = 1; row < _height - 1; row++)
            {
                Tile t1 = ChunkData.GetValue(row, 0);
                t1.Top = GetTop(t1);
                t1.Bottom = GetBottom(t1);
                t1.Left = GetLeft(t1);
                t1.Right = GetRight(t1);

                Tile t2 = ChunkData.GetValue(row, _width - 1);
                t2.Top = GetTop(t2);
                t2.Bottom = GetBottom(t2);
                t2.Left = GetLeft(t2);
                t2.Right = GetRight(t2);
            }
        }


        #region Find Tile Neighbors
        public Tile GetTop(Tile t)
        {
            int newY = t.FrameY + 1;

            if (newY >= 0 && newY < _height)
            {
                return ChunkData.GetValue(t.FrameX, newY);
            }
            else if (newY == _height)
            {
                if (Top != null)
                {
                    return Top.ChunkData.GetValue(t.FrameX, 0);
                }
            }
            return null; // Handle the case where the index is out of range.   
        }
        public Tile GetBottom(Tile t)
        {
            int newY = t.FrameY - 1;
            if (newY >= 0 && newY < _height)
            {
                return ChunkData.GetValue(t.FrameX, newY);
            }
            else if (newY == -1)
            {
                if (Bottom != null)
                {
                    return Bottom.ChunkData.GetValue(t.FrameX, _height - 1);
                }
            }
            return null; // Handle the case where the index is out of range.
        }
        public Tile GetLeft(Tile t)
        {
            int newX = t.FrameX - 1;
            if (newX >= 0 && newX < _width)
            {
                return ChunkData.GetValue(newX, t.FrameY);
            }
            else if (newX == -1)
            {
                if (Left != null)
                {
                    return Left.ChunkData.GetValue(_width - 1, t.FrameY);
                }
            }
            return null; // Handle the case where the index is out of range.
        }
        public Tile GetRight(Tile t)
        {
            int newX = t.FrameX + 1;
            if (newX >= 0 && newX < _width)
            {
                return ChunkData.GetValue(newX, t.FrameY);
            }
            else if (newX == _width)
            {
                if (Right != null)
                {
                    return Right.ChunkData.GetValue(0, t.FrameY);
                }
            }
            return null; // Handle the case where the index is out of range.
        }


        public Tile GetTopWithinChunk(Tile t)
        {
            int newY = t.FrameY + 1;

            if (newY >= 0 && newY < _height)
            {
                return ChunkData.GetValue(t.FrameX, newY);
            }
            return null; // Handle the case where the index is out of range.   
        }
        public Tile GetBottomWithinChunk(Tile t)
        {
            int newY = t.FrameY - 1;
            if (newY >= 0 && newY < _height)
            {
                return ChunkData.GetValue(t.FrameX, newY);
            }
            return null; // Handle the case where the index is out of range.
        }
        public Tile GetLeftWithinChunk(Tile t)
        {
            int newX = t.FrameX - 1;
            if (newX >= 0 && newX < _width)
            {
                return ChunkData.GetValue(newX, t.FrameY);
            }
            return null; // Handle the case where the index is out of range.
        }
        public Tile GetRightWithinChunk(Tile t)
        {
            int newX = t.FrameX + 1;
            if (newX >= 0 && newX < _width)
            {
                return ChunkData.GetValue(newX, t.FrameY);
            }
            return null; // Handle the case where the index is out of range.
        }
        #endregion

        public void SetTile(Vector2 worldPosition, TileBase tileBase)
        {
            Vector3Int cellPosition = Grid.WorldToCell(worldPosition);
            cellPosition.z = 0;
            LandTileMap.SetTile(cellPosition, tileBase);
        }

        public void SetDrawRenderMode(TilemapRenderer.Mode mode)
        {
            LandTileMap.GetComponent<TilemapRenderer>().mode = mode;
        }



        #region Utilities
        // Function to convert world coordinates to tile coordinates
        public Vector3Int WorldToTilePosition(Vector3 worldPosition)
        {
            Vector3Int tilePosition = LandTileMap.WorldToCell(worldPosition);
            return tilePosition;
        }

        // Function to convert tile coordinates to world coordinates
        public Vector3 TileToWorldPosition(Vector3Int tilePosition)
        {
            Vector3 worldPosition = LandTileMap.GetCellCenterWorld(tilePosition);
            return worldPosition;
        }
        #endregion
    }
}

