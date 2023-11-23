using System.Collections.Generic;
using UnityEngine;
using LibNoise;
using LibNoise.Generator;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Sirenix.OdinInspector;
using PixelMiner.Utilities;
using System;
using PixelMiner.WorldGen.Utilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PixelMiner.WorldGen
{
    public class WorldGeneration : MonoBehaviour
    {
        private readonly object lockObject = new object(); // Define a lock object for thread safety
        public static WorldGeneration Instance { get; private set; }

        #region Fileds and Variables
        [FoldoutGroup("References"), SerializeField] private Chunk _chunkPrefab;
        [FoldoutGroup("References"), SerializeField] private Transform _chunkParent;


        // Tilemap Settings
        [FoldoutGroup("Tilemap Settings"), Indent(1), ShowInInspector, ReadOnly] public readonly float IsometricAngle = 26.565f;
        [FoldoutGroup("Tilemap Settings"), Indent(1), ShowInInspector, ReadOnly] public readonly Vector3 CELL_SIZE = new Vector3(2.0f, 1.0f, 1.0f);
        [FoldoutGroup("Tilemap Settings"), Indent(1), ShowInInspector, ReadOnly] public readonly Vector3 CELL_GAP = new Vector3(0.0f, 0.0f, 0.0f);

        // Min and Max Height used for normalize noise value in range [0-1]
        [ShowInInspector, ReadOnly] public float MinWorldNoiseValue { get; private set; } = float.MaxValue;
        [ShowInInspector, ReadOnly] public float MaxWorldNoiseValue { get; private set; } = float.MinValue;

        //[Header("World Settings")]

        [FoldoutGroup("World Settings"), Indent(1)] public string SeedInput = "7";
        [FoldoutGroup("World Settings"), Indent(1), ReadOnly, ShowInInspector] public int Seed { get; private set; }
        [FoldoutGroup("World Settings"), Indent(1)] public byte ChunkWidth = 16;      // Size of each chunk in tiles
        [FoldoutGroup("World Settings"), Indent(1)] public byte ChunkHeight = 16;      // Size of each chunk in tiles
        [Space(5)]
        [FoldoutGroup("World Settings"), Indent(1)] public byte InitWorldWidth = 3;
        [FoldoutGroup("World Settings"), Indent(1)] public byte InitWorldHeight = 3;
        [FoldoutGroup("World Settings"), Indent(1)] public byte LoadChunkOffsetWidth = 1;
        [FoldoutGroup("World Settings"), Indent(1)] public byte LoadChunkOffsetHeight = 1;

        // Compute noise range sample count.
        private byte _calculateNoiseRangeSampleMultiplier = 15;  // 50 times. 50 * 100000 = 5 mil times.
        private int _calculateNoiseRangeCount = 1000000;    // 1 mil times.



        // Height map
        // ==========
        [InfoBox("Height map noise settings")]
        [FoldoutGroup("Height map"), Indent(1)] public int Octaves = 6;
        [FoldoutGroup("Height map"), Indent(1)] public double Frequency = 0.02f;
        [FoldoutGroup("Height map"), Indent(1)] public double Lacunarity = 2.0f;
        [FoldoutGroup("Height map"), Indent(1)] public double Persistence = 0.5f;
      
        [InfoBox("Height map threshold"), Space(10)]
        [FoldoutGroup("Height map"), Indent(1), ProgressBar(0f, 1f, r: 0f, g: 0f, b: 0.5f, Height = 20)]
        public float DeepWater = 0.2f;
        [FoldoutGroup("Height map"), Indent(1), ProgressBar(0f, 1f, r: 25 / 255f, g: 25 / 255f, b: 150 / 255f, Height = 20)]
        public float Water = 0.4f;
        [FoldoutGroup("Height map"), Indent(1), ProgressBar(0f, 1f, r: 240 / 255f, g: 240 / 255f, b: 64 / 255f, Height = 20)]
        public float Sand = 0.5f;
        [FoldoutGroup("Height map"), Indent(1), ProgressBar(0f, 1f, r: 50 / 255f, g: 220 / 255f, b: 20 / 255f, Height = 20)]
        public float Grass = 0.7f;
        [FoldoutGroup("Height map"), Indent(1), ProgressBar(0f, 1f, r: 16 / 255f, g: 160 / 255f, b: 0f, Height = 20)]
        public float Forest = 0.8f;
        [FoldoutGroup("Height map"), Indent(1), ProgressBar(0f, 1f, r: 0.5f, g: 0.5f, b: 0.5f, Height = 20)]
        public float Rock = 0.9f;
        [FoldoutGroup("Height map"), Indent(1), ProgressBar(0f, 1f, r: 1f, g: 1f, b: 1f, Height = 20)]
        public float Snow = 1;
        private ModuleBase _heightModule;


        // Heapmap
        // ======
        [InfoBox("Heat map noise settings")]
        [FoldoutGroup("Heatmap"), Indent(1)] public int HeatOctaves = 4;
        [FoldoutGroup("Heatmap"), Indent(1)] public double HeatFrequency = 0.02;
        [FoldoutGroup("Heatmap"), Indent(1)] public double HeatLacunarity = 2.0f;
        [FoldoutGroup("Heatmap"), Indent(1)] public double HeatPersistence = 0.5f;

        [InfoBox("Gradient map"), Space(10)]
        [FoldoutGroup("Heatmap"), Indent(1)] public ushort GradientHeatmapSize = 256;
        [FoldoutGroup("Heatmap"), Indent(1), Range(0f, 1f)] public float HeatMapBlendFactor = 0.5f;
            
        [InfoBox("Heat map threshold"), Space(10)]
        [FoldoutGroup("Heatmap"), Indent(1), ProgressBar(0f, 1f, r: 0f, g: 1.0f, b: 1.0f, Height = 20)] public float ColdestValue = 0.1f;
        [FoldoutGroup("Heatmap"), Indent(1), ProgressBar(0f, 1f, r: 170/255f, g: 1f, b: 1f, Height = 20)] public float ColderValue = 0.2f;
        [FoldoutGroup("Heatmap"), Indent(1), ProgressBar(0f, 1f, r: 0, g: 229/255f, b: 133/255f, Height = 20)] public float ColdValue = 0.4f;
        [FoldoutGroup("Heatmap"), Indent(1), ProgressBar(0f, 1f, r: 1, g:1, b:100/255f, Height = 20)] public float WarmValue = 0.6f;
        [FoldoutGroup("Heatmap"), Indent(1), ProgressBar(0f, 1f, r: 1, g: 100/255f, b: 0, Height = 20)] public float WarmerValue = 0.8f;
        [FoldoutGroup("Heatmap"), Indent(1), ProgressBar(0f, 1f, r: 241/255f, g: 12/255f, b: 0f, Height = 20)] public float WarmestValue = 1.0f;
        private ModuleBase _heatModule;



        // Moisture map
        // ======
        [InfoBox("Moisture map noise settings")]
        [FoldoutGroup("Moisture map"), Indent(1)] public int MoistureOctaves = 4;
        [FoldoutGroup("Moisture map"), Indent(1)] public double MoistureFrequency = 0.03;
        [FoldoutGroup("Moisture map"), Indent(1)] public double MoistureLacunarity = 2.0f;
        [FoldoutGroup("Moisture map"), Indent(1)] public double MoisturePersistence = 0.5f;
        [InfoBox("Moisture map noise settings"), Space(10)]
        [FoldoutGroup("Moisture map"), Indent(1), ProgressBar(0f, 1f, r: 255/255f, 139/255f, 17/255f, Height = 20)] public float DryestValue = 0.22f;
        [FoldoutGroup("Moisture map"), Indent(1), ProgressBar(0f, 1f, r: 245/255f, g: 245/255f, b: 23/255f, Height = 20)] public float DryerValue = 0.35f;
        [FoldoutGroup("Moisture map"), Indent(1), ProgressBar(0f, 1f, r: 80/255f, g: 255/255f, b: 0/255f, Height = 20)] public float DryValue = 0.55f;
        [FoldoutGroup("Moisture map"), Indent(1), ProgressBar(0f, 1f, r: 85/255f, g: 255/255f, b: 255/255f, Height = 20)] public float WetValue = 0.75f;
        [FoldoutGroup("Moisture map"), Indent(1), ProgressBar(0f, 1f, r: 20/255f, g: 70/255f, b: 255/255f, Height = 20)] public float WetterValue = 0.85f;
        [FoldoutGroup("Moisture map"), Indent(1), ProgressBar(0f, 1f, r: 0/255f, g: 0/255f, b: 100/255f, Height = 20)] public float WettestValue = 1.0f;
        private ModuleBase _moistureModule;



        // River
        // =====
        [FoldoutGroup("River"), Indent(1)] public int RiverOctaves = 4;
        [FoldoutGroup("River"), Indent(1)] public double RiverFrequency = 0.02;
        [FoldoutGroup("River"), Indent(1)] public double RiverLacunarity = 2.0f;
        [FoldoutGroup("River"), Indent(1)] public double RiverPersistence = 0.5f;
        [FoldoutGroup("River"), Indent(1), MinMaxSlider(0f, 1f, true)] public Vector2 RiverRange = new Vector2(0.7f, 0.75f);
        private ModuleBase _riverModule;


        [Header("World Generation Utilities")]
        public bool AutoLoadChunk = true;
        public bool AutoUnloadChunk = true;
        public bool ShowChunksBorder = false;
        public bool ShowTilegroupMaps = false;
        public bool InitWorldWithHeatmap = false;
        public bool InitWorldWithMoisturemap = false;
        public bool InitWorldWithRiver = false;
        public bool PaintTileNeighbors = false;

        [Header("Performance Options")]
        public bool InitFastDrawChunk;



        // Cached
        private Vector2Int lastChunkISOFrame;
        private Vector2 _centerPoint;
        private Vector2Int _centerPointFrame;
        private Main _main;

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

            Seed = WorldGenUtilities.StringToSeed(SeedInput);

            // Set seed
            UnityEngine.Random.InitState(Seed);

            IsometricUtilities.CELLSIZE_X = CELL_SIZE.x;
            IsometricUtilities.CELLSIZE_Y = CELL_SIZE.y;

            MinWorldNoiseValue = float.MaxValue;
            MaxWorldNoiseValue = float.MinValue;


        }

        private void Start()
        {
            _main = Main.Instance;

            _centerPoint = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2f, Screen.height / 2f));


            // Init noise module
            _heightModule = new Perlin(Frequency, Lacunarity, Persistence, Octaves, Seed, QualityMode.High);
            _heatModule = new Perlin(HeatFrequency, HeatLacunarity, HeatPersistence, HeatOctaves, Seed, QualityMode.High);
            _moistureModule = new Perlin(MoistureFrequency, MoistureLacunarity, MoisturePersistence, MoistureOctaves, Seed, QualityMode.High);
            _riverModule = new Perlin(RiverFrequency, RiverLacunarity, RiverPersistence, RiverOctaves, Seed, QualityMode.Low);


            // Load chunks around the player's starting position
            lastChunkISOFrame = IsometricUtilities.ReverseConvertWorldPositionToIsometricFrame(_centerPoint, ChunkWidth, ChunkHeight);


            // World Initialization
            InitWorldAsyncInParallel(lastChunkISOFrame.x, lastChunkISOFrame.y, widthInit: InitWorldWidth, heightInit: InitWorldHeight, () =>
            {
                if (InitFastDrawChunk)
                    LoadChunksAroundPositionInParallel(lastChunkISOFrame.x, lastChunkISOFrame.y, offsetWidth: LoadChunkOffsetWidth, offsetHeight: LoadChunkOffsetHeight);
                else
                    LoadChunksAroundPositionInSequence(lastChunkISOFrame.x, lastChunkISOFrame.y, offsetWidth: LoadChunkOffsetWidth, offsetHeight: LoadChunkOffsetHeight);
            });

        }

        private void Update()
        {
            if(AutoLoadChunk)
            {
                _centerPoint = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2f, Screen.height / 2f));
                _centerPointFrame = IsometricUtilities.ReverseConvertWorldPositionToIsometricFrame(_centerPoint, ChunkWidth, ChunkHeight);

                if (_centerPointFrame != lastChunkISOFrame)
                {
                    lastChunkISOFrame = _centerPointFrame;
                    LoadChunksAroundPositionInSequence(_centerPointFrame.x, _centerPointFrame.y, offsetWidth: LoadChunkOffsetWidth, offsetHeight: LoadChunkOffsetHeight);
                }
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
                            //UIGameManager.Instance.CanvasWorldGen.SetWorldGenSlider(mapProgress);
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
            float minNoiseValue = float.MaxValue;
            float maxNoiseValue = float.MinValue;
            int completedTaskCount = 0;

            //UIGameManager.Instance.CanvasWorldGen.SetWorldGenSlider(0);

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
                        //UIGameManager.Instance.CanvasWorldGen.SetWorldGenSlider(mapProgress);
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
        private async void InitWorldAsyncInSequence(int initIsoFrameX, int initIsoFrameY, byte widthInit, byte heightInit, System.Action onFinished = null)
        {
            //UIGameManager.Instance.DisplayWorldGenSlider(true);
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
                    Chunk newChunk = await GenerateNewChunkDataAsync(x, y);
                    _main.AddNewChunk(newChunk);
                    newChunk.UnloadChunk();


                    // Update the slider value based on progress
                    currentIteration++;
                    float progress = (float)currentIteration / totalIterations;
                    float mapProgress = MathHelper.Map(progress, 0f, 1f, 0.1f, 1.0f);
                    //UIGameManager.Instance.CanvasWorldGen.SetWorldGenSlider(mapProgress);
                }
            }

            await Task.Delay(100);
            //UIGameManager.Instance.DisplayWorldGenSlider(false);
            onFinished?.Invoke();
        }
        private async void InitWorldAsyncInParallel(int initIsoFrameX, int initIsoFrameY, byte widthInit, byte heightInit, System.Action onFinished = null)
        {
            //UIGameManager.Instance.DisplayWorldGenSlider(true);
            await ComputeNoiseRangeAsyncInParallel();

            int completedTaskCount = 0;
            Task<Chunk>[] tasks = new Task<Chunk>[(widthInit * 2 + 1) * (heightInit * 2 + 1)];

   

            for (int x = initIsoFrameX - widthInit; x <= initIsoFrameX + widthInit; x++)
            {
                for (int y = initIsoFrameY - heightInit; y <= initIsoFrameY + heightInit; y++)
                {
                    int index = x - (initIsoFrameX - widthInit) + (y - (initIsoFrameY - heightInit)) * (2 * widthInit + 1);
                    tasks[index] = GenerateNewChunkDataAsync(x, y);

                    // Increment the completed tasks counter
                    Interlocked.Increment(ref completedTaskCount);

                    // Play Slider UI
                    float progress = (float)completedTaskCount / tasks.Length;
                    float mapProgress = MathHelper.Map(progress, 0f, 1f, 0.1f, 1.0f);
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        //UIGameManager.Instance.CanvasWorldGen.SetWorldGenSlider(mapProgress);
                    });
                }
            }

            await Task.WhenAll(tasks);

            foreach (var task in tasks)
            {
                Chunk newChunk = task.Result;
                _main.AddNewChunk(newChunk);
                newChunk.UnloadChunk();
            }

            //UIGameManager.Instance.DisplayWorldGenSlider(false);
            onFinished?.Invoke();
        }
        private async Task<Chunk> GenerateNewChunkDataAsync(int isoFrameX, int isoFrameY)
        {
            Vector2 frame = IsometricUtilities.IsometricFrameToWorldFrame(isoFrameX, isoFrameY);
            Vector3 worldPosition = IsometricUtilities.ConvertIsometricFrameToWorldPosition(isoFrameX, isoFrameY, ChunkWidth, ChunkHeight);
            Chunk newChunk = Instantiate(_chunkPrefab, worldPosition, Quaternion.identity, _chunkParent.transform);
            newChunk.Init(frame.x, frame.y, isoFrameX, isoFrameY, ChunkWidth, ChunkHeight);

            // Create new data
            float[,] heightValues = await GetHeightMapDataAsync(isoFrameX, isoFrameY, ChunkWidth, ChunkHeight);
            float[,] heatValues = await GetHeatMapDataAysnc(isoFrameX, isoFrameY);
            float[,] moisetureValues = await GetMoistureMapDataAsync(isoFrameX, isoFrameY);

            float[,] riverValues = await GetRiverDataAsync(isoFrameX, isoFrameY, ChunkWidth, ChunkHeight);

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

            if(InitWorldWithRiver)
            {
                await newChunk.LoadRiverDataAsync(riverValues);
            }

            return newChunk;
        }

        /// <summary>
        /// Load each chunk in sequence and draw each chunk in sequence. -> Less drop FPS but slow.
        /// </summary>
        /// <param name="isoFrameX"></param>
        /// <param name="isoFrameY"></param>
        /// <param name="offsetWidth"></param>
        /// <param name="offsetHeight"></param>
        private async void LoadChunksAroundPositionInSequence(int isoFrameX, int isoFrameY, byte offsetWidth = 1, byte offsetHeight = 1)
        {
            for (int x = isoFrameX - offsetWidth; x <= isoFrameX + offsetWidth; x++)
            {
                for (int y = isoFrameY - offsetHeight; y <= isoFrameY + offsetHeight; y++)
                {
                    Vector2Int nbIsoFrame = new Vector2Int(x, y);
                    Chunk chunk = _main.GetChunk(nbIsoFrame);
                    if (chunk == null)   // Create new chunk
                    {
                        if (x == isoFrameX && y == isoFrameY)
                        {
                            // Center
                            // ......
                        }

                        Chunk newChunk = await GenerateNewChunkDataAsync(x, y);

                        // Cached chunk data
                        if (_main.HasChunk(nbIsoFrame) == false)
                            _main.AddNewChunk(newChunk);
                        _main.ActiveChunks.Add(newChunk);


                        if (newChunk.ChunkHasDrawn == false)
                        {
                            await newChunk.DrawChunkAsync();
                            _main.Chunks[nbIsoFrame].ShowTextTest();

                            if (InitWorldWithHeatmap)
                                newChunk.PaintHeatMap();
                            if (InitWorldWithMoisturemap)
                                newChunk.PaintMoistureMap();
                        }

                        _main.GetChunk(nbIsoFrame).LoadChunk();
                    }
                    else // Load chunk cached.
                    {
                        if (x == isoFrameX && y == isoFrameY)
                        {
                            // Center
                            // ......
                        }

                        _main.ActiveChunks.Add(_main.Chunks[nbIsoFrame]);

                        if (_main.Chunks[nbIsoFrame].ChunkHasDrawn == false)
                        {
                            await _main.Chunks[nbIsoFrame].DrawChunkAsync();
                            _main.Chunks[nbIsoFrame].ShowTextTest();

                            if (InitWorldWithHeatmap)
                                _main.Chunks[nbIsoFrame].PaintHeatMap();
                            if (InitWorldWithMoisturemap)
                                _main.Chunks[nbIsoFrame].PaintMoistureMap();
                        }
                        _main.Chunks[nbIsoFrame].LoadChunk();
                    }
                }
            }

            if (ShowChunksBorder)
            {
                SortActiveChunkByDepth();
            }

            UpdateAllActiveChunkTileNeighborsAsync();
        }
        /// <summary>
        /// Load all chunks and draw all chunks at the same time. -> Fast but drop a bit FPS in low end devices.
        /// </summary>
        /// <param name="isoFrameX"></param>
        /// <param name="isoFrameY"></param>
        /// <param name="offsetWidth"></param>
        /// <param name="offsetHeight"></param>
        private async void LoadChunksAroundPositionInParallel(int isoFrameX, int isoFrameY, byte offsetWidth = 1, byte offsetHeight = 1)
        {
            Task[] drawChunkTasks = new Task[(offsetWidth * 2 + 1) * (offsetHeight * 2 + 1)];

            for (int x = isoFrameX - offsetWidth; x <= isoFrameX + offsetWidth; x++)
            {
                for (int y = isoFrameY - offsetHeight; y <= isoFrameY + offsetHeight; y++)
                {
                    int index = x - (isoFrameX - offsetWidth) + (y - (isoFrameY - offsetHeight)) * (2 * offsetWidth + 1);
                    Vector2Int nbIsoFrame = new Vector2Int(x, y);
                    Chunk chunk = _main.GetChunk(nbIsoFrame);

                    if (chunk == null)   // Create new chunk
                    {
                        if (x == isoFrameX && y == isoFrameY)
                        {
                            // Center
                            // ......
                        }

                        Chunk newChunk = await GenerateNewChunkDataAsync(x, y);
                        drawChunkTasks[index] = newChunk.DrawChunkAsync();

                        // Cached chunk data
                        if (_main.HasChunk(nbIsoFrame) == false)
                        {
                            _main.AddNewChunk(newChunk);
                        }

                        _main.ActiveChunks.Add(newChunk);
                        _main.Chunks[nbIsoFrame].LoadChunk();

                    }
                    else // Load chunk cached.
                    {
                        if (x == isoFrameX && y == isoFrameY)
                        {
                            // Center
                            // ......
                        }

                        _main.GetChunk(nbIsoFrame).LoadChunk();
                        _main.ActiveChunks.Add(_main.Chunks[nbIsoFrame]);

                        if (_main.Chunks[nbIsoFrame].ChunkHasDrawn == false)
                        {
                            drawChunkTasks[index] = _main.Chunks[nbIsoFrame].DrawChunkAsync();
                        }
                        else
                        {
                            drawChunkTasks[index] = Task.CompletedTask;
                        }
                    }

                }
            }

            // When all chunk has drawn.
            await Task.WhenAll(drawChunkTasks);


            foreach (var chunk in _main.Chunks.Values)
            {
                if (InitWorldWithHeatmap)
                    chunk.PaintHeatMap();

                if (InitWorldWithMoisturemap)
                    chunk.PaintMoistureMap();

                if (!chunk.HasNeighbors())
                {
                    UpdateChunkTileNeighbors(chunk);
                }
            }


            if (ShowChunksBorder)
            {
                SortActiveChunkByDepth();
            }
        }
        #endregion




        #region Generate noise map data.
        public async Task<float[,]> GetHeightMapDataAsync(int isoFrameX, int isoFrameY, int width, int height)
        {
            float[,] heightValues = new float[width, height];

            await Task.Run(() =>
            {
                Parallel.For(0, width, x =>
                {
                    for (int y = 0; y < height; y++)
                    {
                        float offsetX = isoFrameX * width + x;
                        float offsetY = isoFrameY * height + y;
                        float heightValue = (float)_heightModule.GetValue(offsetX, offsetY, 0);
                        float normalizeHeightValue = (heightValue - MinWorldNoiseValue) / (MaxWorldNoiseValue - MinWorldNoiseValue);
                        heightValues[x, y] = normalizeHeightValue;
                    }
                });
            });

            return heightValues;
        }
        public async Task<float[,]> GetHeightMapDataAsync(int isoFrameX, int isoFrameY, int width, int height, float zoom = 0, float offsetXOffset = 0, float offsetYOffset = 0)
        {
            float[,] heightValues = new float[width, height];

            await Task.Run(() =>
            {
                Parallel.For(0, width, x =>
                {
                    for (int y = 0; y < height; y++)
                    {
                        // Calculate the offset from the zoom center
                        float offsetXFromCenter = x - width / 2.0f;
                        float offsetYFromCenter = y - height / 2.0f;

                        // Apply zoom around the specified center
                        offsetXFromCenter *= zoom;
                        offsetYFromCenter *= zoom;

                        // Adjust offsetX and offsetY based on zoom, center, and offsets
                        float offsetX = isoFrameX * width + offsetXFromCenter + offsetXOffset;
                        float offsetY = isoFrameY * height + offsetYFromCenter + offsetYOffset;

                        float heightValue = (float)_heightModule.GetValue(offsetX, offsetY, 0);
                        float normalizeHeightValue = (heightValue - MinWorldNoiseValue) / (MaxWorldNoiseValue - MinWorldNoiseValue);
                        heightValues[x, y] = normalizeHeightValue;
                    }
                });
            });

            return heightValues;
        }
        public async Task<float[,]> GetGradientMapDataAsync(int isoFrameX, int isoFrameY)
        {
            //Debug.Log("GetGradientMapAsync Start");
            float[,] gradientData = new float[ChunkWidth, ChunkHeight];
            isoFrameX = -isoFrameX;
            isoFrameY = -isoFrameY;

            await Task.Run(() =>
            {
                int gradientFrameX = (int)(isoFrameX * ChunkWidth / (float)GradientHeatmapSize);
                int gradientFrameY = -Mathf.CeilToInt(isoFrameY * ChunkHeight / (float)GradientHeatmapSize);

                // Calculate the center of the texture with the offset
                Vector2 gradientOffset = new Vector2(gradientFrameX * GradientHeatmapSize, gradientFrameY * GradientHeatmapSize);
                Vector2 gradientCenterOffset = gradientOffset + new Vector2(isoFrameX * ChunkWidth, isoFrameY * ChunkHeight);


                for (int x = 0; x < ChunkWidth; x++)
                {
                    for (int y = 0; y < ChunkHeight; y++)
                    {
                        Vector2 center = new Vector2(Mathf.FloorToInt(x / GradientHeatmapSize), Mathf.FloorToInt(y / GradientHeatmapSize)) * new Vector2(GradientHeatmapSize, GradientHeatmapSize) + new Vector2(GradientHeatmapSize / 2f, GradientHeatmapSize / 2f);
                        Vector2 centerWithOffset = center + gradientCenterOffset;

                        float distance = Mathf.Abs(y - centerWithOffset.y);
                        float normalizedDistance = 1.0f - Mathf.Clamp01(distance / (GradientHeatmapSize / 2f));
                        gradientData[x, y] = normalizedDistance;
                    }
                }
            });

            //Debug.Log("GetGradientMapAsync Finish");
            return gradientData;
        }
        public async Task<float[,]> GetFractalHeatMapDataAsync(int isoFrameX, int isoFrameY)
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
        public async Task<float[,]> GetHeatMapDataAysnc(int isoFrameX, int isoFrameY)
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
        public async Task<float[,]> GetMoistureMapDataAsync(int isoFrameX, int isoFrameY)
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
        public async Task<float[,]> GetRiverDataAsync(int isoFrameX, int isoFrameY, int width, int height)
        {
            float[,] riverValues = new float[width, height];

            await Task.Run(() =>
            {
                Parallel.For(0, width, x =>
                {
                    for (int y = 0; y < height; y++)
                    {
                        float offsetX = isoFrameX * width + x;
                        float offsetY = isoFrameY * height + y;
                        float riverValue = (float)_riverModule.GetValue(offsetX, offsetY, 0);
                        float normalizeRiverValue = (riverValue - MinWorldNoiseValue) / (MaxWorldNoiseValue - MinWorldNoiseValue);
                        riverValues[x, y] = normalizeRiverValue;
                    }
                });
            });

            return riverValues;
        }
        #endregion





        #region Neighbors
        private void AddChunkFourDirectionNeighbors(Chunk chunk)
        {
            Chunk nbAbove = _main.GetChunkNeighborAbove(chunk);
            Chunk nbBelow = _main.GetChunkNeighborBelow(chunk);
            Chunk nbLeft = _main.GetChunkNeighborLeft(chunk);
            Chunk nbRight = _main.GetChunkNeighborRight(chunk);
            chunk.SetTwoSidesChunkNeighbors(nbLeft, nbRight, nbAbove, nbBelow);
        }
        private void UpdateChunkTileNeighbors(Chunk chunk)
        {
            // Find chunk neighbors
            Chunk nbAbove = _main.GetChunkNeighborAbove(chunk);
            Chunk nbBelow = _main.GetChunkNeighborBelow(chunk);
            Chunk nbLeft = _main.GetChunkNeighborLeft(chunk);
            Chunk nbRight = _main.GetChunkNeighborRight(chunk);
            chunk.SetTwoSidesChunkNeighbors(nbLeft, nbRight, nbAbove, nbBelow);

            if (chunk.HasNeighbors() && !chunk.AllTileHasNeighbors)
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

            if (nbAbove != null && nbAbove.HasNeighbors() && !nbAbove.AllTileHasNeighbors)
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
            if (nbBelow != null && nbBelow.HasNeighbors() && !nbBelow.AllTileHasNeighbors)
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
            if (nbLeft != null && nbLeft.HasNeighbors() && !nbLeft.AllTileHasNeighbors)
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
            if (nbRight != null && nbRight.HasNeighbors() && !nbRight.AllTileHasNeighbors)
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
        private async void UpdateChunkTileNeighborsAsync(Chunk chunk)
        {
            // Find chunk neighbors
            Chunk nbAbove = _main.GetChunkNeighborAbove(chunk);
            Chunk nbBelow = _main.GetChunkNeighborBelow(chunk);
            Chunk nbLeft = _main.GetChunkNeighborLeft(chunk);
            Chunk nbRight = _main.GetChunkNeighborRight(chunk);
            chunk.SetTwoSidesChunkNeighbors(nbLeft, nbRight, nbAbove, nbBelow);

            Task[] tasks = new Task[5];

            if (chunk.HasNeighbors() && !chunk.AllTileHasNeighbors)
            {
                tasks[0] = chunk.UpdateAllTileNeighborsAsync();
            }
            else
                tasks[0] = Task.CompletedTask;

            if (nbAbove != null && nbAbove.HasNeighbors() && !nbAbove.AllTileHasNeighbors)
                tasks[1] = nbAbove.UpdateAllTileNeighborsAsync();
            else
                tasks[1] = Task.CompletedTask;

            if (nbBelow != null && nbBelow.HasNeighbors() && !nbBelow.AllTileHasNeighbors)
                tasks[2] = nbBelow.UpdateAllTileNeighborsAsync();
            else
                tasks[2] = Task.CompletedTask;

            if (nbLeft != null && nbLeft.HasNeighbors() && !nbLeft.AllTileHasNeighbors)
                tasks[3] = nbLeft.UpdateAllTileNeighborsAsync();
            else
                tasks[3] = Task.CompletedTask;

            if (nbRight != null && nbRight.HasNeighbors() && !nbRight.AllTileHasNeighbors)
                tasks[4] = nbRight.UpdateAllTileNeighborsAsync();
            else
                tasks[4] = Task.CompletedTask;

            await Task.WhenAll(tasks);


            if (chunk.AllTileHasNeighbors)
            {
                if (PaintTileNeighbors)
                    chunk.PaintNeighborsColor();

                if (chunk.Waters.Count == 0 && chunk.Lands.Count == 0)
                {
                    FloodFill(chunk);
                    chunk.PaintTilegroupMap();
                }
            }
            if (nbAbove != null && nbAbove.AllTileHasNeighbors)
            {
                if (PaintTileNeighbors)
                    nbAbove.PaintNeighborsColor();

                if (nbAbove.Waters.Count == 0 && nbAbove.Lands.Count == 0)
                {
                    FloodFill(nbAbove);
                    nbAbove.PaintTilegroupMap();
                }
            }
            if (nbBelow != null && nbBelow.AllTileHasNeighbors)
            {
                if (PaintTileNeighbors)
                    nbBelow.PaintNeighborsColor();

                if (nbBelow.Waters.Count == 0 && nbBelow.Lands.Count == 0)
                {
                    FloodFill(nbBelow);
                    nbBelow.PaintTilegroupMap();
                }
            }
            if (nbLeft != null && nbLeft.AllTileHasNeighbors)
            {
                if (PaintTileNeighbors)
                    nbLeft.PaintNeighborsColor();

                if (nbLeft.Waters.Count == 0 && nbLeft.Lands.Count == 0)
                {
                    FloodFill(nbLeft);
                    nbLeft.PaintTilegroupMap();
                }
            }
            if (nbRight != null && nbRight.AllTileHasNeighbors)
            {
                if (PaintTileNeighbors)
                    nbRight.PaintNeighborsColor();

                if (nbRight.Waters.Count == 0 && nbRight.Lands.Count == 0)
                {
                    FloodFill(nbRight);
                    nbRight.PaintTilegroupMap();
                }
            }

        }
        private void UpdateAllActiveChunkTileNeighborsAsync()
        {
            foreach (Chunk activeChunk in _main.ActiveChunks)
            {
                if (activeChunk.AllTileHasNeighbors == false)
                {
                    UpdateChunkTileNeighborsAsync(activeChunk);
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

        /// <summary>
        /// Sort active chunks fix some isometric chunk has wrong order (Visualization).
        /// </summary>
        private void SortActiveChunkByDepth(bool inverse = false)
        {
            int depth = 0;
            List<Chunk> chunkList = _main.ActiveChunks.ToList();

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

