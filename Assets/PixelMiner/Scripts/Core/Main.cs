using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;
using PixelMiner.Enums;
using PixelMiner.Utilities;
using System.Threading.Tasks;
using PixelMiner.WorldBuilding;
using PixelMiner.Core;

namespace PixelMiner.WorldGen
{
    public class Main : MonoBehaviour
    {
        public static Main Instance { get; private set; }

        // BlockUV
        public BlockDataSO BlockDataSO;
        public float TileSizeX, TileSizeY;  
        public Dictionary<BlockType, BlockData> BlockDataDict = new Dictionary<BlockType, BlockData>();


        // Tiles data
        [AssetList(Path = "/PixelMiner/Tiles/")]
        public List<CustomTileBase> TileBaseList = new List<CustomTileBase>();
        [AssetList(Path = "/PixelMiner/Tiles/Animated Tiles")]
        public List<CustomAnimatedTileBase> AnimatedTileBaseList = new List<CustomAnimatedTileBase>();
        private Dictionary<TileType, CustomTileBase> _tileBaseDict = new Dictionary<TileType, CustomTileBase>();
        private Dictionary<TileType, CustomAnimatedTileBase> _animatedTileBaseDict = new Dictionary<TileType, CustomAnimatedTileBase>();


        // Chunk data
        [Header("Data Cached")]
        public Dictionary<Vector2Int, Chunk> Chunks;
        public HashSet<Chunk> ActiveChunks;

        // Tilemap Settings
        [FoldoutGroup("Tilemap Settings"), Indent(1), ShowInInspector, ReadOnly] public readonly float IsometricAngle = 26.565f;
        [FoldoutGroup("Tilemap Settings"), Indent(1), ShowInInspector, ReadOnly] public readonly Vector3 CELL_SIZE = new Vector3(2.0f, 1.0f, 1.0f);
        [FoldoutGroup("Tilemap Settings"), Indent(1), ShowInInspector, ReadOnly] public readonly Vector3 CELL_GAP = new Vector3(0.0f, 0.0f, 0.0f);


        public string SeedInput = "7";

        private readonly Vector2 HORIZONTAL_DISTANCE_TO_NB = new Vector2(2.0f, 1.0f);
        private readonly Vector2 VERTICAL_DISTANCE_TO_NB = new Vector2(1.0f, 0.5f);

        public byte ChunkWidth = 32;
        public byte ChunkHeight = 32;

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
            LoadTileBaseDictionary();


            // Initialize the chunks data
            Chunks = new Dictionary<Vector2Int, Chunk>();
            ActiveChunks = new HashSet<Chunk>();
        }
        private void Start()
        {
            InitBlockDataDictionary();
        }

       
        #region Initialize tile data
        private void LoadTileBaseDictionary()
        {
            foreach (var tilebase in TileBaseList)
            {
                _tileBaseDict.Add(tilebase.Type, tilebase);
            }

            foreach (var animatedTile in AnimatedTileBaseList)
            {
                _animatedTileBaseDict.Add(animatedTile.Type, animatedTile);
            }
        }
        public TileBase GetTileBase(TileType tileType)
        {
            if (_tileBaseDict.ContainsKey(tileType))
            {
                return _tileBaseDict[tileType];
            }
            if (_animatedTileBaseDict.ContainsKey(tileType))
            {
                return _animatedTileBaseDict[tileType];
            }
            return null;
        }
        private void InitBlockDataDictionary()
        {
            TileSizeX = BlockDataSO.TileSizeX;
            TileSizeY = BlockDataSO.TileSizeY;
            foreach(var blockData in BlockDataSO.BLockDataList)
            {
                if(!BlockDataDict.ContainsKey(blockData.BlockType))
                {
                    BlockDataDict.Add(blockData.BlockType, blockData);
                }
            }
        }
        #endregion

        #region Get, Set Chunk
        public bool HasChunk(int isoFrameX, int isoFrameY)
        {
            return Chunks.ContainsKey(new Vector2Int(isoFrameX, isoFrameY));
        }
        public bool HasChunk(Vector2Int isoFrame)
        {
            return Chunks.ContainsKey(isoFrame);
        }
        public Chunk GetChunk(Vector2 worldPosition, int chunkWidth, int chunkHeight)
        {
            var isoFrame = IsometricUtilities.ReverseConvertWorldPositionToIsometricFrame(worldPosition,
                                                                               chunkWidth,
                                                                               chunkHeight);
            return GetChunk(isoFrame);
        }
        public Chunk GetChunk(int isoFrameX, int isoFrameY)
        {
            Vector2Int isoFrame = new Vector2Int(isoFrameX, isoFrameY);
            if (Chunks.ContainsKey(isoFrame))
            {
                return Chunks[isoFrame];
            }
            return null;
        }
        public Chunk GetChunk(Vector2Int isoFrame)
        {
            if (Chunks.ContainsKey(isoFrame))
            {
                return Chunks[isoFrame];
            }
            return null;
        }
        public void AddNewChunk(Chunk chunk)
        {
            Vector2Int isoFrame = new Vector2Int(chunk.IsometricFrameX, chunk.IsometricFrameY);
            Chunks.Add(isoFrame, chunk);
        }
        public void AddNewChunk(Chunk chunk, Vector2Int isoFrame)
        {
            Chunks.Add(isoFrame, chunk);
        }
        #endregion

