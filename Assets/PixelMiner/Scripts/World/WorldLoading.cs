using System.Threading.Tasks;
using UnityEngine;
using Sirenix.OdinInspector;
using PixelMiner.WorldBuilding;


namespace PixelMiner.WorldGen
{
    public class WorldLoading : MonoBehaviour
    {
        public static WorldLoading Instance { get; private set; }
        private Main _main;
        private WorldGeneration _worldGen;

        [FoldoutGroup("World Settings"), Indent(1)] public byte InitWorldWidth = 3;
        [FoldoutGroup("World Settings"), Indent(1)] public byte InitWorldHeight = 3;
        [FoldoutGroup("World Settings"), Indent(1)] public byte LoadChunkOffsetWidth = 1;
        [FoldoutGroup("World Settings"), Indent(1)] public byte LoadChunkOffsetHeight = 1;

        [Header("Performance Options")]
        public bool InitFastDrawChunk;


        // Cached
        private Vector3Int lastChunkFrame;
        private Vector2 _centerPoint;
        private Vector2Int _centerPointFrame;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _main = Main.Instance;
            _worldGen = WorldGeneration.Instance;

            _worldGen.OnWorldGenWhenStartFinished += () =>
            {
                //if (InitFastDrawChunk)
                //    LoadChunksAroundPositionInParallel(lastChunkFrame.x, lastChunkFrame.y, lastChunkFrame.z, offsetWidth: LoadChunkOffsetWidth, offsetHeight: LoadChunkOffsetHeight);
                //else
                //    LoadChunksAroundPositionInSequence(lastChunkFrame.x, lastChunkFrame.y, offsetWidth: LoadChunkOffsetWidth, offsetDepth: LoadChunkOffsetHeight);

                LoadChunksAroundPositionInSequence(0, 0, 0, offsetWidth: LoadChunkOffsetWidth, offsetDepth: LoadChunkOffsetHeight);
            };

            Chunk.OnChunkFarAway += (chunk) =>
            {
                if (chunk.ChunkHasDrawn && !chunk.Processing
                    && Main.Instance.AutoUnloadChunk)
                {
                    UnloadChunk(chunk);
                }
            };
        }

        private void Update()
        {
            //if (_main.AutoLoadChunk)
            //{
            //    _centerPoint = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2f, Screen.height / 2f));
            //    _centerPointFrame = IsometricUtilities.ReverseConvertWorldPositionToIsometricFrame(_centerPoint, _main.ChunkWidth, _main.ChunkHeight);

            //    if (_centerPointFrame != lastChunkFrame)
            //    {
            //        lastChunkFrame = _centerPointFrame;
            //        LoadChunksAroundPositionInSequence(_centerPointFrame.x, _centerPointFrame.y, offsetWidth: LoadChunkOffsetWidth, offsetDepth: LoadChunkOffsetHeight);
            //    }
            //}
        }

  

