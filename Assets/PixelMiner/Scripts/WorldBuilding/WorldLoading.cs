using UnityEngine;
using Sirenix.OdinInspector;
using PixelMiner.Core;
using PixelMiner.World;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PixelMiner.WorldBuilding
{
    public class WorldLoading : MonoBehaviour
    {
        public static WorldLoading Instance { get; private set; }
        private Main _main;
        private WorldGeneration _worldGen;

        [FoldoutGroup("World Settings"), Indent(1)] public byte InitWorldWidth = 3;
        [FoldoutGroup("World Settings"), Indent(1)] public byte InitWorldHeight = 3;
        [FoldoutGroup("World Settings"), Indent(1)] public byte InitWorldDepth = 3;
        [FoldoutGroup("World Settings"), Indent(1)] public byte LoadChunkOffsetWidth = 1;
        [FoldoutGroup("World Settings"), Indent(1)] public byte LoadChunkOffsetHeight = 1;
        [FoldoutGroup("World Settings"), Indent(1)] public byte LoadChunkOffsetDepth = 1;


        // Cached
        [SerializeField] private Transform _playerTrans;
        public Vector3Int LastChunkFrame { get; private set; }
        private Vector3Int _currentFrame;
        // Performance
        private float _updateTimer = 0.0f;
        private float _updateTime = 0.2f;
        private float _unloadChunkDistance = 200f;


        private List<Chunk> _preDrawChunkList = new List<Chunk>();
        private List<Task> _preDrawChunkTaskList = new List<Task>();
        private List<Task> _drawChunkList = new List<Task>();
        //private List<Task> _propageAmbientLightTaskList = new List<Task>();
        private List<Chunk> _loadChunkList = new List<Chunk>();
        private List<Chunk> _unloadChunkList = new List<Chunk>();
        private bool _finishLoadChunk = true;


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


            _worldGen.OnWorldGenWhenStartFinished += LoadChunkAroundAysnc;

            //Chunk.OnChunkFarAway += OnChunkFarAway;
        }

        private void OnDestroy()
        {
            //Chunk.OnChunkFarAway -= OnChunkFarAway;
        }

        private async void Update()
        {

            if (Time.time - _updateTime > _updateTimer)
            {
                _updateTimer = Time.time;
                if (_main.AutoLoadChunk)
                {
                    _currentFrame = new Vector3Int(
                        Mathf.FloorToInt(_playerTrans.position.x / _main.ChunkDimension[0]),
                        Mathf.FloorToInt(_playerTrans.position.y / _main.ChunkDimension[1]),
                        Mathf.FloorToInt(_playerTrans.position.z / _main.ChunkDimension[2]));


                    if (_currentFrame != LastChunkFrame && _finishLoadChunk)
                    {
                        _finishLoadChunk = false;
                        LastChunkFrame = _currentFrame;
                        LoadChunkAroundAysnc();
                    }
                }
            }


            if (Input.GetKeyDown(KeyCode.J))
            {
                foreach (var chunk in Main.Instance.ActiveChunks)
                {
                    await _worldGen.ReDrawChunkTask(chunk);
                }
            }
        }


        private async void LoadChunkAroundAysnc()
        {
            await LoadChunksAroundPositionTask(LastChunkFrame.x, LastChunkFrame.y, LastChunkFrame.z,
                                                         offsetWidth: LoadChunkOffsetWidth,
                                                         offsetHeight: LoadChunkOffsetHeight,
                                                         offsetDepth: LoadChunkOffsetDepth);
        }



        /// <summary>
        /// Load each chunk in sequence and draw each chunk in sequence. -> Less drop FPS but slow.
        /// </summary>
        /// <param name="frameX"></param>
        /// <param name="frameZ"></param>
        /// <param name="offsetWidth"></param>
        /// <param name="offsetDepth"></param>
        public async Task LoadChunksAroundPositionTask(int frameX, int frameY, int frameZ, byte offsetWidth = 1, byte offsetHeight = 1, byte offsetDepth = 1)
        {
            for (int x = frameX - offsetWidth; x <= frameX + offsetWidth; x++)
            {
                for (int y = frameY - offsetHeight; y <= frameY + offsetHeight; y++)
                {
                    for (int z = frameZ - offsetDepth; z <= frameZ + offsetDepth; z++)
                    {
                        // Only need 1 down neighbor
                        if (y < 0)
                        {
                            //if (!(x == frameX && z == frameZ))
                            //{
                            //    continue;
                            //}
                            y = 0;
                        }


                        Vector3Int nbFrame = new Vector3Int(x, y, z);
                        Chunk chunk = _main.GetChunk(nbFrame);
                        if (chunk == null)   // Create new chunk
                        {
                            if (x == frameX && z == frameZ)
                            {
                                // Center
                                // ......
                            }


                            Chunk newChunk = await _worldGen.GenerateNewChunk(x, y, z, _main.ChunkDimension);
                            _loadChunkList.Add(newChunk);
                            //LoadChunk(newChunk);
                            if (newChunk.ChunkHasDrawn == false)
                            {

                            }


                        }
                        else // Load chunk cached.
                        {
                            if (x == frameX && z == frameZ)
                            {
                                // Center
                                // ......
                            }
                            //LoadChunk(_main.Chunks[nbFrame]);
                            _loadChunkList.Add(_main.Chunks[nbFrame]);

                            if (_main.Chunks[nbFrame].ChunkHasDrawn == false)
                            {

                            }
                        }
                    }
                }
            }

            int count = 0;
            int maxChunkDrawInStage = 10;


            for (int i = 0; i < _loadChunkList.Count; i++)
            {
                LoadChunk(_loadChunkList[i]);
            }

            // Pre-draw chunk
            foreach (var activeChunk in _main.ActiveChunks)
            {
                if (_worldGen.UpdateChunkNeighbors(activeChunk))
                {
                    //_worldGen.PropagateAmbientLight(activeChunk);
                    //await _worldGen.UpdateChunkWhenHasAllNeighborsTask(activeChunk);

                   
                    _preDrawChunkTaskList.Add(_worldGen.UpdateChunkWhenHasAllNeighborsTask(activeChunk));
                    _preDrawChunkList.Add(activeChunk);
                }
            }

            //await Task.WhenAll(_propageAmbientLightTaskList);
            await Task.WhenAll(_preDrawChunkTaskList);
 



            // Draw chunk
            for (int i = 0; i < _preDrawChunkList.Count; i++)
            {
                _drawChunkList.Add(_worldGen.DrawChunkTask(_preDrawChunkList[i]));

                if (count > maxChunkDrawInStage)
                {
                    count = 0;
                    await Task.WhenAll(_drawChunkList);
                    _drawChunkList.Clear();
                }
                count++;
            }
            
            if (_drawChunkList.Count > 0)
            {
                Debug.Log($"Draw last: {_drawChunkList.Count}");
                await Task.WhenAll(_drawChunkList);
                _drawChunkList.Clear();
            }



            // Unload chunk
            if (Main.Instance.AutoUnloadChunk)
            {
                foreach (var activeChunk in _main.ActiveChunks)
                {
                    if (Vector3.Distance(_playerTrans.position, activeChunk.transform.position) > _unloadChunkDistance)
                    {
                        _unloadChunkList.Add(activeChunk);
                    }
                }

                for (int i = 0; i < _unloadChunkList.Count; i++)
                {
                    UnloadChunk(_unloadChunkList[i]);
                }
            }

            //_propageAmbientLightTaskList.Clear();
            _preDrawChunkTaskList.Clear();
            _preDrawChunkList.Clear();
            _unloadChunkList.Clear();
            _loadChunkList.Clear();
            _finishLoadChunk = true;
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
    }
}

