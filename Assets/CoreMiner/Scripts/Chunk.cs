using UnityEngine;
using CoreMiner.Utilities;
using UnityEngine.Tilemaps;
using LibNoise;
using CoreMiner.Utilities.NoiseGeneration;
using LibNoise.Generator;
using System.Threading.Tasks;

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

        // Min and Max Height used for normalize noise value in range [0-1]
        private float _minHeight = float.MaxValue;
        private float _maxHeight = float.MinValue;

        [Header("Tilemap visualization")]
        public Tilemap TileMap;

        [Header("Debug")]
        public bool ShowMinMax = true;



        private async void Start()
        {
            return;
            //Init(FrameX, FrameY, _width, _height, _cellSize);


            //LoadHeightMap();
            //await LoadHeightMapAsync();
            LoadTiles();


            // Debug Log
            ShowMinMaxHeightMapLog();
        }


        private void Update()
        {
            if(Vector2.Distance(Camera.main.transform.position, transform.position) > 200f)
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
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();


            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    float value = heightValues[x, y];
                    //if (value > _maxHeight) _maxHeight = value;
                    //if (value < _minHeight) _minHeight = value;

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

            stopwatch.Stop();
            //Debug.Log($"Load Chunk Data: {stopwatch.ElapsedMilliseconds / 1000f} s");
        }


        public void LoadMap()
        {
            //await LoadHeightMapAsync();
            //LoadHeightMap();
            LoadTiles();
        }
        //private async Task LoadHeightMapAsync()
        //{
        //    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        //    stopwatch.Start();
        //    _heightNoise = new Perlin(Frequency, Lacunarity, Persistence, Octaves, Seed, QualityMode.High);


        //    await Task.Run(() =>
        //    {
        //        Parallel.For(0, _width, x =>
        //        {
        //            for (int y = 0; y < _height; y++)
        //            {
        //                float offsetX = x + FrameX * _width;
        //                float offsetY = y + FrameY * _height;
        //                //Debug.Log($"offset: {offsetX}\t{offsetY}\t{FrameX}\t{FrameY}");
        //                float value = (float)_heightNoise.GetValue(-offsetX, offsetY, 0);
        //                // Use a lock to sately update minHeight and maxHeight
        //                lock (this)
        //                {
        //                    if (value > _maxHeight) _maxHeight = value;
        //                    if (value < _minHeight) _minHeight = value;
        //                }

        //                Tile tile = new Tile(x, y);
        //                tile.HeightValue = value;
        //                ChunkData.SetValue(x, y, tile);
        //            }
        //        });

        //    });


        //    // Normalize the height values
        //    await Task.Run(() =>
        //    {
        //        Parallel.For(0, _width, x =>
        //        {
        //            for (int y = 0; y < _height; y++)
        //            {
        //                float value = ChunkData.GetValue(x, y).HeightValue;
        //                //normalize our value between 0 and 1
        //                value = (value - _minHeight) / (_maxHeight - _minHeight);

        //                Tile t = new Tile(x, y);
        //                t.HeightValue = value;
        //                ChunkData.SetValue(x, y, t);
        //            }
        //        });

        //    });

        //    stopwatch.Stop();
        //    //Debug.Log($"Load Chunk Data: {stopwatch.ElapsedMilliseconds / 1000f} s");
        //}

        public void LoadTiles()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    float heightValue = ChunkData.GetValue(x, y).HeightValue;

                    if (heightValue < 0.5f)
                    {
                        TileMap.SetTile(new Vector3Int(x, y, 0), Main.Instance.GetTileBase(TileType.DirtGrass));
                        if (x == 0 && y == 0)
                        {
                            TileMap.SetColor(new Vector3Int(x, y, 0), Color.red);
                        }

                        if (x == _width - 1 && y == _height - 1)
                        {
                            TileMap.SetColor(new Vector3Int(x, y, 0), Color.blue);
                        }
                    }
                }
            }
            stopwatch.Stop();
            //Debug.Log($"Set tiles: {stopwatch.ElapsedMilliseconds / 1000f} s");
        }




        #region Debug Logs
        private void ShowMinMaxHeightMapLog()
        {
            if (ShowMinMax)
            {
                //Debug.Log($"Min: {_minHeight}");
                //Debug.Log($"Max: {_maxHeight}");
            }
        }
        #endregion
    }
}

