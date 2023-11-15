using System.Collections.Generic;
using UnityEngine;
using LibNoise;
using LibNoise.Generator;
using System.Linq;
using System.Threading.Tasks;
using CoreMiner.UI;
using System.Collections;
using CoreMiner.Utilities;
using System.Threading;
using LibNoise.Operator;

namespace CoreMiner
{
    public class WorldGeneration : MonoBehaviour
    {
        private readonly object lockObject = new object(); // Define a lock object for thread safety
        public static WorldGeneration Instance { get; private set; }


        #region Fileds and Variables
        [SerializeField] private Chunk _chunkPrefab;


        [Header("World Settings")]
        public int ChunkWidth = 16;      // Size of each chunk in tiles
        public int ChunkHeight = 16;      // Size of each chunk in tiles
        [Space(5)]
        public int InitWorldWidth = 3;
        public int InitWorldHeight = 3;
        public int LoadChunkOffsetWidth = 1;
        public int LoadChunkOffsetHeight = 1;
        // Compute noise range sample count.
        private int _calculateNoiseRangeSampleMultiplier = 15;  // 50 times. 50 * 100000 = 5 mil times.
        private int _calculateNoiseRangeCount = 1000000;    // 1 mil times.


        // Min and Max Height used for normalize noise value in range [0-1]
        public float MinWorldNoiseValue { get; private set; } = float.MaxValue;
        public float MaxWorldNoiseValue { get; private set; } = float.MinValue;


        [Header("Noise Settings")]
        public int Octaves = 6;
        public double Frequency = 0.02f;
        public double Lacunarity = 2.0f;
        public double Persistence = 0.5f;
        public int Seed = 7;
        private ModuleBase _heightModule;


        [Header("Height Threshold")]
        public float DeepWater = 0.2f;
        public float Water = 0.4f;
        public float Sand = 0.5f;
        public float Grass = 0.7f;
        public float Forest = 0.8f;
        public float Rock = 0.9f;
        public float Snow = 1;


        [Header("Heat Map")]
        [Range(0f, 1f)]
        public float HeatMapBlendFactor = 0.5f;
        private float _gradientHeatmapSize = 256;
        private ModuleBase _heatModule;
        // Heatmap Gradient
        public float ColdestValue = 0.1f;
        public float ColderValue = 0.2f;
        public float ColdValue = 0.4f;
        public float WarmValue = 0.6f;
        public float WarmerValue = 0.8f;
        // Heatmap fratal
        public int HeatOctaves = 4;
        public double HeatFrequency = 0.02;
        public double HeatLacunarity = 2.0f;
        public double HeatPersistence = 0.5f;



        [Header("Moisture Map")]
        public int MoistureOctaves = 4;
        public double MoistureFrequency = 0.03;
        public double MoistureLacunarity = 2.0f;
        public double MoisturePersistence = 0.5f;
        [Space(5)]
        public float DryerValue = 0.22f;
        public float DryValue = 0.35f;
        public float WetValue = 0.55f;
        public float WetterValue = 0.75f;
        public float WettestValue = 0.85f;
        private ModuleBase _moistureModule;


        [Header("River")]
        public int RiverCount = 40;
        public float MinRiverHeight = 0.6f;
        public int MaxRiverAttempts = 1000;
        public int MinRiverTurns = 18;
        public int MinRiverLength = 20;
        public int MaxRiverIntersections = 2;
        public List<River> Rivers = new List<River>();
        public List<RiverGroup> RiverGroups = new List<RiverGroup>();


        [Header("World Generation Utilities")]
        public bool AutoUnloadChunk = true;
        public bool ShowChunksBorder = false;
        public bool ShowTilegroupMaps = false;
        public bool InitWorldWithHeatmap = false;
        public bool InitWorldWithMoisturemap = false;
        public bool PaintTileNeighbors = false;


        [Header("Tilemap")]
        public readonly Vector3 CELL_SIZE = new Vector3(2.0f, 1.0f, 1.0f);
        public readonly Vector3 CELL_GAP = new Vector3(0.0f, 0.0f, 0.0f);



        [Header("Data Cached")]
        private Dictionary<Vector2Int, Chunk> _chunks;
        public HashSet<Chunk> ActiveChunks;

        [Header("Color")]
        // Height
        // Heat
        public static Color ColdestColor = new Color(0, 1, 1, 1);
        public static Color ColderColor = new Color(170 / 255f, 1, 1, 1);
        public static Color ColdColor = new Color(0, 229 / 255f, 133 / 255f, 1);
        public static Color WarmColor = new Color(1, 1, 100 / 255f, 1);
        public static Color WarmerColor = new Color(1, 100 / 255f, 0, 1);
        public static Color WarmestColor = new Color(241 / 255f, 12 / 255f, 0, 1);

        // Moisture
        public static Color Dryest = new Color(255 / 255f, 139 / 255f, 17 / 255f, 1);
        public static Color Dryer = new Color(245 / 255f, 245 / 255f, 23 / 255f, 1);
        public static Color Dry = new Color(80 / 255f, 255 / 255f, 0 / 255f, 1);
        public static Color Wet = new Color(85 / 255f, 255 / 255f, 255 / 255f, 1);
        public static Color Wetter = new Color(20 / 255f, 70 / 255f, 255 / 255f, 1);
        public static Color Wettest = new Color(0 / 255f, 0 / 255f, 100 / 255f, 1);



