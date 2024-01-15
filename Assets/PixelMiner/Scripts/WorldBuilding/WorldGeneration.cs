using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using PixelMiner.Utilities;
using PixelMiner.Enums;
using PixelMiner.Core;
using PixelMiner.Lighting;
using PixelMiner.World;
using TMPro;
using System.Collections;
using System.Linq;


namespace PixelMiner.WorldBuilding
{
    /* 
     * 1. Generate Chunk (Instantiate, Init data, 
     * 2. Generate and load data into chunk (data include: Height map, heat map, moisture map,...)
     * 3. Draw chunk - Create mesh object can be shown in Unity Engine.
     */
    public class WorldGeneration : MonoBehaviour
    {
        public static WorldGeneration Instance { get; private set; }
        public event System.Action OnWorldGenWhenStartFinished;

        #region Fileds and Variables
        [FoldoutGroup("References"), SerializeField] private Chunk _chunkPrefab;
        [FoldoutGroup("References"), SerializeField] private Transform _chunkParent;

        // Height map
        // ==========
        [InfoBox("Height map noise settings")]
        [FoldoutGroup("Height map"), Indent(1)] public int Octaves = 6;
        [FoldoutGroup("Height map"), Indent(1)] public float Frequency = 0.02f;
        [FoldoutGroup("Height map"), Indent(1)] public float Lacunarity = 2.0f;
        [FoldoutGroup("Height map"), Indent(1)] public float Persistence = 0.5f;

        [InfoBox("Height map threshold"), Space(10)]
        [FoldoutGroup("Height map")]
        public float DeepWater = 0.2f;
        [FoldoutGroup("Height map")]
        public float Water = 0.4f;
        [FoldoutGroup("Height map")]
        public float Sand = 0.5f;
        [FoldoutGroup("Height map")]
        public float Grass = 0.7f;
        [FoldoutGroup("Height map")]
        public float Forest = 0.8f;
        [FoldoutGroup("Height map")]
        public float Rock = 0.9f;
        [FoldoutGroup("Height map")]
        public float Snow = 1;
        private FastNoiseLite _heightSimplex;
        private FastNoiseLite _heightVoronoi;



        // Heapmap
        // ======
        [InfoBox("Heat map noise settings")]
        [FoldoutGroup("Heatmap"), Indent(1)] public int HeatOctaves = 4;
        [FoldoutGroup("Heatmap"), Indent(1)] public float HeatFrequency = 0.02f;
        [FoldoutGroup("Heatmap"), Indent(1)] public float HeatLacunarity = 2.0f;
        [FoldoutGroup("Heatmap"), Indent(1)] public float HeatPersistence = 0.5f;

        [InfoBox("Gradient map"), Space(10)]
        [FoldoutGroup("Heatmap"), Indent(1)] public ushort GradientHeatmapSize = 256;
        [FoldoutGroup("Heatmap"), Indent(1), Range(0f, 1f)] public float HeatMapBlendFactor = 0.5f;

        [InfoBox("Heat map threshold"), Space(10)]
        [FoldoutGroup("Heatmap")] public float ColdestValue = 0.1f;
        [FoldoutGroup("Heatmap")] public float ColderValue = 0.2f;
        [FoldoutGroup("Heatmap")] public float ColdValue = 0.4f;
        [FoldoutGroup("Heatmap")] public float WarmValue = 0.6f;
        [FoldoutGroup("Heatmap")] public float WarmerValue = 0.8f;
        [FoldoutGroup("Heatmap")] public float WarmestValue = 1.0f;
        private FastNoiseLite _heatSimplex;
        private FastNoiseLite _heatVoronoi;



        // Moisture map
        // ======
        [InfoBox("Moisture map noise settings")]
        [FoldoutGroup("Moisture map"), Indent(1)] public int MoistureOctaves = 4;
        [FoldoutGroup("Moisture map"), Indent(1)] public float MoistureFrequency = 0.03f;
        [FoldoutGroup("Moisture map"), Indent(1)] public float MoistureLacunarity = 2.0f;
        [FoldoutGroup("Moisture map"), Indent(1)] public float MoisturePersistence = 0.5f;
        [InfoBox("Moisture map noise settings"), Space(10)]
        [FoldoutGroup("Moisture map")] public float DryestValue = 0.22f;
        [FoldoutGroup("Moisture map")] public float DryerValue = 0.35f;
        [FoldoutGroup("Moisture map")] public float DryValue = 0.55f;
        [FoldoutGroup("Moisture map")] public float WetValue = 0.75f;
        [FoldoutGroup("Moisture map")] public float WetterValue = 0.85f;
        [FoldoutGroup("Moisture map")] public float WettestValue = 1.0f;
        private FastNoiseLite _moistureSimplex;
        private FastNoiseLite _moistureVoronoi;



        // River
        // =====
        [FoldoutGroup("River"), Indent(1)] public int RiverOctaves = 4;
        [FoldoutGroup("River"), Indent(1)] public float RiverFrequency = 0.02f;
        [FoldoutGroup("River"), Indent(1)] public float RiverLacunarity = 2.0f;
        [FoldoutGroup("River"), Indent(1)] public float RiverPersistence = 0.5f;
        private FastNoiseLite _riverSimplex;
        private FastNoiseLite _riverVoronoi;



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


        // Biomes
        public static Color Ice = Color.white;
        public static Color Desert = new Color(238 / 255f, 218 / 255f, 130 / 255f, 1);
        public static Color Savanna = new Color(177 / 255f, 209 / 255f, 110 / 255f, 1);
        public static Color TropicalRainforest = new Color(66 / 255f, 123 / 255f, 25 / 255f, 1);
        public static Color Tundra = new Color(96 / 255f, 131 / 255f, 112 / 255f, 1);
        public static Color TemperateRainforest = new Color(29 / 255f, 73 / 255f, 40 / 255f, 1);
        public static Color Grassland = new Color(164 / 255f, 225 / 255f, 99 / 255f, 1);
        public static Color SeasonalForest = new Color(73 / 255f, 100 / 255f, 35 / 255f, 1);
        public static Color BorealForest = new Color(95 / 255f, 115 / 255f, 62 / 255f, 1);
        public static Color Woodland = new Color(139 / 255f, 175 / 255f, 90 / 255f, 1);



        // BIOMES
        public BiomeType[,] BiomeTable = new BiomeType[6, 6]
        {
              //COLDEST         //COLDER            //COLD                  //HOT                          //HOTTER                       //HOTTEST
		    { BiomeType.Ice,    BiomeType.Ice,   BiomeType.Grassland,   BiomeType.Grassland,             BiomeType.Grassland,              BiomeType.Desert },              //DRYEST
		    { BiomeType.Ice,    BiomeType.Ice,   BiomeType.Grassland,   BiomeType.Grassland,             BiomeType.Grassland,              BiomeType.Desert },              //DRYER
		    { BiomeType.Ice,    BiomeType.Ice,   BiomeType.Grassland,   BiomeType.Woodland,              BiomeType.Woodland,              BiomeType.Desert },               //DRY
		    { BiomeType.Ice,    BiomeType.Ice,   BiomeType.Woodland,    BiomeType.Woodland,              BiomeType.Woodland,              BiomeType.Desert },               //WET
		    { BiomeType.Ice,    BiomeType.Ice,   BiomeType.Forest,      BiomeType.Forest,                BiomeType.Forest,              BiomeType.Forest },                 //WETTER
		    { BiomeType.Ice,    BiomeType.Ice,   BiomeType.Forest,      BiomeType.Forest,                BiomeType.Forest,              BiomeType.Forest }                  //WETTEST
        };



