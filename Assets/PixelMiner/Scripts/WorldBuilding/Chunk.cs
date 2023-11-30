using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using PixelMiner.Enums;
using PixelMiner.Utilities;
using PixelMiner.DataStructure;

namespace PixelMiner.WorldBuilding
{
    [SelectionBase]
    public class Chunk : MonoBehaviour
    {
        public static System.Action<Chunk> OnChunkFarAway;

        // OLD
        public int FrameX;
        public int FrameY;
        public int FrameZ;   

        public bool HasChunkNeighbors = false;
        public bool AllTileHasNeighbors = false;
        public bool ChunkHasDrawn;
        public bool Processing { get; set; } = false;

        private float _unloadChunkDistance = 200;
        private float _updateFrequency = 1.0f;
        private float _updateTimer = 0.0f;

      

        // Neighbors
        public Chunk Left;
        public Chunk Right;
        public Chunk Front;
        public Chunk Back;



        // NEW
        [SerializeField] private MeshFilter _solidMeshFilter;
        [SerializeField] private MeshFilter _fluidMeshFilter;

        public byte Width;
        public byte Height;
        public byte Depth;

        // Data
        [HideInInspector] public Block[,,] Blocks;  // Mesh (create unity mesh)
        [HideInInspector] public BlockType[] ChunkData; // Block Data (Define block type)
        [HideInInspector] public HeatType[] HeatData;
        [HideInInspector] public MoistureType[] MoistureData;
        [HideInInspector] public float[] HeightValues;
        [HideInInspector] public float[] HeatValues;
        [HideInInspector] public float[] MoistureValues;

        private void Awake()
        {


        }

        private void Start()
        {
           
        }

        private void Update()
        {
            if (Time.time - _updateTimer > _updateFrequency)
            {
                _updateTimer = Time.time;
                if (Vector2.Distance(Camera.main.transform.position, transform.position) > _unloadChunkDistance)
                {
                    OnChunkFarAway?.Invoke(this);
                }
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
            this.Depth  = depth;

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


            // Build
            //BuildChunk();

            // Draw
            //DrawChunkAsync();

            Processing = false;
        }

        

        private void BuildChunk()
        {
            int blockCount = Width * Height * Depth;
            ChunkData = new BlockType[blockCount];
            for (int i = 0; i < blockCount; i++)
            {
                ChunkData[i] = BlockType.Stone;
            }
        }



        public async void DrawChunkAsync()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            List<MeshData> solidMeshDataList = await GetSolidMeshDataAsync();
            List<MeshData> fluidMeshDataList = await GetFluidMeshDataAsync();
            _solidMeshFilter.mesh = await MeshUtils.MergeMeshAsyncParallel(solidMeshDataList.ToArray());
            _fluidMeshFilter.mesh = await MeshUtils.MergeMeshAsyncParallel(fluidMeshDataList.ToArray());

            sw.Stop();
            //Debug.Log($"Draw chunk in: {sw.ElapsedMilliseconds / 1000f} s");
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
                            if(blockType != BlockType.Water)
                            {
                                bool[] solidNeighbors = GetSolidBlockNeighbors(x, y, z);
                                Blocks[x, y, z] = new Block(ChunkData[IndexOf(x, y, z)], solidNeighbors, new Vector3(x, y, z));
                                if (Blocks[x, y, z].MeshDataArray != null)
                                {
                                    solidMeshDataList.AddRange(Blocks[x, y, z].MeshDataArray);
                                }
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
                                Blocks[x, y, z] = new Block(ChunkData[IndexOf(x, y, z)], fluidNeighbors, new Vector3(x, y, z));
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
            if (x < 0 || x >= Width || y < 0 || y >= Height || z < 0 || z >= Depth)
            {
                return false;
            }

            //if (x < 0 || x >= Width || z < 0 || z >= Depth)
            //{
            //    return true;
            //}
            //if (y < 0)
            //{
            //    return true;
            //}
            //if (y >= Height)
            //{
            //    return false;
            //}


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
        public bool[] GetSolidBlockNeighbors(int x, int y, int z)
        {
            bool[] solidNeighbors = new bool[6] { true, true, true, true, true, true }; // maximum 6 neighbors [ Top, Bottom, Front , Back, Left, Right]
    
            if (!BlockHasSolidNeighbors(x, y + 1, z))
            {
                solidNeighbors[0] = false;
            }
            if (!BlockHasSolidNeighbors(x, y - 1, z))
            {
                solidNeighbors[1] = false;
            }
            if (!BlockHasSolidNeighbors(x, y, z + 1))
            {
                solidNeighbors[2] = false;
            }
            if (!BlockHasSolidNeighbors(x, y, z - 1))
            {
                solidNeighbors[3] = false;
            }
            if (!BlockHasSolidNeighbors(x - 1, y, z))
            {
                solidNeighbors[4] = false;
            }
            if (!BlockHasSolidNeighbors(x + 1, y, z))
            {
                solidNeighbors[5] = false;
            }

            return solidNeighbors;
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

        #endregion

        #region Utilities
        private int IndexOf(int x, int y, int z)
        {
            return x + Width * (y + Height * z);
        }
        #endregion
    }
}
