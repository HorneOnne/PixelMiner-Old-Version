using UnityEngine;
using PixelMiner.Enums;
using System.Collections.Generic;

namespace PixelMiner.World
{
    [SelectionBase]
    public class Chunk : MonoBehaviour
    {
        public static System.Action<Chunk> OnChunkFarAway;
        public static System.Action<Chunk> OnChunkHasNeighbors;

        [SerializeField] public MeshFilter SolidMeshFilter;
        [SerializeField] public MeshFilter SolidTransparentMeshFilter;
        [SerializeField] public MeshFilter GrassMeshFilter;
        [SerializeField] public MeshFilter WaterMeshFilter;
        public MeshCollider MeshCollider;

        private Transform _playerTrans;


        public enum ChunkState
        {
            Processing, Idle
        }
        public ChunkState State = ChunkState.Idle;


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

        [field: SerializeField] public Chunk Up { get; set; }
        [field: SerializeField] public Chunk Down { get; set; }


        [field: SerializeField] public Chunk UpWest { get; set; }
        [field: SerializeField] public Chunk UpEast { get; set; }
        [field: SerializeField] public Chunk UpNorth { get; set; }
        [field: SerializeField] public Chunk UpSouth { get; set; }
        [field: SerializeField] public Chunk UpNorthwest { get; set; }
        [field: SerializeField] public Chunk UpNortheast { get; set; }
        [field: SerializeField] public Chunk UpSouthwest { get; set; }
        [field: SerializeField] public Chunk UpSoutheast { get; set; }

        [Header("Data")]
        [HideInInspector] public BlockType[] ChunkData;
        [HideInInspector] public HeatType[] HeatData;
        [HideInInspector] public MoistureType[] MoistureData;
        [HideInInspector] public BiomeType[] BiomesData;
        [HideInInspector] public byte[] VoxelLightData;
        [HideInInspector] public byte[] AmbientLightData;
        public float[] HeatValues;

        public Queue<RiverNode> RiverBfsQueue = new Queue<RiverNode>();
        public BiomeType[] RiverBiomes;
        public bool HasOceanBiome;

        private Vector3Int[] _faceNeighbors = new Vector3Int[6];

        private void Awake()
        {
            MeshCollider = SolidMeshFilter.GetComponent<MeshCollider>();
            HasOceanBiome = false;
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

                if (Vector3.Distance(_playerTrans.position, transform.position) > _unloadChunkDistance)
                {
                    OnChunkFarAway?.Invoke(this);
                }
            }
        }

        public void Init(int frameX, int frameY, int frameZ, int width, int height, int depth)
        {

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

            RiverBiomes = new BiomeType[_width * _depth];

            // Set all light dark by default
            for (int i = 0; i < VoxelLightData.Length; i++)
            {
                VoxelLightData[i] = 0;
            }

            for (int i = 0; i < AmbientLightData.Length; i++)
            {
                AmbientLightData[i] = 0;
            }
        }


