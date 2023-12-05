using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using LibNoise;
using LibNoise.Generator;
using Sirenix.OdinInspector;
using PixelMiner.Utilities;
using PixelMiner.Enums;
using PixelMiner.WorldBuilding;
using System;

namespace PixelMiner.WorldGen
{
    /* 
     * 1. Generate Chunk (Instantiate, Init data, 
     * 2. Generate and load data into chunk (data include: Height map, heat map, moisture map,...)
     * 3. Draw chunk - Create mesh object can be shown in Unity Engine.
     */
    public class WorldGeneration : MonoBehaviour
    {
        private readonly object lockObject = new object(); // Define a lock object for thread safety
        public static WorldGeneration Instance { get; private set; }
        public event System.Action OnWorldGenWhenStartFinished;

        #region Fileds and Variables
        [FoldoutGroup("References"), SerializeField] private Chunk _chunkPrefab;
        [FoldoutGroup("References"), SerializeField] private Transform _chunkParent;

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
        [FoldoutGroup("Heatmap"), Indent(1), ProgressBar(0f, 1f, r: 170 / 255f, g: 1f, b: 1f, Height = 20)] public float ColderValue = 0.2f;
        [FoldoutGroup("Heatmap"), Indent(1), ProgressBar(0f, 1f, r: 0, g: 229 / 255f, b: 133 / 255f, Height = 20)] public float ColdValue = 0.4f;
        [FoldoutGroup("Heatmap"), Indent(1), ProgressBar(0f, 1f, r: 1, g: 1, b: 100 / 255f, Height = 20)] public float WarmValue = 0.6f;
        [FoldoutGroup("Heatmap"), Indent(1), ProgressBar(0f, 1f, r: 1, g: 100 / 255f, b: 0, Height = 20)] public float WarmerValue = 0.8f;
        [FoldoutGroup("Heatmap"), Indent(1), ProgressBar(0f, 1f, r: 241 / 255f, g: 12 / 255f, b: 0f, Height = 20)] public float WarmestValue = 1.0f;
        private ModuleBase _heatModule;



        // Moisture map
        // ======
        [InfoBox("Moisture map noise settings")]
        [FoldoutGroup("Moisture map"), Indent(1)] public int MoistureOctaves = 4;
        [FoldoutGroup("Moisture map"), Indent(1)] public double MoistureFrequency = 0.03;
        [FoldoutGroup("Moisture map"), Indent(1)] public double MoistureLacunarity = 2.0f;
        [FoldoutGroup("Moisture map"), Indent(1)] public double MoisturePersistence = 0.5f;
        [InfoBox("Moisture map noise settings"), Space(10)]
        [FoldoutGroup("Moisture map"), Indent(1), ProgressBar(0f, 1f, r: 255 / 255f, 139 / 255f, 17 / 255f, Height = 20)] public float DryestValue = 0.22f;
        [FoldoutGroup("Moisture map"), Indent(1), ProgressBar(0f, 1f, r: 245 / 255f, g: 245 / 255f, b: 23 / 255f, Height = 20)] public float DryerValue = 0.35f;
        [FoldoutGroup("Moisture map"), Indent(1), ProgressBar(0f, 1f, r: 80 / 255f, g: 255 / 255f, b: 0 / 255f, Height = 20)] public float DryValue = 0.55f;
        [FoldoutGroup("Moisture map"), Indent(1), ProgressBar(0f, 1f, r: 85 / 255f, g: 255 / 255f, b: 255 / 255f, Height = 20)] public float WetValue = 0.75f;
        [FoldoutGroup("Moisture map"), Indent(1), ProgressBar(0f, 1f, r: 20 / 255f, g: 70 / 255f, b: 255 / 255f, Height = 20)] public float WetterValue = 0.85f;
        [FoldoutGroup("Moisture map"), Indent(1), ProgressBar(0f, 1f, r: 0 / 255f, g: 0 / 255f, b: 100 / 255f, Height = 20)] public float WettestValue = 1.0f;
        private ModuleBase _moistureModule;



        // River
        // =====
        [FoldoutGroup("River"), Indent(1)] public int RiverOctaves = 4;
        [FoldoutGroup("River"), Indent(1)] public double RiverFrequency = 0.02;
        [FoldoutGroup("River"), Indent(1)] public double RiverLacunarity = 2.0f;
        [FoldoutGroup("River"), Indent(1)] public double RiverPersistence = 0.5f;
        [FoldoutGroup("River"), Indent(1), MinMaxSlider(0f, 1f, true)] public Vector2 RiverRange = new Vector2(0.7f, 0.75f);
        private ModuleBase _riverModule;



        [Header("Color")]
        // Height
        public static Color RiverColor = new Color(30 / 255f, 120 / 255f, 200 / 255f, 1);
        public static Color DeepColor = new Color(0, 0, 0.5f, 1);
        public static Color ShallowColor = new Color(25 / 255f, 25 / 255f, 150 / 255f, 1);
        public static Color SandColor = new Color(240 / 255f, 240 / 255f, 64 / 255f, 1);
        public static Color GrassColor = new Color(50 / 255f, 220 / 255f, 20 / 255f, 1);
        public static Color ForestColor = new Color(16 / 255f, 160 / 255f, 0, 1);
        public static Color RockColor = new Color(0.5f, 0.5f, 0.5f, 1);
        public static Color SnowColor = new Color(1, 1, 1, 1);
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
        private byte _chunkWidth;      // Size of each chunk in tiles
        private byte _chunkHeight;      // Size of each chunk in tiles
        private byte _chunkDepth;
        private byte _calculateNoiseRangeSampleMultiplier = 15;  // 50 times. 50 * 100000 = 5 mil times.
        private int _calculateNoiseRangeCount = 1000000;    // 1 mil times.
        private float _minWorldNoiseValue = float.MaxValue;
        private float _maxWorldNoiseValue = float.MinValue;
        private Main _main;
        private WorldLoading _worldLoading;
        #endregion


        #region Properties
        [FoldoutGroup("World Properties"), Indent(1), ReadOnly, ShowInInspector] public int Seed { get; private set; }
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

            _minWorldNoiseValue = float.MaxValue;
            _maxWorldNoiseValue = float.MinValue;

