using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using PixelMiner.Enums;
using PixelMiner.DataStructure;
using Sirenix.OdinInspector;


namespace PixelMiner.WorldBuilding
{
    [SelectionBase]
    public class Chunk : MonoBehaviour
    {
        public static System.Action<Chunk> OnChunkFarAway;
        public static System.Action<Chunk> OnChunkHasNeighbors;

        public enum ChunkState
        {
            Init, Processing, Stable
        }
        public ChunkState State;
        public bool HasData = false;

        // OLD
        public int FrameX;
        public int FrameY;
        public int FrameZ;


        public bool ChunkHasDrawn = false;


        private float _unloadChunkDistance = 100;
        private float _updateFrequency = 1.0f;
        private float _updateTimer = 0.0f;


        bool[] _solidNeighbors;
        // Neighbors
        [ShowInInspector] public Chunk Left { get; private set; }
        [ShowInInspector] public Chunk Right { get; private set; }
        [ShowInInspector] public Chunk Front { get; private set; }
        [ShowInInspector] public Chunk Back { get; private set; }



        // NEW
        [SerializeField] public MeshFilter SolidMeshFilter;
        [SerializeField] public MeshFilter WaterMeshFilter;

        public byte _width;
        public byte _height;
        public byte _depth;


        [HideInInspector] public BlockType[] ChunkData;
        [HideInInspector] public HeatType[] HeatData;
        [HideInInspector] public MoistureType[] MoistureData;
        [HideInInspector] public float[] HeightValues;
        [HideInInspector] public float[] HeatValues;
        [HideInInspector] public float[] MoistureValues;

        private Transform _playerTrans;


        private void Start()
        {
            // Set player for detect unload chunk when player far away.
            _playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
            if (_playerTrans == null)
                _playerTrans = Camera.main.transform;

            _solidNeighbors = new bool[6];
        }

        private void OnDestroy()
        {
            //ChunkData = null;
            //HeatData = null;
            //MoistureData = null;
            //HeightValues = null;
            //HeatValues = null;
            //MoistureValues = null;
            //_solidNeighbors = null;

            Object.Destroy(SolidMeshFilter.sharedMesh);
            Object.Destroy(WaterMeshFilter.sharedMesh);
            //Destroy(SolidMeshFilter);
            //Destroy(WaterMeshFilter);
        }

        private void Update()
        {
            if (Time.time - _updateTimer > _updateFrequency)
            {
                _updateTimer = Time.time;

                if (Vector3.Distance(_playerTrans.position, transform.position) > _unloadChunkDistance
                    && State == ChunkState.Stable)
                {
                    OnChunkFarAway?.Invoke(this);
                }
            }
        }

        public void Init(int frameX, int frameY, int frameZ, byte width, byte height, byte depth)
        {
            State = ChunkState.Init;

            // Set properties
            this.FrameX = frameX;
            this.FrameY = frameY;
            this.FrameZ = frameZ;
            this._width = width;
            this._height = height;
            this._depth = depth;

            // Init data
            int size3D = _width * _height * _depth;
            int size2D = _width * _depth;
            //Blocks = new Block[Width, Height, Depth];
            ChunkData = new BlockType[size3D];
            HeatData = new HeatType[size3D];
            MoistureData = new MoistureType[size3D];
            HeightValues = new float[size2D];
            HeatValues = new float[size2D];
            MoistureValues = new float[size2D];

            State = ChunkState.Stable;
        }



        public async void DrawChunkAsync()
        {      
            if (ChunkHasDrawn) return;
    
  
            ChunkMeshData chunkMeshData = await GetSolidLargeMeshDataAsync();
            SolidMeshFilter.sharedMesh = await MeshUtils.MergeLargeMeshDataAsyncParallel(chunkMeshData);
            ChunkMeshDataPool.Release(chunkMeshData);

            //return;
            //List<MeshData> solidMeshDataList = await GetSolidMeshDataAsync();       
            //List<MeshData> fluidMeshDataList = await GetFluidMeshDataAsync();

            //_solidMeshFilter.mesh = await MeshUtils.MergeMeshAsyncParallel(solidMeshDataList.ToArray());
            //_fluidMeshFilter.mesh = await MeshUtils.MergeMeshAsyncParallel(fluidMeshDataList.ToArray());

            //var solidCollider = _solidMeshFilter.gameObject.AddComponent<MeshCollider>();
            //solidCollider.sharedMesh = _solidMeshFilter.mesh;


            //_solidMeshFilter.mesh = MeshUtils.MergeMeshTesting(solidMeshDataList.ToArray());
            //_fluidMeshFilter.mesh = MeshUtils.MergeMeshTesting(fluidMeshDataList.ToArray());

            ChunkHasDrawn = true;
        }