        // Density noise
        private FastNoiseLite _grassNoiseDistribute;
        private float _grassNoiseFrequency = 5;
        private int _grassNoiseR = 1;
        private FastNoiseLite _treeNoiseDistribute;
        private float _treeNoiseFrequency = 1;
        private int _treeNoiseR = 3;


        // Cached
        [HideInInspector] private int _groundLevel = 7;
        [HideInInspector] private int _underGroundLevel = 0;
        private Vector3Int _chunkDimension;
        private byte _calculateNoiseRangeSampleMultiplier = 15;  // 50 times. 50 * 100000 = 5 mil times.
        private int _calculateNoiseRangeCount = 1000000;    // 1 mil times.
        private float _minWorldNoiseValue = float.MaxValue;
        private float _maxWorldNoiseValue = float.MinValue;
        private Main _main;
        private WorldLoading _worldLoading;
        #endregion


        private float _voronoiFrequency = 0.006f;
        public AnimationCurve LightAnimCurve;
        public GameObject TextPrefab;

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

            // HEIGHT
            // ------
            _heightSimplex = new FastNoiseLite(Seed);
            _heightSimplex.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            _heightSimplex.SetFractalType(FastNoiseLite.FractalType.FBm);
            _heightSimplex.SetFrequency(Frequency);
            _heightSimplex.SetFractalLacunarity(Lacunarity);
            _heightSimplex.SetFractalGain(Persistence);

            _heightVoronoi = new FastNoiseLite(Seed);
            _heightVoronoi.SetFrequency(_voronoiFrequency);
            //_heightVoronoi.SetFractalOctaves(4);
            _heightVoronoi.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            _heightVoronoi.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);



            // HEAT
            // ----
            _heatSimplex = new FastNoiseLite(Seed);
            _heatSimplex.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            _heatSimplex.SetFractalType(FastNoiseLite.FractalType.FBm);
            _heatSimplex.SetFrequency(HeatFrequency);
            _heatSimplex.SetFractalLacunarity(HeatLacunarity);
            _heatSimplex.SetFractalGain(HeatPersistence);

