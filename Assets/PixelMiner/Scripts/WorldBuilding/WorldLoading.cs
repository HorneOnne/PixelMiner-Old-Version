using UnityEngine;
using Sirenix.OdinInspector;
using PixelMiner.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PixelMiner.WorldBuilding
{
    public class WorldLoading : MonoBehaviour
    {
        public static WorldLoading Instance { get; private set; }
        public static System.Action OnFirstLoadChunks;
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
        private float _updateTime = 0.1f;
        private float _unloadChunkDistance = 200f;


        private List<Chunk> _preDrawChunkList = new List<Chunk>();
        private List<Task> _preDrawChunkTaskList = new List<Task>();
        private List<Task> _drawChunkTaskList = new List<Task>();
        private HashSet<Chunk> _redrawChunkSet = new HashSet<Chunk>();
        private List<Task> _redrawChunkTaskList = new List<Task>();
        private List<Chunk> _loadChunkList = new List<Chunk>();
        private List<Chunk> _unloadChunkList = new List<Chunk>();
        private bool _finishLoadChunk = true;
        private bool _worldGenStartFinish = false;

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


            WorldGeneration.OnWorldLoadFinished += () =>
            {
                LastChunkFrame = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
            };
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
                        //LoadChunkAroundAysnc();
                        await LoadChunksAroundPositionTask(LastChunkFrame.x, LastChunkFrame.y, LastChunkFrame.z,
                                                         offsetWidth: LoadChunkOffsetWidth,
                                                         offsetHeight: LoadChunkOffsetHeight,
                                                         offsetDepth: LoadChunkOffsetDepth);
                        OnFirstLoadChunks?.Invoke();
                    }
                }
            }


            if (Input.GetKeyDown(KeyCode.J))
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                foreach (var chunk in Main.Instance.ActiveChunks)
                {
                    if(chunk.HasDrawnFirstTime)
                        await _worldGen.ReDrawChunkTask(chunk);
                }

                sw.Stop();
                Debug.Log(sw.ElapsedMilliseconds / 1000f);
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
                            if (newChunk.HasDrawnFirstTime == false)
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
                            Chunk targetChunk = _main.GetChunk(nbFrame);
                            _loadChunkList.Add(targetChunk);
                            if (targetChunk.HasDrawnFirstTime == false)
                            {

                            }
                        }
                    }
                }
            }


            int preDrawChunkCount = 0;
            int maxChunkPreDrawInStage = 2;

            for (int i = 0; i < _loadChunkList.Count; i++)
            {
                LoadChunk(_loadChunkList[i]);
            }

            // Pre-draw chunk
            foreach (var activeChunk in _main.ActiveChunks)
            {
                if (preDrawChunkCount > maxChunkPreDrawInStage)
                {                 
                    await Task.WhenAll(_preDrawChunkTaskList);
                    preDrawChunkCount = 0;
                    _preDrawChunkTaskList.Clear();
                }

                if (_worldGen.UpdateChunkNeighbors(activeChunk))
                {
                    _preDrawChunkTaskList.Add(_worldGen.UpdateChunkWhenHasAllNeighborsTask(activeChunk));
                    //await _worldGen.UpdateChunkWhenHasAllNeighborsTask(activeChunk);
                    _preDrawChunkList.Add(activeChunk);
                    preDrawChunkCount++;
                }            
            }


            if(_preDrawChunkTaskList.Count > 0)
            {
                //Debug.Log($"Pre-Draw last: {_preDrawChunkTaskList.Count}");
                await Task.WhenAll(_preDrawChunkTaskList);
                _preDrawChunkTaskList.Clear();
            }
           



            int drawChunkCount = 0;
            int maxChunkDrawInStage = 2;
            // Draw chunk
            for (int i = 0; i < _preDrawChunkList.Count; i++)
            {
                _drawChunkTaskList.Add(_worldGen.DrawChunkTask(_preDrawChunkList[i]));

                // Redraw fix ao artifact (Only need redraw chunk at ground level)
                if (_preDrawChunkList[i].FrameY == 0)
                {
                    if (!_redrawChunkSet.Contains(_preDrawChunkList[i].West) && _preDrawChunkList[i].West.HasDrawnFirstTime)
                        _redrawChunkSet.Add(_preDrawChunkList[i].West);
                    if (!_redrawChunkSet.Contains(_preDrawChunkList[i].East) && _preDrawChunkList[i].East.HasDrawnFirstTime)
                        _redrawChunkSet.Add(_preDrawChunkList[i].East);
                    if (!_redrawChunkSet.Contains(_preDrawChunkList[i].South) && _preDrawChunkList[i].South.HasDrawnFirstTime)
                        _redrawChunkSet.Add(_preDrawChunkList[i].South);
                    if (!_redrawChunkSet.Contains(_preDrawChunkList[i].North) && _preDrawChunkList[i].North.HasDrawnFirstTime)
                        _redrawChunkSet.Add(_preDrawChunkList[i].North);
                }
                

                if (drawChunkCount > maxChunkDrawInStage)
                {
                    drawChunkCount = 0;
                    await Task.WhenAll(_drawChunkTaskList);
                    _drawChunkTaskList.Clear();
                }
                drawChunkCount++;
            }



            if (_drawChunkTaskList.Count > 0)
            {
                Debug.Log($"Draw last: {_drawChunkTaskList.Count}");
                await Task.WhenAll(_drawChunkTaskList);
                _drawChunkTaskList.Clear();
            }


            // Redraw chunk fix ao artiface
            int redrawChunkCount = 0;
            int maxChunkRedrawInStage = 2;
            foreach (var c in _redrawChunkSet)
            {
                _redrawChunkTaskList.Add(_worldGen.ReDrawChunkTask(c));

                if(redrawChunkCount > maxChunkRedrawInStage)
                {
                    redrawChunkCount = 0;
                    await Task.WhenAll(_redrawChunkTaskList);
                    _redrawChunkTaskList.Clear();
                }
                redrawChunkCount++;
            }
            if(_redrawChunkTaskList.Count > 0)
            {
                await Task.WhenAll(_redrawChunkTaskList);
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

      
            _preDrawChunkTaskList.Clear();
            _preDrawChunkList.Clear();
            _redrawChunkSet.Clear();
            _redrawChunkTaskList.Clear();
            _unloadChunkList.Clear();
            _loadChunkList.Clear();
      
            _finishLoadChunk = true;
        }



        public void LoadChunk(Chunk chunk)
        {
            Vector3Int frame = new Vector3Int(chunk.FrameX, chunk.FrameY, chunk.FrameZ);
            //if (!_main.Chunks.ContainsKey(frame))
            //    _main.Chunks.Add(frame, chunk);
            _main.TryAddChunks(frame, chunk);
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

