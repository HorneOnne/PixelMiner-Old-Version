using System.Collections.Generic;
using UnityEngine;
using LibNoise;
using LibNoise.Generator;
using System.Linq;

namespace CoreMiner
{


    public class WorldGeneration : MonoBehaviour
    {
        public static WorldGeneration Instance { get; private set; }

        [SerializeField] private Chunk _chunkPrefab;

        [SerializeField] private Vector2 _centerPoint;
        [SerializeField] private Vector2Int _centerPointFrame;

        public int ChunkWidth = 16;      // Size of each chunk in tiles
        public int ChunkHeight = 16;      // Size of each chunk in tiles
        public int WorldSize = 1;       // Number of chunks in the world

        private Dictionary<Vector2Int, Chunk> chunks;
        [SerializeField] private Vector2Int lastChunkISOFrame;

        private float TileSize = 1.0f;

        [Header("Noise Settings")]
        public int Octaves = 6;
        public double Frequency = 0.02f;
        public double Lacunarity = 2.0f;
        public double Persistence = 0.5f;
        public int Seed = 7;
        private ModuleBase _heightNoise;


        public readonly Vector3 CELL_SIZE = new Vector3(2.0f, 1.0f, 1.0f);
        public readonly Vector3 CELL_GAP = new Vector3(0.0f, 0.0f, 0.0f);

        // Min and Max Height used for normalize noise value in range [0-1]
        private float _minHeight = float.MaxValue;
        private float _maxHeight = float.MinValue;
        public int Size = 32;
        public HashSet<Chunk> ActiveChunks;


        private void Awake()
        {
            Instance = this;
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
        private void LoadChunksAroundPosition(int isoFrameX, int isoFrameY, int offset = 1)
        {
            Debug.Log("LoadChunksAroundPositionl");

            for (int x = isoFrameX - offset; x <= isoFrameX + offset; x++)
            {
                for (int y = isoFrameY - offset; y <= isoFrameY + offset; y++)
                {
                    Vector2Int nbIsoFrame = new Vector2Int(x, y);
                    if (chunks.ContainsKey(new Vector2Int(x, y)) == false)
                    {
                        if (x == isoFrameX && y == isoFrameY)
                        {
                            // Center
                            // ......
                        }


                        Chunk newChunk = AddNewChunk(x, y);
                        ActiveChunks.Add(newChunk);
                    }
                    else
                    {
                        chunks[nbIsoFrame].gameObject.SetActive(true);
                        ActiveChunks.Add(chunks[nbIsoFrame]);
                    }

                }
            }

            int depth = 0;
            List<Chunk> chunkList = ActiveChunks.ToList();
            chunkList.Sort((v1, v2) =>
            {
                int xComparison = v1.IsometricFrameX.CompareTo(v2.IsometricFrameX);
                return xComparison != 0 ? xComparison : v1.IsometricFrameY.CompareTo(v2.IsometricFrameY);
            });

            foreach (var chunk in chunkList)
            {
                chunk.transform.position = new Vector3(chunk.transform.position.x, 
                                                       chunk.transform.position.y, 
                                                       depth++);
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


        private Vector3 FrameToWorldIsometricPosition(int frameX, int frameY)
        {
            return new Vector3(frameX * ChunkWidth / 2 * CELL_SIZE.x,
                               frameY * ChunkHeight / 2 * CELL_SIZE.y,
                               0 * CELL_SIZE.z);
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
            newChunk.LoadMap();

            // Store chunk into dictionary
            chunks.Add(new Vector2Int(isoFrameX, isoFrameY), newChunk);

            return newChunk;
        }
    }
}