        /// <summary>
        /// Load each chunk in sequence and draw each chunk in sequence. -> Less drop FPS but slow.
        /// </summary>
        /// <param name="frameX"></param>
        /// <param name="frameZ"></param>
        /// <param name="offsetWidth"></param>
        /// <param name="offsetDepth"></param>
        private async void LoadChunksAroundPositionInSequence(int frameX, int frameY, int frameZ, byte offsetWidth = 1, byte offsetDepth = 1)
        {
            for (int x = frameX - offsetWidth; x <= frameX + offsetWidth; x++)
            {
                for (int z = frameZ - offsetDepth; z <= frameZ + offsetDepth; z++)
                {
                    Vector3Int nbFrame = new Vector3Int(x,0, z);
                    Chunk chunk = _main.GetChunk(nbFrame);
                    if (chunk == null)   // Create new chunk
                    {
                        if (x == frameX && z == frameZ)
                        {
                            // Center
                            // ......
                        }

                        Chunk newChunk = await _worldGen.GenerateNewChunkDataAsync(x, 0, z);

                        // Cached chunk data
                        if (_main.HasChunk(nbFrame) == false)
                            _main.AddNewChunk(newChunk);
                        _main.ActiveChunks.Add(newChunk);


                        if (newChunk.ChunkHasDrawn == false)
                        {
                            newChunk.DrawChunkAsync();
                            //_main.Chunks[nbIsoFrame].ShowTextTest();

                            //if (_main.InitWorldWithHeatmap)
                            //    _worldGen.PaintHeatMap(newChunk);
                            //if (_main.InitWorldWithMoisturemap)
                            //    _worldGen.PaintMoistureMap(newChunk);
                        }

                        //_main.GetChunk(nbFrame).LoadChunk();
                    }
                    else // Load chunk cached.
                    {
                        if (x == frameX && z == frameZ)
                        {
                            // Center
                            // ......
                        }

                        _main.ActiveChunks.Add(_main.Chunks[nbFrame]);

                        if (_main.Chunks[nbFrame].ChunkHasDrawn == false)
                        {
                            _main.Chunks[nbFrame].DrawChunkAsync();
                            //_main.Chunks[nbIsoFrame].ShowTextTest();

                            //if (_main.InitWorldWithHeatmap)
                            //    _worldGen.PaintHeatMap(_main.Chunks[nbFrame]);
                            //if (_main.InitWorldWithMoisturemap)
                            //    _worldGen.PaintMoistureMap(_main.Chunks[nbFrame]);
                        }
                        //_main.Chunks[nbFrame].LoadChunk();
                    }
                }
            }


            //_worldGen.UpdateAllActiveChunkTileNeighborsAsync();
        }
        /// <summary>
        /// Load all chunks and draw all chunks at the same time. -> Fast but drop a bit FPS in low end devices.
        /// </summary>
        /// <param name="frameX"></param>
        /// <param name="frameZ"></param>
        /// <param name="offsetWidth"></param>
        /// <param name="offsetHeight"></param>
        //private async void LoadChunksAroundPositionInParallel(int frameX, int frameY, int frameZ, byte offsetWidth = 1, byte offsetHeight = 1)
        //{
        //    Task[] drawChunkTasks = new Task[(offsetWidth * 2 + 1) * (offsetHeight * 2 + 1)];

        //    for (int x = frameX - offsetWidth; x <= frameX + offsetWidth; x++)
        //    {
        //        for (int z = frameZ - offsetHeight; z <= frameZ + offsetHeight; z++)
        //        {
        //            int index = x - (frameX - offsetWidth) + (z - (frameZ - offsetHeight)) * (2 * offsetWidth + 1);
        //            Vector3Int nbFrame = new Vector3Int(x,0,z);
        //            Chunk chunk = _main.GetChunk(nbFrame);

        //            if (chunk == null)   // Create new chunk
        //            {
        //                if (x == frameX && z == frameZ)
        //                {
        //                    // Center
        //                    // ......
        //                }

        //                Chunk newChunk = await _worldGen.GenerateNewChunkDataAsync(x, 0, z);
        //                drawChunkTasks[index] = _worldGen.DrawChunkAsync(_main.Chunks[nbFrame]);

        //                // Cached chunk data
        //                if (_main.HasChunk(nbFrame) == false)
        //                {
        //                    _main.AddNewChunk(newChunk);
        //                }

        //                _main.ActiveChunks.Add(newChunk);
        //                _main.Chunks[nbFrame].LoadChunk();

        //            }
        //            else // Load chunk cached.
        //            {
        //                if (x == frameX && z == frameZ)
        //                {
        //                    // Center
        //                    // ......
        //                }

        //                _main.GetChunk(nbFrame).LoadChunk();
        //                _main.ActiveChunks.Add(_main.Chunks[nbFrame]);

        //                if (_main.Chunks[nbFrame].ChunkHasDrawn == false)
        //                {
        //                    drawChunkTasks[index] = _worldGen.DrawChunkAsync(_main.Chunks[nbFrame]);
        //                }
        //                else
        //                {
        //                    drawChunkTasks[index] = Task.CompletedTask;
        //                }
        //            }

        //        }
        //    }

        //    // When all chunk has drawn.
        //    await Task.WhenAll(drawChunkTasks);


        //    foreach (var chunk in _main.Chunks.Values)
        //    {
        //        if (_main.InitWorldWithHeatmap)
        //            _worldGen.PaintHeatMap(chunk);

        //        if (_main.InitWorldWithMoisturemap)
        //            _worldGen.PaintMoistureMap(chunk);

        //        if (!chunk.HasNeighbors())
        //        {
        //            _worldGen.UpdateChunkTileNeighbors(chunk);
        //        }
        //    }


        //    if (_main.ShowChunksBorder)
        //    {
        //        _worldGen.SortActiveChunkByDepth();
        //    }
        //}

        private void UnloadChunk(Chunk chunk)
        {
            _main.ActiveChunks.Remove(chunk);
            chunk.gameObject.SetActive(false);
        }
    }
}

