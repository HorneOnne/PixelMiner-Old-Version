using UnityEngine;
using PixelMiner.Enums;
using Sirenix.OdinInspector;
using System.Collections.Generic;


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
        private MeshCollider _meshCollider;

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
        public int _width;
        public int _height;
        public int _depth;
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
        [HideInInspector] public BiomeType[] BiomesData;
        [HideInInspector] public byte[] LightData;

        private Vector3Int[] _neighborsPosition = new Vector3Int[6];

        private void Awake()
        {
            _meshCollider = SolidMeshFilter.GetComponent<MeshCollider>();
        }

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

        public void Init(int frameX, int frameY, int frameZ, int width, int height, int depth)
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
            ChunkData = new BlockType[size3D];
            HeatData = new HeatType[size3D];
            MoistureData = new MoistureType[size3D];
            BiomesData = new BiomeType[size3D];
            Dimensions = new Vector3Int(_width, _height, _depth);
            LightData = new byte[size3D];


            for(int i = 0;i < LightData.Length; i++)
            {
                LightData[i] = 0;
            }

           

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

                if (position.x == -1)
                {
                    return Left.ChunkData[IndexOf(_width - 1, position.y, position.z)];
                }
                if (position.x == _width)
                {
                    return Right.ChunkData[IndexOf(0, position.y, position.z)];
                }

                if (position.z == -1)
                {
                    return Back.ChunkData[IndexOf(position.x, position.y, _depth - 1)];
                }
                if (position.z == _depth)
                {
                    return Front.ChunkData[IndexOf(position.x, position.y, 0)];
                }

                return BlockType.Air;
            }

            return ChunkData[IndexOf(position.x, position.y, position.z)];
        }
        public void SetBlock(Vector3Int globalPosition, BlockType blockType)
        {
            int x = globalPosition[0] % Dimensions[0];
            int y = globalPosition[1] % Dimensions[1];
            int z = globalPosition[2] % Dimensions[2];
            ChunkData[IndexOf(x, y, z)] = blockType;
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

            MeshData solidMeshData = await MeshUtils.SolidGreedyMeshingAsync(this);
            //MeshData waterMeshData = await MeshUtils.WaterGreedyMeshingAsync(this);
            MeshData colliderMeshData = await MeshUtils.SolidGreedyMeshingForColliderAsync(this);

            SolidMeshFilter.sharedMesh = CreateMesh(solidMeshData);
            //WaterMeshFilter.sharedMesh =  CreateMesh(waterMeshData);

            _meshCollider.sharedMesh = null;
            _meshCollider.sharedMesh = CreateColliderMesh(colliderMeshData);


            // Release mesh data
            MeshDataPool.Release(solidMeshData);
            //MeshDataPool.Release(waterMeshData);
            MeshDataPool.Release(colliderMeshData);

            //LogUtils.WriteMeshToFile(SolidMeshFilter.sharedMesh, "Meshdata.txt");
            ChunkHasDrawn = true;
        }


        public async void ReDrawChunkAsync()
        {
            MeshData solidMeshData = await MeshUtils.SolidGreedyMeshingAsync(this);
            MeshData colliderMeshData = await MeshUtils.SolidGreedyMeshingForColliderAsync(this);

            SolidMeshFilter.sharedMesh = CreateMesh(solidMeshData);

            _meshCollider.sharedMesh = null;
            _meshCollider.sharedMesh = CreateColliderMesh(colliderMeshData);


            // Release mesh data
            MeshDataPool.Release(solidMeshData);
            MeshDataPool.Release(colliderMeshData);
        }

    
        public Mesh CreateMesh(MeshData meshData)
        {
            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;
            mesh.SetVertices(meshData.Vertices);
            mesh.SetColors(meshData.Colors);
            mesh.SetTriangles(meshData.Triangles, 0);
            mesh.SetUVs(0, meshData.UVs);
            mesh.SetUVs(1, meshData.UV2s);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
        private Mesh CreateColliderMesh(MeshData meshData)
        {
            Mesh colliderMesh = new Mesh();
            colliderMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;
            colliderMesh.SetVertices(meshData.Vertices);
            colliderMesh.SetTriangles(meshData.Triangles, 0);
            return colliderMesh;
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




        #region Light
        public byte GetLight(int x, int y, int z)
        {
            return LightData[IndexOf(x,y,z)];
        }
        public byte GetLight(Vector3Int position)
        {
            if (position.x < 0 || position.x >= Dimensions[0] ||
                 position.y < 0 || position.y >= Dimensions[1] ||
                 position.z < 0 || position.z >= Dimensions[2])
            {
                if(position.x == _width && position.z == _depth)
                {
                    //Debug.Log("Top Right");
                    return 0;
                }
                else if(position.x < 0 && position.z == Dimensions[2])
                {
                    //Debug.Log("Top left");
                    return 0;
                }
                else if (position.x == Dimensions[0] && position.z == -1)
                {
                    //Debug.Log("Bottom right");
                    return 0;
                }


                if (position.x < 0)
                {
                    return Left.LightData[IndexOf(_width - 1, position.y, position.z)];
                }
                if (position.x == _width)
                {
                    return Right.LightData[IndexOf(0, position.y, position.z)];
                }

                if (position.z < 0)
                {
                    return Back.LightData[IndexOf(position.x, position.y, _depth - 1)];
                }
                if (position.z == _depth)
                {
                    return Front.LightData[IndexOf(position.x, position.y, 0)];
                }

                if (position.y < 0 || position.y >= Dimensions[1])
                {
                    return 0;
                }

                return 0;
            }

            return LightData[IndexOf(position.x, position.y, position.z)];
        }


        public void SetLight(int x, int y, int z, byte intensity)
        {
            LightData[IndexOf(x,y,z)] = intensity;
        }
        public void SetLight(Vector3Int position, byte intensity)
        {

            if (position.x < 0 || position.x >= Dimensions[0] ||
                 position.y < 0 || position.y >= Dimensions[1] ||
                 position.z < 0 || position.z >= Dimensions[2])
            {

                if (position.x < 0)
                {
                    Left.LightData[IndexOf(_width - 1, position.y, position.z)] = intensity;
                    return;
                }
                if (position.x >= _width)
                {
                    Right.LightData[IndexOf(0, position.y, position.z)] = intensity;
                    return;
                }

                if (position.z < 0)
                {
                    Back.LightData[IndexOf(position.x, position.y, _depth - 1)] = intensity;
                    return;
                }
                if (position.z >= _depth)
                {
                    Front.LightData[IndexOf(position.x, position.y, 0)] = intensity;
                    return;
                }

                return;
            }

            LightData[IndexOf(position.x, position.y, position.z)] = intensity;
        }

        public async void RedrawLightAsync()
        {
            MeshData solidMeshData = await MeshUtils.SolidGreedyMeshingAsync(this);
            SolidMeshFilter.sharedMesh = CreateMesh(solidMeshData);
            MeshDataPool.Release(solidMeshData);
        }
        #endregion



        #region Neighbors
        public Vector3Int[] GetVoxelNeighborPosition(Vector3Int position)
        {
            _neighborsPosition[0] = position + new Vector3Int(1, 0, 0);
            _neighborsPosition[1] = position + new Vector3Int(-1, 0, 0);
            _neighborsPosition[2] = position + new Vector3Int(0, 0, 1);
            _neighborsPosition[3] = position + new Vector3Int(0, 0, -1);
            _neighborsPosition[4] = position + new Vector3Int(0, 1, 0);
            _neighborsPosition[5] = position + new Vector3Int(0, -1, 0);
           
            return _neighborsPosition;
        }

        #endregion

        #region Utilities
        public int IndexOf(int x, int y, int z)
        {
            return x + _width * (y + _height * z);
        }
        public int IndexOf(int x, int z)
        {
            return x + z * _width;
        }
        #endregion
    }
}