        // Cached
        private Vector2Int lastChunkISOFrame;
        private Vector2 _centerPoint;
        private Vector2Int _centerPointFrame;
        #endregion

        /*
            World generation slider range.
            0.0f -> 0.1f: Calculate noise range from specific seed.
            0.1f -> 1.0f: Loading chunk data.
         */


        #region Unity life cycle methods
        private void Awake()
        {
            Instance = this;

            IsometricUtilities.CELLSIZE_X = CELL_SIZE.x;
            IsometricUtilities.CELLSIZE_Y = CELL_SIZE.y;
        }

        private void Start()
        {
            // Initialize the chunks dictionary
            _chunks = new Dictionary<Vector2Int, Chunk>();
            ActiveChunks = new HashSet<Chunk>();

            _centerPoint = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2f, Screen.height / 2f));

            // Init noise module
            _heightModule = new Perlin(Frequency, Lacunarity, Persistence, Octaves, Seed, QualityMode.High);
            _heatModule = new Perlin(HeatFrequency, HeatLacunarity, HeatPersistence, HeatOctaves, Seed, QualityMode.High);
            _moistureModule = new Perlin(MoistureFrequency, MoistureLacunarity, MoisturePersistence, MoistureOctaves, Seed, QualityMode.High);

            // Load chunks around the player's starting position
            lastChunkISOFrame = IsometricUtilities.ReverseConvertWorldPositionToIsometricFrame(_centerPoint, ChunkWidth, ChunkHeight);


