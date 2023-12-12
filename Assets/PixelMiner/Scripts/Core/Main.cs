using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;
using PixelMiner.Enums;
using PixelMiner.WorldBuilding;

namespace PixelMiner.WorldGen
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


        #region Tile Utilities
        //public WorldBuilding.Tile GetTile(Vector2 worldPosition, out Chunk2D chunk)
        //{
        //    chunk = GetChunk(worldPosition, ChunkWidth, ChunkHeight);
        //    if (chunk != null)
        //    {
        //        Vector3 localTilePosition = IsometricUtilities.GlobalToLocal(worldPosition.x, worldPosition.y, offsetX: chunk.transform.position.x, offsetY: chunk.transform.position.y);
        //        byte tileFrameX = (byte)Mathf.FloorToInt(localTilePosition.x);
        //        byte tileFrameY = (byte)Mathf.FloorToInt(localTilePosition.y);

        //        return chunk.GetTile(tileFrameX, tileFrameY);
        //    }
        //    return null;
        //}
        //public void SetTileColor(Vector2 worldPosition, Color color)
        //{
        //    WorldBuilding.Tile tile = GetTile(worldPosition, out Chunk2D chunk);
        //    if(chunk != null && tile != null)
        //    {
        //        chunk.PaintTileColor(tile, color);
        //    }
        //}
        //public async void ToggleTileColor(Vector2 worldPosition, Color color, int toggleTime = 200)
        //{
        //    SetTileColor(worldPosition, color);
        //    await Task.Delay(toggleTime);
        //    SetTileColor(worldPosition, Color.white);
        //}
        ///// <summary>
        ///// Get world position of a tile at specific position.
        ///// </summary>
        ///// <param name="worldPosition"></param>
        ///// <returns></returns>
        //public Vector2 GetTileWorldPosition(Vector2 worldPosition)
        //{
        //    WorldBuilding.Tile tile = GetTile(worldPosition, out Chunk2D chunk);
        //    if (chunk != null && tile != null)
        //    {
        //        Vector2 offset = chunk.transform.position;
        //        return IsometricUtilities.LocalToGlobal(tile.FrameX, tile.FrameY, offset.x, offset.y);
        //    }
        //    return Vector2.zero;
        //}


        //public Vector2 GetNeighborWorldPosition(Vector2 worldPosition, Vector2 direction, Vector2 offset = (default))
        //{
        //    WorldBuilding.Tile tile = GetTile(worldPosition, out Chunk2D chunk);
        //    Vector2 nbWorldPosition = Vector2.zero;
        //    if (tile != null)
        //    {
        //        if (direction.x < 0 && direction.y == 0)
        //        {
        //            // Left
        //            nbWorldPosition = GetTileWorldPosition(worldPosition) + Vector2.left;
        //        }
        //        else if (direction.x > 0 && direction.y == 0)
        //        {
        //            // Right
        //            nbWorldPosition = GetTileWorldPosition(worldPosition) + Vector2.right;
        //        }
        //        else if (direction.x == 0 && direction.y > 0)
        //        {
        //            // Up
        //            nbWorldPosition = GetTileWorldPosition(worldPosition) + Vector2.up;
        //        }
        //        else if (direction.x == 0 && direction.y < 0)
        //        {
        //            // Down
        //            nbWorldPosition = GetTileWorldPosition(worldPosition) + Vector2.down;
        //        }
        //        else if (direction.x < 0 && direction.y > 0)
        //        {
        //            // Up Left
        //            nbWorldPosition = GetTileWorldPosition(worldPosition) + MathHelper.UpLeftVector;
        //        }
        //        else if (direction.x > 0 && direction.y > 0)
        //        {
        //            // Up Right
        //            nbWorldPosition = GetTileWorldPosition(worldPosition) + MathHelper.UpRightVector;
        //        }
        //        else if (direction.x < 0 && direction.y < 0)
        //        {
        //            // Down Left
        //            nbWorldPosition = GetTileWorldPosition(worldPosition) + MathHelper.DownLeftVector;
        //        }
        //        else if (direction.x > 0 && direction.y < 0)
        //        {
        //            // Down Right
        //            nbWorldPosition = GetTileWorldPosition(worldPosition) + MathHelper.DownRightVector;
        //        }

        //        nbWorldPosition += offset;
        //    }
        //    return nbWorldPosition;
        //}
        #endregion
    }
}