            // Init noise module
            _heightModule = new Perlin(Frequency, Lacunarity, Persistence, Octaves, Seed, QualityMode.High);
            _heatModule = new Perlin(HeatFrequency, HeatLacunarity, HeatPersistence, HeatOctaves, Seed, QualityMode.High);
            _moistureModule = new Perlin(MoistureFrequency, MoistureLacunarity, MoisturePersistence, MoistureOctaves, Seed, QualityMode.High);
            _riverModule = new Perlin(RiverFrequency, RiverLacunarity, RiverPersistence, RiverOctaves, Seed, QualityMode.Low);
        }

        private void Start()
        {
            _main = Main.Instance;
            _worldLoading = WorldLoading.Instance;

            Seed = WorldGenUtilities.StringToSeed(_main.SeedInput);
            UnityEngine.Random.InitState(Seed);

           
            _chunkWidth = _main.ChunkWidth;
            _chunkHeight = _main.ChunkHeight;
            _chunkDepth = _main.ChunkDepth;


            // World Initialization
            InitWorldAsyncInSequence(_worldLoading.LastChunkFrame.x, _worldLoading.LastChunkFrame.z, widthInit: _worldLoading.InitWorldWidth, depthInit: _worldLoading.InitWorldDepth, () =>
            {
                OnWorldGenWhenStartFinished?.Invoke();
            });

            Chunk.OnChunkHasNeighbors += DrawChunk;

        }

        private void OnDestroy()
        {
            Chunk.OnChunkHasNeighbors -= DrawChunk;
        }