            // World Initialization
            InitWorldAsyncInParallel(lastChunkISOFrame.x, lastChunkISOFrame.y, widthInit: InitWorldWidth, heightInit: InitWorldHeight, () =>
            {
                LoadChunksAroundPosition(lastChunkISOFrame.x, lastChunkISOFrame.y, offsetWidth: LoadChunkOffsetWidth, offsetHeight: LoadChunkOffsetHeight);
            });

        }

        private void Update()
        {
            _centerPoint = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2f, Screen.height / 2f));
            _centerPointFrame = IsometricUtilities.ReverseConvertWorldPositionToIsometricFrame(_centerPoint, ChunkWidth, ChunkHeight);

            if (_centerPointFrame != lastChunkISOFrame)
            {
                lastChunkISOFrame = _centerPointFrame;
                LoadChunksAroundPosition(_centerPointFrame.x, _centerPointFrame.y, offsetWidth: LoadChunkOffsetWidth, offsetHeight: LoadChunkOffsetHeight);
            }
        }
        #endregion




        #region Compute noise range [min, max] when start
        public async Task ComputeNoiseRangeAsyncInSequence()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            float minNoiseValue = float.MaxValue;
            float maxNoiseValue = float.MinValue;
            System.Random rand = new System.Random(Seed);

            await Task.Run(() =>
            {
                for (int i = 0; i < _calculateNoiseRangeCount; i++)
                {
                    double x = rand.Next();
                    double y = rand.Next();
                    float noiseValue = (float)_heightModule.GetValue(x, y, 0); // Generate noise value

                    // Update min and max values
                    if (noiseValue < minNoiseValue)
                        minNoiseValue = noiseValue;
                    if (noiseValue > maxNoiseValue)
                        maxNoiseValue = noiseValue;

                    // 46655 = 6**6 - 1 (use & operator compare to improve performance)
                    if ((i & 46655) == 0)
                    {
                        float progress = (float)i / _calculateNoiseRangeCount;
                        float mapProgress = MathHelper.Map(progress, 0f, 1f, 0.0f, 0.3f);
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            UIGameManager.Instance.CanvasWorldGen.SetWorldGenSlider(mapProgress);
                        });
                    }
                }
            });


            MinWorldNoiseValue = minNoiseValue;
            MaxWorldNoiseValue = maxNoiseValue;

            sw.Stop();
            Debug.Log($"Compute noise time: {sw.ElapsedMilliseconds / 1000f} s");
        }
        public async Task ComputeNoiseRangeAsyncInParallel()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            float minNoiseValue = float.MaxValue;
            float maxNoiseValue = float.MinValue;
            int completedTaskCount = 0;

            UIGameManager.Instance.CanvasWorldGen.SetWorldGenSlider(0);

            Task<MinMax>[] tasks = new Task<MinMax>[_calculateNoiseRangeSampleMultiplier];
            List<Task> continuationTasks = new List<Task>();
            for (int i = 0; i < tasks.Length; i++)
            {
                int newSeed = WorldGenUtilities.GenerateNewSeed(Seed + i);
                bool updateLoadingUI = i == 0 ? true : false;
                tasks[i] = ComputeNoiseRangeAsync(newSeed);

                Task continuationTask = tasks[i].ContinueWith(task =>
                {
                    // Increment the completed tasks counter
                    Interlocked.Increment(ref completedTaskCount);

                    // Play Slider UI
                    float progress = (float)completedTaskCount / tasks.Length;
                    float mapProgress = MathHelper.Map(progress, 0f, 1f, 0.0f, 0.1f);
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        UIGameManager.Instance.CanvasWorldGen.SetWorldGenSlider(mapProgress);
                    });
                }, TaskContinuationOptions.OnlyOnRanToCompletion);

                continuationTasks.Add(continuationTask);
            }
            await Task.WhenAll(tasks);
            await Task.WhenAll(continuationTasks);

            for (int i = 0; i < tasks.Length; i++)
            {
                float minValue = tasks[i].Result.MinValue;
                float maxValue = tasks[i].Result.MaxValue;

                if (minNoiseValue > minValue)
                    minNoiseValue = minValue;
                if (maxNoiseValue < maxValue)
                    maxNoiseValue = maxValue;
            }
            MinWorldNoiseValue = minNoiseValue;
            MaxWorldNoiseValue = maxNoiseValue;

            sw.Stop();
            Debug.Log($"Compute noise time: {sw.ElapsedMilliseconds / 1000f} s");
        }
        struct MinMax
        {
            public float MinValue;
            public float MaxValue;
        }
        private async Task<MinMax> ComputeNoiseRangeAsync(int seed)
        {
            MinMax minMax = new MinMax()
            {
                MinValue = float.MaxValue,
                MaxValue = float.MinValue
            };
            System.Random rand = new System.Random(seed);
            int sampleCount = 100000;

            await Task.Run(() =>
            {
                for (int i = 0; i < sampleCount; i++)
                {
                    double x = rand.Next();
                    double y = rand.Next();
                    float noiseValue = (float)_heightModule.GetValue(x, y, 0); // Generate noise value

                    // Update min and max values
                    if (noiseValue < minMax.MinValue)
                        minMax.MinValue = noiseValue;
                    if (noiseValue > minMax.MaxValue)
                        minMax.MaxValue = noiseValue;
                }
            });

            return minMax;
        }
        #endregion Compute Noise Range




        #region Init Chunks
        private async void InitWorldAsyncInSequence(int initIsoFrameX, int initIsoFrameY, int widthInit, int heightInit, System.Action onFinished = null)
        {
            UIGameManager.Instance.DisplayWorldGenCanvas(true);
            //await ComputeNoiseRangeAsyncInSequence();
            await ComputeNoiseRangeAsyncInParallel();

            Debug.Log($"min: {MinWorldNoiseValue}");
            Debug.Log($"max: {MaxWorldNoiseValue}");

            int totalIterations = (2 * widthInit + 1) * (2 * heightInit + 1);
            int currentIteration = 0;
            for (int x = initIsoFrameX - widthInit; x <= initIsoFrameX + widthInit; x++)
            {
                for (int y = initIsoFrameY - heightInit; y <= initIsoFrameY + heightInit; y++)
                {
                    Chunk newChunk = await GenerateNewChunkAsync(x, y);
                    AddNewChunk(newChunk);
                    newChunk.UnloadChunk();


                    // Update the slider value based on progress
                    currentIteration++;
                    float progress = (float)currentIteration / totalIterations;
                    float mapProgress = MathHelper.Map(progress, 0f, 1f, 0.1f, 1.0f);
                    UIGameManager.Instance.CanvasWorldGen.SetWorldGenSlider(mapProgress);
                }
            }

            await Task.Delay(100);
            UIGameManager.Instance.DisplayWorldGenCanvas(false);
            onFinished?.Invoke();
        }
        private async void InitWorldAsyncInParallel(int initIsoFrameX, int initIsoFrameY, int widthInit, int heightInit, System.Action onFinished = null)
        {
            UIGameManager.Instance.DisplayWorldGenCanvas(true);
            //await ComputeNoiseRangeAsyncInSequence();
            await ComputeNoiseRangeAsyncInParallel();

            Debug.Log($"min: {MinWorldNoiseValue}");
            Debug.Log($"max: {MaxWorldNoiseValue}");

            int completedTaskCount = 0;

            Task<Chunk>[] tasks = new Task<Chunk>[(widthInit * 2 + 1) * (heightInit * 2 + 1)];
            List<Task> continuationTasks = new List<Task>();

            for (int x = initIsoFrameX - widthInit; x <= initIsoFrameX + widthInit; x++)
            {
                for (int y = initIsoFrameY - heightInit; y <= initIsoFrameY + heightInit; y++)
                {
                    int index = x - (initIsoFrameX - widthInit) + (y - (initIsoFrameY - heightInit)) * (2 * widthInit + 1);
                    tasks[index] = GenerateNewChunkAsync(x, y);

                    Task continuationTask = tasks[index].ContinueWith(task =>
                    {
                        // Increment the completed tasks counter
                        Interlocked.Increment(ref completedTaskCount);

                        // Play Slider UI
                        float progress = (float)completedTaskCount / tasks.Length;
                        float mapProgress = MathHelper.Map(progress, 0f, 1f, 0.1f, 1.0f);
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            UIGameManager.Instance.CanvasWorldGen.SetWorldGenSlider(mapProgress);
                        });
                    }, TaskContinuationOptions.OnlyOnRanToCompletion);

                    continuationTasks.Add(continuationTask);
                }
            }

            await Task.WhenAll(tasks);
            await Task.WhenAll(continuationTasks);

            foreach(var task in tasks)
            {
                Chunk newChunk = task.Result;
                AddNewChunk(newChunk);
                newChunk.UnloadChunk();
            }

            UIGameManager.Instance.DisplayWorldGenCanvas(false);
            onFinished?.Invoke();
        }
        private async Task<Chunk> GenerateNewChunkAsync(int isoFrameX, int isoFrameY)
        {
            Vector2 frame = IsometricUtilities.IsometricFrameToWorldFrame(isoFrameX, isoFrameY);
            Vector3 worldPosition = IsometricUtilities.ConvertIsometricFrameToWorldPosition(isoFrameX, isoFrameY, ChunkWidth, ChunkHeight);
            Chunk newChunk = Instantiate(_chunkPrefab, worldPosition, Quaternion.identity);
            newChunk.Init(frame.x, frame.y, isoFrameX, isoFrameY, ChunkWidth, ChunkHeight);

            // Create new data
            float[,] heightValues = await GetHeightMapDataAsycn(isoFrameX, isoFrameY);
            float[,] heatValues = await GetHeatMapDataAysnc(isoFrameX, isoFrameY);
            float[,] moisetureValues = await GetMoistureMapDataAsync(isoFrameX, isoFrameY);

            if (InitWorldWithHeatmap)
            {
                await newChunk.LoadHeightAndHeatMap(heightValues, heatValues);
            }
            else
            {
                await newChunk.LoadHeightMapDataAsync(heightValues);
            }

            if (InitWorldWithMoisturemap)
            {
                await newChunk.LoadMoistureMapDataAsync(moisetureValues);
            }
            else
            {

            }

            return newChunk;
        }
        // Load chunks around a given chunk position
        private async void LoadChunksAroundPosition(int isoFrameX, int isoFrameY, int offsetWidth = 1, int offsetHeight = 1)
        {
            for (int x = isoFrameX - offsetWidth; x <= isoFrameX + offsetWidth; x++)
            {
                for (int y = isoFrameY - offsetHeight; y <= isoFrameY + offsetHeight; y++)
                {
                    Vector2Int nbIsoFrame = new Vector2Int(x, y);
                    Chunk chunk = GetChunk(nbIsoFrame);
                    if (chunk == null)   // Create new chunk
                    {
                        if (x == isoFrameX && y == isoFrameY)
                        {
                            // Center
                            // ......
                        }

                        Chunk newChunk =  await GenerateNewChunkAsync(x,y);
                        await newChunk.DrawChunkAsync();

                        // Cached chunk data
                        if (_chunks.ContainsKey(nbIsoFrame) == false)
                        {
                            AddNewChunk(newChunk);
                        }
                            
                        ActiveChunks.Add(newChunk);

                        _chunks[nbIsoFrame].LoadChunk();


                        if (!newChunk.HasNeighbors())
                        {
                            UpdateChunkTileNeighbors(newChunk);
                        }         
                    }
                    else // Load chunk cached.
                    {
                        if (x == isoFrameX && y == isoFrameY)
                        {
                            // Center
                            // ......
                        }

                        _chunks[nbIsoFrame].LoadChunk();
                        ActiveChunks.Add(_chunks[nbIsoFrame]);

                        if (_chunks[nbIsoFrame].ChunkHasDrawn == false)
                        {
                            //_chunks[nbIsoFrame].DrawChunk();
                            await _chunks[nbIsoFrame].DrawChunkAsync();

                            if (InitWorldWithHeatmap)
                                _chunks[nbIsoFrame].PaintHeatMap();

                            if (InitWorldWithMoisturemap)
                                _chunks[nbIsoFrame].PaintMoistureMap();
                        }

                        if (!_chunks[nbIsoFrame].HasNeighbors())
                        {
                            UpdateChunkTileNeighbors(_chunks[nbIsoFrame]);
                        }
                    }

                }
            }

            if (ShowChunksBorder == false)
            {
                SortActiveChunkByDepth();
            }
        }
        #endregion



        #region Get, Set Chunk
        public Chunk GetChunk(Vector2 worldPosition)
        {
            var isoFrame = IsometricUtilities.ReverseConvertWorldPositionToIsometricFrame(worldPosition,
                                                                               ChunkWidth,
                                                                               ChunkHeight);
            return GetChunk(isoFrame);
        }
        public Chunk GetChunk(int isoFrameX, int isoFrameY)
        {
            Vector2Int isoFrame = new Vector2Int(isoFrameX, isoFrameY);
            if (_chunks.ContainsKey(isoFrame))
            {
                return _chunks[isoFrame];
            }
            return null;
        }
        public Chunk GetChunk(Vector2Int isoFrame)
        {
            if (_chunks.ContainsKey(isoFrame))
            {
                return _chunks[isoFrame];
            }
            return null;
        }
        public void AddNewChunk(Chunk chunk)
        {
            Vector2Int isoFrame = new Vector2Int(chunk.IsometricFrameX, chunk.IsometricFrameY);
            _chunks.Add(isoFrame, chunk);
        }
        public void AddNewChunk(Chunk chunk, Vector2Int isoFrame)
        {
            _chunks.Add(isoFrame, chunk);
        }
        #endregion


        #region Generate noise map data.
        private async Task<float[,]> GetHeightMapDataAsycn(int isoFrameX, int isoFrameY)
        {
            float[,] heightValues = new float[ChunkWidth, ChunkHeight];

            await Task.Run(() =>
            {
                Parallel.For(0, ChunkWidth, x =>
                {
                    for (int y = 0; y < ChunkHeight; y++)
                    {
                        float offsetX = isoFrameX * ChunkWidth + x;
                        float offsetY = isoFrameY * ChunkHeight + y;
                        float heightValue = (float)_heightModule.GetValue(offsetX, offsetY, 0);
                        float normalizeHeightValue = (heightValue - MinWorldNoiseValue) / (MaxWorldNoiseValue - MinWorldNoiseValue);
                        heightValues[x, y] = normalizeHeightValue;
                    }
                });
            });

            return heightValues;
        }
        private async Task<float[,]> GetGradientMapDataAsync(int isoFrameX, int isoFrameY)
        {
            //Debug.Log("GetGradientMapAsync Start");
            float[,] gradientData = new float[ChunkWidth, ChunkHeight];
            isoFrameX = -isoFrameX;
            isoFrameY = -isoFrameY;

            await Task.Run(() =>
            {
                int gradientFrameX = (int)(isoFrameX * ChunkWidth / _gradientHeatmapSize);
                int gradientFrameY = -Mathf.CeilToInt(isoFrameY * ChunkHeight / _gradientHeatmapSize);

                // Calculate the center of the texture with the offset
                Vector2 gradientOffset = new Vector2(gradientFrameX * _gradientHeatmapSize, gradientFrameY * _gradientHeatmapSize);
                Vector2 gradientCenterOffset = gradientOffset + new Vector2(isoFrameX * ChunkWidth, isoFrameY * ChunkHeight);


                for (int x = 0; x < ChunkWidth; x++)
                {
                    for (int y = 0; y < ChunkHeight; y++)
                    {
                        Vector2 center = new Vector2(Mathf.FloorToInt(x / _gradientHeatmapSize), Mathf.FloorToInt(y / _gradientHeatmapSize)) * new Vector2(_gradientHeatmapSize, _gradientHeatmapSize) + new Vector2(_gradientHeatmapSize / 2f, _gradientHeatmapSize / 2f);
                        Vector2 centerWithOffset = center + gradientCenterOffset;

                        float distance = Mathf.Abs(y - centerWithOffset.y);
                        float normalizedDistance = 1.0f - Mathf.Clamp01(distance / (_gradientHeatmapSize / 2f));
                        gradientData[x, y] = normalizedDistance;
                    }
                }
            });

            //Debug.Log("GetGradientMapAsync Finish");
            return gradientData;
        }
        private async Task<float[,]> GetFractalHeatMapDataAsync(int isoFrameX, int isoFrameY)
        {
            //Debug.Log("GetFractalHeatMapAsync Start");

            float[,] fractalNoiseData = new float[ChunkWidth, ChunkHeight];

            await Task.Run(() =>
            {
                for (int x = 0; x < ChunkWidth; x++)
                {
                    for (int y = 0; y < ChunkHeight; y++)
                    {
                        float offsetX = isoFrameX * ChunkWidth + x;
                        float offsetY = isoFrameY * ChunkHeight + y;
                        float heatValue = (float)_heatModule.GetValue(offsetX, offsetY, 0);
                        float normalizeHeatValue = (heatValue - MinWorldNoiseValue) / (MaxWorldNoiseValue - MinWorldNoiseValue);

                        fractalNoiseData[x, y] = normalizeHeatValue;
                    }
                }
            });

            //Debug.Log("GetFractalHeatMapAsync Finish");
            return fractalNoiseData;
        }
        private async Task<float[,]> GetHeatMapDataAysnc(int isoFrameX, int isoFrameY)
        {
            /*
             * Heatmap created by blend gradient map and fractal noise map.
             */

            // Sequence way
            // ===========
            //float[,] gradientValues = await GetGradientMapAsync(isoFrameX, isoFrameY);
            //float[,] fractalNoiseValues = await GetFractalHeatMapAsync(isoFrameX, isoFrameY);
            //float[,] heatValues = BlendMapData(gradientValues, fractalNoiseValues, HeatMapBlendFactor);


            // Simultaneous way
            // ===============
            Task<float[,]> gradientTask = GetGradientMapDataAsync(isoFrameX, isoFrameY);
            Task<float[,]> fractalNoiseTask = GetFractalHeatMapDataAsync(isoFrameX, isoFrameY);

            // Await for both tasks to complete
            await Task.WhenAll(gradientTask, fractalNoiseTask);
            float[,] gradientValues = gradientTask.Result;
            float[,] fractalNoiseValues = fractalNoiseTask.Result;

            // Blend the maps
            float[,] heatValues = WorldGenUtilities.BlendMapData(gradientValues, fractalNoiseValues, HeatMapBlendFactor);
            return heatValues;
        }
        private async Task<float[,]> GetMoistureMapDataAsync(int isoFrameX, int isoFrameY)
        {
            float[,] moistureData = new float[ChunkWidth, ChunkHeight];

            await Task.Run(() =>
            {
                for (int x = 0; x < ChunkWidth; x++)
                {
                    for (int y = 0; y < ChunkHeight; y++)
                    {
                        float offsetX = isoFrameX * ChunkWidth + x;
                        float offsetY = isoFrameY * ChunkHeight + y;
                        float moisetureValue = (float)_moistureModule.GetValue(offsetX, 0, offsetY);
                        float normalizeMoistureValue = (moisetureValue - MinWorldNoiseValue) / (MaxWorldNoiseValue - MinWorldNoiseValue);

                        moistureData[x, y] = normalizeMoistureValue;
                    }
                }
            });

            return moistureData;
        }
        #endregion




        #region River
        private void GenerateRivers(int isoFrameX, int isoFrameY)
        {
            int attempts = 0;
            int riverCount = RiverCount;
            Rivers = new List<River>();

            // Generate some rivers
            while(riverCount > 0 && attempts < MaxRiverAttempts)
            {
                // Get a random tile
                int x = Random.Range(0, ChunkWidth);
                int y = Random.Range(0, ChunkHeight);
                Tile tile = _chunks[new Vector2Int(isoFrameX, isoFrameY)].ChunkData.GetValue(x, y);

                // validate the tile
                if (!tile.Collidable) continue;
                if (tile.Rivers.Count > 0) continue;

                if(tile.HeightValue > MinRiverHeight)
                {
                    // Tile is good to start from
                    River river = new River(riverCount);

                    // Figure out the direction this river will try to flow
                    river.CurrentDirection = tile.GetLowestNeighbors();

                    // Recursively find a path to water

                }
            }
        }

        private void FindPathToWater(Tile tile, Direction direction, ref River river)
        {
            if (tile.Rivers.Contains(river)) 
                return;

            // Check if there is already a river on this tile.
            if (tile.Rivers.Count > 0)
                river.Insersections++;

            river.AddTile(tile);

            // Get Neighbors
            Tile leftNb = tile.Left;
            Tile rightNb = tile.Right;
            Tile topNb = tile.Top;
            Tile bottomNb = tile.Bottom;

            float leftValue = int.MaxValue;
            float rightValue = int.MaxValue;
            float topValue = int.MaxValue;
            float bottomValue = int.MaxValue;

            // Query height values of neighbors
            if (leftNb.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(leftNb))
                leftValue = leftNb.HeightValue;
            if (rightNb.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(rightNb))
                rightValue = rightNb.HeightValue; 
            if (topNb.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(topNb))
                topValue = topNb.HeightValue; 
            if (bottomNb.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(bottomNb))
                bottomValue = bottomNb.HeightValue;


            // if neighbor is existing river that is not this one, flow into it
            if (bottomNb.Rivers.Count == 0 && !bottomNb.Collidable)
                bottomValue = 0;
            if (topNb.Rivers.Count == 0 && !topNb.Collidable)
                bottomValue = 0;
            if (bottomNb.Rivers.Count == 0 && !bottomNb.Collidable)
                bottomValue = 0;
            if (bottomNb.Rivers.Count == 0 && !bottomNb.Collidable)
                bottomValue = 0;


            // Override flow direction if a tile is significantly lower
            if(direction == Direction.Left)
            {
                if(Mathf.Abs(rightValue - leftValue) < 0.1f)
                {
                    rightValue = int.MaxValue;
                }
            }
            if(direction == Direction.Right)
            {
                if(Mathf.Abs(rightValue - leftValue) < 0.1)
                {
                    leftValue = int.MaxValue;
                }
            }
            if(direction == Direction.Top)
            {
                if(Mathf.Abs(topValue - bottomValue) < 0.1f)
                {
                    bottomValue = int.MaxValue;
                }
            }
            if(direction == Direction.Bottom)
            {
                if (Mathf.Abs(topValue - bottomValue) < 0.1f)
                { 
                    topValue = int.MaxValue;
                }
            }


            // Find minimum
            float min = Mathf.Min(leftValue, rightValue);
            min = Mathf.Min(min, topValue);
            min = Mathf.Min(min, bottomValue);

            // if no minimum found -> exit
            if (min == int.MaxValue)
                return;

            // Move to next neighbor
            if(min == leftValue)
            {
                if(leftNb.Collidable)
                {
                    if(river.CurrentDirection != Direction.Left)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = Direction.Left;
                    }
                    FindPathToWater(leftNb, direction, ref river);
                }
            }
            else if (min == rightValue)
            {
                if (rightNb.Collidable)
                {
                    if (river.CurrentDirection != Direction.Right)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = Direction.Right;
                    }
                    FindPathToWater(leftNb, direction, ref river);
                }
            }
            else if (min == bottomValue)
            {
                if (bottomNb.Collidable)
                {
                    if (river.CurrentDirection != Direction.Bottom)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = Direction.Bottom;
                    }
                    FindPathToWater(leftNb, direction, ref river);
                }
            }
            else if (min == topValue)
            {
                if (topNb.Collidable)
                {
                    if (river.CurrentDirection != Direction.Top)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = Direction.Top;
                    }
                    FindPathToWater(leftNb, direction, ref river);
                }
            }
        }
        #endregion



       

        #region Neighbors
        private void AddChunkFourDirectionNeighbors(Chunk chunk)
        {
            Chunk nbAbove = GetChunkNeighborAbove(chunk);
            Chunk nbBelow = GetChunkNeighborBelow(chunk);
            Chunk nbLeft = GetChunkNeighborLeft(chunk);
            Chunk nbRight = GetChunkNeighborRight(chunk);
            chunk.SetTwoSidesChunkNeighbors(nbLeft, nbRight, nbAbove, nbBelow);
        }
        private Chunk GetChunkNeighborAbove(Chunk chunk)
        {
            Vector2Int isoFrameChunkNb = new Vector2Int(chunk.IsometricFrameX, chunk.IsometricFrameY + 1);
            return _chunks.TryGetValue(isoFrameChunkNb, out Chunk neighborChunk) ? neighborChunk : null;
        }
        private Chunk GetChunkNeighborBelow(Chunk chunk)
        {
            Vector2Int isoFrameChunkNb = new Vector2Int(chunk.IsometricFrameX, chunk.IsometricFrameY - 1);
            return _chunks.TryGetValue(isoFrameChunkNb, out Chunk neighborChunk) ? neighborChunk : null;
        }
        private Chunk GetChunkNeighborLeft(Chunk chunk)
        {
            Vector2Int isoFrameChunkNb = new Vector2Int(chunk.IsometricFrameX - 1, chunk.IsometricFrameY);
            return _chunks.TryGetValue(isoFrameChunkNb, out Chunk neighborChunk) ? neighborChunk : null;
        }
        private Chunk GetChunkNeighborRight(Chunk chunk)
        {
            Vector2Int isoFrameChunkNb = new Vector2Int(chunk.IsometricFrameX + 1, chunk.IsometricFrameY);
            return _chunks.TryGetValue(isoFrameChunkNb, out Chunk neighborChunk) ? neighborChunk : null;
        }
        private void UpdateChunkTileNeighbors(Chunk chunk)
        {
            // Find chunk neighbors
            Chunk nbAbove = GetChunkNeighborAbove(chunk);
            Chunk nbBelow = GetChunkNeighborBelow(chunk);
            Chunk nbLeft = GetChunkNeighborLeft(chunk);
            Chunk nbRight = GetChunkNeighborRight(chunk);
            chunk.SetTwoSidesChunkNeighbors(nbLeft, nbRight, nbAbove, nbBelow);

            if (chunk.HasNeighbors())
            {
                chunk.UpdateAllTileNeighbors();

                if (PaintTileNeighbors)
                    chunk.PaintNeighborsColor();

                if (chunk.Waters.Count == 0 && chunk.Lands.Count == 0)
                {
                    FloodFill(chunk);
                    chunk.PaintTilegroupMap();
                }
            }

            if (nbAbove != null && nbAbove.HasNeighbors())
            {
                nbAbove.UpdateAllTileNeighbors();

                if (PaintTileNeighbors)
                    nbAbove.PaintNeighborsColor();

                if (nbAbove.Waters.Count == 0 && nbAbove.Lands.Count == 0)
                {
                    FloodFill(nbAbove);
                    nbAbove.PaintTilegroupMap();
                }
            }
            if (nbBelow != null && nbBelow.HasNeighbors())
            {
                nbBelow.UpdateAllTileNeighbors();

                if (PaintTileNeighbors)
                    nbBelow.PaintNeighborsColor();

                if (nbBelow.Waters.Count == 0 && nbBelow.Lands.Count == 0)
                {
                    FloodFill(nbBelow);
                    nbBelow.PaintTilegroupMap();
                }

            }
            if (nbLeft != null && nbLeft.HasNeighbors())
            {
                nbLeft.UpdateAllTileNeighbors();

                if (PaintTileNeighbors)
                    nbLeft.PaintNeighborsColor();

                if (nbLeft.Waters.Count == 0 && nbLeft.Lands.Count == 0)
                {
                    FloodFill(nbLeft);
                    nbLeft.PaintTilegroupMap();
                }
            }
            if (nbRight != null && nbRight.HasNeighbors())
            {
                nbRight.UpdateAllTileNeighbors();

                if (PaintTileNeighbors)
                    nbRight.PaintNeighborsColor();

                if (nbRight.Waters.Count == 0 && nbRight.Lands.Count == 0)
                {
                    FloodFill(nbRight);
                    nbRight.PaintTilegroupMap();
                }

            }
        }
        #endregion


        

        #region Grid Algorithm
        private void FloodFill(Chunk chunk)
        {
            Stack<Tile> stack = new Stack<Tile>();

            for (int x = 0; x < ChunkWidth; x++)
            {
                for (int y = 0; y < ChunkHeight; y++)
                {
                    Tile t = chunk.ChunkData.GetValue(x, y);

                    // Tile already flood filled, skip
                    if (t.FloodFilled)
                    {
                        continue;
                    }

                    // Land
                    if (t.Collidable)
                    {
                        TileGroup group = new TileGroup();
                        group.Type = TileGroupType.Land;
                        stack.Push(t);

                        while (stack.Count > 0)
                        {
                            Tile tile = stack.Pop();
                            FloodFill(tile, ref group, ref stack, chunk);
                        }

                        if (group.Tiles.Count > 0)
                        {
                            chunk.Lands.Add(group);
                        }
                    }
                    else // Water
                    {
                        TileGroup group = new TileGroup();
                        group.Type = TileGroupType.Water;
                        stack.Push(t);

                        while (stack.Count > 0)
                        {
                            Tile tile = stack.Pop();
                            FloodFill(tile, ref group, ref stack, chunk);
                        }

                        if (group.Tiles.Count > 0)
                        {
                            chunk.Waters.Add(group);
                        }
                    }
                }
            }
        }
        private void FloodFill(Tile tile, ref TileGroup tiles, ref Stack<Tile> stack, Chunk chunk)
        {
            // Validate
            if (tile.FloodFilled)
                return;
            if (tiles.Type == TileGroupType.Land && tile.Collidable == false)
                return;
            if (tiles.Type == TileGroupType.Water && tile.Collidable)
                return;

            tiles.Tiles.Add(tile);
            tile.FloodFilled = true;


            // FloodFill into neighbors
            Tile nb = chunk.GetTopWithinChunk(tile);
            if (nb != null && nb.FloodFilled == false && tile.Collidable == nb.Collidable)
            {
                stack.Push(nb);
            }

            nb = chunk.GetBottomWithinChunk(tile);
            if (nb != null && nb.FloodFilled == false && tile.Collidable == nb.Collidable)
            {
                stack.Push(nb);
            }

            nb = chunk.GetLeftWithinChunk(tile);
            if (nb != null && nb.FloodFilled == false && tile.Collidable == nb.Collidable)
            {
                stack.Push(nb);
            }

            nb = chunk.GetRightWithinChunk(tile);
            if (nb != null && nb.FloodFilled == false && tile.Collidable == nb.Collidable)
            {
                stack.Push(nb);
            }
        }
        #endregion



        #region Utilities
        public Color GetGradientColor(float heatValue)
        {
            // predefine heat value threshold
            float ColdestValue = 0.05f;
            float ColderValue = 0.18f;
            float ColdValue = 0.4f;
            float WarmValue = 0.6f;
            float WarmerValue = 0.8f;


            if (heatValue < ColdestValue)
            {
                return ColdestColor;
            }
            else if (heatValue < ColderValue)
            {
                return ColderColor;
            }
            else if (heatValue < ColdValue)
            {
                return ColdColor;
            }
            else if (heatValue < WarmValue)
            {
                return WarmColor;
            }
            else if (heatValue < WarmerValue)
            {
                return WarmerColor;
            }
            else
            {
                return WarmestColor;
            }
        }
        /// <summary>
        /// Sort active chunks fix some isometric chunk has wrong order (Visualization).
        /// </summary>
        private void SortActiveChunkByDepth(bool inverse = false)
        {
            int depth = 0;
            List<Chunk> chunkList = ActiveChunks.ToList();

            chunkList.Sort((v1, v2) =>
            {
                int xComparison = v1.IsometricFrameX.CompareTo(v2.IsometricFrameX);
                int yComparison = v1.IsometricFrameY.CompareTo(v2.IsometricFrameY);

                if (inverse)
                {
                    xComparison = -xComparison; // Reverse xComparison if 'inverse' is true
                    yComparison = -yComparison; // Reverse yComparison if 'inverse' is true
                }

                return xComparison != 0 ? xComparison : yComparison;
            });

            foreach (var chunk in chunkList)
            {
                chunk.transform.position = new Vector3(chunk.transform.position.x,
                    chunk.transform.position.y,
                    depth++);
            }
        }
        #endregion



