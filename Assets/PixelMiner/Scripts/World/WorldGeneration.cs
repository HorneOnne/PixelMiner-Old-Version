using System;
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
using PixelMiner.Core;
using PixelMiner.Lighting;


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
        [FoldoutGroup("Heatmap")] public float ColdestValue = 0.1f;
        [FoldoutGroup("Heatmap")] public float ColderValue = 0.2f;
        [FoldoutGroup("Heatmap")] public float ColdValue = 0.4f;
        [FoldoutGroup("Heatmap")] public float WarmValue = 0.6f;
        [FoldoutGroup("Heatmap")] public float WarmerValue = 0.8f;
        [FoldoutGroup("Heatmap")] public float WarmestValue = 1.0f;
        private ModuleBase _heatModule;



        // Moisture map
        // ======
        [InfoBox("Moisture map noise settings")]
        [FoldoutGroup("Moisture map"), Indent(1)] public int MoistureOctaves = 4;
        [FoldoutGroup("Moisture map"), Indent(1)] public double MoistureFrequency = 0.03;
        [FoldoutGroup("Moisture map"), Indent(1)] public double MoistureLacunarity = 2.0f;
        [FoldoutGroup("Moisture map"), Indent(1)] public double MoisturePersistence = 0.5f;
        [InfoBox("Moisture map noise settings"), Space(10)]
        [FoldoutGroup("Moisture map")] public float DryestValue = 0.22f;
        [FoldoutGroup("Moisture map")] public float DryerValue = 0.35f;
        [FoldoutGroup("Moisture map")] public float DryValue = 0.55f;
        [FoldoutGroup("Moisture map")] public float WetValue = 0.75f;
        [FoldoutGroup("Moisture map")] public float WetterValue = 0.85f;
        [FoldoutGroup("Moisture map")] public float WettestValue = 1.0f;
        private ModuleBase _moistureModule;



        // River
        // =====
        [FoldoutGroup("River"), Indent(1)] public int RiverOctaves = 4;
        [FoldoutGroup("River"), Indent(1)] public double RiverFrequency = 0.02;
        [FoldoutGroup("River"), Indent(1)] public double RiverDisplacement = 1.0f;
        [FoldoutGroup("River"), Indent(1)] public bool RiverDistance = false;
        [FoldoutGroup("River"), Indent(1)] public double RiverLacunarity = 2.0f;
        [FoldoutGroup("River"), Indent(1)] public double RiverPersistence = 0.5f;
        [FoldoutGroup("River"), Indent(1), MinMaxSlider(0f, 1f, true)] public Vector2 RiverRange = new Vector2(0.7f, 0.75f);
        private ModuleBase _riverModuleVoronoi;
        private ModuleBase _riverModulePerlin;



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
		    { BiomeType.Ice,    BiomeType.Tundra,   BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYEST
		    { BiomeType.Ice,    BiomeType.Tundra,   BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYER
		    { BiomeType.Ice,    BiomeType.Tundra,   BiomeType.Woodland,     BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //DRY
		    { BiomeType.Ice,    BiomeType.Tundra,   BiomeType.BorealForest, BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //WET
		    { BiomeType.Ice,    BiomeType.Tundra,   BiomeType.BorealForest, BiomeType.SeasonalForest,      BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest },  //WETTER
		    { BiomeType.Ice,    BiomeType.Tundra,   BiomeType.BorealForest, BiomeType.TemperateRainforest, BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest }   //WETTEST
        };



        // Cached
        private Vector3Int _chunkDimension;
        private byte _calculateNoiseRangeSampleMultiplier = 15;  // 50 times. 50 * 100000 = 5 mil times.
        private int _calculateNoiseRangeCount = 1000000;    // 1 mil times.
        private float _minWorldNoiseValue = float.MaxValue;
        private float _maxWorldNoiseValue = float.MinValue;
        private Main _main;
        private WorldLoading _worldLoading;
        #endregion


        public AnimationCurve LightAnimCurve;

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
            _heatModule = new Perlin(HeatFrequency, HeatLacunarity, HeatPersistence, HeatOctaves, Seed + 1, QualityMode.High);
            _moistureModule = new Perlin(MoistureFrequency, MoistureLacunarity, MoisturePersistence, MoistureOctaves, Seed + 2, QualityMode.High);

            _riverModulePerlin = new Perlin(RiverFrequency, RiverLacunarity, RiverPersistence, RiverOctaves, Seed + 3, QualityMode.Medium);
            _riverModuleVoronoi = new Voronoi(RiverFrequency, RiverDisplacement, Seed, RiverDistance);
        }

        private void Start()
        {
            _main = Main.Instance;
            _worldLoading = WorldLoading.Instance;

            Seed = WorldGenUtilities.StringToSeed(_main.SeedInput);
            UnityEngine.Random.InitState(Seed);

            _chunkDimension = _main.ChunkDimension;


            // World Initialization
            InitWorldAsyncInSequence(_worldLoading.LastChunkFrame.x, _worldLoading.LastChunkFrame.z, widthInit: _worldLoading.InitWorldWidth, depthInit: _worldLoading.InitWorldDepth, () =>
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


        private async void DrawChunk(Chunk chunk)
        {
            if (!chunk.ChunkHasDrawn)
            {
                //chunk.DrawChunkAsync();


                MeshData solidMeshData = await MeshUtils.SolidGreedyMeshingAsync(chunk, LightAnimCurve);
                //MeshData waterMeshData = await MeshUtils.WaterGreedyMeshingAsync(this);
                MeshData colliderMeshData = await MeshUtils.SolidGreedyMeshingForColliderAsync(chunk);

                chunk.SolidMeshFilter.sharedMesh = CreateMesh(solidMeshData);
                //WaterMeshFilter.sharedMesh =  CreateMesh(waterMeshData);

                chunk.MeshCollider.sharedMesh = null;
                chunk.MeshCollider.sharedMesh = CreateColliderMesh(colliderMeshData);


                // Release mesh data
                MeshDataPool.Release(solidMeshData);
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
                    Chunk newChunk = await GenerateNewChunk(x, 0, z, _main.ChunkDimension);
                    _worldLoading.LoadChunk(newChunk);
                }
            }

            onFinished?.Invoke();
            //_worldLoading.LoadChunksAroundPositionInSequence();
        }



        public async Task<Chunk> GenerateNewChunk(int frameX, int frameY, int frameZ, Vector3Int chunkDimension, bool applyDefaultData = true)
        {
            Vector3Int frame = new Vector3Int(frameX, frameY, frameZ);
            Vector3 worldPosition = frame * new Vector3Int(chunkDimension[0], chunkDimension[1], chunkDimension[2]);
            Chunk newChunk = Instantiate(_chunkPrefab, worldPosition, Quaternion.identity, _chunkParent.transform);
            newChunk.Init(frameX, frameY, frameZ, chunkDimension[0], chunkDimension[1], chunkDimension[2]);


            if (applyDefaultData)
            {
                float[] heightValues = await GetHeightMapDataAsync(newChunk.FrameX, newChunk.FrameZ, chunkDimension[0], chunkDimension[2]);
                float[] heatValues = await GetHeatMapDataAysnc(newChunk.FrameX, newChunk.FrameZ, chunkDimension[0], chunkDimension[2]);
                float[] moistureValues = await GetMoistureMapDataAsync(newChunk.FrameX, newChunk.FrameZ, chunkDimension[0], chunkDimension[2]);
                float[] riverValues = await GetRiverDataAsync(newChunk.FrameX, newChunk.FrameZ, chunkDimension[0], chunkDimension[2]);
                heightValues = await DigRiverAsync(heightValues, riverValues, chunkDimension[0], chunkDimension[2]);
                moistureValues = await ApplyHeightDataToMoistureDataAsync(heightValues, moistureValues, chunkDimension[0], chunkDimension[2]);

                await LoadHeightMapDataAsync(newChunk, heightValues);
                await LoadHeatMapDataAsync(newChunk, heatValues);
                await LoadMoistureMapDataAsync(newChunk, moistureValues);
                await GenerateBiomeMapDataAsync(newChunk);

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
            }

            return newChunk;
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
                        float heightValue = (float)_heightModule.GetValue(offsetX, 0, offsetZ);
                        float normalizeHeightValue = (heightValue - _minWorldNoiseValue) / (_maxWorldNoiseValue - _minWorldNoiseValue);
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
                for (int z = 0; z < height; z++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        float offsetX = frameX * width + x;
                        float offsetZ = frameZ * height + z;
                        float heatValue = (float)_heatModule.GetValue(offsetX, 0, offsetZ);
                        float normalizeHeatValue = (heatValue - _minWorldNoiseValue) / (_maxWorldNoiseValue - _minWorldNoiseValue);

                        fractalNoiseData[WorldGenUtilities.IndexOf(x, z, width)] = normalizeHeatValue;
                    }
                }
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
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < height; z++)
                    {
                        float offsetX = frameX * width + x;
                        float offsetZ = frameZ * height + z;
                        float moisetureValue = (float)_moistureModule.GetValue(offsetX, 0, offsetZ);
                        float normalizeMoistureValue = (moisetureValue - _minWorldNoiseValue) / (_maxWorldNoiseValue - _minWorldNoiseValue);

                        moistureData[WorldGenUtilities.IndexOf(x, z, width)] = normalizeMoistureValue;
                    }
                }
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
                        float riverValue = (float)_riverModulePerlin.GetValue(offsetX, 0, offsetZ);
                        float normalizeRiverValue = (riverValue - _minWorldNoiseValue) / (_maxWorldNoiseValue - _minWorldNoiseValue);

                        riverValues[x + y * width] = normalizeRiverValue;
                        //riverValues[x + y * width] = normalizeRiverValue > 0.3f && normalizeRiverValue < 0.4f ? normalizeRiverValue : 0;
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
                int width = chunk.Dimensions[0];
                int height = chunk.Dimensions[1];
                int depth = chunk.Dimensions[2];

                //Debug.Log($"Ground: {Mathf.FloorToInt(Water * _chunkHeight)}");
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

                            int terrainHeight = Mathf.FloorToInt(heightValue * height);


                            if (y <= averageGroundLayer)
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
                                else if (y < height * Water)
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

                            if (heatValue < WorldGeneration.Instance.ColdestValue)
                            {
                                chunk.HeatData[index3D] = HeatType.Coldest;
                            }
                            else if (heatValue < WorldGeneration.Instance.ColderValue)
                            {
                                chunk.HeatData[index3D] = HeatType.Colder;
                            }
                            else if (heatValue < WorldGeneration.Instance.ColdValue)
                            {
                                chunk.HeatData[index3D] = HeatType.Cold;
                            }
                            else if (heatValue < WorldGeneration.Instance.WarmValue)
                            {
                                chunk.HeatData[index3D] = HeatType.Warm;
                            }
                            else if (heatValue < WorldGeneration.Instance.WarmerValue)
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
        public async Task GenerateBiomeMapDataAsync(Chunk chunk)
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
                            chunk.BiomesData[index3D] = GetBiome(chunk, x, y, z);
                        }
                    }
                }
            });
        }

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
        public async Task<float[]> DigRiverAsync(float[] heightValues, float[] riverValues, int width, int height)
        {
            await Task.Run(() =>
            {
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < height; z++)
                    {
                        int index = x + z * width;
                        float heightValue = heightValues[index];
                        float riverValue = riverValues[index];

                        if (riverValue > 0.5f && riverValue < 0.6f && heightValue >= Water)
                        {
                            heightValues[index] -= 0.6f * riverValue;
                            heightValues[index] = Mathf.Clamp(heightValues[index], 0.4f, 1f);
                        }

                    }
                }

            });

            return heightValues;
        }
        public void LoadBiomesMap(Chunk chunk)
        {
            int width = chunk.Dimensions[0];
            int height = chunk.Dimensions[1];
            int depth = chunk.Dimensions[2];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        chunk.BiomesData[chunk.IndexOf(x, y, z)] = GetBiome(chunk, x, y, z);
                    }
                }
            }
        }


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
#if false
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
#endif


        #region Neighbors
        public void UpdateChunkNeighbors(Chunk chunk)
        {
            if (chunk.HasNeighbors()) return;


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

            if (_main.HasChunk(chunk.RelativePosition + new Vector3Int(-1, 0, 1)))
            {
                // Northwest
                chunk.Northwest = _main.GetChunk(chunk.RelativePosition + new Vector3Int(-1, 0, 1));
            }
            if (_main.HasChunk(chunk.RelativePosition + new Vector3Int(1, 0, 1)))
            {
                // Northeast
                chunk.Northeast = _main.GetChunk(chunk.RelativePosition + new Vector3Int(1, 0, 1));
            }
            if (_main.HasChunk(chunk.RelativePosition + new Vector3Int(-1, 0, -1)))
            {
                // Southwest
                chunk.Southwest = _main.GetChunk(chunk.RelativePosition + new Vector3Int(-1, 0, -1));
            }
            if (_main.HasChunk(chunk.RelativePosition + new Vector3Int(1, 0, -1)))
            {
                // Southeast
                chunk.Southeast = _main.GetChunk(chunk.RelativePosition + new Vector3Int(1, 0, -1));
            }

            if(chunk.HasNeighbors())
            {
                Chunk.OnChunkHasNeighbors?.Invoke(chunk);
            }
            

            return;
            if (chunk.West == null)
            {
                Chunk leftNeighborChunk = Main.Instance.GetChunkNeighborLeft(chunk);
                if (leftNeighborChunk != null)
                {
                    chunk.SetNeighbors(BlockSide.Left, leftNeighborChunk);
                    leftNeighborChunk.SetNeighbors(BlockSide.Right, chunk);
                }
            }
            if (chunk.East == null)
            {
                Chunk rightNeighborChunk = Main.Instance.GetChunkNeighborRight(chunk);
                if (rightNeighborChunk != null)
                {
                    chunk.SetNeighbors(BlockSide.Right, rightNeighborChunk);
                    rightNeighborChunk.SetNeighbors(BlockSide.Left, chunk);
                }
            }
            if (chunk.North == null)
            {
                Chunk frontNeighborChunk = Main.Instance.GetChunkNeighborFront(chunk);
                if (frontNeighborChunk != null)
                {
                    chunk.SetNeighbors(BlockSide.Front, frontNeighborChunk);
                    frontNeighborChunk.SetNeighbors(BlockSide.Back, chunk);
                }
            }
            if (chunk.South == null)
            {
                Chunk backNeighborChunk = Main.Instance.GetChunkNeighborBack(chunk);
                if (backNeighborChunk != null)
                {
                    chunk.SetNeighbors(BlockSide.Back, backNeighborChunk);
                    backNeighborChunk.SetNeighbors(BlockSide.Front, chunk);
                }
            }
        }
        #endregion


        #region Lighting
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
    }
}