        private async void DrawChunk(Chunk chunk)
        {
            chunk.State = Chunk.ChunkState.Processing;
            chunk.Left.State = Chunk.ChunkState.Processing;
            chunk.Right.State = Chunk.ChunkState.Processing;
            chunk.Front.State = Chunk.ChunkState.Processing;
            chunk.Back.State = Chunk.ChunkState.Processing;
            if (!chunk.ChunkHasDrawn)
            {
               
                chunk.DrawChunkAsync();
            }
            chunk.State = Chunk.ChunkState.Stable;
            chunk.Left.State = Chunk.ChunkState.Stable;
            chunk.Right.State = Chunk.ChunkState.Stable;
            chunk.Front.State = Chunk.ChunkState.Stable;
            chunk.Back.State = Chunk.ChunkState.Stable;
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


            _minWorldNoiseValue = minNoiseValue;
            _maxWorldNoiseValue = maxNoiseValue;

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
            _minWorldNoiseValue = minNoiseValue;
            _maxWorldNoiseValue = maxNoiseValue;
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
        private async void InitWorldAsyncInSequence(int initFrameX, int initFrameZ, byte widthInit, byte depthInit, System.Action onFinished = null)
        {
            await ComputeNoiseRangeAsyncInParallel();

            Debug.Log($"min: {_minWorldNoiseValue}");
            Debug.Log($"max: {_maxWorldNoiseValue}");

            for (int x = initFrameX - widthInit; x <= initFrameX + widthInit; x++)
            {
                for (int z = initFrameZ - depthInit; z <= initFrameZ + depthInit; z++)
                {
                    Chunk newChunk = await GenerateNewChunk(x,0,z);
                    _worldLoading.LoadChunk(newChunk);
                    UpdateChunkNeighbors(newChunk);

                    //float[] heightValues = await GetHeightMapDataAsync(newChunk.FrameX, newChunk.FrameZ, _chunkWidth, _chunkDepth);
                    //await LoadHeightMapDataAsync(newChunk, heightValues);
                    //UpdateChunkNeighbors(newChunk);      
                }
            }

            onFinished?.Invoke();
        }



        public async Task<Chunk> GenerateNewChunk(int frameX, int frameY, int frameZ)
        {
            Vector3Int frame = new Vector3Int(frameX, frameY, frameZ);
            Vector3 worldPosition = frame * new Vector3Int(Main.Instance.ChunkWidth, Main.Instance.ChunkHeight, Main.Instance.ChunkDepth);
            Chunk newChunk = Instantiate(_chunkPrefab, worldPosition, Quaternion.identity, _chunkParent.transform);
            newChunk.Init(frameX, frameY, frameZ, _chunkWidth, _chunkHeight, _chunkDepth);

            float[] heightValues = await GetHeightMapDataAsync(newChunk.FrameX, newChunk.FrameZ, Main.Instance.ChunkWidth, Main.Instance.ChunkDepth);
            await LoadHeightMapDataAsync(newChunk, heightValues);

            // Create new data
            //float[] heightValues = await GetHeightMapDataAsync(frameX, frameZ, _chunkWidth, _chunkHeight, _chunkDepth);
            //float[] heatValues = await GetHeatMapDataAysnc(frameX, frameZ);
            //float[] moisetureValues = await GetMoistureMapDataAsync(frameX, frameZ);
            //float[,] riverValues = await GetRiverDataAsync(frameX, frameZ, _chunkWidth, _chunkHeight);

            //if (_main.InitWorldWithHeatmap)
            //{
            //    await LoadHeightAndHeatMap(newChunk, heightValues, heatValues);
            //}
            //else
            //{
            //    await LoadHeightMapDataAsync(newChunk, heightValues);
            //}

            //if (_main.InitWorldWithMoisturemap)
            //{
            //    await LoadMoistureMapDataAsync(newChunk, moisetureValues);
            //}
            //else
            //{

            //}

            //if (_main.InitWorldWithRiver)
            //{
            //    await LoadRiverDataAsync(newChunk, riverValues);
            //}

            return newChunk;
        }


   

        #endregion




        #region Generate noise map data.
        /// <summary>
        /// Return 2D noise height map.
        /// </summary>
        /// <param name="frameX"></param>
        /// <param name="frameZ"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public async Task<float[]> GetHeightMapDataAsync(int frameX, int frameZ, int width, int depth)
        {
            float[] heightValues = new float[width * depth];

            await Task.Run(() =>
            {
                Parallel.For(0, width, x =>
                {
                    for (int z = 0; z < depth; z++)
                    {
                        float offsetX = frameX * width + x;
                        float offsetZ = frameZ * depth + z;
                        float heightValue = (float)_heightModule.GetValue(offsetX, 0, offsetZ);
                        float normalizeHeightValue = (heightValue - _minWorldNoiseValue) / (_maxWorldNoiseValue - _minWorldNoiseValue);
                        heightValues[WorldGenUtilities.IndexOf(x, z, width)] = normalizeHeightValue;
                    }
                });
            });

            return heightValues;
        }
        //public async Task<float[]> GetHeightMapDataAsync(int isoFrameX, int isoFrameY, int width, int height, int depth, float zoom = 0, float offsetXOffset = 0, float offsetYOffset = 0)
        //{
        //    float[] heightValues = new float[width * height * depth];

        //    await Task.Run(() =>
        //    {
        //        Parallel.For(0, width, x =>
        //        {
        //            for (int y = 0; y < height; y++)
        //            {
        //                // Calculate the offset from the zoom center
        //                float offsetXFromCenter = x - width / 2.0f;
        //                float offsetYFromCenter = y - height / 2.0f;

        //                // Apply zoom around the specified center
        //                offsetXFromCenter *= zoom;
        //                offsetYFromCenter *= zoom;

        //                // Adjust offsetX and offsetY based on zoom, center, and offsets
        //                float offsetX = isoFrameX * width + offsetXFromCenter + offsetXOffset;
        //                float offsetY = isoFrameY * height + offsetYFromCenter + offsetYOffset;

        //                float heightValue = (float)_heightModule.GetValue(offsetX, offsetY, 0);
        //                float normalizeHeightValue = (heightValue - _minWorldNoiseValue) / (_maxWorldNoiseValue - _minWorldNoiseValue);
        //                heightValues[x, y] = normalizeHeightValue;
        //            }
        //        });
        //    });

        //    return heightValues;
        //}
        public async Task<float[]> GetGradientMapDataAsync(int frameX, int frameZ)
        {

            float[] gradientData = new float[_chunkWidth * _chunkDepth];


            await Task.Run(() =>
            {
                int gradientFrameX = (int)(frameX * _chunkWidth / (float)GradientHeatmapSize);
                int gradientFrameZ = -Mathf.CeilToInt(frameZ * _chunkDepth / (float)GradientHeatmapSize);

                // Calculate the center of the texture with the offset
                Vector2 gradientOffset = new Vector2(gradientFrameX * GradientHeatmapSize, gradientFrameZ * GradientHeatmapSize);
                Vector2 gradientCenterOffset = gradientOffset + new Vector2(frameX * _chunkWidth, frameZ * _chunkDepth);


                for (int x = 0; x < _chunkWidth; x++)
                {
                    for (int z = 0; z < _chunkDepth; z++)
                    {
                        Vector2 center = new Vector2(Mathf.FloorToInt(x / GradientHeatmapSize), Mathf.FloorToInt(z / GradientHeatmapSize)) * new Vector2(GradientHeatmapSize, GradientHeatmapSize) + new Vector2(GradientHeatmapSize / 2f, GradientHeatmapSize / 2f);
                        Vector2 centerWithOffset = center + gradientCenterOffset;

                        float distance = Mathf.Abs(z - centerWithOffset.y);
                        float normalizedDistance = 1.0f - Mathf.Clamp01(distance / (GradientHeatmapSize / 2f));
                        gradientData[WorldGenUtilities.IndexOf(x, z, _chunkWidth)] = normalizedDistance;
                    }
                }
            });

            //Debug.Log("GetGradientMapAsync Finish");
            return gradientData;
        }
        public async Task<float[]> GetFractalHeatMapDataAsync(int frameX, int frameZ)
        {
            //Debug.Log("GetFractalHeatMapAsync Start");

            float[] fractalNoiseData = new float[_chunkWidth * _chunkDepth];

            await Task.Run(() =>
            {
                for (int x = 0; x < _chunkWidth; x++)
                {
                    for (int z = 0; z < _chunkDepth; z++)
                    {
                        float offsetX = frameX * _chunkWidth + x;
                        float offsetZ = frameZ * _chunkDepth + z;
                        float heatValue = (float)_heatModule.GetValue(offsetX, 0, offsetZ);
                        float normalizeHeatValue = (heatValue - _minWorldNoiseValue) / (_maxWorldNoiseValue - _minWorldNoiseValue);

                        fractalNoiseData[WorldGenUtilities.IndexOf(x, z, _chunkWidth)] = normalizeHeatValue;
                    }
                }
            });

            //Debug.Log("GetFractalHeatMapAsync Finish");
            return fractalNoiseData;
        }
        public async Task<float[]> GetHeatMapDataAysnc(int frameX, int frameZ)
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
            Task<float[]> gradientTask = GetGradientMapDataAsync(frameX, frameZ);
            Task<float[]> fractalNoiseTask = GetFractalHeatMapDataAsync(frameX, frameZ);

            // Await for both tasks to complete
            await Task.WhenAll(gradientTask, fractalNoiseTask);
            float[] gradientValues = gradientTask.Result;
            float[] fractalNoiseValues = fractalNoiseTask.Result;

            // Blend the maps
            float[] heatValues = WorldGenUtilities.BlendMapData(gradientValues, fractalNoiseValues, HeatMapBlendFactor);
            return heatValues;
        }
        public async Task<float[]> GetMoistureMapDataAsync(int frameX, int frameZ)
        {
            float[] moistureData = new float[_chunkWidth * _chunkDepth];

            await Task.Run(() =>
            {
                for (int x = 0; x < _chunkWidth; x++)
                {
                    for (int z = 0; z < _chunkDepth; z++)
                    {
                        float offsetX = frameX * _chunkWidth + x;
                        float offsetZ = frameZ * _chunkDepth + z;
                        float moisetureValue = (float)_moistureModule.GetValue(offsetX, 0, offsetZ);
                        float normalizeMoistureValue = (moisetureValue - _minWorldNoiseValue) / (_maxWorldNoiseValue - _minWorldNoiseValue);

                        moistureData[WorldGenUtilities.IndexOf(x, z, _chunkWidth)] = normalizeMoistureValue;
                    }
                }
            });

            return moistureData;
        }
        #endregion




