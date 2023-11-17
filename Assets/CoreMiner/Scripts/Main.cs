using CoreMiner.WorldGen;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;

namespace CoreMiner
{
    public class Main : MonoBehaviour
    {
        public static Main Instance { get; private set; }

        // Tiles data
        [AssetList(Path = "/CoreMiner/Tiles/")]
        public List<CustomTileBase> TileBaseList = new List<CustomTileBase>();
        [AssetList(Path = "/CoreMiner/Tiles/Animated Tiles")]
        public List<CustomAnimatedTileBase> AnimatedTileBaseList = new List<CustomAnimatedTileBase>();
        private Dictionary<TileType, CustomTileBase> _tileBaseDict = new Dictionary<TileType, CustomTileBase>();
        private Dictionary<TileType, CustomAnimatedTileBase> _animatedTileBaseDict = new Dictionary<TileType, CustomAnimatedTileBase>();


        // Chunk data
        [Header("Data Cached")]
        public Dictionary<Vector2Int, Chunk> Chunks;
        public HashSet<Chunk> ActiveChunks;



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
            foreach(var tilebase in TileBaseList)
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
            if(_tileBaseDict.ContainsKey(tileType))
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
    }
}

