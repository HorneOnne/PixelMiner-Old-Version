using UnityEngine;
using PixelMiner.Enums;


namespace PixelMiner.World
{
    [SelectionBase]
    public class Chunk : MonoBehaviour
    {
        public static System.Action<Chunk> OnChunkFarAway;
        public static System.Action<Chunk> OnChunkHasNeighbors;

        [SerializeField] public MeshFilter SolidMeshFilter;
        [SerializeField] public MeshFilter WaterMeshFilter;
        public MeshCollider MeshCollider;

        private Transform _playerTrans;


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

        // Neighbors
        [field: SerializeField] public Chunk West { get; set; }
        [field: SerializeField] public Chunk East { get; set; }
        [field: SerializeField] public Chunk North { get; set; }
        [field: SerializeField] public Chunk South { get; set; }
        [field: SerializeField] public Chunk Northwest { get; set; }
        [field: SerializeField] public Chunk Northeast { get; set; }
        [field: SerializeField] public Chunk Southwest { get; set; }
        [field: SerializeField] public Chunk Southeast { get; set; }

        [Header("Data")]
        [HideInInspector] public BlockType[] ChunkData;
        [HideInInspector] public HeatType[] HeatData;
        [HideInInspector] public MoistureType[] MoistureData;
        [HideInInspector] public BiomeType[] BiomesData;
        [HideInInspector] public byte[] VoxelLightData;
        [HideInInspector] public byte[] AmbientLightData;

        public AnimationCurve LightAnimCurve;
    

        private void Awake()
        {
            MeshCollider = SolidMeshFilter.GetComponent<MeshCollider>();
        }

        private void Start()
        {
            // Set player for detect unload chunk when player far away.
            _playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
            if (_playerTrans == null)
                _playerTrans = Camera.main.transform;
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
            VoxelLightData = new byte[size3D];
            AmbientLightData = new byte[size3D];



            // Set all light dark by default
            for (int i = 0; i < VoxelLightData.Length; i++)
            {
                VoxelLightData[i] = 0;
            }

            for (int i = 0; i < AmbientLightData.Length; i++)
            {
                AmbientLightData[i] = 0;
            }


            State = ChunkState.Stable;
        }


        public bool IsSolid(Vector3Int relativePosition)
        {
            BlockType block = GetBlock(relativePosition);
            return block != BlockType.Air && block != BlockType.Water;
        }
        public bool IsWater(Vector3Int relativePosition)
        {
            BlockType block = GetBlock(relativePosition);
            return block == BlockType.Water;
        }


