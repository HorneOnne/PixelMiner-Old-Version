using UnityEngine;
using CoreMiner.Utilities;
using UnityEngine.Tilemaps;
using LibNoise;
using CoreMiner.Utilities.NoiseGeneration;
using LibNoise.Generator;
using System.Threading.Tasks;

namespace CoreMiner.WorldGen
{
    public class Chunk : MonoBehaviour
    {
        public Vector3Int ChunkPosition;
        public Vector3Int WorldPosition;

        public Grid<CoreMiner.WorldGen.Tile> ChunkData;
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private int _cellSize;

        public Tilemap TileMap;
        public CustomTileBase GroundTile;

        [Header("Noise Settings")]
        public int Octaves = 6;
        public double Frequency = 0.02f;
        public double Lacunarity = 2.0f;
        public double Persistence = 0.5f;
        public int Seed = 7;
        private ModuleBase _heightNoise;

        private float _minHeight = float.MaxValue;
        private float _maxHeight = float.MinValue;


        [Header("Debug")]
        public bool ShowMinMax = true;

        private async void Start()
        {
            Init(_width, _height, _cellSize, Vector3.zero);
            //LoadHeightMap();
            await LoadHeightMapAsync();
            LoadTiles();


            // Debug Log
            ShowMinMaxHeightMapLog();
        }

        public void Init(int _width, int height, float cellSize, Vector3 offset)
        {
            this._width = _width;
            this._height = height;
            ChunkData = new Utilities.Grid<CoreMiner.WorldGen.Tile>(_width, height, cellSize, offset);
        }


        private void LoadHeightMap()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            _heightNoise = new Perlin(Frequency, Lacunarity, Persistence, Octaves, Seed, QualityMode.High);

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    float value = (float)_heightNoise.GetValue(x, y, 0);

                    if (value > _maxHeight) _maxHeight = value;
                    if (value < _minHeight) _minHeight = value;

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
                    value = (value - _minHeight) / (_maxHeight - _minHeight);

                    Tile t = new Tile(x,y);           
                    t.HeightValue = value;
                    ChunkData.SetValue(x, y, t);                  
                }
            }

            stopwatch.Stop();
            Debug.Log($"Load Chunk Data: {stopwatch.ElapsedMilliseconds / 1000f} s");
        }

        private async Task LoadHeightMapAsync()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            _heightNoise = new Perlin(Frequency, Lacunarity, Persistence, Octaves, Seed, QualityMode.High);


            await Task.Run(() =>
            {
                Parallel.For(0, _width, x =>
                {
                    for (int y = 0; y < _height; y++)
                    {
                        float value = (float)_heightNoise.GetValue(x, y, 0);
                        // Use a lock to sately update minHeight and maxHeight
                        lock (this)
                        {
                            if (value > _maxHeight) _maxHeight = value;
                            if (value < _minHeight) _minHeight = value;
                        }

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
                        value = (value - _minHeight) / (_maxHeight - _minHeight);

                        Tile t = new Tile(x, y);
                        t.HeightValue = value;
                        ChunkData.SetValue(x, y, t);              
                    }
                });

            });

            stopwatch.Stop();
            Debug.Log($"Load Chunk Data: {stopwatch.ElapsedMilliseconds / 1000f} s");
        }

        private void LoadTiles()
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
                        TileMap.SetTile(new Vector3Int(x, y, 0), GroundTile);
                    }
                }
            }
            stopwatch.Stop();
            Debug.Log($"Set tiles: {stopwatch.ElapsedMilliseconds / 1000f} s");
        }




        #region Debug Logs
        private void ShowMinMaxHeightMapLog()
        {
            if (ShowMinMax)
            {
                Debug.Log($"Min: {_minHeight}");
                Debug.Log($"Max: {_maxHeight}");
            }
        }
        #endregion
    }
}

