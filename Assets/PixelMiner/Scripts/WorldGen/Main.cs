using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;
using PixelMiner.Enums;
using PixelMiner.Utilities;


namespace PixelMiner.WorldGen
{
    public class Main : MonoBehaviour
    {
        public static Main Instance { get; private set; }

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


        private readonly Vector2 HORIZONTAL_DISTANCE_TO_NB = new Vector2(2.0f, 1.0f);
        private readonly Vector2 VERTICAL_DISTANCE_TO_NB = new Vector2(1.0f, 0.5f);




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
        public Tile GetTile(Vector2 worldPosition)
        {
            Chunk chunk = GetChunk(worldPosition, WorldGeneration.Instance.ChunkWidth, WorldGeneration.Instance.ChunkHeight);
            if (chunk != null)
            {
                return chunk.GetTile(worldPosition);
            }
            return null;
        }
        public void SetTileColor(Vector2 worldPosition, Color color)
        {
            Chunk chunk = GetChunk(worldPosition, WorldGeneration.Instance.ChunkWidth, WorldGeneration.Instance.ChunkHeight);
            if (chunk != null)
            {
                chunk.PaintTileColor(chunk.GetTile(worldPosition), color);
            }
        }
        public Vector2 GetTileWorldPosition(Vector2 worldPosition)
        {
            Chunk chunk = GetChunk(worldPosition, WorldGeneration.Instance.ChunkWidth, WorldGeneration.Instance.ChunkHeight);
            if (chunk != null)
            {
                return chunk.GetTileWorldPosition(worldPosition);
            }
            return Vector2.zero;
        }
        public Vector2 GetNeighborWorldPosition(Vector2 worldPosition, Direction nbDirection)
        {
            Tile tile = GetTile(worldPosition);
            Vector2 direction = Vector2.zero;
            if (tile != null)
            {
                switch (nbDirection)
                {
                    default:
                    case Direction.Left:
                        direction = Vector2.left;
                        break;
                    case Direction.Right:
                        direction = Vector2.right;
                        break;
                    case Direction.Up:
                        direction = Vector2.up;
                        break;
                    case Direction.Down:
                        direction = Vector2.down;
                        break;
                    case Direction.UpLeft:
                        direction = MathHelper.UpLeftVector;
                        break;
                    case Direction.UpRight:
                        direction = MathHelper.UpRightVector;
                        break;
                    case Direction.DownLeft:
                        direction = MathHelper.DownLeftVector;
                        break;
                    case Direction.DownRight:
                        direction = MathHelper.DownRightVector;
                        break;
                }
            }
            return GetNeighborWorldPosition(worldPosition, direction);
        }
        public Vector2 GetNeighborWorldPosition(Vector2 worldPosition, Vector2 direction)
        {
            Tile tile = GetTile(worldPosition);
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
            }
            return nbWorldPosition;
        }
        #endregion
    }
}

