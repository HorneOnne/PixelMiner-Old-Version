using System.Collections.Generic;
using UnityEngine;
using LibNoise;
using LibNoise.Generator;


namespace CoreMiner
{
    public static class IsometricUtilities
    {
        public static int CELLSIZE_X = 1;
        public static int CELLSIZE_Y = 1;
        public enum ChunkNeighborDirection
        {
            UL,
            U,
            UR,
            L,
            R,
            DL,
            D,
            DR
        }
        public static Vector2Int CalculateChunkNeighborPosition(Vector2Int chunkPosition, ChunkNeighborDirection direction)
        {
            switch (direction)
            {
                case ChunkNeighborDirection.UL:
                    return new Vector2Int(chunkPosition.x - 1, chunkPosition.y - 1);
                case ChunkNeighborDirection.U:
                    return new Vector2Int(chunkPosition.x, chunkPosition.y - 1);
                case ChunkNeighborDirection.UR:
                    return new Vector2Int(chunkPosition.x + 1, chunkPosition.y - 1);
                case ChunkNeighborDirection.L:
                    return new Vector2Int(chunkPosition.x - 1, chunkPosition.y);
                case ChunkNeighborDirection.R:
                    return new Vector2Int(chunkPosition.x + 1, chunkPosition.y);
                case ChunkNeighborDirection.DL:
                    return new Vector2Int(chunkPosition.x - 1, chunkPosition.y + 1);
                case ChunkNeighborDirection.D:
                    return new Vector2Int(chunkPosition.x, chunkPosition.y + 1);
                case ChunkNeighborDirection.DR:
                    return new Vector2Int(chunkPosition.x + 1, chunkPosition.y + 1);
                default:
                    return chunkPosition;
            }
        }

        public static Vector3 ConvertIsometricFrameToWorld(Vector2 chunkPosition, int chunkWidth, int chunkHeight)
        {
            float tileWidth = CELLSIZE_X;  // Width of an isometric tile
            float tileHeight = CELLSIZE_Y; // Height of an isometric tile

            float chunkX = chunkPosition.x;
            float chunkY = chunkPosition.y;

            float worldX = (chunkX - chunkY) * tileWidth * 0.5f * chunkWidth;
            float worldY = (chunkX + chunkY) * tileHeight * 0.5f * chunkHeight;

            return new Vector3(worldX, worldY, 0);
        }
        public static Vector2Int ReverseConvertWorldToIsometricFrame(Vector3 worldPosition, int chunkWidth, int chunkHeight)
        {
            float tileWidth = CELLSIZE_X;  // Width of an isometric tile
            float tileHeight = CELLSIZE_Y; // Height of an isometric tile

            float worldX = worldPosition.x;
            float worldY = worldPosition.y;

            int chunkX = Mathf.FloorToInt((worldX / (tileWidth * 0.5f * chunkWidth) + worldY / (tileHeight * 0.5f * chunkHeight)) / 2);
            int chunkY = Mathf.FloorToInt((worldY / (tileHeight * 0.5f * chunkHeight) - worldX / (tileWidth * 0.5f * chunkWidth)) / 2);

            return new Vector2Int(chunkX, chunkY);
        }

        public static Vector2 ConvertWorldToIsometricFrame(Vector3 worldPosition, int chunkWidth, int chunkHeight)
        {
            float tileWidth = CELLSIZE_X;   // Width of an isometric tile (replace with your actual tile size)
            float tileHeight = CELLSIZE_Y;  // Height of an isometric tile (replace with your actual tile size)

            float worldX = worldPosition.x;
            float worldY = worldPosition.y;

            float chunkX = worldX / (tileWidth * chunkWidth) + worldY / (tileHeight * chunkHeight) * 0.5f;
            float chunkY = worldY / (tileHeight * chunkHeight) - worldX / (tileWidth * chunkWidth) * 0.5f;

            return new Vector2(chunkX, chunkY);
        }

        public static Vector2Int FrameToISOFrame(int frameX, int frameY, int chunkWidth, int chunkHeight) 
        {
            float cellSizeX = CELLSIZE_X;
            float cellSizeY = CELLSIZE_Y;
            return new Vector2Int(Mathf.FloorToInt(frameX * (cellSizeX / 2)),
                                  Mathf.FloorToInt(frameY * (cellSizeY / 2)));
        }

        public static Vector2Int WorldToFrame(Vector2 worldPosition, int chunkWidth, int chunkHeight)
        {
            int frameX = Mathf.FloorToInt(worldPosition.x / chunkWidth);
            int frameY = Mathf.FloorToInt(worldPosition.y / chunkHeight);
            return new Vector2Int(frameX, frameY);
        }

