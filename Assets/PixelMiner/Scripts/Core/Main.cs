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
        //public byte ChunkWidth = 32;
        //public byte ChunkHeight = 1;
        //public byte ChunkDepth = 32;

        public bool AutoLoadChunk = true;
        public bool AutoUnloadChunk = true;
        public bool ShowChunksBorder = false;
        public bool ShowTilegroupMaps = false;
        public bool InitWorldWithHeatmap = false;
        public bool InitWorldWithMoisturemap = false;
        public bool InitWorldWithRiver = false;
        public bool PaintTileNeighbors = false;


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
            chunk = null;
            if (Chunks.ContainsKey(relativePosition))
            {
                chunk = Chunks[relativePosition];
                return true;
            }
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

        #region Neighbors
        public Chunk GetChunkNeighborFront(Chunk chunk)
        {
            Vector3Int nbChunkFrame = new Vector3Int(chunk.FrameX, chunk.FrameY, chunk.FrameZ + 1);
            return Chunks.TryGetValue(nbChunkFrame, out Chunk neighborChunk) ? neighborChunk : null;
        }
        public Chunk GetChunkNeighborBack(Chunk chunk)
        {
            Vector3Int nbChunkFrame = new Vector3Int(chunk.FrameX, chunk.FrameY, chunk.FrameZ - 1);
            return Chunks.TryGetValue(nbChunkFrame, out Chunk neighborChunk) ? neighborChunk : null;
        }
        public Chunk GetChunkNeighborLeft(Chunk chunk)
        {
            Vector3Int nbChunkFrame = new Vector3Int(chunk.FrameX - 1, chunk.FrameY, chunk.FrameZ);
            return Chunks.TryGetValue(nbChunkFrame, out Chunk neighborChunk) ? neighborChunk : null;
        }
        public Chunk GetChunkNeighborRight(Chunk chunk)
        {
            Vector3Int nbChunkFrame = new Vector3Int(chunk.FrameX + 1, chunk.FrameY, chunk.FrameZ);
            return Chunks.TryGetValue(nbChunkFrame, out Chunk neighborChunk) ? neighborChunk : null;
        }
        public Chunk GetChunkNeighborTop(Chunk chunk)
        {
            Vector3Int nbChunkFrame = new Vector3Int(chunk.FrameX, chunk.FrameY + 1, chunk.FrameZ);
            return Chunks.TryGetValue(nbChunkFrame, out Chunk neighborChunk) ? neighborChunk : null;
        }
        public Chunk GetChunkNeighborBottom(Chunk chunk)
        {
            Vector3Int nbChunkFrame = new Vector3Int(chunk.FrameX, chunk.FrameY - 1, chunk.FrameZ);
            return Chunks.TryGetValue(nbChunkFrame, out Chunk neighborChunk) ? neighborChunk : null;
        }
        #endregion


        public bool ShowLog = false;
        #region Block
        public BlockType GetBlock(Vector3 globalPosition)
        {
            Vector3Int relativePosition = GlobalToRelativeBlockPosition(globalPosition);
       
            if (TryGetChunk(globalPosition, out Chunk chunk))
            {
                return chunk.GetBlock(relativePosition);              
            }
            return BlockType.Air;
        }
        public BlockType TryGetBlock(ref Chunk chunk, Vector3 globalPosition)
        {
            Vector3Int chunkRelativePosition = GlobalToRelativeChunkPosition(globalPosition);
            if(Chunks.ContainsKey(chunkRelativePosition) == false)
            {
                return BlockType.Air;
            }
            else
            {
                if (chunk?.FrameX == chunkRelativePosition.x &&
                   chunk?.FrameY == chunkRelativePosition.y &&
                   chunk?.FrameZ == chunkRelativePosition.z)
                {
                    Vector3Int relativePosition = GlobalToRelativeBlockPosition(globalPosition);
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
            Vector3Int relativePosition = GlobalToRelativeBlockPosition(globalPosition);
            if (TryGetChunk(globalPosition, out Chunk chunk))
            {
                chunk.SetBlock(relativePosition, blockType);
            }
        }
        #endregion


        #region Light
        public byte GetBlockLight(Vector3 globalPosition)
        {
            Vector3Int relativePosition = GlobalToRelativeBlockPosition(globalPosition);
            if (TryGetChunk(globalPosition, out Chunk chunk))
            {
                return chunk.GetBlockLight(relativePosition);
            }
            return byte.MinValue;
        }
        public void SetBlockLight(Vector3 globalPosition, byte intensity)
        {
            Vector3Int relativePosition = GlobalToRelativeBlockPosition(globalPosition);
            if (TryGetChunk(globalPosition, out Chunk chunk))
            {
                chunk.SetBlockLight(relativePosition, intensity);
            }
        }

        public float GetAmbientLightIntensity()
        {
            return DayNightCycle.Instance.AmbientlightIntensity;
            //return LightUtils.CalculateSunlightIntensity(hour + _worldTime.Minutes / 60f, SunLightIntensityCurve));
            return 0;
        }

        public byte GetAmbientLight(Vector3 globalPosition)
        {
            Vector3Int chunkFrame = new Vector3Int(Mathf.FloorToInt(globalPosition.x / ChunkDimension[0]),
               Mathf.FloorToInt(globalPosition.y / ChunkDimension[1]), Mathf.FloorToInt(globalPosition.z / ChunkDimension[2]));
            Vector3Int relativePosition = GlobalToRelativeBlockPosition(globalPosition);

            if (TryGetChunk(globalPosition, out Chunk chunk))
            {
                return chunk.GetAmbientLight(relativePosition);
            }
            return byte.MinValue;
        }
        #endregion

        public static Vector3Int GlobalToRelativeBlockPosition(Vector3 globalPosition,
          int chunkWidth = 32, int chunkHeight = 10, int chunkDepth = 32)
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
            int chunkWidth = 32, int chunkHeight = 10, int chunkDepth = 32)
        {
            int relativeX = Mathf.FloorToInt(globalPosition.x / chunkWidth);
            int relativeY = Mathf.FloorToInt(globalPosition.y / chunkHeight);
            int relativeZ = Mathf.FloorToInt(globalPosition.z / chunkDepth);

            return new Vector3Int(relativeX, relativeY, relativeZ);
        }
    }


}

