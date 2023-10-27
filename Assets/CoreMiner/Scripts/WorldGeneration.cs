using System.Collections.Generic;
using UnityEngine;
using LibNoise;
using LibNoise.Generator;
using System.Linq;
using System.Threading.Tasks;
using CoreMiner.UI;
using System.Collections;
using CoreMiner.Utilities;
using UnityEditor;
using System.Threading;

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
        private float TileSize = 1.0f;
        // Min and Max Height used for normalize noise value in range [0-1]
        public float MinHeightNoise { get; private set; } = float.MaxValue;
        public float MaxHeightNoise { get; private set; } = float.MinValue;


        [Header("Noise Settings")]
        public int Octaves = 6;
        public double Frequency = 0.02f;
        public double Lacunarity = 2.0f;
        public double Persistence = 0.5f;
        public int Seed = 7;
        private ModuleBase _heightNoise;

        [Header("Height Threshold")]
        public float DeepWater = 0.2f;
        public float Water = 0.4f;
        public float Sand = 0.5f;
        public float Grass = 0.7f;
        public float Forest = 0.8f;
        public float Rock = 0.9f;
        public float Snow = 1;


        [Header("World Generation Utilities")]
        public bool AutoUnloadChunk = true;
        public bool ShowChunksBorder = false;


        [Header("Tilemap")]
        public readonly Vector3 CELL_SIZE = new Vector3(2.0f, 1.1547f, 1.0f);
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
            _heightNoise = new Perlin(Frequency, Lacunarity, Persistence, Octaves, Seed, QualityMode.High);

            // Load chunks around the player's starting position
            lastChunkISOFrame = IsometricUtilities.ReverseConvertWorldPositionToIsometricFrame(_centerPoint, ChunkWidth, ChunkHeight);

            
            // World Initialization
            InitWorldAsync(lastChunkISOFrame.x, lastChunkISOFrame.y, widthInit: InitWorldWidth, heightInit: InitWorldHeight, ()=>
            {
                LoadChunksAroundPosition(lastChunkISOFrame.x, lastChunkISOFrame.y, offset: WorldSize);
            });
      
        }

        private float _updateFrequency = 0.2f;
        private float _updateTimer = 0.0f;
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


        private async void InitWorldAsync(int initIsoFrameX, int initIsoFrameY,int widthInit, int heightInit, System.Action onFinished = null)
        {        
            UIGameManager.Instance.DisplayWorldGenCanvas(true);

            await ComputeNoiseRangeAsync();


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
                double noiseValue = _heightNoise.GetValue(x, y, 0); // Generate noise value

                // Update min and max values
                if (noiseValue < minValue)
                    minValue = noiseValue;
                if (noiseValue > maxValue)
                    maxValue = noiseValue;
            }
        }
        public async Task ComputeNoiseRangeAsync()
        {
            int sampleCount = 1000000; // Number of noise samples to generate
            float minNoiseValue = float.MaxValue;
            float maxNoiseValue = float.MinValue;
            System.Random rand = new System.Random(Seed);

            int progressCounter = sampleCount - 1;
 
            await Task.Run(() =>
            {
                for (int i = 0; i < sampleCount; i++)
                {
                    double x = rand.Next();
                    double y = rand.Next();
                    float noiseValue = (float)_heightNoise.GetValue(x, y, 0); // Generate noise value

                    // Update min and max values
                    if (noiseValue < minNoiseValue)
                        minNoiseValue = noiseValue;
                    if (noiseValue > maxNoiseValue)
                        maxNoiseValue = noiseValue;

                    // 46655 = 6**6 - 1 (use & operator compare to improve performance)
                    if ((i & 46655) == 0)
                    {
                        float progress = (float)i/ sampleCount;
                        float mapProgress = MathHelper.Map(progress, 0f, 1f, 0.0f, 0.3f);     
                        UnityMainThreadDispatcher.Instance().Enqueue(() => {
                            UIGameManager.Instance.CanvasWorldGen.SetWorldGenSlider(mapProgress);;
                        });
                    }
                }
            });

            MinHeightNoise = minNoiseValue;
            MaxHeightNoise = maxNoiseValue;
        }


        // Load chunks around a given chunk position
        private void LoadChunksAroundPosition(int isoFrameX, int isoFrameY, int offset = 1)
        {
            for (int x = isoFrameX - offset; x <= isoFrameX + offset; x++)
            {
                for (int y = isoFrameY - offset; y <= isoFrameY + offset; y++)
                {
                    Vector2Int nbIsoFrame = new Vector2Int(x, y);
                    if (_chunks.ContainsKey(nbIsoFrame) == false)
                    {
                        if (x == isoFrameX && y == isoFrameY)
                        {
                            // Center
                            // ......
                        }

                        //Chunk newChunk =  await AddNewChunkAsync(x,y);
                        Chunk newChunk = AddNewChunk(x, y);
                        newChunk.DrawChunk();

                        // Cached chunk data
                        if (_chunks.ContainsKey(nbIsoFrame) == false)
                            _chunks.Add(nbIsoFrame, newChunk);
                        ActiveChunks.Add(newChunk);

                        // Find chunk neighbors
                        Chunk nbAbove = GetChunkNeighborAbove(newChunk);
                        Chunk nbBelow = GetChunkNeighborBelow(newChunk);
                        Chunk nbLeft = GetChunkNeighborLeft(newChunk);
                        Chunk nbRight = GetChunkNeighborRight(newChunk);
                        newChunk.SetChunkNeighbors(nbLeft, nbRight, nbAbove, nbBelow);
                        if (nbAbove != null)
                        {
                            if(nbAbove.HasNeighbors() == false)
                            {
                                AddChunkFourDirectionNeighbors(nbAbove);
                                //nbAbove.UpdateAllTileNeighbors();
                                nbAbove.UpdateEdgeOfChunkTileNeighbors();
                                nbAbove.PaintNeighborsColor();
                            }  
                        }
                        if (nbBelow != null)
                        {
                            if (nbBelow.HasNeighbors() == false)
                            {
                                AddChunkFourDirectionNeighbors(nbBelow);
                                //nbBelow.UpdateAllTileNeighbors();
                                nbBelow.UpdateEdgeOfChunkTileNeighbors();
                                nbBelow.PaintNeighborsColor();
                            }                          
                        }
                        if (nbLeft != null)
                        {
                            if(nbLeft.HasNeighbors() == false)
                            {
                                AddChunkFourDirectionNeighbors(nbLeft);
                                //nbLeft.UpdateAllTileNeighbors();
                                nbLeft.UpdateEdgeOfChunkTileNeighbors();
                                nbLeft.PaintNeighborsColor();
                            }           
                        }
                        if (nbRight != null)
                        {
                            if(nbRight.HasNeighbors() == false)
                            {
                                AddChunkFourDirectionNeighbors(nbRight);
                                //nbRight.UpdateAllTileNeighbors();
                                nbRight.UpdateEdgeOfChunkTileNeighbors();
                                nbRight.PaintNeighborsColor();
                            }                         
                        }


                        newChunk.UpdateAllTileNeighbors();
                        newChunk.PaintNeighborsColor();
                    }
                    else
                    {
                        _chunks[nbIsoFrame].gameObject.SetActive(true);
                        ActiveChunks.Add(_chunks[nbIsoFrame]);
                        if (_chunks[nbIsoFrame].ChunkHasDrawn == false)
                        {
                            _chunks[nbIsoFrame].DrawChunk();
                        }
                    }

                }
            }

            if (ShowChunksBorder == false)
            {
                SortActiveChunkByDepth();
            }

        }



        private float[,] GetHeightMapNoise(int frameX, int frameY)
        {
            float[,] heightValues = new float[ChunkWidth, ChunkHeight];
            for (int x = 0; x < ChunkWidth; x++)
            {
                for (int y = 0; y < ChunkHeight; y++)
                {
                    float offsetX = frameX * ChunkWidth + x;
                    float offsetY = frameY * ChunkHeight + y;
                    heightValues[x, y] = (float)_heightNoise.GetValue(offsetX, offsetY, 0);

                    if (heightValues[x, y] > MaxHeightNoise) MaxHeightNoise = heightValues[x, y];
                    if (heightValues[x, y] < MinHeightNoise) MinHeightNoise = heightValues[x, y];
                }
            }
            return heightValues;
        }

        private async Task<float[,]> GetHeightMapNoiseAsync(int frameX, int frameY)
        {
            float[,] heightValues = new float[ChunkWidth, ChunkHeight];
            await Task.Run(() =>
            {
                Parallel.For(0, ChunkWidth, x =>
                {
                    for (int y = 0; y < ChunkHeight; y++)
                    {
                        float offsetX = frameX * ChunkWidth + x;
                        float offsetY = frameY * ChunkHeight + y;
                        heightValues[x, y] = (float)_heightNoise.GetValue(offsetX, offsetY, 0);

                        lock (lockObject)
                        {
                            if (heightValues[x, y] > MaxHeightNoise) MaxHeightNoise = heightValues[x, y];
                            if (heightValues[x, y] < MinHeightNoise) MinHeightNoise = heightValues[x, y];
                        }
                    }
                });
            });

            return heightValues;
        }




        private Chunk AddNewChunk(int isoFrameX, int isoFrameY)
        {
            Vector2 frame = IsometricUtilities.IsometricFrameToWorldFrame(isoFrameX, isoFrameY);
            Vector3 worldPosition = IsometricUtilities.ConvertIsometricFrameToWorldPosition(isoFrameX, isoFrameY, ChunkWidth, ChunkHeight);
            Chunk newChunk = Instantiate(_chunkPrefab, worldPosition, Quaternion.identity);
            newChunk.Init(frame.x, frame.y, isoFrameX, isoFrameY, ChunkWidth, ChunkHeight, TileSize);

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
            newChunk.Init(frame.x, frame.y, isoFrameX, isoFrameY, ChunkWidth, ChunkHeight, TileSize);

            // Create new data
            float[,] heightValues = await GetHeightMapNoiseAsync(isoFrameX, isoFrameY);
            await newChunk.LoadHeightMapAsync(heightValues);          
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
            chunk.SetChunkNeighbors(nbLeft, nbRight, nbAbove, nbBelow);
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
#endif
    }
}