        public bool IsSolid(Vector3Int relativePosition)
        {
            BlockType block = GetBlock(relativePosition);
            return block.IsSolid();
            //return block != BlockType.Air && block != BlockType.Water;
        }
        public bool IsWater(Vector3Int relativePosition)
        {
            BlockType block = GetBlock(relativePosition);
            return block == BlockType.Water;
        }
        public bool CanSee(Vector3Int relativePosition, ref Vector3Int[] faceNeighbors)
        {
            GetFaceNeighbors(relativePosition, ref faceNeighbors);

            for(int i = 0; i < _faceNeighbors.Length; i++)
            {
                if (IsValidRelativePosition(_faceNeighbors[i]))
                {
                    if(ChunkData[IndexOf(_faceNeighbors[i].x, _faceNeighbors[i].y, _faceNeighbors[i].z)].IsTransparentSolidBlock())
                    {
                        return false;
                    }
                }
                else
                {
                    if (FindNeighbor(_faceNeighbors[i], out Chunk neighborChunk, out Vector3Int nbRelativePosition))
                    {
                        if(neighborChunk.ChunkData[IndexOf(nbRelativePosition.x, nbRelativePosition.y, nbRelativePosition.z)].IsTransparentSolidBlock())
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        public bool IsNeighborFaceHasSameBlock(Vector3Int relativePosition, BlockType blockType, ref Vector3Int[] faceNeighbors)
        {
            GetFaceNeighbors(relativePosition, ref faceNeighbors);

            for (int i = 0; i < _faceNeighbors.Length; i++)
            {
                if (IsValidRelativePosition(_faceNeighbors[i]))
                {
                    if (ChunkData[IndexOf(_faceNeighbors[i].x, _faceNeighbors[i].y, _faceNeighbors[i].z)] != blockType)
                    {
                        return false;
                    }
                }
                else
                {
                    if (FindNeighbor(_faceNeighbors[i], out Chunk neighborChunk, out Vector3Int nbRelativePosition))
                    {
                        if (neighborChunk.ChunkData[IndexOf(nbRelativePosition.x, nbRelativePosition.y, nbRelativePosition.z)] != blockType)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        public bool IsNeighborHasAirBlock(Vector3Int relativePosition, ref Vector3Int[] faceNeighbors)
        {
            GetFaceNeighbors(relativePosition, ref faceNeighbors);

            for (int i = 0; i < _faceNeighbors.Length; i++)
            {
                if (IsValidRelativePosition(_faceNeighbors[i]))
                {

                    if (ChunkData[IndexOf(_faceNeighbors[i].x, _faceNeighbors[i].y, _faceNeighbors[i].z)] == BlockType.Air)
                    {
                        return true;
                    }

                    //try
                    //{
                    //    if (ChunkData[IndexOf(_faceNeighbors[i].x, _faceNeighbors[i].y, _faceNeighbors[i].z)] == BlockType.Air)
                    //    {
                    //        return true;
                    //    }
                    //}
                    //catch
                    //{
                    //    Debug.Log($"{_faceNeighbors[i]}  {IsValidRelativePosition(_faceNeighbors[i])}" );
                    //}
                }
                else
                {
                    if (FindNeighbor(_faceNeighbors[i], out Chunk neighborChunk, out Vector3Int nbRelativePosition))
                    {
                        if (neighborChunk.ChunkData[IndexOf(nbRelativePosition.x, nbRelativePosition.y, nbRelativePosition.z)] == BlockType.Air)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        public float GetHeat(Vector3Int relativePosition)
        {
            if (IsValidRelativePosition(relativePosition))
            {
                return HeatValues[IndexOf(relativePosition.x, relativePosition.z)];
            }
            else
            {
                if (FindNeighbor(relativePosition, out Chunk neighborChunk, out Vector3Int nbRelativePosition))
                {
                    return neighborChunk.HeatValues[IndexOf(nbRelativePosition.x, nbRelativePosition.z)];
                }
                else
                {
                    return 0;
                }
            }
        }


        public BlockType GetBlock(Vector3Int relativePosition)
        {
            if (IsValidRelativePosition(relativePosition))
            {
                return ChunkData[IndexOf(relativePosition.x, relativePosition.y, relativePosition.z)];
            }
            else
            {
                if (FindNeighbor(relativePosition, out Chunk neighborChunk, out Vector3Int nbRelativePosition))
                {
                    return neighborChunk.ChunkData[IndexOf(nbRelativePosition.x, nbRelativePosition.y, nbRelativePosition.z)];
                }
                else
                {
                    return BlockType.Air;
                }
            }   
        }

        public void SetBlock(Vector3Int relativePosition, BlockType blockType)
        {
            ChunkData[IndexOf(relativePosition.x, relativePosition.y, relativePosition.z)] = blockType;
        }


        public void SetBiome(Vector3Int relativePosition, BiomeType biomeType)
        {
            BiomesData[IndexOf(relativePosition.x, relativePosition.y, relativePosition.z)] = biomeType;
        }
        public BiomeType GetBiome(Vector3Int relativePosition)
        {
            if (IsValidRelativePosition(relativePosition))
            {
                return BiomesData[IndexOf(relativePosition.x, relativePosition.y, relativePosition.z)];
            }
            else
            {
                if (FindNeighbor(relativePosition, out Chunk neighborChunk, out Vector3Int nbRelativePosition))
                {
                    return neighborChunk.BiomesData[IndexOf(nbRelativePosition.x, nbRelativePosition.y, nbRelativePosition.z)];
                }
                else
                {
                    return BiomeType.Ocean;
                }
            }
        }


        public bool IsBlockFaceVisible(Vector3Int position, int dimension, bool isBackFace)
        {
            position[dimension] += isBackFace ? -1 : 1;
            //return GetBlock(position).IsSolid() == false;
            return GetBlock(position).IsSolid() == false ;
        }
        public bool IsTransparentBlockFaceVisible(Vector3Int position, int dimension, bool isBackFace)
        {
            position[dimension] += isBackFace ? -1 : 1;
            return GetBlock(position).IsSolid() == false || GetBlock(position).IsTransparentSolidBlock();
        }

        public bool IsWaterFaceVisible(Vector3Int position, int dimension, bool isBackFace)
        {
            position[dimension] += isBackFace ? -1 : 1;
            return IsWater(position) == false;
        }


        #region Neighbors

        public bool HasNeighbors()
        {
            return West != null && East != null && North != null && South != null &&
                   Northeast != null && Northwest != null && Southeast != null && Southwest != null && 
                   
                   Up != null &&
                   //Down != null &&

                   UpWest != null && UpEast != null && UpNorth != null && UpSouth != null &&
                   UpNortheast != null && UpNorthwest != null && UpSoutheast != null && UpSouthwest != null;
        }
        public bool FindNeighbor(Vector3Int relativePosition, out Chunk neighborChunk, out Vector3Int nbRelativePosition)
        {
            Vector3Int neighborOffset = default;
            nbRelativePosition = default;
            bool foundNeighborChunk = false;

            if (relativePosition.x >= _width)
            {
                neighborOffset.x = 1;
            }
            else if (relativePosition.x < 0)
            {
                neighborOffset.x = -1;
            }
            if (relativePosition.y >= _height)
            {
                neighborOffset.y = 1;
            }
            else if (relativePosition.y < 0)
            {
                neighborOffset.y = -1;
            }
            if (relativePosition.z >= _depth)
            {
                neighborOffset.z = 1;
            }
            else if (relativePosition.z < 0)
            {
                neighborOffset.z = -1;
            }



            neighborChunk = FindNeighbor(neighborOffset);
            if (neighborChunk != null)
            {
                foundNeighborChunk = true;

                if (neighborOffset.x == -1)
                {
                    nbRelativePosition.x = _width - 1;
                }
                else if (neighborOffset.x == 0)
                {
                    nbRelativePosition.x = relativePosition.x;
                }
                else if (neighborOffset.x == 1)
                {
                    nbRelativePosition.x = 0;
                }
                else
                {
                    Debug.LogError("Out of range width!");
                }

                if (neighborOffset.y == -1)
                {
                    nbRelativePosition.y = _height - 1;
                }
                else if (neighborOffset.y == 0)
                {
                    nbRelativePosition.y = relativePosition.y;
                }
                else if (neighborOffset.y == 1)
                {
                    nbRelativePosition.y = 0;
                }
                else
                {
                    Debug.LogError("Out of range height!");
                }

                if (neighborOffset.z == -1)
                {
                    nbRelativePosition.z = _depth - 1;
                }
                else if (neighborOffset.z == 0)
                {
                    nbRelativePosition.z = relativePosition.z;
                }
                else if (neighborOffset.z == 1)
                {
                    nbRelativePosition.z = 0;
                }
                else
                {
                    Debug.LogError("Out of range depth!");
                }
            }

            return foundNeighborChunk;
        }
        private Chunk FindNeighbor(Vector3Int neighborOffset)
        {
            if (neighborOffset == new Vector3Int(-1, 0, 0))
            {
                return West;
            }
            else if (neighborOffset == new Vector3Int(1, 0, 0))
            {
                return East;
            }
            else if (neighborOffset == new Vector3Int(0, 0, 1))
            {
                return North;
            }
            else if (neighborOffset == new Vector3Int(0, 0, -1))
            {
                return South;
            }
            else if (neighborOffset == new Vector3Int(-1, 0, -1))
            {
                return Southwest;
            }
            else if (neighborOffset == new Vector3Int(1, 0, -1))
            {
                return Southeast;
            }
            else if (neighborOffset == new Vector3Int(-1, 0, 1))
            {
                return Northwest;
            }
            else if (neighborOffset == new Vector3Int(1, 0, 1))
            {
                return Northeast;
            }


            else if (neighborOffset == new Vector3Int(0, 1, 0))
            {
                return Up;
            }
            else if (neighborOffset == new Vector3Int(0, -1, 0))
            {
                return Down;
            }


            else if (neighborOffset == new Vector3Int(-1, 1, 0))
            {
                return UpWest;
            }
            else if (neighborOffset == new Vector3Int(1, 1, 0))
            {
                return UpEast;
            }
            else if (neighborOffset == new Vector3Int(0, 1, 1))
            {
                return UpNorth;
            }
            else if (neighborOffset == new Vector3Int(0, 1, -1))
            {
                return UpSouth;
            }
            else if (neighborOffset == new Vector3Int(-1, 1, -1))
            {
                return UpSouthwest;
            }
            else if (neighborOffset == new Vector3Int(1, 1, -1))
            {
                return UpSoutheast;
            }
            else if (neighborOffset == new Vector3Int(-1, 1, 1))
            {
                return UpNorthwest;
            }
            else if (neighborOffset == new Vector3Int(1, 1, 1))
            {
                return UpNortheast;
            }


            return null;
        }

        private void GetFaceNeighbors(Vector3Int relativePosition, ref Vector3Int[] faceNeighbors)
        {
            //_faceNeighbors[0] = relativePosition + Vector3Int.left;
            //_faceNeighbors[1] = relativePosition + Vector3Int.right;
            //_faceNeighbors[2] = relativePosition + Vector3Int.forward;
            //_faceNeighbors[3] = relativePosition + Vector3Int.back;
            //_faceNeighbors[4] = relativePosition + Vector3Int.up;
            //_faceNeighbors[5] = relativePosition + Vector3Int.down;

            faceNeighbors[0] = relativePosition + Vector3Int.left;
            faceNeighbors[1] = relativePosition + Vector3Int.right;
            faceNeighbors[2] = relativePosition + Vector3Int.forward;
            faceNeighbors[3] = relativePosition + Vector3Int.back;
            faceNeighbors[4] = relativePosition + Vector3Int.up;
            faceNeighbors[5] = relativePosition + Vector3Int.down;

        }

        #endregion



        #region Lighting
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
                if (FindNeighbor(relativePosition, out Chunk neighborChunk, out Vector3Int nbRelativePosition))
                {    
                    return neighborChunk.VoxelLightData[IndexOf(nbRelativePosition.x, nbRelativePosition.y, nbRelativePosition.z)];
                }
                else
                {
                    //Debug.LogError("Not found this chunk");
                    return 0;
                }
            }
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
                if (FindNeighbor(relativePosition, out Chunk neighborChunk, out Vector3Int nbRelativePosition))
                {
                    return neighborChunk.AmbientLightData[IndexOf(nbRelativePosition.x, nbRelativePosition.y, nbRelativePosition.z)];
                }
                else
                {
                    Debug.LogError("Not found this chunk");
                    return 0;
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
                if (FindNeighbor(relativePosition, out Chunk neighborChunk, out Vector3Int nbRelativePosition))
                {
                    neighborChunk.AmbientLightData[IndexOf(nbRelativePosition.x, nbRelativePosition.y, nbRelativePosition.z)] = intensity;
                }
                else
                {
                    Debug.LogError("Not found this chunk");
                }
            }
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
            if (IsValidRelativePosition(relativePosition))
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

        public bool OnGroundLevel(Vector3Int relativePosition)
        {
            if(IsValidRelativePosition(relativePosition))
            {
                Vector3Int belowRelativePos = relativePosition + Vector3Int.down;
                Vector3Int upperRelativePos = relativePosition + Vector3Int.up;

                if(GetBlock(relativePosition) == BlockType.Air)
                {
                    if (GetBlock(belowRelativePos) != BlockType.Air)
                    {
                        if (GetBlock(upperRelativePos) == BlockType.Air)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            Debug.LogError($"Out of chunk volume range. {relativePosition}");
            return false;
        }
        #endregion
    }

    public struct RiverNode
    {
        public Vector3Int RelativePosition;
        public int Density;
    }
}
