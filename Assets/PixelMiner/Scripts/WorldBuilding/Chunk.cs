using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using PixelMiner.Enums;
using PixelMiner.Utilities;
using PixelMiner.DataStructure;
using Sirenix.OdinInspector;

namespace PixelMiner.WorldBuilding
{
    [SelectionBase]
    public class Chunk : MonoBehaviour
    {
        public static System.Action<Chunk> OnChunkFarAway;
        public static System.Action<Chunk> OnChunkHasNeighbors;

        // OLD
        public int FrameX;
        public int FrameY;
        public int FrameZ;

        public bool HasChunkNeighbors = false;
        public bool AllTileHasNeighbors = false;
        public bool ChunkHasDrawn = false;
        public bool Processing { get; set; } = false;

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
        [SerializeField] private MeshFilter _solidMeshFilter;
        [SerializeField] private MeshFilter _fluidMeshFilter;

        public byte Width;
        public byte Height;
        public byte Depth;

        // Data
        [HideInInspector] public Block[,,] Blocks;  // Mesh (create unity mesh)
        public BlockType[] ChunkData; // Block Data (Define block type)
        [HideInInspector] public HeatType[] HeatData;
        [HideInInspector] public MoistureType[] MoistureData;
        [HideInInspector] public float[] HeightValues;
        [HideInInspector] public float[] HeatValues;
        [HideInInspector] public float[] MoistureValues;

        private Transform _playerTrans;

        private void Awake()
        {


        }

        private void Start()
        {
            // Set player for detect unload chunk when player far away.
            _playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
            if (_playerTrans == null)
                _playerTrans = Camera.main.transform;

            _solidNeighbors = new bool[6];
        }

        private void Update()
        {
            if (Time.time - _updateTimer > _updateFrequency)
            {
                _updateTimer = Time.time;
                if (Vector3.Distance(_playerTrans.position, transform.position) > _unloadChunkDistance
                    && Processing == false)
                {
                    OnChunkFarAway?.Invoke(this);
                }
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                if (HasNeighbors())
                    DrawChunkAsync();
            }
        }

        public void Init(int frameX, int frameY, int frameZ, byte width, byte height, byte depth)
        {
            Processing = true;

            // Set properties
            this.FrameX = frameX;
            this.FrameY = frameY;
            this.FrameZ = frameZ;
            this.Width = width;
            this.Height = height;
            this.Depth = depth;

            // Init data
            int size3D = Width * Height * Depth;
            int size2D = Width * Depth;
            Blocks = new Block[Width, Height, Depth];
            ChunkData = new BlockType[size3D];
            HeatData = new HeatType[size3D];
            MoistureData = new MoistureType[size3D];
            HeightValues = new float[size2D];
            HeatValues = new float[size2D];
            MoistureValues = new float[size2D];


            Processing = false;
        }





        public async void DrawChunkAsync()
        {
            Processing = true;
            ChunkHasDrawn = true;

            //await GetSolidMeshDataAsync();
            //return;
            List<MeshData> solidMeshDataList = await GetSolidMeshDataAsync();       
            //List<MeshData> fluidMeshDataList = await GetFluidMeshDataAsync();

            _solidMeshFilter.mesh = await MeshUtils.MergeMeshAsyncParallel(solidMeshDataList.ToArray());
            //_fluidMeshFilter.mesh = await MeshUtils.MergeMeshAsyncParallel(fluidMeshDataList.ToArray());

            //var solidCollider = _solidMeshFilter.gameObject.AddComponent<MeshCollider>();
            //solidCollider.sharedMesh = _solidMeshFilter.mesh;


            //_solidMeshFilter.mesh = MeshUtils.MergeMeshTesting(solidMeshDataList.ToArray());
            //_fluidMeshFilter.mesh = MeshUtils.MergeMeshTesting(fluidMeshDataList.ToArray());

            Processing = false;
        }


        private async Task<List<MeshData>> GetSolidMeshDataAsync()
        {
            List<MeshData> solidMeshDataList = new List<MeshData>();
            await Task.Run(() =>
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int z = 0; z < Depth; z++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            BlockType blockType = ChunkData[IndexOf(x, y, z)];
                            if (blockType != BlockType.Water)
                            {
                                bool[] solidNeighbors = GetSolidBlockNeighbors(x, y, z, solidNB: _solidNeighbors);
                                Blocks[x, y, z] = BlockPool.Get();

                                //Blocks[x, y, z] = new Block();
                                Blocks[x, y, z].DrawSolid(ChunkData[IndexOf(x, y, z)], solidNeighbors, new Vector3(x, y, z));
                                if (Blocks[x, y, z].MeshDataArray != null)
                                {
                                    solidMeshDataList.AddRange(Blocks[x, y, z].MeshDataArray);
                                }

                                BlockPool.Release(Blocks[x, y, z]);
                            }
                        }
                    }
                }
            });
            return solidMeshDataList;
        }
        private async Task<List<MeshData>> GetFluidMeshDataAsync()
        {
            List<MeshData> fluidMeshDataList = new List<MeshData>();
            await Task.Run(() =>
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int z = 0; z < Depth; z++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            BlockType blockType = ChunkData[IndexOf(x, y, z)];
                            if (blockType == BlockType.Water)
                            {
                                bool[] fluidNeighbors = GetFluidBlockNeighbors(x, y, z);
                                Blocks[x, y, z] = new Block();
                                Blocks[x, y, z].DrawFluid(ChunkData[IndexOf(x, y, z)], fluidNeighbors, HeightValues[IndexOf(x, z)], new Vector3(x, y, z));
                                if (Blocks[x, y, z].MeshDataArray != null)
                                {
                                    fluidMeshDataList.AddRange(Blocks[x, y, z].MeshDataArray);
                                }
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
            if (y < 0)
                return true;    
            if (y >= Height)
                return false;

            //if (x < 0 || x >= Width || z < 0 || z >= Depth)
            //    return false;

            if (x < 0)
            {
                var leftNBBType = Left.ChunkData[IndexOf(Width - 1, y, z)];
                if (leftNBBType == BlockType.Air || leftNBBType == BlockType.Water)
                    return false;
                else
                    return true;
            }
            if (x >= Width)
            {
                var rightNBBType = Right.ChunkData[IndexOf(0, y, z)];
                if (rightNBBType == BlockType.Air || rightNBBType == BlockType.Water)
                    return false;
                else
                    return true;
            }

            if (z < 0)
            {
                var backNBBType = Back.ChunkData[IndexOf(x, y, Depth - 1)];
                if (backNBBType == BlockType.Air || backNBBType == BlockType.Water)
                    return false;
                else
                    return true;
            }
            if (z >= Depth)
            {
                var frontNBBType = Front.ChunkData[IndexOf(x, y, 0)];
                if (frontNBBType == BlockType.Air || frontNBBType == BlockType.Water)
                    return false;
                else
                    return true;
            }

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
            if (x < 0 || x >= Width || y < 0 || y >= Height || z < 0 || z >= Depth)
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
            return x + Width * (y + Height * z);
        }
        private int IndexOf(int x, int z)
        {
            return x + z * Depth;
        }
        #endregion
    }
}
