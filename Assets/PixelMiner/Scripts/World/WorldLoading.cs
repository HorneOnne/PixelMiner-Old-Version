using System.Threading.Tasks;
using UnityEngine;
using Sirenix.OdinInspector;


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
        private Vector2Int lastChunkISOFrame;
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
                if (InitFastDrawChunk)
                    LoadChunksAroundPositionInParallel(lastChunkISOFrame.x, lastChunkISOFrame.y, offsetWidth: LoadChunkOffsetWidth, offsetHeight: LoadChunkOffsetHeight);
                else
                    LoadChunksAroundPositionInSequence(lastChunkISOFrame.x, lastChunkISOFrame.y, offsetWidth: LoadChunkOffsetWidth, offsetHeight: LoadChunkOffsetHeight);
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
            if (_main.AutoLoadChunk)
            {
                _centerPoint = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2f, Screen.height / 2f));
                _centerPointFrame = IsometricUtilities.ReverseConvertWorldPositionToIsometricFrame(_centerPoint, _main.ChunkWidth, _main.ChunkHeight);

                if (_centerPointFrame != lastChunkISOFrame)
                {
                    lastChunkISOFrame = _centerPointFrame;
                    LoadChunksAroundPositionInSequence(_centerPointFrame.x, _centerPointFrame.y, offsetWidth: LoadChunkOffsetWidth, offsetHeight: LoadChunkOffsetHeight);
                    Debug.Log("Load");
                }
            }
        }

  

        /// <summary>
        /// Load each chunk in sequence and draw each chunk in sequence. -> Less drop FPS but slow.
        /// </summary>
        /// <param name="isoFrameX"></param>
        /// <param name="isoFrameY"></param>
        /// <param name="offsetWidth"></param>
        /// <param name="offsetHeight"></param>
        private async void LoadChunksAroundPositionInSequence(int isoFrameX, int isoFrameY, byte offsetWidth = 1, byte offsetHeight = 1)
        {
            for (int x = isoFrameX - offsetWidth; x <= isoFrameX + offsetWidth; x++)
            {
                for (int y = isoFrameY - offsetHeight; y <= isoFrameY + offsetHeight; y++)
                {
                    Vector2Int nbIsoFrame = new Vector2Int(x, y);
                    Chunk chunk = _main.GetChunk(nbIsoFrame);
                    if (chunk == null)   // Create new chunk
                    {
                        if (x == isoFrameX && y == isoFrameY)
                        {
                            // Center
                            // ......
                        }

                        Chunk newChunk = await _worldGen.GenerateNewChunkDataAsync(x, y);

                        // Cached chunk data
                        if (_main.HasChunk(nbIsoFrame) == false)
                            _main.AddNewChunk(newChunk);
                        _main.ActiveChunks.Add(newChunk);


                        if (newChunk.ChunkHasDrawn == false)
                        {
                            await _worldGen.DrawChunkAsync(newChunk);
                            //_main.Chunks[nbIsoFrame].ShowTextTest();

                            if (_main.InitWorldWithHeatmap)
                                _worldGen.PaintHeatMap(newChunk);
                            if (_main.InitWorldWithMoisturemap)
                                _worldGen.PaintMoistureMap(newChunk);
                        }

                        _main.GetChunk(nbIsoFrame).LoadChunk();
                    }
                    else // Load chunk cached.
                    {
                        if (x == isoFrameX && y == isoFrameY)
                        {
                            // Center
                            // ......
                        }

                        _main.ActiveChunks.Add(_main.Chunks[nbIsoFrame]);

                        if (_main.Chunks[nbIsoFrame].ChunkHasDrawn == false)
                        {
                            await _worldGen.DrawChunkAsync(_main.Chunks[nbIsoFrame]);
                            //_main.Chunks[nbIsoFrame].ShowTextTest();

                            if (_main.InitWorldWithHeatmap)
                                _worldGen.PaintHeatMap(_main.Chunks[nbIsoFrame]);
                            if (_main.InitWorldWithMoisturemap)
                                _worldGen.PaintMoistureMap(_main.Chunks[nbIsoFrame]);
                        }
                        _main.Chunks[nbIsoFrame].LoadChunk();
                    }
                }
            }

            if (_main.ShowChunksBorder)
            {
                _worldGen.SortActiveChunkByDepth();
            }

            _worldGen.UpdateAllActiveChunkTileNeighborsAsync();
        }
        /// <summary>
        /// Load all chunks and draw all chunks at the same time. -> Fast but drop a bit FPS in low end devices.
        /// </summary>
        /// <param name="isoFrameX"></param>
        /// <param name="isoFrameY"></param>
        /// <param name="offsetWidth"></param>
        /// <param name="offsetHeight"></param>
        private async void LoadChunksAroundPositionInParallel(int isoFrameX, int isoFrameY, byte offsetWidth = 1, byte offsetHeight = 1)
        {
            Task[] drawChunkTasks = new Task[(offsetWidth * 2 + 1) * (offsetHeight * 2 + 1)];

            for (int x = isoFrameX - offsetWidth; x <= isoFrameX + offsetWidth; x++)
            {
                for (int y = isoFrameY - offsetHeight; y <= isoFrameY + offsetHeight; y++)
                {
                    int index = x - (isoFrameX - offsetWidth) + (y - (isoFrameY - offsetHeight)) * (2 * offsetWidth + 1);
                    Vector2Int nbIsoFrame = new Vector2Int(x, y);
                    Chunk chunk = _main.GetChunk(nbIsoFrame);

                    if (chunk == null)   // Create new chunk
                    {
                        if (x == isoFrameX && y == isoFrameY)
                        {
                            // Center
                            // ......
                        }

                        Chunk newChunk = await _worldGen.GenerateNewChunkDataAsync(x, y);
                        drawChunkTasks[index] = _worldGen.DrawChunkAsync(_main.Chunks[nbIsoFrame]);

                        // Cached chunk data
                        if (_main.HasChunk(nbIsoFrame) == false)
                        {
                            _main.AddNewChunk(newChunk);
                        }

                        _main.ActiveChunks.Add(newChunk);
                        _main.Chunks[nbIsoFrame].LoadChunk();

                    }
                    else // Load chunk cached.
                    {
                        if (x == isoFrameX && y == isoFrameY)
                        {
                            // Center
                            // ......
                        }

                        _main.GetChunk(nbIsoFrame).LoadChunk();
                        _main.ActiveChunks.Add(_main.Chunks[nbIsoFrame]);

                        if (_main.Chunks[nbIsoFrame].ChunkHasDrawn == false)
                        {
                            drawChunkTasks[index] = _worldGen.DrawChunkAsync(_main.Chunks[nbIsoFrame]);
                        }
                        else
                        {
                            drawChunkTasks[index] = Task.CompletedTask;
                        }
                    }

                }
            }

            // When all chunk has drawn.
            await Task.WhenAll(drawChunkTasks);


            foreach (var chunk in _main.Chunks.Values)
            {
                if (_main.InitWorldWithHeatmap)
                    _worldGen.PaintHeatMap(chunk);

                if (_main.InitWorldWithMoisturemap)
                    _worldGen.PaintMoistureMap(chunk);

                if (!chunk.HasNeighbors())
                {
                    _worldGen.UpdateChunkTileNeighbors(chunk);
                }
            }


            if (_main.ShowChunksBorder)
            {
                _worldGen.SortActiveChunkByDepth();
            }
        }

        private void UnloadChunk(Chunk chunk)
        {
            _main.ActiveChunks.Remove(chunk);
            chunk.gameObject.SetActive(false);
        }
    }
}

