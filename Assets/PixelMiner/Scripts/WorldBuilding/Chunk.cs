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
        // NEW
        [SerializeField] public MeshFilter SolidMeshFilter;
        [SerializeField] public MeshFilter WaterMeshFilter;

        private Transform _playerTrans;
        private const int DRAW_HIDDEN_BLOCK = 1 << 0;
        private const int DRAW_HIDDEN_SURFACE = 1 << 1;
        private int DRAW_PERMISSION = 0;

        public enum ChunkState
        {
            Init, Processing, Stable
        }
        public ChunkState State;


        public int FrameX;
        public int FrameY;
        public int FrameZ;
        public byte _width;
        public byte _height;
        public byte _depth;
        public Vector3Int Dimensions;
        private float _unloadChunkDistance = 100;
        private float _updateFrequency = 1.0f;
        private float _updateTimer = 0.0f;
        public bool ChunkHasDrawn = false;

        bool[] _neighbors;
        // Neighbors
        [ShowInInspector] public Chunk Left { get; private set; }
        [ShowInInspector] public Chunk Right { get; private set; }
        [ShowInInspector] public Chunk Front { get; private set; }
        [ShowInInspector] public Chunk Back { get; private set; }

        [Header("Data")]
        [HideInInspector] public BlockType[] ChunkData;
        [HideInInspector] public HeatType[] HeatData;
        [HideInInspector] public MoistureType[] MoistureData;
        [HideInInspector] public float[] HeightValues;
        [HideInInspector] public float[] HeatValues;
        [HideInInspector] public float[] MoistureValues;

        private BlockType[] WaterTypes = new BlockType[] { BlockType.Water };

        private void Start()
        {
            // Set player for detect unload chunk when player far away.
            _playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
            if (_playerTrans == null)
                _playerTrans = Camera.main.transform;

            _neighbors = new bool[6];
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

            ChunkData = new BlockType[size3D];
            HeatData = new HeatType[size3D];
            MoistureData = new MoistureType[size3D];
            HeightValues = new float[size2D];
            HeatValues = new float[size2D];
            MoistureValues = new float[size2D];
            Dimensions = new Vector3Int(_width, _height, _depth);

            State = ChunkState.Stable;
        }


        public bool IsSolid(Vector3Int position)
        {
            BlockType block = GetBlock(position);
            return block != BlockType.Air && block != BlockType.Water;
        }
        public bool IsWater(Vector3Int position)
        {
            BlockType block = GetBlock(position);
            return block == BlockType.Water;
        }


        public BlockType GetBlock(byte x, byte y, byte z)
        {
            return ChunkData[IndexOf(x, y, z)];
        }
        public BlockType GetBlock(Vector3Int position)
        {
            if (position.x < 0 || position.x >= Dimensions[0] ||
                position.y < 0 || position.y >= Dimensions[1] ||
                position.z < 0 || position.z >= Dimensions[2])
            {

                if (position.x < 0)
                {
                    return Left.ChunkData[IndexOf(_width - 1, position.y, position.z)]; 
                }
                if (position.x >= _width)
                {
                    return Right.ChunkData[IndexOf(0, position.y, position.z)];
                }

                if (position.z < 0)
                {
                    return Back.ChunkData[IndexOf(position.x, position.y, _depth - 1)];
                }
                if (position.z >= _depth)
                {
                    return Front.ChunkData[IndexOf(position.x, position.y, 0)];
                }

                return BlockType.Air;
            }

            return ChunkData[IndexOf(position.x, position.y, position.z)];
        }
        public bool IsBlockFaceVisible(Vector3Int position, int dimension, bool isBackFace)
        {
            position[dimension] += isBackFace ? -1 : 1;
            return IsSolid(position) == false;
        }
        public bool IsWaterFaceVisible(Vector3Int position, int dimension, bool isBackFace)
        {
            position[dimension] += isBackFace ? -1 : 1;
            return IsWater(position) == false;
        }

        public async void DrawChunkAsync()
        {
            if (ChunkHasDrawn) return;

            var solidMeshData = await MeshUtils.SolidGreedyMeshingAsync(this);
            var waterMeshData = await MeshUtils.WaterGreedyMeshingAsync(this);

            SolidMeshFilter.ApplyMeshData(solidMeshData);
            WaterMeshFilter.ApplyMeshData(waterMeshData);

            //List<Quad> quads = await GetSolidChunkMeshDataAsync02();
            //SolidMeshFilter.sharedMesh = await MeshUtils.MergeLargeMeshDataAsyncParallel(quads, ChunkData);
            //for (int i = 0; i < quads.Count; i++)
            //{
            //    QuadPool.Release(quads[i]);
            //}

            //ChunkMeshData solidChunkMeshData = await GetSolidChunkMeshDataAsync();
            //ChunkMeshData fluidChunkMeshData = await GetFluidChunkMeshDataAsync();

            //SolidMeshFilter.sharedMesh = await MeshUtils.MergeLargeMeshDataAsyncParallel(solidChunkMeshData);
            //WaterMeshFilter.sharedMesh = await MeshUtils.MergeLargeMeshDataAsyncParallel(fluidChunkMeshData);

            //ChunkMeshDataPool.Release(solidChunkMeshData);
            //ChunkMeshDataPool.Release(fluidChunkMeshData);

            //var solidCollider = _solidMeshFilter.gameObject.AddComponent<MeshCollider>();
            //solidCollider.sharedMesh = _solidMeshFilter.mesh;

            ChunkHasDrawn = true;
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

            if ((DRAW_PERMISSION & DRAW_HIDDEN_SURFACE) != 0)
            {
                if (x < 0 || x >= _width || y < 0 || y >= _height || z < 0 || z >= _depth)
                {
                    return false;
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
            else
            {
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


        public void GetSolidBlockNeighbors(int x, int y, int z, bool[] neighbors)
        {
            if ((DRAW_PERMISSION & DRAW_HIDDEN_BLOCK) != 0)
            {
                for (int i = 0; i < 6; i++)
                    neighbors[i] = false;
            }
            else
            {
                for (int i = 0; i < 6; i++)
                    neighbors[i] = true;
                if (!BlockHasSolidNeighbors(x, y + 1, z))
                {
                    _neighbors[0] = false;
                }
                if (!BlockHasSolidNeighbors(x, y - 1, z))
                {
                    _neighbors[1] = false;
                }
                if (!BlockHasSolidNeighbors(x, y, z + 1))
                {
                    _neighbors[2] = false;
                }
                if (!BlockHasSolidNeighbors(x, y, z - 1))
                {
                    _neighbors[3] = false;
                }
                if (!BlockHasSolidNeighbors(x - 1, y, z))
                {
                    _neighbors[4] = false;
                }
                if (!BlockHasSolidNeighbors(x + 1, y, z))
                {
                    _neighbors[5] = false;
                }
            }
        }

        public void GetFluidBlockNeighbors(int x, int y, int z, bool[] neighbors)
        {
            for (int i = 0; i < neighbors.Length; i++)
            {
                neighbors[i] = true;
            }
            if (!BlockHasFuildNeighbors(x, y + 1, z))
            {
                neighbors[0] = false;
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