#if DEV_CONSOLE
        [ConsoleCommand("unload_chunk", value: "0")]
        private void AutomaticUnloadChunk(int index)
        {
            // 0: Disable automatic unloadchunk
            // 1: Enable automatic unloadchunk
            switch (index)
            {
                case 0:
                    AutoUnloadChunk = false;
                    Debug.Log($"Disable auto unload chunk.");
                    break;
                case 1:
                    AutoUnloadChunk = true;
                    Debug.Log($"Enable auto unload chunk.");
                    break;
                default: break;
            }
        }

        [ConsoleCommand("show_chunk_border")]
        private void ShowBorders()
        {
            ShowChunksBorder = true;
            SortActiveChunkByDepth(inverse: true);
        }

        [ConsoleCommand("hide_chunk_border")]
        private void HideBorders()
        {
            ShowChunksBorder = false;
            SortActiveChunkByDepth(inverse: false);
        }

        [ConsoleCommand("show_tilegroup_map")]
        private void ShowTilegroupMap()
        {
            ShowTilegroupMaps = true;
            foreach (var chunk in ActiveChunks)
            {
                chunk.LoadChunk();
            }
        }

        [ConsoleCommand("hide_tilegroup_map")]
        private void HideTilegroupMap()
        {
            ShowTilegroupMaps = false;
            foreach (var chunk in ActiveChunks)
            {
                chunk.LoadChunk();
            }
        }