        public static Vector2 IsometricFrameToFrame(Vector2Int IsometricFrame, int chunkWidth, int chunkHeight)
        {
            Vector3 isoWorldPosition = IsometricUtilities.ConvertIsometricFrameToWorld(new Vector2(IsometricFrame.x, IsometricFrame.y), chunkWidth, chunkHeight);
            Debug.Log($"isoFrame: {IsometricFrame} \t iso World {isoWorldPosition}"); ;
            return new Vector2(isoWorldPosition.x / chunkWidth,
                               isoWorldPosition.y / chunkHeight);
        }

        public static Vector2Int FrameToIsometricFrame(Vector2 resultFrame, int chunkWidth, int chunkHeight)
        {
            Debug.Log($"From: {resultFrame}");
            // Reverse the calculations
            Vector3 isoWorldPosition = new Vector3(resultFrame.x * chunkWidth, resultFrame.y * chunkHeight, 0f);

            // Adjust for the offset
            isoWorldPosition.x += chunkWidth / 2;
            isoWorldPosition.y += chunkHeight / 2;

            // Calculate the IsometricFrame
            int isometricX = Mathf.FloorToInt(isoWorldPosition.x / chunkWidth);
            int isometricY = Mathf.FloorToInt(isoWorldPosition.y / chunkHeight);

            return new Vector2Int(isometricX, isometricY);
        }


        // Convert Cartesian to Isometric
        public static Vector2 CartesianToIsometric(Vector2 cartesian)
        {
            float isoX = cartesian.x + cartesian.y / 2.0f;
            float isoY = cartesian.y / 2.0f - cartesian.x;
            return new Vector2(isoX, isoY);
        }

        // Convert Isometric to Cartesian
        public static Vector2 IsometricToCartesian(Vector2 isometric)
        {
            float carX = isometric.x - isometric.y / 2.0f;
            float carY = isometric.y * 2.0f + isometric.x;
            return new Vector2(carX, carY);
        }

        public static Vector2 FrameToIsometric(int frameX, int frameY, int chunkSize)
        {
            float isox = frameX * (chunkSize / 2) - frameY * (chunkSize / 2);
            float isoy = frameY * (chunkSize / 2) + frameX * (chunkSize / 2);
            return new Vector2(isox, isoy);
        }

        public static Vector2 NormalToIsometric(Vector2 normalCoords)
        {
            float isometricX = normalCoords.x - normalCoords.y;
            float isometricY = (normalCoords.x + normalCoords.y) * 0.5f;
            return new Vector2(isometricX, isometricY);
        }
    }
    public class  WorldGeneration : MonoBehaviour
    {
        [SerializeField] private Chunk _chunkPrefab;

        [SerializeField] private Vector2 _centerPoint;
        [SerializeField] private Vector2Int _centerPointFrame;

        public int ChunkWidth = 16;      // Size of each chunk in tiles
        public int ChunkHeight = 16;      // Size of each chunk in tiles
        public int worldSize = 5;       // Number of chunks in the world

        private Dictionary<Vector2Int, Chunk> chunks;
        [SerializeField ] private Vector2Int lastChunkFrame;

        private float TileSize = 1.0f;

        [Header("Noise Settings")]
        public int Octaves = 6;
        public double Frequency = 0.02f;
        public double Lacunarity = 2.0f;
        public double Persistence = 0.5f;
        public int Seed = 7;
        private ModuleBase _heightNoise;
      

        public readonly Vector3 CELL_SIZE = new Vector3(1.0f, 1.0f, 1.0f);
        public readonly Vector3 CELL_GAP = new Vector3(0.0f, 0.0f, 0.0f);


        public int Size = 32;

        [Header("Test")]
        public Vector2 Offset;

        private void Start()
        {
            // Initialize the chunks dictionary
            chunks = new Dictionary<Vector2Int, Chunk>();
            _centerPoint = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2f, Screen.height / 2f));
            _heightNoise = new Perlin(Frequency, Lacunarity, Persistence, Octaves, Seed, QualityMode.High);

            // Load chunks around the player's starting position
            lastChunkFrame = IsometricUtilities.ReverseConvertWorldToIsometricFrame(_centerPoint, ChunkWidth, ChunkHeight);
            var isoFrame = IsometricUtilities.IsometricFrameToFrame(lastChunkFrame, ChunkWidth, ChunkHeight);
            var frame = IsometricUtilities.FrameToIsometricFrame(isoFrame, ChunkWidth, ChunkHeight);
            Debug.Log($"center: {_centerPoint} \tchunkFrame :{lastChunkFrame}\t iso: {lastChunkFrame}\t{isoFrame}\t{frame}");
            LoadChunksAroundPosition(lastChunkFrame.x, lastChunkFrame.y);

