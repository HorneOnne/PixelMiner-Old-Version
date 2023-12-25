using UnityEngine;
using Sirenix.OdinInspector;
using PixelMiner.WorldBuilding;
using PixelMiner.Core;


namespace PixelMiner.WorldGen
{
    public class WorldLoading : MonoBehaviour
    {
        public static WorldLoading Instance { get; private set; }
        private Main _main;
        private WorldGeneration _worldGen;

        [FoldoutGroup("World Settings"), Indent(1)] public byte InitWorldWidth = 3;
        [FoldoutGroup("World Settings"), Indent(1)] public byte InitWorldDepth = 3;
        [FoldoutGroup("World Settings"), Indent(1)] public byte LoadChunkOffsetWidth = 1;
        [FoldoutGroup("World Settings"), Indent(1)] public byte LoadChunkOffsetDepth = 1;

        [Header("Performance Options")]
        public bool InitFastDrawChunk;


        // Cached
        [SerializeField] private Transform _playerTrans;
        public Vector3Int LastChunkFrame { get; private set; }
        private Vector3Int _currentFrame;
        // Performance
        private float _updateTimer = 0.0f;
        private float _updateTime = 0.5f;


        private void Awake()
        {
            Instance = this;
            _currentFrame = new Vector3Int(Mathf.FloorToInt(_playerTrans.position.x / Main.Instance.ChunkDimension[0]), 0,
                   Mathf.FloorToInt(_playerTrans.position.z / Main.Instance.ChunkDimension[2]));
            LastChunkFrame = _currentFrame;
        }

        private void Start()
        {
            _main = Main.Instance;
            _worldGen = WorldGeneration.Instance;


            _worldGen.OnWorldGenWhenStartFinished += () =>
            {
                LoadChunksAroundPositionInSequence(LastChunkFrame.x, 0, LastChunkFrame.z, offsetWidth: LoadChunkOffsetWidth, offsetDepth: LoadChunkOffsetDepth);
                //if (InitFastDrawChunk)
                //    LoadChunksAroundPositionInParallel(lastChunkFrame.x, lastChunkFrame.y, lastChunkFrame.z, offsetWidth: LoadChunkOffsetWidth, offsetHeight: LoadChunkOffsetHeight);
                //else
                //    LoadChunksAroundPositionInSequence(lastChunkFrame.x, lastChunkFrame.y, offsetWidth: LoadChunkOffsetWidth, offsetDepth: LoadChunkOffsetHeight);

                //LoadChunksAroundPositionInSequence(_lastChunkFrame.x, 0, _lastChunkFrame.z, offsetWidth: LoadChunkOffsetWidth, offsetDepth: LoadChunkOffsetHeight);
            };



            Chunk.OnChunkFarAway += OnChunkFarAway;
        }

        private void OnDestroy()
        {
            Chunk.OnChunkFarAway -= OnChunkFarAway;
        }

        private void Update()
        {

            if (Time.time - _updateTime > _updateTimer)
            {
                _updateTimer = Time.time;
                if (_main.AutoLoadChunk)
                {
                    _currentFrame = new Vector3Int(
                        Mathf.FloorToInt(_playerTrans.position.x / _main.ChunkDimension[0]), 0,
                        Mathf.FloorToInt(_playerTrans.position.z / _main.ChunkDimension[2]));


                    if (_currentFrame != LastChunkFrame)
                    {
                        LastChunkFrame = _currentFrame;
                        LoadChunksAroundPositionInSequence(LastChunkFrame.x, 0, LastChunkFrame.z, offsetWidth: LoadChunkOffsetWidth, offsetDepth: LoadChunkOffsetDepth);
                    }
                }
            }

        }


        /// <summary>
        /// Load each chunk in sequence and draw each chunk in sequence. -> Less drop FPS but slow.
        /// </summary>
        /// <param name="frameX"></param>
        /// <param name="frameZ"></param>
        /// <param name="offsetWidth"></param>
        /// <param name="offsetDepth"></param>
        public async void LoadChunksAroundPositionInSequence(int frameX, int frameY, int frameZ, byte offsetWidth = 1, byte offsetDepth = 1)
        {
            Debug.Log("LoadChunksAroundPositionInSequence");
            for (int x = frameX - offsetWidth; x <= frameX + offsetWidth; x++)
            {
                for (int z = frameZ - offsetDepth; z <= frameZ + offsetDepth; z++)
                {
                    Vector3Int nbFrame = new Vector3Int(x, 0, z);
                    Chunk chunk = _main.GetChunk(nbFrame);
                    if (chunk == null)   // Create new chunk
                    {
                        if (x == frameX && z == frameZ)
                        {
                            // Center
                            // ......
                        }

                        Chunk newChunk = await _worldGen.GenerateNewChunk(x, 0, z, _main.ChunkDimension);
                        LoadChunk(newChunk);

                        if (newChunk.ChunkHasDrawn == false)
                        {
                            //newChunk.DrawChunkAsync();
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
                        LoadChunk(_main.Chunks[nbFrame]);


                        if (_main.Chunks[nbFrame].ChunkHasDrawn == false)
                        {
                            //_main.Chunks[nbFrame].DrawChunkAsync();

                            //_main.Chunks[nbIsoFrame].ShowTextTest();

                            //if (_main.InitWorldWithHeatmap)
                            //    _worldGen.PaintHeatMap(_main.Chunks[nbFrame]);
                            //if (_main.InitWorldWithMoisturemap)
                            //    _worldGen.PaintMoistureMap(_main.Chunks[nbFrame]);
                        }
                    }
                }
            }



            foreach (var activeChunk in _main.ActiveChunks)
            {
                _worldGen.UpdateChunkNeighbors(activeChunk);
            }
        }


        public void LoadChunk(Chunk chunk)
        {
            Vector3Int frame = new Vector3Int(chunk.FrameX, chunk.FrameY, chunk.FrameZ);
            if (!_main.Chunks.ContainsKey(frame))
                _main.Chunks.Add(frame, chunk);
            _main.ActiveChunks.Add(chunk);
            chunk.gameObject.SetActive(true);
        }
        public void UnloadChunk(Chunk chunk)
        {
            _main.ActiveChunks.Remove(chunk);
            chunk.gameObject.SetActive(false);
        }

        private void OnChunkFarAway(Chunk chunk)
        {
            if (Main.Instance.AutoUnloadChunk)
            {
                UnloadChunk(chunk);

                //_main.ActiveChunks.Remove(chunk);
                //_main.Chunks.Remove(new Vector3Int(chunk.FrameX, chunk.FrameY, chunk.FrameZ));
                //Destroy(chunk.gameObject);
            }
        }
    }
}

