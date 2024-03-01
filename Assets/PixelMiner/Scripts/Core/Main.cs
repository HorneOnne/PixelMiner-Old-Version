using System.Collections.Generic;
using UnityEngine;
using PixelMiner.Enums;
using System.Threading.Tasks;

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


        // Update chunks data structures
        private Queue<LightNode> _blockLightBfsQueue = new Queue<LightNode>();
        private Queue<LightNode> _ambientLightBfsQueue = new Queue<LightNode>();
        private Queue<LightNode> _blockLightRemovalBfsQueue = new Queue<LightNode>();
        private Queue<LightNode> _ambientLightRemovalBfsQueue = new Queue<LightNode>();
        private HashSet<Chunk> chunksNeedUpdate = new HashSet<Chunk>();
        public const int MAX_LIGHT_INTENSITY = 150;

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
            Debug.Log(ActiveChunks.Count);
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
            //Vector3Int relativePosition = new Vector3Int(Mathf.FloorToInt(globalPosition.x / ChunkDimension[0]),
            //                                             Mathf.FloorToInt(globalPosition.y / ChunkDimension[1]),
            //                                             Mathf.FloorToInt(globalPosition.z / ChunkDimension[2]));
            Vector3Int relativePosition = GlobalToRelativeChunkPosition(globalPosition, ChunkDimension[0], ChunkDimension[1], ChunkDimension[2]);
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
            if (chunkFound != null && chunkFound.HasDrawnFirstTime)
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






        #region World 
        public bool PlaceBlock(Vector3 globalPosition, BlockType blockType)
        {
            Chunk targetChunk = GetChunk(globalPosition);
            if (targetChunk.HasDrawnFirstTime == false)
                return false;

            Vector3Int blockRelativePosition = GlobalToRelativeBlockPosition(globalPosition, targetChunk._width, targetChunk._height, targetChunk._depth);
            BlockType currBlock = targetChunk.GetBlock(blockRelativePosition);
            if (currBlock.IsSolidVoxel()) return false;
            if (currBlock.IsTransparentVoxel()) return false;

            targetChunk.SetBlock(blockRelativePosition, blockType);
            Vector3Int blockGPosition = GetBlockGPos(globalPosition);
            AfterPlaceBlock(blockGPosition, blockType);
            return true;
        }


      
        public bool RemoveBlock(Vector3 globalPosition, out BlockType removedBlock)
        {
            Chunk targetChunk = GetChunk(globalPosition);
            Vector3Int blockRelativePosition = GlobalToRelativeBlockPosition(globalPosition, targetChunk._width, targetChunk._height, targetChunk._depth);
            BlockType currBlock = targetChunk.GetBlock(blockRelativePosition);
            removedBlock = currBlock;

            if (targetChunk.HasDrawnFirstTime == false)
                return false;
            Vector3Int blockGPosition = GetBlockGPos(globalPosition);
            if (currBlock != BlockType.Air)
            {
                targetChunk.SetBlock(blockRelativePosition, BlockType.Air);

                // Tempt use for destroy grass block if below air block
                BlockType upperOneBlock = GetBlock(new Vector3(globalPosition.x, globalPosition.y + 1, globalPosition.z));
                BlockType upperTwoBlock = GetBlock(new Vector3(globalPosition.x, globalPosition.y + 2, globalPosition.z));

                if (upperOneBlock.IsGrassType()) SetBlock(new Vector3(globalPosition.x, globalPosition.y + 1, globalPosition.z), BlockType.Air);
                if (upperTwoBlock.IsGrassType()) SetBlock(new Vector3(globalPosition.x, globalPosition.y + 2, globalPosition.z), BlockType.Air);


                AfterRemoveBlock(blockGPosition);
                return true;
            }

            return false;
        }

        private async void AfterPlaceBlock(Vector3Int blockGPosition, BlockType blockType)
        {
            _blockLightBfsQueue.Enqueue(new LightNode() { GlobalPosition = blockGPosition, Intensity = LightUtils.BlocksLight[(ushort)blockType]});
            await LightCalculator.PropagateBlockLightAsync(_blockLightBfsQueue, chunksNeedUpdate);
            DrawChunksAtOnce(chunksNeedUpdate);
        }


        private async void AfterRemoveBlock(Vector3Int blockGPosition)
        {
            _blockLightRemovalBfsQueue.Enqueue(new LightNode() { GlobalPosition = blockGPosition, Intensity = GetBlockLight(blockGPosition)});
            await LightCalculator.RemoveBlockLightAsync(_blockLightRemovalBfsQueue, chunksNeedUpdate);


            _ambientLightBfsQueue.Enqueue(new LightNode() { GlobalPosition = blockGPosition, Intensity = GetAmbientLight(blockGPosition) });
            await LightCalculator.RemoveAmbientLightAsync(_ambientLightBfsQueue, chunksNeedUpdate);
            DrawChunksAtOnce(chunksNeedUpdate);
        }


        private async void DrawChunksAtOnce(HashSet<Chunk> chunks)
        {
            List<Task> drawChunkTasks = new List<Task>();
            //Debug.Log($"Draw at once: {chunks.Count}");
            foreach (var chunk in chunks)
            {
                drawChunkTasks.Add(chunk.RenderChunkTask());
                //await WorldGeneration.Instance.ReDrawChunkTask(chunk);
            }
            await Task.WhenAll(drawChunkTasks);
            chunks.Clear();
        }

        // Use to detect block that has height > 1. like(door, tall grass, cactus,...)
        // This method only check downward
        public int GetBlockHeightFromOrigin(Chunk chunk, Vector3Int relativePosition)
        {
            int heightFromOrigin = 0;   // At origin
            int attempt = 0;
            BlockType blockNeedCheck = chunk.GetBlock(relativePosition);
            Vector3Int currBlockPos = relativePosition;
            while (true)
            {
                Vector3Int nextRelativePosition = new Vector3Int(currBlockPos.x, currBlockPos.y - 1, currBlockPos.z);
                if (blockNeedCheck == chunk.GetBlock(nextRelativePosition))
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
            if (offset == Vector3Int.zero)
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