        private async Task<List<MeshData>> GetSolidMeshDataAsync()
        {
            List<MeshData> solidMeshDataList = new List<MeshData>();
            await Task.Run(() =>
            {
                for (int x = 0; x < _width; x++)
                {
                    for (int z = 0; z < _depth; z++)
                    {
                        for (int y = 0; y < _height; y++)
                        {
                            BlockType blockType = ChunkData[IndexOf(x, y, z)];
                            if (blockType != BlockType.Water)
                            {
                                //bool[] solidNeighbors = GetSolidBlockNeighbors(x, y, z, solidNB: _solidNeighbors);
                                //Blocks[x, y, z] = BlockPool.Get();
                                
                                //Blocks[x, y, z].DrawSolid(ChunkData[IndexOf(x, y, z)], solidNeighbors, new Vector3(x, y, z));
                                //if (Blocks[x, y, z].MeshDataList.Count > 0)
                                //{
                                //    solidMeshDataList.AddRange(Blocks[x, y, z].MeshDataList);
                                //}

                                //BlockPool.Release(Blocks[x, y, z]);
                            }
                        }
                    }
                }
            });
            return solidMeshDataList;
        }
        private async Task<ChunkMeshData> GetSolidLargeMeshDataAsync()
        {
            ChunkMeshData largeMeshData = ChunkMeshDataPool.Get();
            await Task.Run(() =>
            {
                for (int x = 0; x < _width; x++)
                {
                    for (int z = 0; z < _depth; z++)
                    {
                        for (int y = 0; y < _height; y++)
                        {
                            BlockType blockType = ChunkData[IndexOf(x, y, z)];
                            if (blockType != BlockType.Water)
                            {
                                GetSolidBlockNeighbors(x, y, z, solidNB: _solidNeighbors);
                                Block block = BlockPool.Get();

                                block.DrawSolid(ChunkData[IndexOf(x, y, z)], _solidNeighbors, new Vector3(x, y, z));


                                if (block.QuadCount > 0)
                                {
                                    largeMeshData.AddData(block);
                                }

                                BlockPool.Release(block);
                            }
                        }
                    }
                }
            });
            return largeMeshData;
        }

        private async Task<List<MeshData>> GetFluidMeshDataAsync()
        {
            List<MeshData> fluidMeshDataList = new List<MeshData>();
            await Task.Run(() =>
            {
                for (int x = 0; x < _width; x++)
                {
                    for (int z = 0; z < _depth; z++)
                    {
                        for (int y = 0; y < _height; y++)
                        {
                            BlockType blockType = ChunkData[IndexOf(x, y, z)];
                            if (blockType == BlockType.Water)
                            {
                                //bool[] fluidNeighbors = GetFluidBlockNeighbors(x, y, z);
                                //Blocks[x, y, z] = new Block();
                                //Blocks[x, y, z].DrawFluid(ChunkData[IndexOf(x, y, z)], fluidNeighbors, HeightValues[IndexOf(x, z)], new Vector3(x, y, z));
                                //if (Blocks[x, y, z].MeshDataList != null)
                                //{
                                //    fluidMeshDataList.AddRange(Blocks[x, y, z].MeshDataList);
                                //}
                            }

                        }
                    }
                }
            });

            return fluidMeshDataList;
        }



        #region Block
        private bool IsOnEdge(int x, int y, int z, int width, int height, int depth)
        {
            // Check if the position is on any edge
            bool onXEdge = x == 0 || x == width - 1;
            bool onYEdge = y == 0 || y == height - 1;
            bool onZEdge = z == 0 || z == depth - 1;

            // Return true if on any edge
            return onXEdge || onYEdge || onZEdge;
        }
        public bool BlockHasSolidNeighbors(int x, int y, int z)
        {
            if (!HasNeighbors()) return false;

            

 
            if (x < 0)
            {
                var leftNBBType = Left.ChunkData[IndexOf(_width - 1, y, z)];
                if (leftNBBType == BlockType.Air || leftNBBType == BlockType.Water)
                    return false;
                else
                    return true;
            }
            if (x >= _width)
            {
                var rightNBBType = Right.ChunkData[IndexOf(0, y, z)];
                if (rightNBBType == BlockType.Air || rightNBBType == BlockType.Water)
                    return false;
                else
                    return true;
            }

            if (z < 0)
            {
                var backNBBType = Back.ChunkData[IndexOf(x, y, _depth - 1)];
                if (backNBBType == BlockType.Air || backNBBType == BlockType.Water)
                    return false;
                else
                    return true;
            }
            if (z >= _depth)
            {
                var frontNBBType = Front.ChunkData[IndexOf(x, y, 0)];
                if (frontNBBType == BlockType.Air || frontNBBType == BlockType.Water)
                    return false;
                else
                    return true;
            }

            if (y < 0)
                return true;
            if (y >= _height)
                return false;

            BlockType blockType = ChunkData[IndexOf(x, y, z)];
            switch (blockType)
            {
                case BlockType.Water:
                case BlockType.Air:
                    return false;
                default:
                    return true;
            }

         
        }