            _heatVoronoi = new FastNoiseLite(Seed);
            _heatVoronoi.SetFrequency(_voronoiFrequency);
            _heatVoronoi.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            _heatVoronoi.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);




            // MOISTURE
            // --------
            _moistureSimplex = new FastNoiseLite(Seed);
            _moistureSimplex.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            _moistureSimplex.SetFractalType(FastNoiseLite.FractalType.FBm);
            _moistureSimplex.SetFrequency(MoistureFrequency);
            _moistureSimplex.SetFractalLacunarity(MoistureLacunarity);
            _moistureSimplex.SetFractalGain(MoisturePersistence);

            _moistureVoronoi = new FastNoiseLite(Seed);
            _moistureVoronoi.SetFrequency(_voronoiFrequency);
            _moistureVoronoi.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            _moistureVoronoi.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);




            // RIVER
            // -----
            _riverSimplex = new FastNoiseLite(Seed + 3);
            _riverSimplex.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            _riverSimplex.SetFractalType(FastNoiseLite.FractalType.FBm);
            _riverSimplex.SetFrequency(RiverFrequency);
            _riverSimplex.SetFractalOctaves(RiverOctaves);
            _riverSimplex.SetFractalLacunarity(RiverLacunarity);
            _riverSimplex.SetFractalGain(RiverPersistence);

            _riverVoronoi = new FastNoiseLite(Seed + 3);
            _riverVoronoi.SetFrequency(RiverFrequency);
            _riverVoronoi.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            _riverVoronoi.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);




            // DISTRIBUTION
            _grassNoiseDistribute = new FastNoiseLite(Seed);
            _grassNoiseDistribute.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            _grassNoiseDistribute.SetFrequency(_grassNoiseFrequency);

            _treeNoiseDistribute = new FastNoiseLite(Seed);
            _treeNoiseDistribute.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            _treeNoiseDistribute.SetFrequency(_treeNoiseFrequency);
        }

        private void Start()
        {
            _main = Main.Instance;
            _worldLoading = WorldLoading.Instance;

            Seed = WorldGenUtilities.StringToSeed(_main.SeedInput);
            UnityEngine.Random.InitState(Seed);

            _chunkDimension = _main.ChunkDimension;


            // World Initialization
            InitWorldAsyncInSequence(_worldLoading.LastChunkFrame.x,
                                     _worldLoading.LastChunkFrame.y,
                                     _worldLoading.LastChunkFrame.z,
                                     widthInit: _worldLoading.InitWorldWidth,
                                     heightInit: _worldLoading.InitWorldHeight,
                                     depthInit: _worldLoading.InitWorldDepth, () =>
            {
                OnWorldGenWhenStartFinished?.Invoke();
            });

            Chunk.OnChunkHasNeighbors += PropagateAmbientLight;
            Chunk.OnChunkHasNeighbors += DrawChunk;


        }

        private void OnDestroy()
        {
            Chunk.OnChunkHasNeighbors += PropagateAmbientLight;
            Chunk.OnChunkHasNeighbors -= DrawChunk;
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
                    float x = rand.Next();
                    float y = rand.Next();
                    float noiseValue = (float)_heightSimplex.GetNoise(x, y); // Generate noise value

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
                    float x = rand.Next();
                    float y = rand.Next();
                    float noiseValue = (float)_heightSimplex.GetNoise(x, y); // Generate noise value

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


        #region INIT CHUNK DATA
        private async void InitWorldAsyncInSequence(int initFrameX, int initFrameY, int initFrameZ, byte widthInit, byte heightInit, byte depthInit, System.Action onFinished = null)
        {
            await ComputeNoiseRangeAsyncInParallel();

            Debug.Log($"min: {_minWorldNoiseValue}");
            Debug.Log($"max: {_maxWorldNoiseValue}");
            int totalChunkLoad = 0;
            List<Task<Chunk>> loadChunkTask = new List<Task<Chunk>>();

            for (int x = initFrameX - widthInit; x <= initFrameX + widthInit; x++)
            {
                for (int y = initFrameY - heightInit; y <= initFrameY + heightInit; y++)
                {
                    for (int z = initFrameZ - depthInit; z <= initFrameZ + depthInit; z++)
                    {
                        if (y < 0) continue;
                        totalChunkLoad++;

                        //Chunk newChunk = await GenerateNewChunk(x, y, z, _main.ChunkDimension);
                        //_worldLoading.LoadChunk(newChunk);.

                        loadChunkTask.Add(GenerateNewChunk(x, y, z, _main.ChunkDimension));
                    }
                }
            }


            await Task.WhenAll(loadChunkTask);

            for(int i = 0; i < loadChunkTask.Count; i++)
            {
                _worldLoading.LoadChunk(loadChunkTask[i].Result);
            }

            Debug.Log($"Total chunk loaded: {totalChunkLoad}");
            onFinished?.Invoke();
        }

        public async Task<Chunk> GenerateNewChunk(int frameX, int frameY, int frameZ, Vector3Int chunkDimension)
        {
            //Debug.Log($"GenerateNewChunk: {frameX}  {frameY}  {frameZ}");

            Vector3Int frame = new Vector3Int(frameX, frameY, frameZ);
            Vector3 worldPosition = frame * new Vector3Int(chunkDimension[0], chunkDimension[1], chunkDimension[2]);
            Chunk newChunk = Instantiate(_chunkPrefab, worldPosition, Quaternion.identity, _chunkParent.transform);
            newChunk.Init(frameX, frameY, frameZ, chunkDimension[0], chunkDimension[1], chunkDimension[2]);


            if (frameY <= 0)
            {
                //float[] heightValues = await GetHeightMapDataAsync(newChunk.FrameX, newChunk.FrameZ, chunkDimension[0], chunkDimension[2]);
                //float[] heatValues = await GetFractalHeatMapDataAsync(newChunk.FrameX, newChunk.FrameZ, chunkDimension[0], chunkDimension[2]);
                //float[] moistureValues = await GetMoistureMapDataAsync(newChunk.FrameX, newChunk.FrameZ, chunkDimension[0], chunkDimension[2]);
                //float[] riverValues = await GetRiverDataAsync(newChunk.FrameX, newChunk.FrameZ, chunkDimension[0], chunkDimension[2]);

                Task<float[]> heightTask = GetHeightMapDataAsync(newChunk.FrameX, newChunk.FrameZ, chunkDimension[0], chunkDimension[2]);
                Task<float[]> heatTask = GetFractalHeatMapDataAsync(newChunk.FrameX, newChunk.FrameZ, chunkDimension[0], chunkDimension[2]);
                Task<float[]> moistureTask = GetMoistureMapDataAsync(newChunk.FrameX, newChunk.FrameZ, chunkDimension[0], chunkDimension[2]);
                Task<float[]> riverTask = GetRiverDataAsync(newChunk.FrameX, newChunk.FrameZ, chunkDimension[0], chunkDimension[2]);
                await Task.WhenAll(heightTask, heatTask, moistureTask, riverTask);
                //await Task.WhenAll(heatTask, moistureTask, riverTask);
                float[] heightValues = heightTask.Result;
                float[] heatValues = heatTask.Result;
                float[] moistureValues = moistureTask.Result;
                float[] riverValues = riverTask.Result;


                Task loadHeatTask = LoadHeatMapDataAsync(newChunk, heatValues);
                Task loadMoistureTask = LoadMoistureMapDataAsync(newChunk, moistureValues);
                await Task.WhenAll(loadHeatTask, loadMoistureTask);
                await GenerateBiomeMapDataAsync(newChunk, heightValues);


                // River
                // ----
                //BiomeType[] riverBiomes = new BiomeType[riverValues.Length];
                for (int i = 0; i < riverValues.Length; i++)
                {
                    if (riverValues[i] > 0.6f && newChunk.BiomesData[i] != BiomeType.Ocean)
                    {
                        newChunk.RiverBiomes[i] = BiomeType.River;
                    }
                    else
                    {
                        newChunk.RiverBiomes[i] = BiomeType.Other;
                    }
                }
            }
            else
            {
                //float[] heatValues = await GetFractalHeatMapDataAsync(newChunk.FrameX, newChunk.FrameZ, chunkDimension[0], chunkDimension[2]);
                //float[] moistureValues = await GetMoistureMapDataAsync(newChunk.FrameX, newChunk.FrameZ, chunkDimension[0], chunkDimension[2]);

                await LoadHeightMapDataAsync(newChunk, BlockType.Air);
                //await LoadHeatMapDataAsync(newChunk, heatValues);
                //await LoadMoistureMapDataAsync(newChunk, moistureValues);
                //await GenerateBiomeMapDataAsync(newChunk);
            }




            //// Apply ambient light
            //// I use list instead of queue because this type of light only fall down when start, 
            //// use list can help this method can process in parallel. When this light hit block (not air)
            //// we'll use normal bfs to spread light like with torch.
            //List<LightNode> ambientLightList = new List<LightNode>();
            //for (int z = 0; z < _chunkDimension[2]; z++)
            //{
            //    for (int x = 0; x < _chunkDimension[0]; x++)
            //    {
            //        ambientLightList.Add(new LightNode(new Vector3Int(x, _chunkDimension[1] - 1, z), 15));
            //    }
            //}
            //LightCalculator.PropagateAmbientLight(ambientLightList);

            return newChunk;
        }
        #endregion





        #region DRAW CHUNK
        private async void DrawChunk(Chunk chunk)
        {
            if (!chunk.ChunkHasDrawn)
            {
                // Dig river
                // ---------
                float[] heightValues = await GetHeightMapDataAsync(chunk.FrameX, chunk.FrameZ, chunk._width, chunk._depth);
                GetRiverBfsNodes(chunk, chunk._width, chunk._height);
                if (chunk.RiverBfsQueue.Count > 0)
                {
                    DigRiver(chunk, chunk.RiverBfsQueue);
                }


                await LoadChunkMapDataAsync(chunk, heightValues);
                await PlaceGrassAsync(chunk);



                // Mesh
                // ----

                MeshData solidMeshData = await MeshUtils.SolidGreedyMeshingAsync(chunk, LightAnimCurve);
                MeshData grassMeshData = await MeshUtils.GetChunkGrassMeshData(chunk, LightAnimCurve, _grassNoiseDistribute);
                //MeshData waterMeshData = await MeshUtils.WaterGreedyMeshingAsync(this);
                MeshData colliderMeshData = await MeshUtils.SolidGreedyMeshingForColliderAsync(chunk);



                chunk.SolidMeshFilter.sharedMesh = CreateMesh(solidMeshData);
                //WaterMeshFilter.sharedMesh =  CreateMesh(waterMeshData);


                // Grass
                // -----
                chunk.GrassMeshFilter.sharedMesh = CreateMesh(grassMeshData);



                // Collider
                // -------
                chunk.MeshCollider.sharedMesh = null;
                chunk.MeshCollider.sharedMesh = CreateColliderMesh(colliderMeshData);


                // Release mesh data
                MeshDataPool.Release(solidMeshData);
                MeshDataPool.Release(grassMeshData);
                //MeshDataPool.Release(waterMeshData);
                MeshDataPool.Release(colliderMeshData);

                //LogUtils.WriteMeshToFile(chunk.SolidMeshFilter.sharedMesh, "Meshdata.txt");
                chunk.ChunkHasDrawn = true;
            }
        }
        public async Task ReDrawChunkTask(Chunk chunk)
        {
            MeshData solidMeshData = await MeshUtils.SolidGreedyMeshingAsync(chunk, LightAnimCurve);
            MeshData colliderMeshData = await MeshUtils.SolidGreedyMeshingForColliderAsync(chunk);

            chunk.SolidMeshFilter.sharedMesh = CreateMesh(solidMeshData);

            chunk.MeshCollider.sharedMesh = null;
            chunk.MeshCollider.sharedMesh = CreateColliderMesh(colliderMeshData);


            // Release mesh data
            MeshDataPool.Release(solidMeshData);
            MeshDataPool.Release(colliderMeshData);
        }
        public Mesh CreateMesh(MeshData meshData)
        {
            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;
            mesh.SetVertices(meshData.Vertices);
            mesh.SetColors(meshData.Colors);
            mesh.SetTriangles(meshData.Triangles, 0);
            mesh.SetUVs(0, meshData.UVs);
            mesh.SetUVs(1, meshData.UV2s);
            mesh.SetUVs(2, meshData.UV3s);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
        private Mesh CreateColliderMesh(MeshData meshData)
        {
            Mesh colliderMesh = new Mesh();
            colliderMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;
            colliderMesh.SetVertices(meshData.Vertices);
            colliderMesh.SetTriangles(meshData.Triangles, 0);
            return colliderMesh;
        }
        #endregion





        #region Generate noise map data.
        /// <summary>
        /// Return 2D noise height map.
        /// </summary>
        /// <returns></returns>
        public async Task<float[]> GetHeightMapDataAsync(int frameX, int frameZ, int width, int height)
        {
            float[] heightValues = new float[width * height];
     

            await Task.Run(() =>
            {
                Parallel.For(0, height, z =>
                {
                    for (int x = 0; x < width; x++)
                    {
                        float offsetX = frameX * width + x;
                        float offsetZ = frameZ * height + z;
                        float heightValue = float.PositiveInfinity;


                        float heightSimplex = (float)_heightSimplex.GetNoise(offsetX, offsetZ);
                        float heightVoronoi = DomainWarping(offsetX, offsetZ, _heatSimplex, _heatVoronoi);

                        float normalizeSimplexValue = (heightSimplex - _minWorldNoiseValue) / (_maxWorldNoiseValue - _minWorldNoiseValue);
                        float normalizeVoronoiValue = (heightVoronoi - _minWorldNoiseValue) / (_maxWorldNoiseValue - _minWorldNoiseValue);
                        float normalizeHeightValue = float.PositiveInfinity;


                        if (normalizeVoronoiValue < Water)
                        {
                            normalizeHeightValue = ScaleNoise(heightSimplex, -1f, 1f, 0, Water - 0.0001f);
                        }
                        else
                        {
                            heightValue = (heightSimplex - _minWorldNoiseValue) / (_maxWorldNoiseValue - _minWorldNoiseValue);
                            normalizeHeightValue = ScaleNoise(heightValue, 0f, 1f, Water + 0.0001f, 1f);
                        }

                        heightValues[WorldGenUtilities.IndexOf(x, z, width)] = normalizeHeightValue;
                    }
                });
            });

            return heightValues;
        }
        public async Task<float[]> GetGradientMapDataAsync(int frameX, int frameZ, int width, int height)
        {
            float[] gradientData = new float[width * height];

            await Task.Run(() =>
            {
                int gradientFrameX = (int)(frameX * width / (float)GradientHeatmapSize);
                int gradientFrameZ = -Mathf.CeilToInt(frameZ * height / (float)GradientHeatmapSize);

                // Calculate the center of the texture with the offset
                Vector2 gradientOffset = new Vector2(gradientFrameX * GradientHeatmapSize, gradientFrameZ * GradientHeatmapSize);
                Vector2 gradientCenterOffset = gradientOffset + new Vector2(frameX * width, frameZ * height);


                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < height; z++)
                    {
                        Vector2 center = new Vector2(Mathf.FloorToInt(x / GradientHeatmapSize), Mathf.FloorToInt(z / GradientHeatmapSize)) * new Vector2(GradientHeatmapSize, GradientHeatmapSize) + new Vector2(GradientHeatmapSize / 2f, GradientHeatmapSize / 2f);
                        Vector2 centerWithOffset = center + gradientCenterOffset;

                        float distance = Mathf.Abs(z - centerWithOffset.y);
                        float normalizedDistance = 1.0f - Mathf.Clamp01(distance / (GradientHeatmapSize / 2f));
                        gradientData[WorldGenUtilities.IndexOf(x, z, width)] = normalizedDistance;
                    }
                }
            });

            //Debug.Log("GetGradientMapAsync Finish");
            return gradientData;
        }
        public async Task<float[]> GetFractalHeatMapDataAsync(int frameX, int frameZ, int width, int height)
        {
            //Debug.Log("GetFractalHeatMapAsync Start");

            float[] fractalNoiseData = new float[width * height];

            await Task.Run(() =>
            {
                Parallel.For(0, height, (z) =>
                {
                    for (int x = 0; x < width; x++)
                    {
                        float offsetX = frameX * width + x;
                        float offsetZ = frameZ * height + z;

                        //float heatValue = (float)_heatSimplex.GetNoise(offsetX, offsetZ);
                        //float heatValue = (float)_heatVoronoi.GetNoise(offsetX, offsetZ);
                        float heatValue = DomainWarping(offsetX, offsetZ, _heatSimplex, _heatVoronoi);
                        float normalizeHeatValue = (heatValue - _minWorldNoiseValue) / (_maxWorldNoiseValue - _minWorldNoiseValue);
                        fractalNoiseData[WorldGenUtilities.IndexOf(x, z, width)] = normalizeHeatValue;
                    }
                });
            });

            //Debug.Log("GetFractalHeatMapAsync Finish");
            return fractalNoiseData;
        }
        public async Task<float[]> GetHeatMapDataAysnc(int frameX, int frameZ, int width, int height)
        {
            /*
             * Heatmap created by blend gradient map and fractal noise map.
             */
            Task<float[]> gradientTask = GetGradientMapDataAsync(frameX, frameZ, width, height);
            Task<float[]> fractalNoiseTask = GetFractalHeatMapDataAsync(frameX, frameZ, width, height);

            // Await for both tasks to complete
            await Task.WhenAll(gradientTask, fractalNoiseTask);
            float[] gradientValues = gradientTask.Result;
            float[] fractalNoiseValues = fractalNoiseTask.Result;

            // Blend the maps
            float[] heatValues = WorldGenUtilities.BlendMapData(gradientValues, fractalNoiseValues, HeatMapBlendFactor);
            return heatValues;
        }
        public async Task<float[]> GetMoistureMapDataAsync(int frameX, int frameZ, int width, int height)
        {
            float[] moistureData = new float[width * height];

            await Task.Run(() =>
            {
                Parallel.For(0, height, (z) =>
                {
                    for (int x = 0; x < width; x++)
                    {
                        float offsetX = frameX * width + x;
                        float offsetZ = frameZ * height + z;

                        //float moisetureValue = _moistureNoise.GetNoise(offsetX, offsetZ);
                        float moisetureValue = DomainWarping(offsetX, offsetZ, _moistureSimplex, _moistureVoronoi);
                        float normalizeMoistureValue = (moisetureValue - _minWorldNoiseValue) / (_maxWorldNoiseValue - _minWorldNoiseValue);

                        moistureData[WorldGenUtilities.IndexOf(x, z, width)] = normalizeMoistureValue;
                    }
                });
            });

            return moistureData;
        }
        public async Task<float[]> GetRiverDataAsync(int frameX, int frameZ, int width, int height)
        {
            float[] riverValues = new float[width * height];

            await Task.Run(() =>
            {
                Parallel.For(0, width, x =>
                {
                    for (int y = 0; y < height; y++)
                    {
                        float offsetX = frameX * width + x;
                        float offsetZ = frameZ * height + y;

                        //float riverValue = (float)_riverSimplex.GetNoise(offsetX, offsetZ);
                        float riverValue = DomainWarping(offsetX, offsetZ, _riverSimplex, _riverVoronoi);


                        float normalizeRiverValue = (riverValue - _minWorldNoiseValue) / (_maxWorldNoiseValue - _minWorldNoiseValue);
                        riverValues[x + y * width] = normalizeRiverValue;
                    }
                });
            });

            return riverValues;
        }
        #endregion



        #region GET CHUNK TYPES
        public async Task LoadChunkMapDataAsync(Chunk chunk, float[] heightValues, bool flatWorld = true)
        {
            await Task.Run(() =>
            {
                int width = chunk.Dimensions[0];
                int height = chunk.Dimensions[1];
                int depth = chunk.Dimensions[2];

                for (int z = 0; z < depth; z++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            float heightValue = heightValues[WorldGenUtilities.IndexOf(x, z, width)];

                            int index3D = WorldGenUtilities.IndexOf(x, y, z, width, height);
                            int indexHighestY = WorldGenUtilities.IndexOf(x, height - 1, z, width, height);
                            int averageGroundLayer = Mathf.FloorToInt(Water * height);

                            int terrainHeight = Mathf.FloorToInt(heightValue * _groundLevel);
                            int globalHeight = (chunk.FrameY * chunk._height) + y;
   
                            if (flatWorld)
                            {
                                if (globalHeight <= _underGroundLevel)
                                {
                                    chunk.ChunkData[index3D] = BlockType.Stone;
                                }
                                else if (globalHeight <= _groundLevel)
                                {
                                    switch (chunk.BiomesData[index3D])
                                    {
                                        case BiomeType.Desert:
                                            chunk.ChunkData[index3D] = BlockType.Sand;
                                            break;
                                        case BiomeType.Grassland:
                                            chunk.ChunkData[index3D] = BlockType.Sand;
                                            break;
                                        case BiomeType.Woodland:
                                            if (globalHeight < _groundLevel)
                                            {
                                                chunk.ChunkData[index3D] = BlockType.Dirt;
                                            }
                                            else if (globalHeight == _groundLevel)
                                            {
                                                chunk.ChunkData[index3D] = BlockType.DirtGrass;
                                            }
                                            break;
                                        case BiomeType.Forest:
                                            if (globalHeight < _groundLevel)
                                            {
                                                chunk.ChunkData[index3D] = BlockType.Dirt;
                                            }
                                            else if (globalHeight == _groundLevel)
                                            {
                                                chunk.ChunkData[index3D] = BlockType.DirtGrass;
                                            }
                                            break;
                                        case BiomeType.Ice:
                                            chunk.ChunkData[index3D] = BlockType.Ice;
                                            break;
                                        case BiomeType.Ocean:
                                            if (heightValues[x + y * width] > (y / (float)height) && heightValues[x + y * width] < Water && y < _groundLevel)
                                            {
                                                chunk.ChunkData[index3D] = BlockType.Gravel;
                                            }
                                            else
                                            {
                                                chunk.ChunkData[index3D] = BlockType.Water;
                                            }
                                            break;
                                        case BiomeType.River:
                                            chunk.ChunkData[index3D] = BlockType.Water;
                                            break;
                                        default:
                                            Debug.LogError($"Not found {chunk.BiomesData[index3D]} biome.");
                                            break;
                                    }
                                }
                                else
                                {
                                    chunk.ChunkData[index3D] = BlockType.Air;
                                }
                            }
                            else
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
                                        chunk.ChunkData[index3D] = BlockType.DirtGrass;
                                    else if (heightValue < Rock)
                                        chunk.ChunkData[index3D] = BlockType.Stone;
                                    else
                                        chunk.ChunkData[index3D] = BlockType.Ice;
                                }
                                else if (y < height * Water)
                                {
                                    chunk.ChunkData[index3D] = BlockType.Water;
                                }
                                else
                                {

                                    chunk.ChunkData[index3D] = BlockType.Air;
                                }
                            }
                        }
                    }
                }
            });
        }
        public async Task LoadHeightMapDataAsync(Chunk chunk, BlockType blockType)
        {
            await Task.Run(() =>
            {
                int width = chunk.Dimensions[0];
                int height = chunk.Dimensions[1];
                int depth = chunk.Dimensions[2];

                for (int z = 0; z < depth; z++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int index3D = WorldGenUtilities.IndexOf(x, y, z, width, height);
                            chunk.ChunkData[index3D] = blockType;
                        }
                    }
                }
            });
        }
        public async Task LoadHeatMapDataAsync(Chunk chunk, float[] heatValues)
        {
            await Task.Run(() =>
            {
                int width = chunk.Dimensions[0];
                int height = chunk.Dimensions[1];
                int depth = chunk.Dimensions[2];

                for (int z = 0; z < depth; z++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int index3D = chunk.IndexOf(x, y, z);
                            int flattenedIndex = chunk.IndexOf(x, z);
                            float heatValue = heatValues[flattenedIndex];


                            float fracIndex2 = heatValue % 0.1f;
                            if (fracIndex2 < 0.015f)
                            {
                                chunk.HeatData[index3D] = HeatType.Coldest;
                            }
                            else if (fracIndex2 < 0.03f)
                            {
                                chunk.HeatData[index3D] = HeatType.Colder;
                            }
                            else if (fracIndex2 < 0.045f)
                            {
                                chunk.HeatData[index3D] = HeatType.Cold;
                            }
                            else if (fracIndex2 < 0.06)
                            {
                                chunk.HeatData[index3D] = HeatType.Warm;
                            }
                            else if (fracIndex2 < 0.08)
                            {
                                chunk.HeatData[index3D] = HeatType.Warmer;
                            }
                            else
                            {
                                chunk.HeatData[index3D] = HeatType.Warmest;
                            }


                        }
                    }
                }
            });
        }
        public async Task LoadMoistureMapDataAsync(Chunk chunk, float[] moistureValues)
        {
            await Task.Run(() =>
            {
                int width = chunk.Dimensions[0];
                int height = chunk.Dimensions[1];
                int depth = chunk.Dimensions[2];

                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            int index2D = chunk.IndexOf(x, z);
                            int index3D = chunk.IndexOf(x, y, z);
                            float moistureValue = moistureValues[index2D];

                            if (moistureValue < DryestValue)
                            {
                                chunk.MoistureData[index3D] = MoistureType.Dryest;
                            }
                            else if (moistureValue < DryerValue)
                            {
                                chunk.MoistureData[index3D] = MoistureType.Dryer;
                            }
                            else if (moistureValue < DryValue)
                            {
                                chunk.MoistureData[index3D] = MoistureType.Dry;
                            }
                            else if (moistureValue < WetValue)
                            {
                                chunk.MoistureData[index3D] = MoistureType.Wet;
                            }
                            else if (moistureValue < WetterValue)
                            {
                                chunk.MoistureData[index3D] = MoistureType.Wetter;
                            }
                            else
                            {
                                chunk.MoistureData[index3D] = MoistureType.Wettest;
                            }
                        }
                    }
                }
            });
        }
        public async Task GenerateBiomeMapDataAsync(Chunk chunk, float[] heightValues)
        {
            await Task.Run(() =>
            {
                int width = chunk.Dimensions[0];
                int height = chunk.Dimensions[1];
                int depth = chunk.Dimensions[2];

                for (int z = 0; z < depth; z++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int index2D = chunk.IndexOf(x, z);
                            int index3D = chunk.IndexOf(x, y, z);
                            chunk.BiomesData[index3D] = GetBiome(chunk, x, y, z);

                            if (heightValues[index2D] < Water)
                            {
                                chunk.BiomesData[index3D] = BiomeType.Ocean;  
                            }
                            else
                            {
                                chunk.BiomesData[index3D] = GetBiome(chunk, x, y, z);
                            }
                        }
                    }
                }
            });
        }
        #endregion






        #region MODIFY NOISE DATA
        public async Task<float[]> ApplyHeightDataToMoistureDataAsync(float[] heightValues, float[] moistureValues, int width, int height)
        {
            await Task.Run(() =>
            {
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < height; z++)
                    {
                        int index = x + z * width;
                        float heightValue = heightValues[index];
                        if (heightValue < DeepWater)
                        {
                            moistureValues[index] += 8f * heightValue;
                        }
                        if (heightValue < Water)
                        {
                            moistureValues[index] += 3f * heightValue;
                        }
                        if (heightValue < Sand)
                        {
                            moistureValues[index] += 0.2f * heightValue;
                        }
                        //else if (heightValue < Grass)
                        //{

                        //}
                        //else if (heightValue < Forest)
                        //{

                        //}
                        //else if (heightValue < Rock)
                        //{

                        //}
                        //else
                        //{ 

                        //}
                    }
                }

            });

            return moistureValues;
        }

        #endregion







        #region RIVER
        private Queue<RiverNode> GetRiverBfsNodes(BiomeType[] riverBiome, int width, int depth)
        {
            Queue<RiverNode> bfsRiverQueue = new Queue<RiverNode>();

            int size = width * depth;
            for (int i = 0; i < size; i++)
            {
                int x = i % width;
                int z = i / width;

                if (x == 0 || x == width - 1 || z == 0 || z == depth - 1) continue;

                int bitmask = 0;
                if (riverBiome[x + (z + 1) * width] == riverBiome[i])
                    bitmask += 1;
                if (riverBiome[(x + 1) + z * width] == riverBiome[i])
                    bitmask += 2;
                if (riverBiome[x + (z - 1) * width] == riverBiome[i])
                    bitmask += 4;
                if (riverBiome[(x - 1) + z * width] == riverBiome[i])
                    bitmask += 8;

                if (bitmask != 15)
                {
                    RiverNode riverNode = new RiverNode()
                    {
                        RelativePosition = new Vector3Int(x, _groundLevel, z),
                        Density = 5
                    };

                    bfsRiverQueue.Enqueue(riverNode);
                }
            }
            return bfsRiverQueue;
        }
        private void GetRiverBfsNodes(Chunk chunk, int width, int depth)
        {
            int size = width * depth;
            for (int i = 0; i < size; i++)
            {
                int x = i % width;
                int z = i / width;
                int bitmask = 0;

                if (x == 0 || x == width - 1 || z == 0 || z == depth - 1)
                {
                    //continue;
                    if (chunk.FindNeighbor(new Vector3Int(x, _groundLevel, z + 1), out Chunk nbChunk, out Vector3Int nbRelativePosition))
                    {
                        if (nbChunk.RiverBiomes[nbRelativePosition.x + nbRelativePosition.z * width] == chunk.RiverBiomes[i])
                            bitmask += 1;
                    }
                    else
                    {
                        if (chunk.RiverBiomes[x + (z + 1) * width] == chunk.RiverBiomes[i])
                            bitmask += 1;
                    }


                    if (chunk.FindNeighbor(new Vector3Int(x + 1, _groundLevel, z), out Chunk nbChunk2, out Vector3Int nbRelativePosition2))
                    {
                        if (nbChunk2.RiverBiomes[nbRelativePosition2.x + nbRelativePosition2.z * width] == chunk.RiverBiomes[i])
                            bitmask += 2;
                    }
                    else
                    {
                        if (chunk.RiverBiomes[(x + 1) + z * width] == chunk.RiverBiomes[i])
                            bitmask += 2;
                    }



                    if (chunk.FindNeighbor(new Vector3Int(x, _groundLevel, z - 1), out Chunk nbChunk3, out Vector3Int nbRelativePosition3))
                    {
                        if (nbChunk3.RiverBiomes[nbRelativePosition3.x + nbRelativePosition3.z * width] == chunk.RiverBiomes[i])
                            bitmask += 4;
                    }
                    else
                    {
                        if (chunk.RiverBiomes[x + (z - 1) * width] == chunk.RiverBiomes[i])
                            bitmask += 4;
                    }




                    if (chunk.FindNeighbor(new Vector3Int(x - 1, _groundLevel, z), out Chunk nbChunk4, out Vector3Int nbRelativePosition4))
                    {
                        if (nbChunk4.RiverBiomes[nbRelativePosition4.x + nbRelativePosition4.z * width] == chunk.RiverBiomes[i])
                            bitmask += 8;
                    }
                    else
                    {
                        if (chunk.RiverBiomes[(x - 1) + z * width] == chunk.RiverBiomes[i])
                            bitmask += 8;
                    }

                    if (bitmask != 15)
                    {
                        RiverNode riverNode = new RiverNode()
                        {
                            RelativePosition = new Vector3Int(x, _groundLevel, z),
                            Density = 5
                        };

                        chunk.RiverBfsQueue.Enqueue(riverNode);
                    }
                }
                else
                {
                    if (chunk.RiverBiomes[x + (z + 1) * width] == chunk.RiverBiomes[i])
                        bitmask += 1;
                    if (chunk.RiverBiomes[(x + 1) + z * width] == chunk.RiverBiomes[i])
                        bitmask += 2;
                    if (chunk.RiverBiomes[x + (z - 1) * width] == chunk.RiverBiomes[i])
                        bitmask += 4;
                    if (chunk.RiverBiomes[(x - 1) + z * width] == chunk.RiverBiomes[i])
                        bitmask += 8;

                    if (bitmask != 15)
                    {
                        RiverNode riverNode = new RiverNode()
                        {
                            RelativePosition = new Vector3Int(x, _groundLevel, z),
                            Density = 5
                        };

                        chunk.RiverBfsQueue.Enqueue(riverNode);
                    }
                }
            }
        }

        private void DigRiver(Chunk chunk, Queue<RiverNode> riverSpreadQueue)
        {
            Debug.Log("DigRiver");
            //Dictionary<Vector3Int, GameObject> textDict = new Dictionary<Vector3Int, GameObject>();

            int attempts = 0;
            int[,,] riversDensity = new int[chunk._width, chunk._height, chunk._depth];
            //for (int i = 0; i < riversDensity.Length; i++)
            //{
            //    riversDensity[i] = 0;
            //}
            for (int z = 0; z < chunk._width; z++)
            {
                for (int y = 0; y < chunk._height; y++)
                {
                    for (int x = 0; x < chunk._width; x++)
                    {
                        riversDensity[x, y, z] = 0;
                    }
                }
            }

            foreach (var riverNode in riverSpreadQueue)
            {
                riversDensity[riverNode.RelativePosition.x, riverNode.RelativePosition.y, riverNode.RelativePosition.z] = riverNode.Density;
            }


            RiverNode startNode = riverSpreadQueue.Peek();

            riversDensity[startNode.RelativePosition.x, startNode.RelativePosition.y, startNode.RelativePosition.z] = startNode.Density;

            //if (textDict.ContainsKey(startNode.RelativePosition) == false)
            //{
            //    GameObject textObj = Instantiate(TextPrefab, chunk.GetGlobalPosition(startNode.RelativePosition) + new Vector3(0.0f, 1.01f, 0.0f), Quaternion.Euler(90f, 0f, 0f));
            //    textObj.GetComponent<TextMeshPro>().text = $"{startNode.Density}";
            //    textDict.Add(startNode.RelativePosition, textObj);
            //}

            while (riverSpreadQueue.Count > 0)
            {
                RiverNode currentNode = riverSpreadQueue.Dequeue();
                riversDensity[currentNode.RelativePosition.x, currentNode.RelativePosition.y, currentNode.RelativePosition.z] = currentNode.Density;
                chunk.SetBiome(currentNode.RelativePosition, BiomeType.River);

                //if (textDict.ContainsKey(currentNode.RelativePosition) == false)
                //{
                //    GameObject textObj = Instantiate(TextPrefab, chunk.GetGlobalPosition(currentNode.RelativePosition) + new Vector3(0.0f, 1.01f, 0.0f), Quaternion.Euler(90f, 0f, 0f));
                //    textObj.GetComponent<TextMeshPro>().text = $"{currentNode.Density}";
                //    textDict.Add(currentNode.RelativePosition, textObj);

                //    Debug.Log("Not contain");
                //}
                //else
                //{
                //    Debug.Log($"Contain: {textDict[currentNode.RelativePosition].GetComponent<TextMeshPro>().text}     {currentNode.Density}");
                //    textDict[currentNode.RelativePosition].GetComponent<TextMeshPro>().text = $"{currentNode.Density}";
                //    textDict[currentNode.RelativePosition].GetComponent<TextMeshPro>().color = Color.black;
                //}


                var neighbors = GetNeighborsForBfsRiver(currentNode.RelativePosition);
                for (int i = 0; i < neighbors.Length; i++)
                {
                    if (chunk.IsValidRelativePosition(neighbors[i]) == false)
                    {
                        if (chunk.FindNeighbor(neighbors[i], out Chunk neighborChunk, out Vector3Int nbRelativePosition))
                        {
                            //Debug.Log($"found chunk: {neighbors[i]}");
                            RiverNode nbRiverNode = new RiverNode()
                            {
                                RelativePosition = nbRelativePosition,
                                Density = currentNode.Density - 1
                            };
                            neighborChunk.RiverBfsQueue.Enqueue(nbRiverNode);
                        }
                        else
                        {
                            //Debug.LogWarning($"Not found this chunk at: {neighbors[i]}");
                        }
                        continue;
                    }
                    if (neighbors[i] == currentNode.RelativePosition)
                    {
                        continue;
                    }



                    if ((riversDensity[neighbors[i].x, neighbors[i].y, neighbors[i].z] + 1 < currentNode.Density) && currentNode.Density > 0)
                    {
                        RiverNode nbRiverNode = new RiverNode()
                        {
                            RelativePosition = neighbors[i],
                            Density = currentNode.Density - 1
                        };

                        riverSpreadQueue.Enqueue(nbRiverNode);
                        riversDensity[nbRiverNode.RelativePosition.x, nbRiverNode.RelativePosition.y, nbRiverNode.RelativePosition.z] = nbRiverNode.Density;
                    }
                }



                attempts++;
                if (attempts > 10000)
                {
                    Debug.Log("Infinite loop");
                    break;
                }
            }
            //Debug.Log($"Spread river attempt: {attempts}");
        }

        private Vector3Int[] _riverBfsNeighbors = new Vector3Int[5];
        private Vector3Int[] GetNeighborsForBfsRiver(Vector3Int position)
        {
            _riverBfsNeighbors[0] = position + new Vector3Int(1, 0, 0);
            _riverBfsNeighbors[1] = position + new Vector3Int(-1, 0, 0);
            _riverBfsNeighbors[2] = position + new Vector3Int(0, 0, 1);
            _riverBfsNeighbors[3] = position + new Vector3Int(0, 0, -1);

            _riverBfsNeighbors[3] = position + new Vector3Int(0, -1, 0);

            return _riverBfsNeighbors;
        }
        #endregion






        #region BIOMES
        public BiomeType GetBiome(Vector3 globalPosition)
        {
            Chunk chunk = _main.GetChunk(globalPosition);
            if (chunk != null)
            {
                int localBlockX = Mathf.FloorToInt(globalPosition.x) % _chunkDimension.x;
                int localBlockY = Mathf.FloorToInt(globalPosition.y) % _chunkDimension.y;
                int localBlockZ = Mathf.FloorToInt(globalPosition.z) % _chunkDimension.z;
                int index = chunk.IndexOf(localBlockX, localBlockY, localBlockZ);

                return BiomeTable[(int)chunk.MoistureData[index], (int)chunk.HeatData[index]];
            }
            return default;
        }
        public BiomeType GetBiome(Chunk chunk, int frameX, int frameY, int frameZ)
        {
            int index = chunk.IndexOf(frameX, frameY, frameZ);
            return BiomeTable[(int)chunk.MoistureData[index], (int)chunk.HeatData[index]];
        }
        public BiomeType[] GetBiome(float[] heightMap, float[] heatMap, float moistureMap, int width, int height)
        {
            BiomeType[] biomeMap = new BiomeType[width * height];

            for (byte x = 0; x < _chunkDimension[0]; x++)
            {
                for (byte y = 0; y < _chunkDimension[2]; y++)
                {
                    int index = x + x + y * width;
                    //biomeMap[index] = BiomeTable[(int)moistureMap[index], (int)chunk.HeatData[index]];
                }
            }

            return biomeMap;
        }
        #endregion




        #region DECORs
        public async Task PlaceGrassAsync(Chunk chunk)
        {
            List<Vector2Int> distributedGrassPositions = await GenerateDistributedPositions(_grassNoiseDistribute, chunk._width, chunk._depth, _grassNoiseR);
            await Task.Run(() =>
            {
                for (int i = 0; i < distributedGrassPositions.Count; i++)
                {
                    Vector3Int currRelativePos = new Vector3Int(distributedGrassPositions[i].x, _groundLevel, distributedGrassPositions[i].y);
                    if (chunk.GetBlock(currRelativePos) == BlockType.DirtGrass)
                    {
                        Vector3Int upperRelativePos = new Vector3Int(currRelativePos.x, currRelativePos.y + 1, currRelativePos.z);
                        if (chunk.OnGroundLevel(upperRelativePos))
                        {
                            if (i % 2 == 0)
                            {
                                chunk.SetBlock(upperRelativePos, BlockType.Grass);
                            }
                            else
                            {
                                chunk.SetBlock(upperRelativePos, BlockType.TallGrass);
                                chunk.SetBlock(upperRelativePos + new Vector3Int(0,1,0), BlockType.TallGrass);
                            }

                        }
                    }
                }


                //Parallel.For(0, chunk.BiomesData.Length, (i) =>
                //{
                //    int x = i % chunk._width;
                //    int y = (i / chunk._width) % chunk._height;
                //    int z = i / (chunk._width * chunk._height);

                //    if (chunk.ChunkData[i] == BlockType.DirtGrass)
                //    {
                //        Vector3Int upperRelativePos = new Vector3Int(x, y + 1, z);
                //        if (chunk.OnGroundLevel(upperRelativePos))
                //        {
                //            chunk.SetBlock(upperRelativePos, BlockType.Grass);
                //        }
                //    }
                //});
            });      
        }
        #endregion




        #region DISTRIBUTE
        // Density generation algorithm by https://www.redblobgames.com/maps/terrain-from-noise/
        private async Task<List<Vector2Int>> GenerateDistributedPositions(FastNoiseLite noise, int width, int depth, int R)        // R: random density (smaller -> dense)
        {
            float[] blueNoise = new float[width * depth];
            List<Vector2Int> distributeList = new List<Vector2Int>();
            await Task.Run(() =>
            {
                for (int z = 0; z < depth; z++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        float nx = x / (float)width - 0.5f;
                        float ny = z / (float)depth - 0.5f;

                        // Blue noise is high frequency; adjust this value based on your needs
                        blueNoise[x + z * width] = noise.GetNoise(50 * nx, 50 * ny);
                    }
                }

                for (int zc = 0; zc < depth; zc++)
                {
                    for (int xc = 0; xc < width; xc++)
                    {
                        float max = 0;

                        for (int dy = -R; dy <= R; dy++)
                        {
                            for (int dx = -R; dx <= R; dx++)
                            {
                                int xn = dx + xc;
                                int zn = dy + zc;

                                if (0 <= zn && zn < depth && 0 <= xn && xn < width)
                                {
                                    float e = blueNoise[xn + zn * width];
                                    if (e > max) { max = e; }
                                }
                            }
                        }

                        if (blueNoise[xc + zc * width] == max)
                        {
                            // Place tree at xc, yc
                            distributeList.Add(new Vector2Int(xc, zc));
                        }
                    }
                }
            });
            return distributeList;
        }
        #endregion




        #region NEIGHBORS
        public void UpdateChunkNeighbors(Chunk chunk)
        {
            if (chunk.HasNeighbors()) return;


            // Face neighbors
            if (_main.HasChunk(chunk.RelativePosition + Vector3Int.left))
            {
                chunk.West = _main.GetChunk(chunk.RelativePosition + Vector3Int.left);
            }
            if (_main.HasChunk(chunk.RelativePosition + Vector3Int.right))
            {
                chunk.East = _main.GetChunk(chunk.RelativePosition + Vector3Int.right);
            }
            if (_main.HasChunk(chunk.RelativePosition + Vector3Int.forward))
            {
                chunk.North = _main.GetChunk(chunk.RelativePosition + Vector3Int.forward);
            }
            if (_main.HasChunk(chunk.RelativePosition + Vector3Int.back))
            {
                chunk.South = _main.GetChunk(chunk.RelativePosition + Vector3Int.back);
            }
            if (_main.HasChunk(chunk.RelativePosition + Vector3Int.up))
            {
                chunk.Up = _main.GetChunk(chunk.RelativePosition + Vector3Int.up);
            }
            if (_main.HasChunk(chunk.RelativePosition + Vector3Int.down))
            {
                chunk.Down = _main.GetChunk(chunk.RelativePosition + Vector3Int.down);
            }


            // Edge neighbors
            if (_main.HasChunk(chunk.RelativePosition + new Vector3Int(-1, 0, 1)))
            {
                chunk.Northwest = _main.GetChunk(chunk.RelativePosition + new Vector3Int(-1, 0, 1));
            }
            if (_main.HasChunk(chunk.RelativePosition + new Vector3Int(1, 0, 1)))
            {
                chunk.Northeast = _main.GetChunk(chunk.RelativePosition + new Vector3Int(1, 0, 1));
            }
            if (_main.HasChunk(chunk.RelativePosition + new Vector3Int(-1, 0, -1)))
            {
                chunk.Southwest = _main.GetChunk(chunk.RelativePosition + new Vector3Int(-1, 0, -1));
            }
            if (_main.HasChunk(chunk.RelativePosition + new Vector3Int(1, 0, -1)))
            {
                chunk.Southeast = _main.GetChunk(chunk.RelativePosition + new Vector3Int(1, 0, -1));
            }

            if (_main.HasChunk(chunk.RelativePosition + new Vector3Int(-1, 1, 0)))
            {
                chunk.UpWest = _main.GetChunk(chunk.RelativePosition + new Vector3Int(-1, 1, 0));
            }
            if (_main.HasChunk(chunk.RelativePosition + new Vector3Int(1, 1, 0)))
            {
                chunk.UpEast = _main.GetChunk(chunk.RelativePosition + new Vector3Int(1, 1, 0));
            }
            if (_main.HasChunk(chunk.RelativePosition + new Vector3Int(0, 1, 1)))
            {
                chunk.UpNorth = _main.GetChunk(chunk.RelativePosition + new Vector3Int(0, 1, 1));
            }
            if (_main.HasChunk(chunk.RelativePosition + new Vector3Int(0, 1, -1)))
            {
                chunk.UpSouth = _main.GetChunk(chunk.RelativePosition + new Vector3Int(0, 1, -1));
            }




            // Corner neighbors
            if (_main.HasChunk(chunk.RelativePosition + new Vector3Int(-1, 1, 1)))
            {
                chunk.UpNorthwest = _main.GetChunk(chunk.RelativePosition + new Vector3Int(-1, 1, 1));
            }
            if (_main.HasChunk(chunk.RelativePosition + new Vector3Int(1, 1, 1)))
            {
                chunk.UpNortheast = _main.GetChunk(chunk.RelativePosition + new Vector3Int(1, 1, 1));
            }
            if (_main.HasChunk(chunk.RelativePosition + new Vector3Int(-1, 1, -1)))
            {
                chunk.UpSouthwest = _main.GetChunk(chunk.RelativePosition + new Vector3Int(-1, 1, -1));
            }
            if (_main.HasChunk(chunk.RelativePosition + new Vector3Int(1, 1, -1)))
            {
                chunk.UpSoutheast = _main.GetChunk(chunk.RelativePosition + new Vector3Int(1, 1, -1));
            }


            if (chunk.HasNeighbors())
            {
                Chunk.OnChunkHasNeighbors?.Invoke(chunk);
            }
        }
        #endregion






        #region LIGHTING
        public void PropagateAmbientLight(Chunk chunk)
        {
            // Apply ambient light
            // I use list instead of queue because this type of light only fall down when start, 
            // use list can help this method can process in parallel. When this light hit block (not air)
            // we'll use normal bfs to spread light like with torch.
            List<LightNode> ambientLightList = new List<LightNode>();
            for (int z = 0; z < _chunkDimension[2]; z++)
            {
                for (int x = 0; x < _chunkDimension[0]; x++)
                {
                    Vector3Int lightNodeGlobalPosition = chunk.GlobalPosition + new Vector3Int(x, _chunkDimension[1] - 1, z);
                    Vector3Int lightNodeRelativePosition = WorldCoordHelper.GlobalToRelativeBlockPosition(lightNodeGlobalPosition);
                    ambientLightList.Add(new LightNode(lightNodeRelativePosition, LightUtils.MaxLightIntensity));
                }
            }
            LightCalculator.PropagateAmbientLight(chunk, ambientLightList);
        }
        #endregion








        #region NOISE
        public float DomainWarping(float x, float y, FastNoiseLite simplex)
        {
            Vector2 p = new Vector2(x, y);

            Vector2 q = new Vector2((float)simplex.GetNoise(p.x, p.y),
                                    (float)simplex.GetNoise(p.x + 52.0f, p.y + 13.0f));


            ////Vector2 l2p1 = (p + 40 * q) + new Vector2(77, 35);
            ////Vector2 l2p2 = (p + 40 * q) + new Vector2(83, 28);

            ////Vector2 r = new Vector3((float)simplex.GetNoise(l2p1.x, l2p1.y),
            ////                        (float)simplex.GetNoise(l2p2.x, l2p2.y));


            //Vector2 l3 = p + 120 * r;
            Vector2 l3 = p + 40 * q;
            return (float)simplex.GetNoise(l3.x, l3.y);
        }

        public float DomainWarping(float x, float y, FastNoiseLite simplex, FastNoiseLite voronoi)
        {
            Vector2 p = new Vector2(x, y);

            Vector2 q = new Vector2((float)simplex.GetNoise(p.x, p.y),
                                    (float)simplex.GetNoise(p.x + 52.0f, p.y + 13.0f));


            //Vector2 l2p1 = (p + 40 * q) + new Vector2(77, 35);
            //Vector2 l2p2 = (p + 40 * q) + new Vector2(83, 28);

            //Vector2 r = new Vector3((float)simplex.GetNoise(l2p1.x, l2p1.y),
            //                        (float)simplex.GetNoise(l2p2.x, l2p2.y));


            //Vector2 l3 = p + 120 * r;
            Vector2 l3 = p + 40 * q;
            return voronoi.GetNoise(l3.x, l3.y);
        }

        public float ScaleNoise(float noiseValue, float oldMin, float oldMax, float newMin, float newMax)
        {
            return (noiseValue - oldMin) * (newMax - newMin) / (oldMax - oldMin) + newMin;
        }
        #endregion



        #region ALGORITHM
        // Use to detect block that has height > 1. like(door, tall grass, cactus,...)
        // This method only check downward
        public int GetBlockHeightFromOrigin(Chunk chunk, Vector3Int relativePosition)
        {
            int heightFromOrigin = 0;   // At origin
            int attempt = 0;
            BlockType blockNeedCheck = chunk.GetBlock(relativePosition);
            Vector3Int currBlockPos = relativePosition;
            while(true)
            {
                Vector3Int nextRelativePosition = new Vector3Int(currBlockPos.x, currBlockPos.y - 1, currBlockPos.z);
                if(blockNeedCheck == chunk.GetBlock(nextRelativePosition))
                {
                    currBlockPos = nextRelativePosition;
                    heightFromOrigin++;
                }
                else
                {
                    break;
                }

                if (attempt++ > 100)
                {
                    Debug.Log("Infinite loop");
                    break;
                }
            }
            return heightFromOrigin;
        }

        #endregion
    }


}