            Debug.Log(IsometricUtilities.ReverseConvertWorldToIsometricFrame(_centerPoint, ChunkWidth, ChunkHeight));
        }

        private float _updateFrequency = 0.2f;
        private float _updateTimer = 0.0f;
        private void Update()
        {
 
            //if (Time.time - _updateTimer > _updateFrequency)
            //{
            //    _updateTimer = Time.time;
            //    foreach (var chunk in chunks.Values)
            //    {
            //        chunk.TileMap.ClearAllTiles();
            //        chunk.Init(chunk.FrameX, chunk.FrameY, ChunkWidth, ChunkHeight, TileSize);
            //        float[,] heightValues = GetHeightMapNoisePlanner(chunk.FrameX, chunk.FrameY);
            //        chunk.LoadHeightMap(heightValues);
            //        chunk.LoadMap();
            //    }
            //}
    
            if (Input.GetKeyDown(KeyCode.G))
            {
                foreach(var chunk in chunks.Values)
                {
                    chunk.TileMap.ClearAllTiles();
                    chunk.Init(chunk.FrameX, chunk.FrameY, ChunkWidth, ChunkHeight, TileSize);
                    float[,] heightValues = GetHeightMapNoise(chunk.FrameX, chunk.FrameY);
                    chunk.LoadHeightMap(heightValues);
                    chunk.LoadMap();
                }
            }

            _centerPoint = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2f, Screen.height / 2f));
            _centerPointFrame = IsometricUtilities.ReverseConvertWorldToIsometricFrame(_centerPoint, ChunkWidth, ChunkHeight);
            // Check if the player has moved to a new chunk
            //Vector2Int currentChunkPosition = IsometricUtilities.WorldToFrame(_centerPointFrame, ChunkWidth, ChunkHeight);

            if (_centerPointFrame != lastChunkFrame)
            {
                lastChunkFrame = _centerPointFrame;
                LoadChunksAroundPosition(_centerPointFrame.x, _centerPointFrame.y);
            }
        }


        // Load chunks around a given chunk position
        private void LoadChunksAroundPosition(int frameX, int frameY)
        {
            if (chunks.ContainsKey(new Vector2Int(frameX, frameY)) == false)
            {
                // Instantiate a new chunk at this position
                Vector3 neighborWorldPosition = IsometricUtilities.ConvertIsometricFrameToWorld(new Vector2(frameX, frameY), ChunkWidth, ChunkHeight);
                //Chunk newChunk = Instantiate(_chunkPrefab, new Vector3(frameX * ChunkWidth, frameY * ChunkHeight), Quaternion.identity);
                Chunk newChunk = Instantiate(_chunkPrefab, neighborWorldPosition, Quaternion.identity);
                newChunk.Init(frameX, frameY, ChunkWidth, ChunkHeight, TileSize);

                float[,] heightValues = GetHeightMapNoise(frameX, frameY);
                newChunk.LoadHeightMap(heightValues);
                newChunk.LoadMap();
                chunks.Add(new Vector2Int(frameX, frameY), newChunk);
            }

            return;
            //foreach (IsometricUtilities.ChunkNeighborDirection direction in (IsometricUtilities.ChunkNeighborDirection[])System.Enum.GetValues(typeof(IsometricUtilities.ChunkNeighborDirection)))
            //{
            //    Vector2Int neighborPosition = IsometricUtilities.CalculateChunkNeighborPosition(position, direction);
            //    Vector3 neighborWorldPosition = IsometricUtilities.ConvertIsometricFrameToWorld(neighborPosition, ChunkWidth, ChunkHeight);

            //    frameX = neighborPosition.x;
            //    frameY = neighborPosition.y;

            //    if (chunks.ContainsKey(new Vector2Int(frameX, frameY)) == false)
            //    {
            //        // Instantiate a new chunk at this position
            //        //Vector3 chunkPosition = new Vector3(frameX * ChunkWidth * 1, frameY * ChunkHeight * 1, 0);
            //        Chunk newChunk = Instantiate(_chunkPrefab, neighborWorldPosition, Quaternion.identity);
            //        newChunk.Init(frameX, frameY, ChunkWidth, ChunkHeight, TileSize);

            //        float[,] heightValues = GetHeightMapNoise(frameX, frameY);
            //        newChunk.LoadHeightMap(heightValues);
            //        newChunk.LoadMap();
            //        chunks.Add(new Vector2Int(frameX, frameY), newChunk);
            //    }
            //}
        }

        private float[,] GetHeightMapNoise(int frameX, int frameY)
        {
            float[,] heightValues = new float[ChunkWidth, ChunkHeight];
            for(int x = 0; x < ChunkWidth; x++)
            {
                for(int y = 0; y < ChunkHeight; y++)
                {
                    float offsetX = frameX * ChunkWidth + x;
                    float offsetY = frameY * ChunkHeight + y;
                    heightValues[x, y] = (float)_heightNoise.GetValue(offsetX, offsetY, 0);
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
    }
}

