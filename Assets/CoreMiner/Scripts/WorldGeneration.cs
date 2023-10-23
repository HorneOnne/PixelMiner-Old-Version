using System.Collections.Generic;
using UnityEngine;
using LibNoise;
using LibNoise.Generator;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using CoreMiner.Utilities;

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
        public int WorldSize = 1;       // Number of chunks in the world
        private float TileSize = 1.0f;


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

        [Header("World Generation Properties")]
        public bool AutoUnloadChunk = true;
        public bool ShowChunksBorder = false;

        public readonly Vector3 CELL_SIZE = new Vector3(2.0f, 1.1547f, 1.0f);
        public readonly Vector3 CELL_GAP = new Vector3(0.0f, 0.0f, 0.0f);

        // Min and Max Height used for normalize noise value in range [0-1]
        private float _minHeight = float.MaxValue;
        private float _maxHeight = float.MinValue;
        private Dictionary<Vector2Int, Chunk> chunks;      
        public HashSet<Chunk> ActiveChunks;


        // Cached
        private Vector2Int lastChunkISOFrame;
        private Vector2 _centerPoint;
        private Vector2Int _centerPointFrame;

        private void Awake()
        {
            Instance = this;

            IsometricUtilities.CELLSIZE_X = CELL_SIZE.x;
            IsometricUtilities.CELLSIZE_Y = CELL_SIZE.y;
        }


        private void Start()
        {
            // Initialize the chunks dictionary
            chunks = new Dictionary<Vector2Int, Chunk>();
            ActiveChunks = new HashSet<Chunk>();

            _centerPoint = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2f, Screen.height / 2f));
            _heightNoise = new Perlin(Frequency, Lacunarity, Persistence, Octaves, Seed, QualityMode.High);

            // Load chunks around the player's starting position
            lastChunkISOFrame = IsometricUtilities.ReverseConvertWorldPositionToIsometricFrame(_centerPoint, ChunkWidth, ChunkHeight);
            LoadChunksAroundPosition(lastChunkISOFrame.x, lastChunkISOFrame.y, offset: WorldSize);
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



        // Load chunks around a given chunk position
        private async void LoadChunksAroundPosition(int isoFrameX, int isoFrameY, int offset = 1)
        {
            for (int x = isoFrameX - offset; x <= isoFrameX + offset; x++)
            {
                for (int y = isoFrameY - offset; y <= isoFrameY + offset; y++)
                {
                    Vector2Int nbIsoFrame = new Vector2Int(x, y);
                    if (chunks.ContainsKey(nbIsoFrame) == false)
                    {
                        if (x == isoFrameX && y == isoFrameY)
                        {
                            // Center
                            // ......
                        }

                        Chunk newChunk =  await AddNewChunkAsync(x,y);
                        //Chunk newChunk = AddNewChunk(x, y);
                        if(chunks.ContainsKey(nbIsoFrame) == false)
                            chunks.Add(nbIsoFrame, newChunk);
                        ActiveChunks.Add(newChunk);
                       
                    }
                    else
                    {
                        chunks[nbIsoFrame].gameObject.SetActive(true);
                        ActiveChunks.Add(chunks[nbIsoFrame]);
                    }

                }
            }

            if(ShowChunksBorder == false)
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

                    if (heightValues[x, y] > _maxHeight) _maxHeight = heightValues[x, y];
                    if (heightValues[x, y] < _minHeight) _minHeight = heightValues[x, y];            
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

                        lock(lockObject)
                        {
                            if (heightValues[x, y] > _maxHeight) _maxHeight = heightValues[x, y];
                            if (heightValues[x, y] < _minHeight) _minHeight = heightValues[x, y];
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
            newChunk.LoadHeightMap(heightValues, _minHeight, _maxHeight);
            newChunk.DrawChunkPerformance();
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
            await newChunk.LoadHeightMapAsync(heightValues, _minHeight, _maxHeight);
            newChunk.DrawChunkPerformance();
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


    


#if DEV_CONSOLE
        [ConsoleCommand("unload_chunk", value: "0")]
        private void AutomaticUnloadChunk(int index)
        {
            // 0: Disable automatic unloadchunk
            // 1: Enable automatic unloadchunk
            switch(index)
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

