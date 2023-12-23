using System.Collections.Generic;
using UnityEngine;
using PixelMiner.WorldBuilding;
using PixelMiner;

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
        public bool HasChunk(int franeX, int frameZ)
        {
            return Chunks.ContainsKey(new Vector3Int(franeX,0, frameZ));
        }
        public bool HasChunk(Vector3Int frame)
        {
            return Chunks.ContainsKey(frame);
        }
        public Chunk GetChunk(Vector3 worldPosition)
        {
            Vector3Int frame = new Vector3Int(Mathf.FloorToInt(worldPosition.x / ChunkDimension[0]),
                Mathf.FloorToInt(worldPosition.y / ChunkDimension[1]), Mathf.FloorToInt(worldPosition.z / ChunkDimension[2]));
            return GetChunk(frame);
        }
        public bool TryGetChunk(Vector3 worldPosition, out Chunk chunk)
        {
            chunk = null;
            Vector3Int frame = new Vector3Int(Mathf.FloorToInt(worldPosition.x / ChunkDimension[0]),
                Mathf.FloorToInt(worldPosition.y / ChunkDimension[1]), Mathf.FloorToInt(worldPosition.z / ChunkDimension[2]));

            if(HasChunk(frame))
            {
                chunk = GetChunk(frame);
                return true;
            }
            return false;
        }
        public Chunk GetChunk(int frameX, int frameZ)
        {
            Vector3Int frame = new Vector3Int(frameX, 0, frameZ);
            if (Chunks.ContainsKey(frame))
            {
                return Chunks[frame];
            }
            return null;
        }
        public Chunk GetChunk(Vector3Int frame)
        {
            if (Chunks.ContainsKey(frame))
            {
                return Chunks[frame];
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



        #region Light
        public byte GetBlockLight(Vector3 globalPosition)
        {
            Vector3Int chunkFrame = new Vector3Int(Mathf.FloorToInt(globalPosition.x / ChunkDimension[0]),
               Mathf.FloorToInt(globalPosition.y / ChunkDimension[1]), Mathf.FloorToInt(globalPosition.z / ChunkDimension[2]));
            Vector3Int relativePosition = new Vector3Int(Mathf.FloorToInt(globalPosition.x % ChunkDimension[0]),
                                                         Mathf.FloorToInt(globalPosition.y % ChunkDimension[1]),
                                                         Mathf.FloorToInt(globalPosition.z % ChunkDimension[2]));

            if (TryGetChunk(globalPosition, out Chunk chunk))
            {
                return chunk.GetBlockLight(relativePosition);
            }
            return byte.MinValue;
        }
        #endregion
    }
}