        public bool BlockHasFuildNeighbors(int x, int y, int z)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height || z < 0 || z >= _depth)
            {
                return false;
            }
            BlockType blockType = ChunkData[IndexOf(x, y, z)];

            switch (blockType)
            {
                case BlockType.Water:
                    return true;
                default:
                    return false;
            }
        }

     
        public bool[] GetSolidBlockNeighbors(int x, int y, int z, bool[] solidNB)
        {
            //bool[] solidNeighbors = new bool[6] { true, true, true, true, true, true }; // maximum 6 neighbors [ Top, Bottom, Front , Back, Left, Right]
            // Reset
            for (int i = 0; i < 6; i++)
                solidNB[i] = true;
            
            if (!BlockHasSolidNeighbors(x, y + 1, z))
            {
                _solidNeighbors[0] = false;
            }
            if (!BlockHasSolidNeighbors(x, y - 1, z))
            {
                _solidNeighbors[1] = false;
            }
            if (!BlockHasSolidNeighbors(x, y, z + 1))
            {
                _solidNeighbors[2] = false;
            }
            if (!BlockHasSolidNeighbors(x, y, z - 1))
            {
                _solidNeighbors[3] = false;
            }
            if (!BlockHasSolidNeighbors(x - 1, y, z))
            {
                _solidNeighbors[4] = false;
            }
            if (!BlockHasSolidNeighbors(x + 1, y, z))
            {
                _solidNeighbors[5] = false;
            }

            return _solidNeighbors;
        }

        public bool[] GetFluidBlockNeighbors(int x, int y, int z)
        {
            bool[] fluidNeighbors = new bool[6] { true, true, true, true, true, true }; // maximum 6 neighbors [ Top, Bottom, Front , Back, Left, Right]

            if (!BlockHasFuildNeighbors(x, y + 1, z))
            {
                fluidNeighbors[0] = false;
            }
            //if (!BlockHasFuildNeighbors(x, y - 1, z))
            //{
            //    fluidNeighbors[1] = false;
            //}
            //if (!BlockHasFuildNeighbors(x, y, z + 1))
            //{
            //    fluidNeighbors[2] = false;
            //}
            //if (!BlockHasFuildNeighbors(x, y, z - 1))
            //{
            //    fluidNeighbors[3] = false;
            //}
            //if (!BlockHasFuildNeighbors(x - 1, y, z))
            //{
            //    fluidNeighbors[4] = false;
            //}
            //if (!BlockHasFuildNeighbors(x + 1, y, z))
            //{
            //    fluidNeighbors[5] = false;
            //}

            return fluidNeighbors;
        }

        public void SetNeighbors(BlockSide side, Chunk neighbour)
        {
            switch (side)
            {
                default:
                    throw new System.Exception();
                case BlockSide.Left:
                    if (Left == null)
                    {
                        Left = neighbour;
                    }
                    break;
                case BlockSide.Right:
                    if (Right == null)
                    {
                        Right = neighbour;
                    }
                    break;
                case BlockSide.Front:
                    if (Front == null)
                    {
                        Front = neighbour;
                    }
                    break;
                case BlockSide.Back:
                    if (Back == null)
                    {
                        Back = neighbour;
                    }
                    break;
            }

            if (HasNeighbors())
            {
                OnChunkHasNeighbors?.Invoke(this);
            }
        }

        public bool HasNeighbors()
        {
            return Left != null && Right != null && Front != null && Back != null;
        }

        #endregion

        #region Utilities
        private int IndexOf(int x, int y, int z)
        {
            return x + _width * (y + _height * z);
        }
        private int IndexOf(int x, int z)
        {
            return x + z * _depth;
        }
        #endregion
    }
}