        #region River
        public async Task<float[,]> GetRiverDataAsync(int frameX, int frameZ, int width, int height)
        {
            float[,] riverValues = new float[width, height];

            await Task.Run(() =>
            {
                Parallel.For(0, width, x =>
                {
                    for (int y = 0; y < height; y++)
                    {
                        float offsetX = frameX * width + x;
                        float offsetZ = frameZ * height + y;
                        float riverValue = (float)_riverModule.GetValue(offsetX, offsetZ, 0);
                        float normalizeRiverValue = (riverValue - _minWorldNoiseValue) / (_maxWorldNoiseValue - _minWorldNoiseValue);
                        riverValues[x, y] = normalizeRiverValue;
                    }
                });
            });

            return riverValues;
        }
        #endregion




        public async Task LoadHeightMapDataAsync(Chunk chunk, float[] heightValues)
        {
            await Task.Run(() =>
            {         
                //Debug.Log($"Ground: {Mathf.FloorToInt(Water * _chunkHeight)}");
                for (int x = 0; x < _chunkWidth; x++)
                {
                    for (int z = 0; z < _chunkDepth; z++)
                    {
                        for (int y = 0; y < _chunkHeight; y++)
                        {
                            float heightValue = heightValues[WorldGenUtilities.IndexOf(x, z, _chunkWidth)];
                            chunk.HeightValues[WorldGenUtilities.IndexOf(x, z, _chunkWidth)] = heightValue;

                            int index3D = WorldGenUtilities.IndexOf(x, y, z, _chunkWidth, _chunkHeight);
                            int indexHighestY = WorldGenUtilities.IndexOf(x, _chunkHeight - 1, z, _chunkWidth, _chunkHeight);
                            int averageGroundLayer = Mathf.FloorToInt(Water * _chunkHeight);
           
                            int terrainHeight = Mathf.FloorToInt(heightValue * _chunkHeight);
                          
                    
                            if(y <= averageGroundLayer) 
                            {
                                if (y < terrainHeight)
                                {
                                    if (heightValue < Water)
                                        chunk.ChunkData[index3D] = BlockType.Water;
                                    if (heightValue < Sand)
                                        chunk.ChunkData[index3D] = BlockType.Sand;
                                    else if (heightValue < Grass)
                                        chunk.ChunkData[index3D] = BlockType.Dirt;
                                    else if (heightValue < Forest)
                                        chunk.ChunkData[index3D] = BlockType.GrassSide;
                                    else if (heightValue < Rock)
                                        chunk.ChunkData[index3D] = BlockType.Stone;
                                    else
                                        chunk.ChunkData[index3D] = BlockType.Glass;
                                }
                                else if (y < _chunkHeight * Water)
                                {
                                    chunk.ChunkData[index3D] = BlockType.Water;
                                }
                                else
                                {
                                    chunk.ChunkData[index3D] = BlockType.Air;
                                }
                            }
                            else
                            {
                                chunk.ChunkData[index3D] = BlockType.Air;
                            }                            
                        }
                    }
                }

                //for (int x = 0; x < _chunkWidth; x++)
                //{
                //    for (int z = 0; z < _chunkDepth; z++)
                //    {
                //        for (int y = 0; y < _chunkHeight; y++)
                //        {
                //            float heightValue = heightValues[WorldGenUtilities.IndexOf(x, z, _chunkWidth)];
                //            chunk.HeightValues[WorldGenUtilities.IndexOf(x, z, _chunkWidth)] = heightValue;

                //            int index3D = WorldGenUtilities.IndexOf(x, y, z, _chunkWidth, _chunkHeight);
                //            int indexHighestY = WorldGenUtilities.IndexOf(x, _chunkHeight - 1, z, _chunkWidth, _chunkHeight);


                //            //if (heightValue < DeepWater)
                //            //{
                //            //    chunk.ChunkData[index3D] = BlockType.Water;
                //            //}
                //            //else if (heightValue < Water)
                //            //{
                //            //    chunk.ChunkData[index3D] = BlockType.Water;
                //            //}
                //            //else if (heightValue < Sand)
                //            //{
                //            //    chunk.ChunkData[index3D] = BlockType.Sand;
                //            //}
                //            //else if (heightValue < Grass)
                //            //{
                //            //    chunk.ChunkData[index3D] = BlockType.GrassSide;
                //            //}
                //            //else if (heightValue < Forest)
                //            //{
                //            //    chunk.ChunkData[index3D] = BlockType.GrassSide;
                //            //}
                //            //else if (heightValue < Rock)
                //            //{
                //            //    chunk.ChunkData[index3D] = BlockType.Stone;
                //            //}
                //            //else
                //            //{
                //            //    chunk.ChunkData[index3D] = BlockType.Stone;
                //            //}



                //            //int terrainHeight = Mathf.FloorToInt(heightValue * _chunkHeight);
                //            //if (y == terrainHeight)
                //            //{
                //            //    chunk.ChunkData[indexHighestY] = BlockType.GrassSide;
                //            //    continue;
                //            //}
                //            //else if (y < terrainHeight)
                //            //{
                //            //    chunk.ChunkData[indexHighestY] = BlockType.Dirt;
                //            //    continue;
                //            //}
                //            //else if (y == _chunkHeight - 1)
                //            //{
                //            //    // Top
                //            //    //chunk.ChunkData[index3D] = BlockType.Glass;
                //            //}
                //            //else if (y < _chunkHeight * Water)
                //            //{
                //            //    chunk.ChunkData[index3D] = BlockType.Water;
                //            //    continue;
                //            //}
                //            //else
                //            //{
                //            //    chunk.ChunkData[index3D] = BlockType.Air;
                //            //}




                //            if (heightValue < Water)
                //            {
                //                if (y < _chunkHeight * Water)
                //                {
                //                    chunk.ChunkData[index3D] = BlockType.Stone;
                //                }
                //                else
                //                    chunk.ChunkData[index3D] = BlockType.Water;
                //            }
                //            else if (heightValue < Sand)
                //            {
                //                chunk.ChunkData[index3D] = BlockType.Sand;
                //            }
                //            else if (heightValue < Grass)
                //            {
                //                chunk.ChunkData[index3D] = BlockType.GrassSide;
                //            }
                //            else if (heightValue < Forest)
                //            {
                //                chunk.ChunkData[index3D] = BlockType.GrassSide;
                //            }
                //            else if (heightValue < Rock)
                //            {
                //                chunk.ChunkData[index3D] = BlockType.Stone;
                //            }
                //            else
                //            {
                //                chunk.ChunkData[index3D] = BlockType.Stone;
                //            }
                //        }
                //    }
                //}

                chunk.HasData = true;
            });
        }


#if false
        public async Task LoadHeatMapDataAsync(Chunk chunk, float[,] heatValues, bool updateHeatType = true)
        {
            chunk.Processing = true;
            await Task.Run(() =>
            {
                Parallel.For(0, _chunkWidth, x =>
                {
                    for (int y = 0; y < _chunkHeight; y++)
                    {
                        Tile tile = chunk.ChunkData.GetValue(x, y);
                        tile.HeatValue = heatValues[x, y];

                        if (updateHeatType)
                        {
                            UpdateHeatType(tile);
                        }
                    }
                });
            });
            chunk.Processing = false;
        }