        public BlockType GetBlock(Vector3Int relativePosition)
        {
            if (IsValidRelativePosition(relativePosition))
            {
                return ChunkData[IndexOf(relativePosition.x, relativePosition.y, relativePosition.z)];
            }
            else
            {
                if (relativePosition.x == _width && relativePosition.z == _depth)
                {
                    return Northeast.ChunkData[IndexOf(0, relativePosition.y, 0)];
                }
                if (relativePosition.x == -1 && relativePosition.z == _depth)
                {
                    return Northwest.ChunkData[IndexOf(_width - 1, relativePosition.y, 0)];
                }
                if (relativePosition.x == _width && relativePosition.z == -1)
                {
                    return Southeast.ChunkData[IndexOf(0, relativePosition.y, _depth - 1)];
                }
                if (relativePosition.x == -1 && relativePosition.z == -1)
                {
                    return Southwest.ChunkData[IndexOf(_width - 1, relativePosition.y, _depth - 1)];
                }


                if (relativePosition.y >= 0 && relativePosition.y < _height && relativePosition.z >= 0 && relativePosition.z < _depth)
                {
                    if (relativePosition.x == -1)
                    {
                        return West.ChunkData[IndexOf(_width - 1, relativePosition.y, relativePosition.z)];
                    }
                    if (relativePosition.x == _width)
                    {
                        return East.ChunkData[IndexOf(0, relativePosition.y, relativePosition.z)];
                    }
                }
                if (relativePosition.y >= 0 && relativePosition.y < _height && relativePosition.x >= 0 && relativePosition.x < _width)
                {
                    if (relativePosition.z == -1)
                    {
                        return South.ChunkData[IndexOf(relativePosition.x, relativePosition.y, _depth - 1)];
                    }
                    if (relativePosition.z == _depth)
                    {
                        return North.ChunkData[IndexOf(relativePosition.x, relativePosition.y, 0)];
                    }
                }

                if(relativePosition.x >= 0 && relativePosition.x < _width && relativePosition.z >= 0 && relativePosition.z < _depth)
                {
                    if(relativePosition.y == -1)
                    {
                        return ChunkData[IndexOf(relativePosition.x, 0, relativePosition.z)];
                    }
                    if(relativePosition.y == _height)
                    {
                        return ChunkData[IndexOf(relativePosition.x, _height - 1, relativePosition.z)];
                    }
                }
            }

            throw new System.Exception($"Currently we not calculate height of chunk. {relativePosition}");
        }
        public void SetBlock(Vector3Int relativePosition, BlockType blockType)
        {
            ChunkData[IndexOf(relativePosition.x, relativePosition.y, relativePosition.z)] = blockType;
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


        //public async void DrawChunkAsync()
        //{
        //    if (ChunkHasDrawn) return;

        //    MeshData solidMeshData = await MeshUtils.SolidGreedyMeshingAsync(this, LightAnimCurve);
        //    //MeshData waterMeshData = await MeshUtils.WaterGreedyMeshingAsync(this);
        //    MeshData colliderMeshData = await MeshUtils.SolidGreedyMeshingForColliderAsync(this);

        //    SolidMeshFilter.sharedMesh = CreateMesh(solidMeshData);
        //    //WaterMeshFilter.sharedMesh =  CreateMesh(waterMeshData);

        //    MeshCollider.sharedMesh = null;
        //    MeshCollider.sharedMesh = CreateColliderMesh(colliderMeshData);


        //    // Release mesh data
        //    MeshDataPool.Release(solidMeshData);
        //    //MeshDataPool.Release(waterMeshData);
        //    MeshDataPool.Release(colliderMeshData);

        //    //LogUtils.WriteMeshToFile(SolidMeshFilter.sharedMesh, "Meshdata.txt");
        //    ChunkHasDrawn = true;
        //}
        //public async void ReDrawChunkAsync()
        //{
        //    MeshData solidMeshData = await MeshUtils.SolidGreedyMeshingAsync(this, LightAnimCurve);
        //    MeshData colliderMeshData = await MeshUtils.SolidGreedyMeshingForColliderAsync(this);

        //    SolidMeshFilter.sharedMesh = CreateMesh(solidMeshData);

        //    MeshCollider.sharedMesh = null;
        //    MeshCollider.sharedMesh = CreateColliderMesh(colliderMeshData);

        //    // Release mesh data
        //    MeshDataPool.Release(solidMeshData);
        //    MeshDataPool.Release(colliderMeshData);
        //}
        //public async Task ReDrawChunkTask()
        //{
        //    MeshData solidMeshData = await MeshUtils.SolidGreedyMeshingAsync(this, LightAnimCurve);
        //    MeshData colliderMeshData = await MeshUtils.SolidGreedyMeshingForColliderAsync(this);

        //    SolidMeshFilter.sharedMesh = CreateMesh(solidMeshData);

        //    MeshCollider.sharedMesh = null;
        //    MeshCollider.sharedMesh = CreateColliderMesh(colliderMeshData);


        //    // Release mesh data
        //    MeshDataPool.Release(solidMeshData);
        //    MeshDataPool.Release(colliderMeshData);
        //}


        //public Mesh CreateMesh(MeshData meshData)
        //{
        //    Mesh mesh = new Mesh();
        //    mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;
        //    mesh.SetVertices(meshData.Vertices);
        //    mesh.SetColors(meshData.Colors);
        //    mesh.SetTriangles(meshData.Triangles, 0);
        //    mesh.SetUVs(0, meshData.UVs);
        //    mesh.SetUVs(1, meshData.UV2s);
        //    mesh.SetUVs(2, meshData.UV3s);
        //    mesh.RecalculateNormals();
        //    mesh.RecalculateBounds();

        //    return mesh;
        //}
        //private Mesh CreateColliderMesh(MeshData meshData)
        //{
        //    Mesh colliderMesh = new Mesh();
        //    colliderMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;
        //    colliderMesh.SetVertices(meshData.Vertices);
        //    colliderMesh.SetTriangles(meshData.Triangles, 0);
        //    return colliderMesh;
        //}




        public void SetNeighbors(BlockSide side, Chunk neighbour)
        {
            switch (side)
            {
                default:
                    throw new System.Exception();
                case BlockSide.Left:
                    if (West == null)
                    {
                        West = neighbour;
                    }
                    break;
                case BlockSide.Right:
                    if (East == null)
                    {
                        East = neighbour;
                    }
                    break;
                case BlockSide.Front:
                    if (North == null)
                    {
                        North = neighbour;
                    }
                    break;
                case BlockSide.Back:
                    if (South == null)
                    {
                        South = neighbour;
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
            return West != null && East != null && North != null && South != null &&
                   Northeast != null && Northwest != null && Southeast != null && Southwest != null;
        }




        #region Light
        public byte GetBlockLight(int x, int y, int z)
        {
            return VoxelLightData[IndexOf(x, y, z)];
        }
        public byte GetBlockLight(Vector3Int relativePosition)
        {
            if (IsValidRelativePosition(relativePosition))
            {
                return VoxelLightData[IndexOf(relativePosition.x, relativePosition.y, relativePosition.z)];
            }
            else
            {
                if (relativePosition.x == _width && relativePosition.z == _depth)
                {
                    return Northeast.VoxelLightData[IndexOf(0, relativePosition.y, 0)];
                }
                if (relativePosition.x == -1 && relativePosition.z == _depth)
                {
                    return Northwest.VoxelLightData[IndexOf(_width - 1, relativePosition.y, 0)];
                }
                if (relativePosition.x == _width && relativePosition.z == -1)
                {
                    return Southeast.VoxelLightData[IndexOf(0, relativePosition.y, _depth - 1)];
                }
                if (relativePosition.x == -1 && relativePosition.z == -1)
                {
                    return Southwest.VoxelLightData[IndexOf(_width - 1, relativePosition.y, _depth - 1)];
                }


                if (relativePosition.y >= 0 && relativePosition.y < _height && relativePosition.z >= 0 && relativePosition.z < _depth)
                {
                    if (relativePosition.x == -1)
                    {
                        return West.VoxelLightData[IndexOf(_width - 1, relativePosition.y, relativePosition.z)];
                    }
                    if (relativePosition.x == _width)
                    {
                        return East.VoxelLightData[IndexOf(0, relativePosition.y, relativePosition.z)];
                    }
                }
                if (relativePosition.y >= 0 && relativePosition.y < _height && relativePosition.x >= 0 && relativePosition.x < _width)
                {
                    if (relativePosition.z == -1)
                    {
                        return South.VoxelLightData[IndexOf(relativePosition.x, relativePosition.y, _depth - 1)];
                    }
                    if (relativePosition.z == _depth)
                    {
                        return North.VoxelLightData[IndexOf(relativePosition.x, relativePosition.y, 0)];
                    }
                }
            }

            if(relativePosition.y < 0 || relativePosition.y > _height - 1)
            {
                return 0;
            }

            throw new System.Exception($"Currently we not calculate height of chunk. {relativePosition}");
        }


        public void SetBlockLight(int x, int y, int z, byte intensity)
        {
            VoxelLightData[IndexOf(x, y, z)] = intensity;
        }
        public void SetBlockLight(Vector3Int relativePosition, byte intensity)
        {

            if (relativePosition.x < 0 || relativePosition.x >= Dimensions[0] ||
                 relativePosition.y < 0 || relativePosition.y >= Dimensions[1] ||
                 relativePosition.z < 0 || relativePosition.z >= Dimensions[2])
            {

                if (relativePosition.x < 0)
                {
                    West.VoxelLightData[IndexOf(_width - 1, relativePosition.y, relativePosition.z)] = intensity;
                    return;
                }
                if (relativePosition.x >= _width)
                {
                    East.VoxelLightData[IndexOf(0, relativePosition.y, relativePosition.z)] = intensity;
                    return;
                }

                if (relativePosition.z < 0)
                {
                    South.VoxelLightData[IndexOf(relativePosition.x, relativePosition.y, _depth - 1)] = intensity;
                    return;
                }
                if (relativePosition.z >= _depth)
                {
                    North.VoxelLightData[IndexOf(relativePosition.x, relativePosition.y, 0)] = intensity;
                    return;
                }

                return;
            }

            VoxelLightData[IndexOf(relativePosition.x, relativePosition.y, relativePosition.z)] = intensity;
        }


        public byte GetAmbientLight(int x, int y, int z)
        {
            return AmbientLightData[IndexOf(x, y, z)];
        }
        public byte GetAmbientLight(Vector3Int relativePosition)
        {
            if (IsValidRelativePosition(relativePosition))
            {
                return AmbientLightData[IndexOf(relativePosition.x, relativePosition.y, relativePosition.z)];
            }
            else
            {
                if (relativePosition.x == _width && relativePosition.z == _depth)
                {
                    return Northeast.AmbientLightData[IndexOf(0, relativePosition.y, 0)];
                }
                if (relativePosition.x == -1 && relativePosition.z == _depth)
                {
                    return Northwest.AmbientLightData[IndexOf(_width - 1, relativePosition.y, 0)];
                }
                if (relativePosition.x == _width && relativePosition.z == -1)
                {
                    return Southeast.AmbientLightData[IndexOf(0, relativePosition.y, _depth - 1)];
                }
                if (relativePosition.x == -1 && relativePosition.z == -1)
                {
                    return Southwest.AmbientLightData[IndexOf(_width - 1, relativePosition.y, _depth - 1)];
                }


                if (relativePosition.y >= 0 && relativePosition.y < _height && relativePosition.z >= 0 && relativePosition.z < _depth)
                {
                    if (relativePosition.x == -1)
                    {
                        return West.AmbientLightData[IndexOf(_width - 1, relativePosition.y, relativePosition.z)];
                    }
                    if (relativePosition.x == _width)
                    {
                        return East.AmbientLightData[IndexOf(0, relativePosition.y, relativePosition.z)];
                    }
                }
                if (relativePosition.y >= 0 && relativePosition.y < _height && relativePosition.x >= 0 && relativePosition.x < _width)
                {
                    if (relativePosition.z == -1)
                    {
                        return South.AmbientLightData[IndexOf(relativePosition.x, relativePosition.y, _depth - 1)];
                    }
                    if (relativePosition.z == _depth)
                    {
                        return North.AmbientLightData[IndexOf(relativePosition.x, relativePosition.y, 0)];
                    }
                }
            }
           
            throw new System.Exception($"Currently we not calculate height of chunk. {relativePosition}");
        }

        public void SetAmbientLight(Vector3Int relativePosition, byte intensity)
        {
            if (IsValidRelativePosition(relativePosition))
            {
                AmbientLightData[IndexOf(relativePosition.x, relativePosition.y, relativePosition.z)] = intensity;
                return;
            }
            else
            {
                if (relativePosition.x == _width && relativePosition.z == _depth)
                {
                    Northeast.AmbientLightData[IndexOf(0, relativePosition.y, 0)] = intensity; 
                    return;
                }
                if (relativePosition.x == -1 && relativePosition.z == _depth)
                {
                    Northwest.AmbientLightData[IndexOf(_width - 1, relativePosition.y, 0)] = intensity;
                    return;
                }
                if (relativePosition.x == _width && relativePosition.z == -1)
                {
                    Southeast.AmbientLightData[IndexOf(0, relativePosition.y, _depth - 1)] = intensity;
                    return;
                }
                if (relativePosition.x == -1 && relativePosition.z == -1)
                {
                    Southwest.AmbientLightData[IndexOf(_width - 1, relativePosition.y, _depth - 1)] = intensity;
                    return;
                }


                if (relativePosition.y >= 0 && relativePosition.y < _height && relativePosition.z >= 0 && relativePosition.z < _depth)
                {
                    if (relativePosition.x == -1)
                    {
                        West.AmbientLightData[IndexOf(_width - 1, relativePosition.y, relativePosition.z)] = intensity;
                        return;
                    }
                    if (relativePosition.x == _width)
                    {
                        East.AmbientLightData[IndexOf(0, relativePosition.y, relativePosition.z)] = intensity;
                        return;
                    }
                }
                if (relativePosition.y >= 0 && relativePosition.y < _height && relativePosition.x >= 0 && relativePosition.x < _width)
                {
                    if (relativePosition.z == -1)
                    {
                        South.AmbientLightData[IndexOf(relativePosition.x, relativePosition.y, _depth - 1)] = intensity;
                        return;
                    }
                    if (relativePosition.z == _depth)
                    {
                        North.AmbientLightData[IndexOf(relativePosition.x, relativePosition.y, 0)] = intensity;
                        return;
                    }
                }
            }

            throw new System.Exception($"Currently we not calculate height of chunk. {relativePosition}");          
        }
        #endregion



      

        #region Utilities
        public bool IsValidRelativePosition(Vector3Int relativePosition)
        {
            return relativePosition.x >= 0 && relativePosition.x < _width &&
                   relativePosition.y >= 0 && relativePosition.y < _height &&
                   relativePosition.z >= 0 && relativePosition.z < _depth;         
        }
            
        public int IndexOf(int x, int y, int z)
        {
            return x + _width * (y + _height * z);
        }
        public int IndexOf(int x, int z)
        {
            return x + z * _width;
        }

        public Vector3Int GlobalPosition => new Vector3Int(FrameX * Dimensions[0], FrameY * Dimensions[1], FrameZ * Dimensions[2]);
        public Vector3Int RelativePosition => new Vector3Int(FrameX, FrameY, FrameZ);
        public Vector3Int GetGlobalPosition(Vector3Int relativePosition)
        {
            if(IsValidRelativePosition(relativePosition))
            {
                return GlobalPosition + relativePosition;
            }
            else
            {
                throw new System.ArgumentOutOfRangeException($"{relativePosition}", "Relative position is out of bounds.");
            }
        }
        public Vector3Int GetRelativePosition(Vector3Int globalPosition)
        {
            Vector3Int relativePosition = new Vector3Int(globalPosition[0] % Dimensions[0],
                                                         globalPosition[1] % Dimensions[1],
                                                         globalPosition[2] % Dimensions[2]);
            return relativePosition;
        }

        public Bounds GetBounds()
        {
            Vector3 center = GlobalPosition + new Vector3(_width / 2.0f, _height / 2.0f, _depth / 2.0f);
            return new Bounds(center, new Vector3(_width, _height, _depth));
        }
        #endregion
    }
}
