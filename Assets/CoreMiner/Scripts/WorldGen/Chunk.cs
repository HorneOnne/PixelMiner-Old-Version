using UnityEngine;
using CoreMiner.Utilities;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Unity.Mathematics;

namespace CoreMiner
{
    public class Chunk : MonoBehaviour
    {
        [Header("Chunk Settings")]
        public CoreMiner.Utilities.Grid<Tile> ChunkData;
        public float FrameX;    // Used for calculate world position
        public float FrameY;    // Used for calculate world position
        public int IsometricFrameX; // Used for calculate world generation (coherent noise)
        public int IsometricFrameY; // Used for calculate world generation (coherent noise)
        [SerializeField] private byte _width;
        [SerializeField] private byte _height;
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
        public Tilemap LandTilemap;
        public Tilemap WaterTilemap;
        public Tilemap Tilegroupmap;
        public Tilemap HeatTilemap;
        public Tilemap MoistureTilemap;
        public Grid Grid;

        public bool HasChunkNeighbors = false;
        public bool AllTileHasNeighbors = false;
        public bool ChunkHasDrawn;
        public bool Processing { get; private set; } = false;


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
                    && WorldGeneration.Instance.AutoUnloadChunk
                    && !Processing)
                {
                    WorldGeneration.Instance.ActiveChunks.Remove(this);
                    gameObject.SetActive(false);
                }
            }
        }

        public async void Init(float frameX, float frameY, int isometricFrameX, int isometricFrameY, byte _width, byte height)
        {
            this.FrameX = frameX;
            this.FrameY = frameY;
            this.IsometricFrameX = isometricFrameX;
            this.IsometricFrameY = isometricFrameY;
            this._width = _width;
            this._height = height;
            ChunkData = new Grid<Tile>(_width, height, 1.0f);
            await InitEmptyTilesDataAsync();
        }

        private async Task InitEmptyTilesDataAsync()
        {
            Processing = true;
            await Task.Run(() =>
            {
                for (byte x = 0; x < _width; x++)
                {
                    for (byte y = 0; y < _height; y++)
                    {
                        Tile tile = new Tile(x, y);
                        ChunkData.SetValue(x, y, tile);
                    }
                }
            });
            Processing = false;
        }

        public void LoadChunk()
        {
            gameObject.SetActive(true);
            if (WorldGeneration.Instance.ShowTilegroupMaps)
            {
                Tilegroupmap.GetComponent<TilemapRenderer>().enabled = true;
            }
            else
            {
                Tilegroupmap.GetComponent<TilemapRenderer>().enabled = false;
            }
        }

        public void UnloadChunk()
        {
            gameObject.SetActive(false);
        }



        #region Load chunk tile data
        public async Task LoadHeightMapDataAsync(float[,] heightValues, bool updateHeightType = true)
        {
            Processing = true;
            await Task.Run(() =>
            {
                Parallel.For(0, _width, x =>
                {
                    for (int y = 0; y < _height; y++)
                    {
                        Tile tile = ChunkData.GetValue(x, y);
                        tile.HeightValue = heightValues[x, y];

                        if(updateHeightType)
                        {
                            UpdateHeightType(tile);
                        }                 
                    }
                });

            });
            Processing = false;
        }
        public async Task LoadHeatMapDataAsync(float[,] heatValues, bool updateHeatType = true)
        {
            Processing = true;
            await Task.Run(() =>
            {
                Parallel.For(0, _width, x =>
                {
                    for (int y = 0; y < _height; y++)
                    {
                        Tile tile = ChunkData.GetValue(x, y);
                        tile.HeatValue = heatValues[x, y];

                        if(updateHeatType)
                        {
                            UpdateHeatType(tile);
                        }    
                    }
                });
            });
            Processing = false;
        }

        /// <summary>
        /// Load heightmap and heatmap simulteneously and
        /// ajdust heatmap based on heightmap.(The higher you go, the lower the temperature)
        /// </summary>
        /// <param name="heightValues"></param>
        /// <returns></returns>
        public async Task LoadHeightAndHeatMap(float[,] heightValues, float[,] heatValues)
        {
            Processing = true;
            Task loadHeightMapDataTask = LoadHeightMapDataAsync(heightValues, updateHeightType: true);
            Task loadHeatMapDataTask = LoadHeatMapDataAsync(heatValues, updateHeatType: false); // updateHeatType = false because we will update it after adjust by heightmap.

            await Task.WhenAll(loadHeatMapDataTask, loadHeightMapDataTask);

            await Task.Run(() =>
            {
                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        Tile tile = ChunkData.GetValue(x, y);


                        // Ajdust Heat map based on height.
                        if (tile.HeightType == HeightType.Forest)
                        {
                            tile.HeatValue -= 0.1f * tile.HeightValue;
                        }
                        else if (tile.HeightType == HeightType.Rock)
                        {
                            tile.HeatValue -= 0.25f * tile.HeightValue;
                        }
                        else if (tile.HeightType == HeightType.Snow)
                        {
                            tile.HeatValue -= 0.4f * tile.HeightValue;
                        }
                        else
                        {
                            tile.HeatValue += 0.01f * tile.HeightValue;
                        }

                        UpdateHeatType(tile);
                    }
                }
            });
            Processing = false;
        }

        public async Task LoadMoistureMapDataAsync(float[,] moisetureValues, bool updateMoistureType = true)
        {
            Processing = true;
            await Task.Run(() =>
            {
                Parallel.For(0, _width, x =>
                {
                    for (int y = 0; y < _height; y++)
                    {
                        Tile tile = ChunkData.GetValue(x, y);
                        tile.MoistureValue = moisetureValues[x, y];


                        // Ajdust moisture based on height
                        if (tile.HeightType == HeightType.DeepWater)
                        {
                            tile.MoistureValue += 8f * tile.HeightValue;
                        }
                        else if (tile.HeightType == HeightType.ShallowWater)
                        {
                            tile.MoistureValue += 3f * tile.HeightValue;
                        }
                        else if (tile.HeightType == HeightType.Shore)
                        {
                            tile.MoistureValue += 1f * tile.HeightValue;
                        }
                        else if (tile.HeightType == HeightType.Sand)
                        {
                            tile.MoistureValue += 0.2f * tile.HeightValue;
                        }


                        if (updateMoistureType)
                        {
                            UpdateMoistureType(tile);
                        }
                    }
                });

            });
            Processing = false;
        }
        #endregion



        #region Set chunk tile type
        private void UpdateHeightType(Tile tile)
        {

            if (tile.HeightValue < WorldGeneration.Instance.DeepWater)
            {
                tile.HeightType = HeightType.DeepWater;
                tile.Collidable = false;
            }
            else if (tile.HeightValue < WorldGeneration.Instance.Water)
            {
                tile.HeightType = HeightType.ShallowWater;
                tile.Collidable = false;
            }
            else if (tile.HeightValue < WorldGeneration.Instance.Sand)
            {
                tile.HeightType = HeightType.Sand;
                tile.Collidable = true;
            }
            else if (tile.HeightValue < WorldGeneration.Instance.Grass)
            {
                tile.HeightType = HeightType.Grass;
                tile.Collidable = true;
            }
            else if (tile.HeightValue < WorldGeneration.Instance.Forest)
            {
                tile.HeightType = HeightType.Forest;
                tile.Collidable = true;
            }
            else if (tile.HeightValue < WorldGeneration.Instance.Rock)
            {
                tile.HeightType = HeightType.Rock;
                tile.Collidable = true;
            }
            else
            {
                tile.HeightType = HeightType.Snow;
                tile.Collidable = true;
            }
        }
        private void UpdateHeatType(Tile tile)
        {
            // Adjust heat type when heat value has changed.
            if (tile.HeatValue < WorldGeneration.Instance.ColdestValue)
            {
                tile.HeatType = HeatType.Coldest;
            }
            else if (tile.HeatValue < WorldGeneration.Instance.ColderValue)
            {
                tile.HeatType = HeatType.Colder;
            }
            else if (tile.HeatValue < WorldGeneration.Instance.ColdValue)
            {
                tile.HeatType = HeatType.Cold;
            }
            else if (tile.HeatValue < WorldGeneration.Instance.WarmValue)
            {
                tile.HeatType = HeatType.Warm;
            }
            else if (tile.HeatValue < WorldGeneration.Instance.WarmerValue)
            {
                tile.HeatType = HeatType.Warmer;
            }
            else
            {
                tile.HeatType = HeatType.Warmest;
            }
        }
        private void UpdateMoistureType(Tile tile)
        {
            if (tile.MoistureValue < WorldGeneration.Instance.DryestValue)
            {
                tile.MoistureType = MoistureType.Dryest;
            }
            else if(tile.MoistureValue < WorldGeneration.Instance.DryerValue)
            {
                tile.MoistureType = MoistureType.Dryer;
            }
            else if (tile.MoistureValue < WorldGeneration.Instance.DryValue)
            {
                tile.MoistureType = MoistureType.Dry;
            }
            else if (tile.MoistureValue < WorldGeneration.Instance.WetValue)
            {
                tile.MoistureType = MoistureType.Wet;
            }
            else if (tile.MoistureValue < WorldGeneration.Instance.WetterValue)
            {
                tile.MoistureType = MoistureType.Wetter;
            }
            else 
            {
                tile.MoistureType = MoistureType.Wettest;
            }
        }
        #endregion



        public async Task DrawChunkAsync()
        {
            Processing = true;

            TileBase[] landTiles = new TileBase[_width * _height];
            TileBase[] waterTiles = new TileBase[_width * _height];
            TileBase[] tilegroupTiles = new TileBase[_width * _height];
            TileBase[] heatTiles = new TileBase[_width * _height];
            TileBase[] moistureTiles = new TileBase[_width * _height];

            Vector3Int[] positionArray = new Vector3Int[_width * _height];

            await Task.Run(() =>
            {
                Parallel.For(0, _width, x =>
                {
                    for (int y = 0; y < _height; y++)
                    {
                        int index = x + y * _width;
                        positionArray[index] = new Vector3Int(x, y, 0);

                        HeightType heightType = ChunkData.GetValue(x, y).HeightType;
                        HeatType heatType = ChunkData.GetValue(x, y).HeatType;
                        MoistureType moistureType = ChunkData.GetValue(x, y).MoistureType;

                       
                        // Height
                        switch(heightType)
                        {
                            case HeightType.DeepWater:
                                landTiles[index] = Main.Instance.GetTileBase(TileType.Water);
                                waterTiles[index] = Main.Instance.GetTileBase(TileType.Water);
                                tilegroupTiles[index] = Main.Instance.GetTileBase(TileType.Color);
                                break;
                            case HeightType.ShallowWater:
                                landTiles[index] = Main.Instance.GetTileBase(TileType.Water);
                                waterTiles[index] = Main.Instance.GetTileBase(TileType.Water);
                                tilegroupTiles[index] = Main.Instance.GetTileBase(TileType.Color);
                                break;
                            case HeightType.Sand:
                                landTiles[index] = Main.Instance.GetTileBase(TileType.Sand);
                                waterTiles[index] = null;
                                tilegroupTiles[index] = Main.Instance.GetTileBase(TileType.Color);
                                break;
                            case HeightType.Grass:
                                landTiles[index] = Main.Instance.GetTileBase(TileType.DirtGrass);
                                waterTiles[index] = null;
                                tilegroupTiles[index] = Main.Instance.GetTileBase(TileType.Color);
                                break;
                            case HeightType.Forest:
                                landTiles[index] = Main.Instance.GetTileBase(TileType.ForestGrass);
                                waterTiles[index] = null;
                                tilegroupTiles[index] = Main.Instance.GetTileBase(TileType.Color);
                                break;
                            case HeightType.Rock:
                                landTiles[index] = Main.Instance.GetTileBase(TileType.Rock);
                                waterTiles[index] = null;
                                tilegroupTiles[index] = Main.Instance.GetTileBase(TileType.Color);
                                break;
                            case HeightType.Snow:
                                landTiles[index] = Main.Instance.GetTileBase(TileType.Snow);
                                waterTiles[index] = null;
                                tilegroupTiles[index] = Main.Instance.GetTileBase(TileType.Color);
                                break;
                            default:
                                landTiles[index] = Main.Instance.GetTileBase(TileType.Snow);
                                waterTiles[index] = null;
                                tilegroupTiles[index] = Main.Instance.GetTileBase(TileType.Color);
                                break;
                        }
               


                        // Heat
                        switch(heatType)
                        {
                            case HeatType.Coldest:
                                heatTiles[index] = Main.Instance.GetTileBase(TileType.Color);
                                break;
                            case HeatType.Colder:
                                heatTiles[index] = Main.Instance.GetTileBase(TileType.Color);
                                break;
                            case HeatType.Cold:
                                heatTiles[index] = Main.Instance.GetTileBase(TileType.Color);
                                break;
                            case HeatType.Warm:
                                heatTiles[index] = Main.Instance.GetTileBase(TileType.Color);
                                break;
                            case HeatType.Warmer:
                                heatTiles[index] = Main.Instance.GetTileBase(TileType.Color);
                                break;
                            case HeatType.Warmest:
                                heatTiles[index] = Main.Instance.GetTileBase(TileType.Color);
                                break;
                        }

                        // Moisture
                        switch (moistureType)
                        {
                            case MoistureType.Dryest:
                                moistureTiles[index] = Main.Instance.GetTileBase(TileType.Color);
                                break;
                            case MoistureType.Dryer:
                                moistureTiles[index] = Main.Instance.GetTileBase(TileType.Color);
                                break;
                            case MoistureType.Dry:
                                moistureTiles[index] = Main.Instance.GetTileBase(TileType.Color);
                                break;
                            case MoistureType.Wet:
                                moistureTiles[index] = Main.Instance.GetTileBase(TileType.Color);
                                break;
                            case MoistureType.Wetter:
                                moistureTiles[index] = Main.Instance.GetTileBase(TileType.Color);
                                break;
                            case MoistureType.Wettest:
                                moistureTiles[index] = Main.Instance.GetTileBase(TileType.Color);
                                break;
                            default:
                                moistureTiles[index] = Main.Instance.GetTileBase(TileType.Color);
                                break;
                        }
                    }
                });
            });


          
            LandTilemap.SetTiles(positionArray, landTiles);
            //WaterTileMap.SetTiles(positionArray, waterTiles);
            //Tilegroupmap.SetTiles(positionArray, tilegroupTiles);

            if(WorldGeneration.Instance.InitWorldWithHeatmap)
            {
                HeatTilemap.SetTiles(positionArray, heatTiles);
                HeatTilemap.gameObject.SetActive(true);
            }
            else
            {
                HeatTilemap.gameObject.SetActive(false);
            }

            if(WorldGeneration.Instance.InitWorldWithMoisturemap)
            {
                MoistureTilemap.SetTiles(positionArray, moistureTiles);
                MoistureTilemap.gameObject.SetActive(true);
            }
            else
            {
                MoistureTilemap.gameObject.SetActive(false);
            }
            
            ChunkHasDrawn = true;
            Processing = false;
        }
        public void ClearChunkDraw()
        {
            LandTilemap.ClearAllTiles();
        }



        #region Paint tilemap color
        public void PaintNeighborsColor()
        {
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    Tile t = ChunkData.GetValue(x, y);
                    if (t.HasNeighbors())
                    {
                        LandTilemap.SetColor(new Vector3Int(t.FrameX, t.FrameY), Color.red);
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
                    Tilegroupmap.SetColor(tileFrame, Color.blue);
                }
            }

            foreach (var group in Lands)
            {
                foreach (var tile in group.Tiles)
                {
                    Vector3Int tileFrame = new Vector3Int(tile.FrameX, tile.FrameY, 0);
                    Tilegroupmap.SetColor(tileFrame, Color.green);
                }
            }
        }

        public void PaintHeatMap()
        {
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    Tile t = ChunkData.GetValue(x, y);
                    HeatTilemap.SetColor(new Vector3Int(t.FrameX, t.FrameY), WorldGenUtilities.GetGradientColor(t.HeatValue));
                }
            }
        }

        public void PaintMoistureMap()
        {
            for(int x = 0; x < _width; x++)
            {
                for(int y = 0; y < _height; y++)
                {
                    Tile t = ChunkData.GetValue(x, y);
                    Color color;
                    switch(t.MoistureType)
                    {
                        case MoistureType.Dryest:
                            color = WorldGenUtilities.Dryest;
                            break;
                        case MoistureType.Dryer:
                            color = WorldGenUtilities.Dryer;
                            break;
                        case MoistureType.Dry:
                            color = WorldGenUtilities.Dry;
                            break;
                        case MoistureType.Wet:
                            color = WorldGenUtilities.Wet;
                            break;
                        case MoistureType.Wetter:
                            color = WorldGenUtilities.Wetter;
                            break;
                        case MoistureType.Wettest:
                            color = WorldGenUtilities.Wettest;
                            break;
                        default:
                            color = WorldGenUtilities.Wettest;
                            break;
                    }

                    MoistureTilemap.SetColor(new Vector3Int(t.FrameX, t.FrameY), color);
                }
            }
        }
        #endregion


        


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
            HasChunkNeighbors = Left != null && Right != null && Top != null && Bottom != null;
        }
        public bool HasNeighbors()
        {
            HasChunkNeighbors = Left != null && Right != null && Top != null && Bottom != null;
            return HasChunkNeighbors;
        }
        public void UpdateAllTileNeighbors()
        {
            Debug.Log("UpdateAllTileNeighbors");
            AllTileHasNeighbors = true;
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    Tile t = ChunkData.GetValue(x, y);

                    t.Top = GetTop(t);
                    t.Bottom = GetBottom(t);
                    t.Left = GetLeft(t);
                    t.Right = GetRight(t);

                    if (t.HasNeighbors() == false)
                        AllTileHasNeighbors = false;
                }
            }
        }
        public async Task UpdateAllTileNeighborsAsync()
        {
            Processing = true;
            Debug.Log("UpdateAllTileNeighborsAsync");
            AllTileHasNeighbors = true;
            await Task.Run(() =>
            {
                for (var x = 0; x < _width; x++)
                {
                    for (var y = 0; y < _height; y++)
                    {
                        Tile t = ChunkData.GetValue(x, y);

                        t.Top = GetTop(t);
                        t.Bottom = GetBottom(t);
                        t.Left = GetLeft(t);
                        t.Right = GetRight(t);

                        if (t.HasNeighbors() == false)
                            AllTileHasNeighbors = false;
                    }
                }
            });
            Processing = false;
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
            LandTilemap.SetTile(cellPosition, tileBase);
        }

        public void SetDrawRenderMode(TilemapRenderer.Mode mode)
        {
            LandTilemap.GetComponent<TilemapRenderer>().mode = mode;
        }


        #region Obsolete method.
        /// <summary>
        /// Obsolete.
        /// </summary>
        public void DrawChunk()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    float heightValue = ChunkData.GetValue(x, y).HeightValue;

                    if (heightValue < WorldGeneration.Instance.DeepWater)
                    {
                        LandTilemap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.DeepWater));
                        WaterTilemap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Water));
                    }
                    else if (heightValue < WorldGeneration.Instance.Water)
                    {
                        //LandTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Water));
                        WaterTilemap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Water));
                    }
                    else if (heightValue < WorldGeneration.Instance.Sand)
                    {
                        LandTilemap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Sand));
                    }
                    else if (heightValue < WorldGeneration.Instance.Grass)
                    {
                        LandTilemap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.DirtGrass));
                    }
                    else if (heightValue < WorldGeneration.Instance.Forest)
                    {
                        //LandTileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Water));
                        WaterTilemap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.ForestGrass));
                    }
                    else if (heightValue < WorldGeneration.Instance.Rock)
                    {
                        LandTilemap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Rock));
                    }
                    else if (heightValue < WorldGeneration.Instance.Snow)
                    {
                        LandTilemap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Snow));
                    }
                    else
                    {
                        LandTilemap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Snow));
                    }
                }
            }
            ChunkHasDrawn = true;
        }

        /// <summary>
        /// Obsolete, use LoadHeightMapAsync instead.
        /// </summary>
        /// <param name="heightValues"></param>
        public void LoadHeightMap(float[,] heightValues)
        {
            for (byte x = 0; x < _width; x++)
            {
                for (byte y = 0; y < _height; y++)
                {
                    Tile tile = new Tile(x, y);
                    tile.HeightValue = heightValues[x, y];
                    ChunkData.SetValue(x, y, tile);
                }
            }
        }

        /// <summary>
        /// Obsolete.
        /// </summary>
        /// <param name="onFinished"></param>
        public void DrawChunkPerformance(System.Action onFinished = null)
        {
            StartCoroutine(DrawChunkCoroutine(onFinished));
        }

        /// <summary>
        /// Obsolete.
        /// </summary>
        /// <param name="onFinished"></param>
        /// <returns></returns>
        private IEnumerator DrawChunkCoroutine(System.Action onFinished)
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    float heightValue = ChunkData.GetValue(x, y).HeightValue;

                    if (heightValue < WorldGeneration.Instance.DeepWater)
                    {
                        LandTilemap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.DeepWater));
                    }
                    else if (heightValue < WorldGeneration.Instance.Water)
                    {
                        LandTilemap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Water));
                    }
                    else if (heightValue < WorldGeneration.Instance.Sand)
                    {
                        LandTilemap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Sand));
                    }
                    else if (heightValue < WorldGeneration.Instance.Grass)
                    {
                        LandTilemap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.DirtGrass));
                    }
                    else if (heightValue < WorldGeneration.Instance.Forest)
                    {
                        LandTilemap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.ForestGrass));
                    }
                    else if (heightValue < WorldGeneration.Instance.Rock)
                    {
                        LandTilemap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Rock));
                    }
                    else if (heightValue < WorldGeneration.Instance.Snow)
                    {
                        LandTilemap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.Snow));
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
        #endregion



        #region Utilities
        // Function to convert world coordinates to tile coordinates
        public Vector3Int WorldToTilePosition(Vector3 worldPosition)
        {
            Vector3Int tilePosition = LandTilemap.WorldToCell(worldPosition);
            return tilePosition;
        }

        // Function to convert tile coordinates to world coordinates
        public Vector3 TileToWorldPosition(Vector3Int tilePosition)
        {
            Vector3 worldPosition = LandTilemap.GetCellCenterWorld(tilePosition);
            return worldPosition;
        }
        #endregion
    }
}

