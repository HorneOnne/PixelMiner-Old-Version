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

        [SerializeField] private Chunk _chunkPrefab;

        [Header("World Settings")]
        public int ChunkWidth = 16;      // Size of each chunk in tiles
        public int ChunkHeight = 16;      // Size of each chunk in tiles
        public int InitWorldWidth = 3;
        public int InitWorldHeight = 3;
        public int WorldSize = 1;       // Number of chunks in the world
        private int _calculateNoiseRangeCount = 500000;


        // Min and Max Height used for normalize noise value in range [0-1]
        public float MinHeightNoise { get; private set; } = float.MaxValue;
        public float MaxHeightNoise { get; private set; } = float.MinValue;

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
        // Heat Gradient
        public float ColdestValue = 0.1f;
        public float ColderValue = 0.2f;
        public float ColdValue = 0.4f;
        public float WarmValue = 0.6f;
        public float WarmerValue = 0.8f;
        private float _gradientHeatmapSize = 256;
        public static Color ColdestColor = new Color(0, 1, 1, 1);
        public static Color ColderColor = new Color(170 / 255f, 1, 1, 1);
        public static Color ColdColor = new Color(0, 229 / 255f, 133 / 255f, 1);
        public static Color WarmColor = new Color(1, 1, 100 / 255f, 1);
        public static Color WarmerColor = new Color(1, 100 / 255f, 0, 1);
        public static Color WarmestColor = new Color(241 / 255f, 12 / 255f, 0, 1);
        // Heat Fratal
        public int HeatOctaves = 4;
        public double HeatFrequency = 0.02;
        public double HeatLacunarity = 2.0f;
        public double HeatPersistence = 0.5f;
        public int HeatSeed = 7;
        private ModuleBase _heatModule;




        [Header("World Generation Utilities")]
        public bool AutoUnloadChunk = true;
        public bool ShowChunksBorder = false;
        public bool ShowTilegroupMaps = false;


        [Header("Tilemap")]
        public readonly Vector3 CELL_SIZE = new Vector3(2.0f, 1.0f, 1.0f);
        public readonly Vector3 CELL_GAP = new Vector3(0.0f, 0.0f, 0.0f);



        [Header("Data Cached")]
        private Dictionary<Vector2Int, Chunk> _chunks;
        public HashSet<Chunk> ActiveChunks;


        // Cached
        private Vector2Int lastChunkISOFrame;
        private Vector2 _centerPoint;
        private Vector2Int _centerPointFrame;


        /*
            World generation slider range.
            0.0f -> 0.3f: Calculate noise range from specific seed.
            0.3f -> 1.0f: Loading chunk data.
         */


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
            _heightModule = new Perlin(Frequency, Lacunarity, Persistence, Octaves, Seed, QualityMode.High);
            _heatModule = new Perlin(HeatFrequency, HeatLacunarity, HeatPersistence, HeatOctaves, HeatSeed, QualityMode.High);

            // Load chunks around the player's starting position
            lastChunkISOFrame = IsometricUtilities.ReverseConvertWorldPositionToIsometricFrame(_centerPoint, ChunkWidth, ChunkHeight);


            // World Initialization
            InitWorldAsync(lastChunkISOFrame.x, lastChunkISOFrame.y, widthInit: InitWorldWidth, heightInit: InitWorldHeight, () =>
            {
                LoadChunksAroundPosition(lastChunkISOFrame.x, lastChunkISOFrame.y, offset: WorldSize);
            });

        }

        private void Update()
        {
            _centerPoint = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2f, Screen.height / 2f));
            _centerPointFrame = IsometricUtilities.ReverseConvertWorldPositionToIsometricFrame(_centerPoint, ChunkWidth, ChunkHeight);

            if (_centerPointFrame != lastChunkISOFrame)
            {
                lastChunkISOFrame = _centerPointFrame;
                LoadChunksAroundPosition(_centerPointFrame.x, _centerPointFrame.y, offset: WorldSize);
            }
        }


        private async void InitWorldAsync(int initIsoFrameX, int initIsoFrameY, int widthInit, int heightInit, System.Action onFinished = null)
        {
            UIGameManager.Instance.DisplayWorldGenCanvas(true);

            await ComputeNoiseRangeAsync();

            Debug.Log($"min: {MinHeightNoise}");
            Debug.Log($"max: {MaxHeightNoise}");

            int totalIterations = (2 * widthInit + 1) * (2 * heightInit + 1);
            int currentIteration = 0;
            for (int x = initIsoFrameX - widthInit; x <= initIsoFrameX + widthInit; x++)
            {
                for (int y = initIsoFrameY - heightInit; y <= initIsoFrameY + heightInit; y++)
                {
                    Vector2Int nbIsoFrame = new Vector2Int(x, y);
                    Chunk newChunk = await AddNewChunkAsync(x, y);
                    _chunks.Add(nbIsoFrame, newChunk);
                    newChunk.UnloadChunk();


                    // Update the slider value based on progress
                    currentIteration++;
                    float progress = (float)currentIteration / totalIterations;
                    float mapProgress = MathHelper.Map(progress, 0f, 1f, 0.3f, 1.0f);
                    UIGameManager.Instance.CanvasWorldGen.SetWorldGenSlider(mapProgress);
                }
            }
            await Task.Delay(100);

            UIGameManager.Instance.DisplayWorldGenCanvas(false);
            onFinished?.Invoke();
        }

        public void ComputeNoiseRange()
        {
            int sampleCount = 1000000; // Number of noise samples to generate
            double minValue = double.MaxValue;
            double maxValue = double.MinValue;
            System.Random rand = new System.Random(Seed);

            for (int i = 0; i < sampleCount; i++)
            {
                double x = rand.Next(); // Replace this with your desired coordinate values
                double y = rand.Next();
                double noiseValue = _heightModule.GetValue(x, y, 0); // Generate noise value

                // Update min and max values
                if (noiseValue < minValue)
                    minValue = noiseValue;
                if (noiseValue > maxValue)
                    maxValue = noiseValue;
            }
        }


        public async Task ComputeNoiseRangeAsync()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            float minNoiseValue = float.MaxValue;
            float maxNoiseValue = float.MinValue;
            System.Random rand = new System.Random(Seed);

            int progressCounter = _calculateNoiseRangeCount - 1;

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
                            UIGameManager.Instance.CanvasWorldGen.SetWorldGenSlider(mapProgress); ;
                        });
                    }
                }
            });


            MinHeightNoise = minNoiseValue;
            MaxHeightNoise = maxNoiseValue;

            sw.Stop();
            Debug.Log($"Compute noise time: {sw.ElapsedMilliseconds / 1000f} s");
        }



        // Load chunks around a given chunk position
        private async void LoadChunksAroundPosition(int isoFrameX, int isoFrameY, int offset = 1)
        {
            for (int x = isoFrameX - offset; x <= isoFrameX + offset; x++)
            {
                for (int y = isoFrameY - offset; y <= isoFrameY + offset; y++)
                {
                    Vector2Int nbIsoFrame = new Vector2Int(x, y);
                    if (_chunks.ContainsKey(nbIsoFrame) == false)
                    {
                        //Chunk newChunk =  await AddNewChunkAsync(x,y);
                        Chunk newChunk = AddNewChunk(x, y);
                        //newChunk.DrawChunk();
                        await newChunk.DrawChunkAsync();

                        if (x == isoFrameX && y == isoFrameY)
                        {
                            // Center
                            // ......
                        }


                        // Cached chunk data
                        if (_chunks.ContainsKey(nbIsoFrame) == false)
                            _chunks.Add(nbIsoFrame, newChunk);
                        ActiveChunks.Add(newChunk);

                        _chunks[nbIsoFrame].LoadChunk();

                        UpdateChunkTileNeighbors(newChunk);
                    }
                    else
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
                            _chunks[nbIsoFrame].PaintGradientMap();
                        }

                        UpdateChunkTileNeighbors(_chunks[nbIsoFrame]);
                    }

                }
            }

            if (ShowChunksBorder == false)
            {
                SortActiveChunkByDepth();
            }
        }



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



        private async Task<float[,]> GetHeightMapNoiseAsyc(int isoFrameX, int isoFrameY)
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
                        float normalizeHeightValue = (heightValue - MinHeightNoise) / (MaxHeightNoise - MinHeightNoise);
                        heightValues[x, y] = normalizeHeightValue;
                    }
                });
            });

            return heightValues;
        }


        public float MinHeatValue = float.MaxValue;
        public float MaxHeatValue = float.MinValue;
        private async Task<float[,]> GetGradientMapAsync(int isoFrameX, int isoFrameY)
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
        private async Task<float[,]> GetFractalHeatMapAsync(int isoFrameX, int isoFrameY)
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
                        float normalizeHeatValue = (heatValue - MinHeightNoise) / (MaxHeightNoise - MinHeightNoise);

                        fractalNoiseData[x, y] = normalizeHeatValue;
                    }
                }
            });

            //Debug.Log("GetFractalHeatMapAsync Finish");
            return fractalNoiseData;
        }
        private async Task<float[,]> GetHeatMapAysnc(int isoFrameX, int isoFrameY)
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
            Task<float[,]> gradientTask = GetGradientMapAsync(isoFrameX, isoFrameY);
            Task<float[,]> fractalNoiseTask = GetFractalHeatMapAsync(isoFrameX, isoFrameY);

            // Await for both tasks to complete
            await Task.WhenAll(gradientTask, fractalNoiseTask);
            float[,] gradientValues = gradientTask.Result;
            float[,] fractalNoiseValues = fractalNoiseTask.Result;

            // Blend the maps
            float[,] heatValues = BlendMapData(gradientValues, fractalNoiseValues, HeatMapBlendFactor);
            return heatValues;
        }


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

        private async Task<Chunk> AddNewChunkAsync(int isoFrameX, int isoFrameY)
        {
            Vector2 frame = IsometricUtilities.IsometricFrameToWorldFrame(isoFrameX, isoFrameY);
            Vector3 worldPosition = IsometricUtilities.ConvertIsometricFrameToWorldPosition(isoFrameX, isoFrameY, ChunkWidth, ChunkHeight);
            Chunk newChunk = Instantiate(_chunkPrefab, worldPosition, Quaternion.identity);
            newChunk.Init(frame.x, frame.y, isoFrameX, isoFrameY, ChunkWidth, ChunkHeight);

            // Create new data
            float[,] heightValues = await GetHeightMapNoiseAsyc(isoFrameX, isoFrameY);
            float[,] heatValues = await GetHeatMapAysnc(isoFrameX, isoFrameY);

            //await newChunk.LoadHeightMapDataAsync(heightValues);
            //await newChunk.LoadHeatMapDataAsync(heatValues);
            await newChunk.LoadHeightAndHeatMap(heightValues, heatValues);

            return newChunk;
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
            if (chunk.HasNeighbors()) return;

            // Find chunk neighbors
            Chunk nbAbove = GetChunkNeighborAbove(chunk);
            Chunk nbBelow = GetChunkNeighborBelow(chunk);
            Chunk nbLeft = GetChunkNeighborLeft(chunk);
            Chunk nbRight = GetChunkNeighborRight(chunk);
            chunk.SetTwoSidesChunkNeighbors(nbLeft, nbRight, nbAbove, nbBelow);

            if (chunk.HasNeighbors())
            {
                chunk.UpdateAllTileNeighbors();
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
                nbRight.PaintNeighborsColor();

                if (nbRight.Waters.Count == 0 && nbRight.Lands.Count == 0)
                {
                    FloodFill(nbRight);
                    nbRight.PaintTilegroupMap();
                }

            }
        }

        public Chunk GetChunkFromWorldPosition(Vector2 mousePosition)
        {
            var frame = IsometricUtilities.ReverseConvertWorldPositionToIsometricFrame(mousePosition,
                                                                               ChunkWidth,
                                                                               ChunkHeight);
            if (_chunks.ContainsKey(frame))
            {
                return _chunks[frame];
            }
            return null;
        }

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


        public float[,] BlendMapData(float[,] data01, float[,] data02, float blendFactor)
        {
            int width = data01.GetLength(0);
            int height = data02.GetLength(1);

            float[,] blendedData = new float[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    blendedData[x, y] = Mathf.Lerp(data01[x, y], data02[x, y], blendFactor);
                }
            }

            return blendedData;
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
    }
}

