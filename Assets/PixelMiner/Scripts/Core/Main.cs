using System.Collections.Generic;
using UnityEngine;
using PixelMiner.World;
using PixelMiner.Enums;


namespace PixelMiner.Core
{
    public class Main : MonoBehaviour
    {
        public static Main Instance { get; private set; }
        
        // Chunk data
        [Header("Data Cached")]
        public Dictionary<Vector3Int, Chunk> Chunks;
        public HashSet<Chunk> ActiveChunks;
        public string SeedInput = "7";


        public Vector3Int ChunkDimension;


        public bool AutoLoadChunk = true;
        public bool AutoUnloadChunk = true;


        private void Awake()
        {
            Instance = this;

            // Initialize the chunks data
            Chunks = new Dictionary<Vector3Int, Chunk>();
            ActiveChunks = new HashSet<Chunk>();
        }
        private void Start()
        {
          
        }

       
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.T))
            {          
                UnityEngine.Profiling.Profiler.BeginSample("test01");
                Chunk c = GetChunk(new Vector3(384, 0, 576));
                int loopCount = 1000000;
                for (int i = 0; i < loopCount; i++)
                {
                    //TryGetChunk(Vector3.zero, out Chunk chunk);
                    GetChunkPerformance(c, new Vector3(384, 0, 576));

                }
                UnityEngine.Profiling.Profiler.EndSample();
            }
           
        }


        #region Get, Set Chunk
        public bool HasChunk(Vector3Int relativePosition)
        {
            return Chunks.ContainsKey(relativePosition);
        }
        public Chunk GetChunk(Vector3 worldPosition)
        {
            Vector3Int frame = new Vector3Int(Mathf.FloorToInt(worldPosition.x / ChunkDimension[0]),
                Mathf.FloorToInt(worldPosition.y / ChunkDimension[1]), Mathf.FloorToInt(worldPosition.z / ChunkDimension[2]));
            return GetChunk(frame);
        }

        public bool TryGetChunk(Vector3 globalPosition, out Chunk chunk)
        {
            Vector3Int relativePosition = new Vector3Int(Mathf.FloorToInt(globalPosition.x / ChunkDimension[0]),
                                                         Mathf.FloorToInt(globalPosition.y / ChunkDimension[1]),
                                                         Mathf.FloorToInt(globalPosition.z / ChunkDimension[2]));

            if (Chunks.ContainsKey(relativePosition))
            {
                chunk = Chunks[relativePosition];
                return true;
            }
            chunk = null;
            return false;
        }

        public Chunk GetChunk(Vector3Int relativePosition)
        {
            if (Chunks.ContainsKey(relativePosition))
            {
                return Chunks[relativePosition];
            }
            return null;
        }
        #endregion


        #region Block
        public BlockType GetBlock(Vector3 globalPosition)
        {
            Vector3Int relativePosition = GlobalToRelativeBlockPosition(globalPosition, ChunkDimension[0], ChunkDimension[1], ChunkDimension[2]);

            if (TryGetChunk(globalPosition, out Chunk chunk))
            {
                if (chunk.HasDrawnFirstTime)
                {
                    return chunk.GetBlock(relativePosition);
                }
                else
                {
                    return BlockType.Air;
                }

            }
            return BlockType.Air;
        }
        public BlockType GetBlockPerformance(Chunk chunk, Vector3 globalPosition)
        {
            Vector3Int relativePosition = GlobalToRelativeBlockPosition(globalPosition, ChunkDimension[0], ChunkDimension[1], ChunkDimension[2]);
            Chunk chunkFound = GetChunkPerformance(chunk, globalPosition);
            if(chunkFound != null && chunkFound.HasDrawnFirstTime)
            {
                return chunkFound.GetBlock(relativePosition);
            }
            return BlockType.Air;
        }

        public BlockType TryGetBlock(ref Chunk chunk, Vector3 globalPosition)
        {
            Vector3Int chunkRelativePosition = GlobalToRelativeChunkPosition(globalPosition, ChunkDimension[0], ChunkDimension[1], ChunkDimension[2]);
            if (Chunks.ContainsKey(chunkRelativePosition) == false)
            {
                return BlockType.Air;
            }
            else
            {
                if (chunk?.FrameX == chunkRelativePosition.x &&
                   chunk?.FrameY == chunkRelativePosition.y &&
                   chunk?.FrameZ == chunkRelativePosition.z)
                {
                    Vector3Int relativePosition = GlobalToRelativeBlockPosition(globalPosition, ChunkDimension[0], ChunkDimension[1], ChunkDimension[2]);
                    return chunk.GetBlock(relativePosition);
                }
                else
                {
                    TryGetChunk(globalPosition, out chunk);
                    return GetBlock(globalPosition);
                }
            }

        }

        public void SetBlock(Vector3 globalPosition, BlockType blockType)
        {
            Vector3Int relativePosition = GlobalToRelativeBlockPosition(globalPosition, ChunkDimension[0], ChunkDimension[1], ChunkDimension[2]);
            if (TryGetChunk(globalPosition, out Chunk chunk))
            {
                chunk.SetBlock(relativePosition, blockType);
            }
        }
        #endregion


        #region Light
        public byte GetBlockLight(Vector3 globalPosition)
        {
            Vector3Int relativePosition = GlobalToRelativeBlockPosition(globalPosition, ChunkDimension[0], ChunkDimension[1], ChunkDimension[2]);
            if (TryGetChunk(globalPosition, out Chunk chunk))
            {
                return chunk.GetBlockLight(relativePosition);
            }
            return byte.MinValue;
        }
        public void SetBlockLight(Vector3 globalPosition, byte intensity)
        {
            Vector3Int relativePosition = GlobalToRelativeBlockPosition(globalPosition, ChunkDimension[0], ChunkDimension[1], ChunkDimension[2]);
            if (TryGetChunk(globalPosition, out Chunk chunk))
            {
                chunk.SetBlockLight(relativePosition, intensity);
            }
        }
        public void SetBlockLightPerformance(Chunk chunk, Vector3 globalPosition, byte intensity)
        {
            Chunk targetChunk = GetChunkPerformance(chunk, globalPosition);
            Vector3Int relativePosition = GlobalToRelativeBlockPosition(globalPosition, ChunkDimension[0], ChunkDimension[1], ChunkDimension[2]);
            targetChunk.SetBlockLight(relativePosition, intensity);
        }
        public byte GetBlockLightPerformance(Chunk chunk, Vector3 globalPosition)
        {
            Chunk targetChunk = GetChunkPerformance(chunk, globalPosition);
            Vector3Int relativePosition = GlobalToRelativeBlockPosition(globalPosition, ChunkDimension[0], ChunkDimension[1], ChunkDimension[2]);
            return targetChunk.GetBlockLight(relativePosition);
        }

        public float GetAmbientLightIntensity()
        {
            return DayNightCycle.Instance.AmbientlightIntensity;
            //return LightUtils.CalculateSunlightIntensity(hour + _worldTime.Minutes / 60f, SunLightIntensityCurve));
        }

        public byte GetAmbientLight(Vector3 globalPosition)
        {
            Vector3Int relativePosition = GlobalToRelativeBlockPosition(globalPosition, ChunkDimension[0], ChunkDimension[1], ChunkDimension[2]);
            if (TryGetChunk(globalPosition, out Chunk chunk))
            {
                return chunk.GetAmbientLight(relativePosition);
            }
            return byte.MinValue;
        }
        public byte GetAmbientLightPerformance(Chunk chunk, Vector3 globalPosition)
        {
            Vector3Int relativePosition = GlobalToRelativeBlockPosition(globalPosition, ChunkDimension[0], ChunkDimension[1], ChunkDimension[2]);
            return GetChunkPerformance(chunk, globalPosition).GetAmbientLight(relativePosition);
        }
        public void SetAmbientLight(Vector3 globalPosition, byte insensity)
        {
            Vector3Int relativePosition = GlobalToRelativeBlockPosition(globalPosition, ChunkDimension[0], ChunkDimension[1], ChunkDimension[2]);
            if (TryGetChunk(globalPosition, out Chunk chunk))
            {
                chunk.SetAmbientLight(relativePosition, insensity);
            }
        }
        #endregion

        public static Vector3Int GlobalToRelativeBlockPosition(Vector3 globalPosition,
          int chunkWidth, int chunkHeight, int chunkDepth)
        {
            // Calculate the relative position within the chunk
            int relativeX = Mathf.FloorToInt(globalPosition.x) % chunkWidth;
            int relativeY = Mathf.FloorToInt(globalPosition.y) % chunkHeight;
            int relativeZ = Mathf.FloorToInt(globalPosition.z) % chunkDepth;

            // Ensure that the result is within the chunk's dimensions
            if (relativeX < 0) relativeX += chunkWidth;
            if (relativeY < 0) relativeY += chunkHeight;
            if (relativeZ < 0) relativeZ += chunkDepth;

            return new Vector3Int(relativeX, relativeY, relativeZ);
        }

        public static Vector3Int GlobalToRelativeChunkPosition(Vector3 globalPosition,
            int chunkWidth, int chunkHeight, int chunkDepth)
        {
            int relativeX = Mathf.FloorToInt(globalPosition.x / chunkWidth);
            int relativeY = Mathf.FloorToInt(globalPosition.y / chunkHeight);
            int relativeZ = Mathf.FloorToInt(globalPosition.z / chunkDepth);

            return new Vector3Int(relativeX, relativeY, relativeZ);
        }

        public bool InSideChunkBound(Chunk chunk, Vector3 globalPosition)
        {
            //if(chunk == null) return false;
            return (globalPosition.x >= chunk.MinXGPos && globalPosition.x < chunk.MaxXGPos &&
                    globalPosition.y >= chunk.MinYGPos && globalPosition.y < chunk.MaxYGPos &&
                    globalPosition.z >= chunk.MinZGPos && globalPosition.z < chunk.MaxZGPos);
        }


        public Chunk GetChunkPerformance(Chunk chunk, Vector3 globalPosition)
        {
            int xOffset = (globalPosition.x < chunk.MinXGPos) ? -1 :
                (globalPosition.x >= chunk.MaxXGPos) ? 1 : 0;

            int yOffset = (globalPosition.y < chunk.MinYGPos) ? -1 :
               (globalPosition.y >= chunk.MaxYGPos) ? 1 : 0;

            int zOffset = (globalPosition.z < chunk.MinZGPos) ? -1 :
                           (globalPosition.z >= chunk.MaxZGPos) ? 1 : 0;

            Vector3Int offset = new Vector3Int(xOffset, yOffset, zOffset);
            if(offset == Vector3Int.zero)
            {
                return chunk;
            }

            return chunk.FindNeighbor(offset);
        }

        public Vector3Int GetBlockGPos(Vector3 globalPosition)
        {
            return new Vector3Int(Mathf.FloorToInt(globalPosition.x),
                                  Mathf.FloorToInt(globalPosition.y),
                                  Mathf.FloorToInt(globalPosition.z));
        }
    }
}