        #region Neighbors
        public Chunk GetChunkNeighborAbove(Chunk chunk)
        {
            Vector2Int isoFrameChunkNb = new Vector2Int(chunk.IsometricFrameX, chunk.IsometricFrameY + 1);
            return Chunks.TryGetValue(isoFrameChunkNb, out Chunk neighborChunk) ? neighborChunk : null;
        }
        public Chunk GetChunkNeighborBelow(Chunk chunk)
        {
            Vector2Int isoFrameChunkNb = new Vector2Int(chunk.IsometricFrameX, chunk.IsometricFrameY - 1);
            return Chunks.TryGetValue(isoFrameChunkNb, out Chunk neighborChunk) ? neighborChunk : null;
        }
        public Chunk GetChunkNeighborLeft(Chunk chunk)
        {
            Vector2Int isoFrameChunkNb = new Vector2Int(chunk.IsometricFrameX - 1, chunk.IsometricFrameY);
            return Chunks.TryGetValue(isoFrameChunkNb, out Chunk neighborChunk) ? neighborChunk : null;
        }
        public Chunk GetChunkNeighborRight(Chunk chunk)
        {
            Vector2Int isoFrameChunkNb = new Vector2Int(chunk.IsometricFrameX + 1, chunk.IsometricFrameY);
            return Chunks.TryGetValue(isoFrameChunkNb, out Chunk neighborChunk) ? neighborChunk : null;
        }
        #endregion


        #region Tile Utilities
        public WorldBuilding.Tile GetTile(Vector2 worldPosition, out Chunk chunk)
        {
            chunk = GetChunk(worldPosition, ChunkWidth, ChunkHeight);
            if (chunk != null)
            {
                Vector3 localTilePosition = IsometricUtilities.GlobalToLocal(worldPosition.x, worldPosition.y, offsetX: chunk.transform.position.x, offsetY: chunk.transform.position.y);
                byte tileFrameX = (byte)Mathf.FloorToInt(localTilePosition.x);
                byte tileFrameY = (byte)Mathf.FloorToInt(localTilePosition.y);

                return chunk.GetTile(tileFrameX, tileFrameY);
            }
            return null;
        }
        public void SetTileColor(Vector2 worldPosition, Color color)
        {
            WorldBuilding.Tile tile = GetTile(worldPosition, out Chunk chunk);
            if(chunk != null && tile != null)
            {
                chunk.PaintTileColor(tile, color);
            }
        }
        public async void ToggleTileColor(Vector2 worldPosition, Color color, int toggleTime = 200)
        {
            SetTileColor(worldPosition, color);
            await Task.Delay(toggleTime);
            SetTileColor(worldPosition, Color.white);
        }
        /// <summary>
        /// Get world position of a tile at specific position.
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public Vector2 GetTileWorldPosition(Vector2 worldPosition)
        {
            WorldBuilding.Tile tile = GetTile(worldPosition, out Chunk chunk);
            if (chunk != null && tile != null)
            {
                Vector2 offset = chunk.transform.position;
                return IsometricUtilities.LocalToGlobal(tile.FrameX, tile.FrameY, offset.x, offset.y);
            }
            return Vector2.zero;
        }
        public Vector2 GetNeighborWorldPosition(Vector2 worldPosition, Vector2 direction, Vector2 offset = (default))
        {
            WorldBuilding.Tile tile = GetTile(worldPosition, out Chunk chunk);
            Vector2 nbWorldPosition = Vector2.zero;
            if (tile != null)
            {
                if (direction.x < 0 && direction.y == 0)
                {
                    // Left
                    nbWorldPosition = GetTileWorldPosition(worldPosition) + Vector2.left * HORIZONTAL_DISTANCE_TO_NB;
                }
                else if (direction.x > 0 && direction.y == 0)
                {
                    // Right
                    nbWorldPosition = GetTileWorldPosition(worldPosition) + Vector2.right * HORIZONTAL_DISTANCE_TO_NB;
                }
                else if (direction.x == 0 && direction.y > 0)
                {
                    // Up
                    nbWorldPosition = GetTileWorldPosition(worldPosition) + Vector2.up * HORIZONTAL_DISTANCE_TO_NB;
                }
                else if (direction.x == 0 && direction.y < 0)
                {
                    // Down
                    nbWorldPosition = GetTileWorldPosition(worldPosition) + Vector2.down * HORIZONTAL_DISTANCE_TO_NB;
                }
                else if (direction.x < 0 && direction.y > 0)
                {
                    // Up Left
                    nbWorldPosition = GetTileWorldPosition(worldPosition) + MathHelper.UpLeftVector * VERTICAL_DISTANCE_TO_NB;
                }
                else if (direction.x > 0 && direction.y > 0)
                {
                    // Up Right
                    nbWorldPosition = GetTileWorldPosition(worldPosition) + MathHelper.UpRightVector * VERTICAL_DISTANCE_TO_NB;
                }
                else if (direction.x < 0 && direction.y < 0)
                {
                    // Down Left
                    nbWorldPosition = GetTileWorldPosition(worldPosition) + MathHelper.DownLeftVector * VERTICAL_DISTANCE_TO_NB;
                }
                else if (direction.x > 0 && direction.y < 0)
                {
                    // Down Right
                    nbWorldPosition = GetTileWorldPosition(worldPosition) + MathHelper.DownRightVector * VERTICAL_DISTANCE_TO_NB;
                }

                nbWorldPosition += offset;
            }
            return nbWorldPosition;
        }
        #endregion


        #region Block
    
        #endregion
    }
}