#endif




        #region Obsolete
        /// <summary>
        /// Obsolete (Need update).
        /// </summary>
        /// <param name="isoFrameX"></param>
        /// <param name="isoFrameY"></param>
        /// <returns></returns>
        private Chunk AddNewChunk(int isoFrameX, int isoFrameY)
        {
            Vector2 frame = IsometricUtilities.IsometricFrameToWorldFrame(isoFrameX, isoFrameY);
            Vector3 worldPosition = IsometricUtilities.ConvertIsometricFrameToWorldPosition(isoFrameX, isoFrameY, ChunkWidth, ChunkHeight);
            Chunk newChunk = Instantiate(_chunkPrefab, worldPosition, Quaternion.identity);
            newChunk.Init(frame.x, frame.y, isoFrameX, isoFrameY, ChunkWidth, ChunkHeight);

            // Create new data
            float[,] heightValues = GetHeightMapNoise(isoFrameX, isoFrameY);
            newChunk.LoadHeightMap(heightValues);
            return newChunk;
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        /// <param name="isoFrameX"></param>
        /// <param name="isoFrameY"></param>
        /// <returns></returns>
        private float[,] GetHeightMapNoise(int isoFrameX, int isoFrameY)
        {
            float[,] heightValues = new float[ChunkWidth, ChunkHeight];
            for (int x = 0; x < ChunkWidth; x++)
            {
                for (int y = 0; y < ChunkHeight; y++)
                {
                    float offsetX = isoFrameX * ChunkWidth + x;
                    float offsetY = isoFrameY * ChunkHeight + y;
                    heightValues[x, y] = (float)_heightModule.GetValue(offsetX, offsetY, 0);
                }
            }
            return heightValues;
        }
        #endregion
    }
}