        /// <summary>
        /// Load heightmap and heatmap simulteneously and
        /// ajdust heatmap based on heightmap.(The higher you go, the lower the temperature)
        /// </summary>
        /// <param name="heightValues"></param>
        /// <returns></returns>
        public async Task LoadHeightAndHeatMap(Chunk chunk, float[,] heightValues, float[,] heatValues)
        {
            chunk.Processing = true;
            Task loadHeightMapDataTask = LoadHeightMapDataAsync(chunk, heightValues, updateHeightType: true);
            Task loadHeatMapDataTask = LoadHeatMapDataAsync(chunk, heatValues, updateHeatType: false); // updateHeatType = false because we will update it after adjust by heightmap.

            await Task.WhenAll(loadHeatMapDataTask, loadHeightMapDataTask);

            await Task.Run(() =>
            {
                for (int x = 0; x < _chunkWidth; x++)
                {
                    for (int y = 0; y < _chunkHeight; y++)
                    {
                        Tile tile = chunk.ChunkData.GetValue(x, y);


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
            chunk.Processing = false;
        }

        public async Task LoadMoistureMapDataAsync(Chunk chunk, float[,] moisetureValues, bool updateMoistureType = true)
        {
            chunk.Processing = true;
            await Task.Run(() =>
            {
                Parallel.For(0, _chunkWidth, x =>
                {
                    for (int y = 0; y < _chunkHeight; y++)
                    {
                        Tile tile = chunk.ChunkData.GetValue(x, y);
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
            chunk.Processing = false;
        }
        public async Task LoadRiverDataAsync(Chunk chunk, float[,] riverValues)
        {
            chunk.Processing = true;
            await Task.Run(() =>
            {
                Parallel.For(0, _chunkWidth, x =>
                {
                    for (int y = 0; y < _chunkHeight; y++)
                    {
                        Tile tile = chunk.ChunkData.GetValue(x, y);
                        float riverValue = riverValues[x, y];

                        if (riverValue > WorldGeneration.Instance.RiverRange.x && riverValue < WorldGeneration.Instance.RiverRange.y)
                        {
                            tile.HeightType = HeightType.River;
                        }
                    }
                });

            });
            chunk.Processing = false;
        }

        private void UpdateHeightType(Tile tile)
        {
            if (tile.HeightValue < DeepWater)
            {
                tile.HeightType = HeightType.DeepWater;
                tile.Collidable = false;
            }
            else if (tile.HeightValue < Water)
            {
                tile.HeightType = HeightType.ShallowWater;
                tile.Collidable = false;
            }
            else if (tile.HeightValue < Sand)
            {
                tile.HeightType = HeightType.Sand;
                tile.Collidable = true;
            }
            else if (tile.HeightValue < Grass)
            {
                tile.HeightType = HeightType.Grass;
                tile.Collidable = true;
            }
            else if (tile.HeightValue < Forest)
            {
                tile.HeightType = HeightType.Forest;
                tile.Collidable = true;
            }
            else if (tile.HeightValue < Rock)
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
            if (tile.HeatValue < ColdestValue)
            {
                tile.HeatType = HeatType.Coldest;
            }
            else if (tile.HeatValue < ColderValue)
            {
                tile.HeatType = HeatType.Colder;
            }
            else if (tile.HeatValue < ColdValue)
            {
                tile.HeatType = HeatType.Cold;
            }
            else if (tile.HeatValue < WarmValue)
            {
                tile.HeatType = HeatType.Warm;
            }
            else if (tile.HeatValue < WarmerValue)
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
            if (tile.MoistureValue < DryestValue)
            {
                tile.MoistureType = MoistureType.Dryest;
            }
            else if (tile.MoistureValue < DryerValue)
            {
                tile.MoistureType = MoistureType.Dryer;
            }
            else if (tile.MoistureValue < DryValue)
            {
                tile.MoistureType = MoistureType.Dry;
            }
            else if (tile.MoistureValue < WetValue)
            {
                tile.MoistureType = MoistureType.Wet;
            }
            else if (tile.MoistureValue < WetterValue)
            {
                tile.MoistureType = MoistureType.Wetter;
            }
            else
            {
                tile.MoistureType = MoistureType.Wettest;
            }
        }
   


        #region Paint Color
        public void PaintNeighborsColor(Chunk2D chunk)
        {
            for (var x = 0; x < _chunkWidth; x++)
            {
                for (var y = 0; y < _chunkHeight; y++)
                {
                    Tile t = chunk.ChunkData.GetValue(x, y);
                    if (t.HasNeighbors())
                    {
                        chunk.LandTilemap.SetColor(new Vector3Int(t.FrameX, t.FrameY), Color.red);
                    }
                }
            }
        }

        public void PaintTilegroupMap(Chunk2D chunk)
        {
            foreach (var group in chunk.Waters)
            {
                foreach (var tile in group.Tiles)
                {
                    Vector3Int tileFrame = new Vector3Int(tile.FrameX, tile.FrameY, 0);
                    chunk.Tilegroupmap.SetColor(tileFrame, Color.blue);
                }
            }

            foreach (var group in chunk.Lands)
            {
                foreach (var tile in group.Tiles)
                {
                    Vector3Int tileFrame = new Vector3Int(tile.FrameX, tile.FrameY, 0);
                    chunk.Tilegroupmap.SetColor(tileFrame, Color.green);
                }
            }
        }

        public void PaintHeatMap(Chunk2D chunk)
        {
            for (var x = 0; x < _chunkWidth; x++)
            {
                for (var y = 0; y < _chunkHeight; y++)
                {
                    Tile t = chunk.ChunkData.GetValue(x, y);
                    chunk.HeatTilemap.SetColor(new Vector3Int(t.FrameX, t.FrameY), GetGradientColor(t.HeatValue));
                }
            }
        }

        public void PaintMoistureMap(Chunk2D chunk)
        {
            for (int x = 0; x < _chunkWidth; x++)
            {
                for (int y = 0; y < _chunkHeight; y++)
                {
                    Tile t = chunk.ChunkData.GetValue(x, y);
                    Color color;
                    switch (t.MoistureType)
                    {
                        case MoistureType.Dryest:
                            color = Dryest;
                            break;
                        case MoistureType.Dryer:
                            color = Dryer;
                            break;
                        case MoistureType.Dry:
                            color = Dry;
                            break;
                        case MoistureType.Wet:
                            color = Wet;
                            break;
                        case MoistureType.Wetter:
                            color = Wetter;
                            break;
                        case MoistureType.Wettest:
                            color = Wettest;
                            break;
                        default:
                            color = Wettest;
                            break;
                    }

                    chunk.MoistureTilemap.SetColor(new Vector3Int(t.FrameX, t.FrameY), color);
                }
            }
        }
        #endregion



        #region Draw chunk
        public async Task DrawChunkAsync(Chunk2D chunk)
        {
            chunk.Processing = true;
            int size = _chunkWidth * _chunkHeight;
            UnityEngine.Tilemaps.TileBase[] landTiles = new UnityEngine.Tilemaps.TileBase[size];
            UnityEngine.Tilemaps.TileBase[] waterTiles = new UnityEngine.Tilemaps.TileBase[size];
            UnityEngine.Tilemaps.TileBase[] tilegroupTiles = new UnityEngine.Tilemaps.TileBase[size];
            UnityEngine.Tilemaps.TileBase[] heatTiles = new UnityEngine.Tilemaps.TileBase[size];
            UnityEngine.Tilemaps.TileBase[] moistureTiles = new UnityEngine.Tilemaps.TileBase[size];

            UnityEngine.Tilemaps.TileBase[] riverTiles = new UnityEngine.Tilemaps.TileBase[size];

            Vector3Int[] positionArray = new Vector3Int[size];

            await Task.Run(() =>
            {
                Parallel.For(0, _chunkWidth, x =>
                {
                    for (int y = 0; y < _chunkHeight; y++)
                    {
                        int index = x + y * _chunkWidth;
                        positionArray[index] = new Vector3Int(x, y, 0);

                        HeightType heightType = chunk.ChunkData.GetValue(x, y).HeightType;
                        HeatType heatType = chunk.ChunkData.GetValue(x, y).HeatType;
                        MoistureType moistureType = chunk.ChunkData.GetValue(x, y).MoistureType;


                        // Height
                        switch (heightType)
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
                            case HeightType.River:
                                landTiles[index] = Main.Instance.GetTileBase(TileType.Water);
                                waterTiles[index] = Main.Instance.GetTileBase(TileType.Water);
                                tilegroupTiles[index] = Main.Instance.GetTileBase(TileType.Color);
                                break;
                            default:
                                landTiles[index] = Main.Instance.GetTileBase(TileType.Snow);
                                waterTiles[index] = null;
                                tilegroupTiles[index] = Main.Instance.GetTileBase(TileType.Color);
                                break;
                        }



                        // Heat
                        switch (heatType)
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


                        // River

                        switch (heightType)
                        {
                            case HeightType.DeepWater:
                                riverTiles[index] = Main.Instance.GetTileBase(TileType.Water);
                                break;
                            case HeightType.ShallowWater:
                                riverTiles[index] = Main.Instance.GetTileBase(TileType.Water);
                                break;
                            case HeightType.Sand:
                                riverTiles[index] = Main.Instance.GetTileBase(TileType.Water);
                                break;
                            case HeightType.Grass:
                                riverTiles[index] = Main.Instance.GetTileBase(TileType.Water);
                                break;
                            case HeightType.Forest:
                                riverTiles[index] = Main.Instance.GetTileBase(TileType.Water);
                                break;
                            case HeightType.Rock:
                                riverTiles[index] = Main.Instance.GetTileBase(TileType.Water);
                                break;
                            case HeightType.Snow:
                                riverTiles[index] = Main.Instance.GetTileBase(TileType.Water);
                                break;
                            case HeightType.River:
                                riverTiles[index] = Main.Instance.GetTileBase(TileType.River);
                                break;
                            default:
                                riverTiles[index] = Main.Instance.GetTileBase(TileType.Water);
                                break;
                        }
                    }
                });
            });



            chunk.LandTilemap.SetTiles(positionArray, landTiles);
            //WaterTilemap.SetTiles(positionArray, waterTiles);
            //Tilegroupmap.SetTiles(positionArray, tilegroupTiles);

            if (Main.Instance.InitWorldWithHeatmap)
            {
                chunk.HeatTilemap.SetTiles(positionArray, heatTiles);
                chunk.HeatTilemap.gameObject.SetActive(true);
            }
            else
            {
                chunk.HeatTilemap.gameObject.SetActive(false);
            }

            if (Main.Instance.InitWorldWithMoisturemap)
            {
                chunk.MoistureTilemap.SetTiles(positionArray, moistureTiles);
                chunk.MoistureTilemap.gameObject.SetActive(true);
            }
            else
            {
                chunk.MoistureTilemap.gameObject.SetActive(false);
            }


            chunk.ChunkHasDrawn = true;
            chunk.Processing = false;
        }
        #endregion

#endif


        #region Neighbors
        public void UpdateChunkNeighbors(Chunk chunk)
        {
            Vector3Int chunkFrame = new Vector3Int(chunk.FrameX, chunk.FrameY, chunk.FrameZ);
            if (chunk.Left == null)
            {
                Chunk leftNeighborChunk = Main.Instance.GetChunkNeighborLeft(chunk);
                if (leftNeighborChunk != null)
                {
                    chunk.SetNeighbors(BlockSide.Left, leftNeighborChunk);
                    leftNeighborChunk.SetNeighbors(BlockSide.Right, chunk);
                }
            }
            if (chunk.Right == null)
            {
                Chunk rightNeighborChunk = Main.Instance.GetChunkNeighborRight(chunk);
                if (rightNeighborChunk != null)
                {
                    chunk.SetNeighbors(BlockSide.Right, rightNeighborChunk);
                    rightNeighborChunk.SetNeighbors(BlockSide.Left, chunk);
                }
            }
            if (chunk.Front == null)
            {
                Chunk frontNeighborChunk = Main.Instance.GetChunkNeighborFront(chunk);
                if (frontNeighborChunk != null)
                {
                    chunk.SetNeighbors(BlockSide.Front, frontNeighborChunk);
                    frontNeighborChunk.SetNeighbors(BlockSide.Back, chunk);
                }
            }
            if (chunk.Back == null)
            {
                Chunk backNeighborChunk = Main.Instance.GetChunkNeighborBack(chunk);
                if (backNeighborChunk != null)
                {
                    chunk.SetNeighbors(BlockSide.Back, backNeighborChunk);
                    backNeighborChunk.SetNeighbors(BlockSide.Front, chunk);
                }
            }
        }

        //public void AddChunkFourDirectionNeighbors(Chunk2D chunk)
        //{
        //    Chunk2D nbAbove = _main.GetChunkNeighborFront(chunk);
        //    Chunk2D nbBelow = _main.GetChunkNeighborBack(chunk);
        //    Chunk2D nbLeft = _main.GetChunkNeighborLeft(chunk);
        //    Chunk2D nbRight = _main.GetChunkNeighborRight(chunk);
        //    chunk.SetTwoSidesChunkNeighbors(nbLeft, nbRight, nbAbove, nbBelow);
        //}
        //public void UpdateChunkTileNeighbors(Chunk2D chunk)
        //{
        //    // Find chunk neighbors
        //    Chunk2D nbAbove = _main.GetChunkNeighborFront(chunk);
        //    Chunk2D nbBelow = _main.GetChunkNeighborBack(chunk);
        //    Chunk2D nbLeft = _main.GetChunkNeighborLeft(chunk);
        //    Chunk2D nbRight = _main.GetChunkNeighborRight(chunk);
        //    chunk.SetTwoSidesChunkNeighbors(nbLeft, nbRight, nbAbove, nbBelow);

        //    if (chunk.HasNeighbors() && !chunk.AllTileHasNeighbors)
        //    {
        //        chunk.UpdateAllTileNeighbors();

        //        if (_main.PaintTileNeighbors)
        //            PaintNeighborsColor(chunk);

        //        if (chunk.Waters.Count == 0 && chunk.Lands.Count == 0)
        //        {
        //            FloodFill(chunk);
        //            PaintTilegroupMap(chunk);
        //        }
        //    }

        //    if (nbAbove != null && nbAbove.HasNeighbors() && !nbAbove.AllTileHasNeighbors)
        //    {
        //        nbAbove.UpdateAllTileNeighbors();

        //        if (_main.PaintTileNeighbors)
        //            PaintNeighborsColor(nbAbove);

        //        if (nbAbove.Waters.Count == 0 && nbAbove.Lands.Count == 0)
        //        {
        //            FloodFill(nbAbove);
        //            PaintTilegroupMap(nbAbove);
        //        }
        //    }
        //    if (nbBelow != null && nbBelow.HasNeighbors() && !nbBelow.AllTileHasNeighbors)
        //    {
        //        nbBelow.UpdateAllTileNeighbors();

        //        if (_main.PaintTileNeighbors)
        //            PaintNeighborsColor(nbBelow);

        //        if (nbBelow.Waters.Count == 0 && nbBelow.Lands.Count == 0)
        //        {
        //            FloodFill(nbBelow);
        //            PaintTilegroupMap(nbBelow);
        //        }

        //    }
        //    if (nbLeft != null && nbLeft.HasNeighbors() && !nbLeft.AllTileHasNeighbors)
        //    {
        //        nbLeft.UpdateAllTileNeighbors();

        //        if (_main.PaintTileNeighbors)
        //            PaintNeighborsColor(nbLeft);

        //        if (nbLeft.Waters.Count == 0 && nbLeft.Lands.Count == 0)
        //        {
        //            FloodFill(nbLeft);
        //            PaintTilegroupMap(nbLeft);
        //        }
        //    }
        //    if (nbRight != null && nbRight.HasNeighbors() && !nbRight.AllTileHasNeighbors)
        //    {
        //        nbRight.UpdateAllTileNeighbors();

        //        if (_main.PaintTileNeighbors)
        //            PaintNeighborsColor(nbRight);

        //        if (nbRight.Waters.Count == 0 && nbRight.Lands.Count == 0)
        //        {
        //            FloodFill(nbRight);
        //            PaintTilegroupMap(nbRight);
        //        }

        //    }
        //}
        //public async void UpdateChunkTileNeighborsAsync(Chunk2D chunk)
        //{
        //    // Find chunk neighbors
        //    Chunk2D nbAbove = _main.GetChunkNeighborFront(chunk);
        //    Chunk2D nbBelow = _main.GetChunkNeighborBack(chunk);
        //    Chunk2D nbLeft = _main.GetChunkNeighborLeft(chunk);
        //    Chunk2D nbRight = _main.GetChunkNeighborRight(chunk);
        //    chunk.SetTwoSidesChunkNeighbors(nbLeft, nbRight, nbAbove, nbBelow);

        //    Task[] tasks = new Task[5];

        //    if (chunk.HasNeighbors() && !chunk.AllTileHasNeighbors)
        //    {
        //        tasks[0] = chunk.UpdateAllTileNeighborsAsync();
        //    }
        //    else
        //        tasks[0] = Task.CompletedTask;

        //    if (nbAbove != null && nbAbove.HasNeighbors() && !nbAbove.AllTileHasNeighbors)
        //        tasks[1] = nbAbove.UpdateAllTileNeighborsAsync();
        //    else
        //        tasks[1] = Task.CompletedTask;

        //    if (nbBelow != null && nbBelow.HasNeighbors() && !nbBelow.AllTileHasNeighbors)
        //        tasks[2] = nbBelow.UpdateAllTileNeighborsAsync();
        //    else
        //        tasks[2] = Task.CompletedTask;

        //    if (nbLeft != null && nbLeft.HasNeighbors() && !nbLeft.AllTileHasNeighbors)
        //        tasks[3] = nbLeft.UpdateAllTileNeighborsAsync();
        //    else
        //        tasks[3] = Task.CompletedTask;

        //    if (nbRight != null && nbRight.HasNeighbors() && !nbRight.AllTileHasNeighbors)
        //        tasks[4] = nbRight.UpdateAllTileNeighborsAsync();
        //    else
        //        tasks[4] = Task.CompletedTask;

        //    await Task.WhenAll(tasks);


        //    if (chunk.AllTileHasNeighbors)
        //    {
        //        if (_main.PaintTileNeighbors)
        //            PaintNeighborsColor(chunk);

        //        if (chunk.Waters.Count == 0 && chunk.Lands.Count == 0)
        //        {
        //            FloodFill(chunk);
        //            PaintTilegroupMap(chunk);
        //        }
        //    }
        //    if (nbAbove != null && nbAbove.AllTileHasNeighbors)
        //    {
        //        if (_main.PaintTileNeighbors)
        //            PaintNeighborsColor(nbAbove);

        //        if (nbAbove.Waters.Count == 0 && nbAbove.Lands.Count == 0)
        //        {
        //            FloodFill(nbAbove);
        //            PaintTilegroupMap(nbAbove);
        //        }
        //    }
        //    if (nbBelow != null && nbBelow.AllTileHasNeighbors)
        //    {
        //        if (_main.PaintTileNeighbors)
        //            PaintNeighborsColor(nbBelow);

        //        if (nbBelow.Waters.Count == 0 && nbBelow.Lands.Count == 0)
        //        {
        //            FloodFill(nbBelow);
        //            PaintTilegroupMap(nbBelow);
        //        }
        //    }
        //    if (nbLeft != null && nbLeft.AllTileHasNeighbors)
        //    {
        //        if (_main.PaintTileNeighbors)
        //            PaintNeighborsColor(nbLeft);

        //        if (nbLeft.Waters.Count == 0 && nbLeft.Lands.Count == 0)
        //        {
        //            FloodFill(nbLeft);
        //            PaintTilegroupMap(nbLeft);
        //        }
        //    }
        //    if (nbRight != null && nbRight.AllTileHasNeighbors)
        //    {
        //        if (_main.PaintTileNeighbors)
        //            PaintNeighborsColor(nbRight);

        //        if (nbRight.Waters.Count == 0 && nbRight.Lands.Count == 0)
        //        {
        //            FloodFill(nbRight);
        //            PaintTilegroupMap(nbRight);
        //        }
        //    }

        //}
        //public void UpdateAllActiveChunkTileNeighborsAsync()
        //{
        //    foreach (Chunk activeChunk in _main.ActiveChunks)
        //    {
        //        if (activeChunk.AllTileHasNeighbors == false)
        //        {
        //            UpdateChunkTileNeighborsAsync(activeChunk);
        //        }
        //    }
        //}
        #endregion




        #region Grid Algorithm
        //private void FloodFill(Chunk chunk)
        //{
        //    Stack<Tile> stack = new Stack<Tile>();

        //    for (int x = 0; x < _chunkWidth; x++)
        //    {
        //        for (int y = 0; y < _chunkHeight; y++)
        //        {
        //            Tile t = chunk.ChunkData.GetValue(x, y);

        //            // Tile already flood filled, skip
        //            if (t.FloodFilled)
        //            {
        //                continue;
        //            }

        //            // Land
        //            if (t.Collidable)
        //            {
        //                TileGroup group = new TileGroup();
        //                group.Type = TileGroupType.Land;
        //                stack.Push(t);

        //                while (stack.Count > 0)
        //                {
        //                    Tile tile = stack.Pop();
        //                    FloodFill(tile, ref group, ref stack, chunk);
        //                }

        //                if (group.Tiles.Count > 0)
        //                {
        //                    chunk.Lands.Add(group);
        //                }
        //            }
        //            else // Water
        //            {
        //                TileGroup group = new TileGroup();
        //                group.Type = TileGroupType.Water;
        //                stack.Push(t);

        //                while (stack.Count > 0)
        //                {
        //                    Tile tile = stack.Pop();
        //                    FloodFill(tile, ref group, ref stack, chunk);
        //                }

        //                if (group.Tiles.Count > 0)
        //                {
        //                    chunk.Waters.Add(group);
        //                }
        //            }
        //        }
        //    }
        //}
        //private void FloodFill(Tile tile, ref TileGroup tiles, ref Stack<Tile> stack, Chunk2D chunk)
        //{
        //    // Validate
        //    if (tile.FloodFilled)
        //        return;
        //    if (tiles.Type == TileGroupType.Land && tile.Collidable == false)
        //        return;
        //    if (tiles.Type == TileGroupType.Water && tile.Collidable)
        //        return;

        //    tiles.Tiles.Add(tile);
        //    tile.FloodFilled = true;


        //    // FloodFill into neighbors
        //    Tile nb = chunk.GetTopWithinChunk(tile);
        //    if (nb != null && nb.FloodFilled == false && tile.Collidable == nb.Collidable)
        //    {
        //        stack.Push(nb);
        //    }

        //    nb = chunk.GetBottomWithinChunk(tile);
        //    if (nb != null && nb.FloodFilled == false && tile.Collidable == nb.Collidable)
        //    {
        //        stack.Push(nb);
        //    }

        //    nb = chunk.GetLeftWithinChunk(tile);
        //    if (nb != null && nb.FloodFilled == false && tile.Collidable == nb.Collidable)
        //    {
        //        stack.Push(nb);
        //    }

        //    nb = chunk.GetRightWithinChunk(tile);
        //    if (nb != null && nb.FloodFilled == false && tile.Collidable == nb.Collidable)
        //    {
        //        stack.Push(nb);
        //    }
        //}
        #endregion



        #region Utilities
        public static Color GetGradientColor(float heatValue)
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
        #endregion
    }
}

