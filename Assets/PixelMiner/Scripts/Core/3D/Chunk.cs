using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using PixelMiner.Enums;
using PixelMiner.WorldBuilding;

namespace PixelMiner.Core
{
    public class Chunk : MonoBehaviour
    {
        // OLD
        public float FrameX;    // Used for calculate world position
        public float FrameY;    // Used for calculate world position
        public bool HasChunkNeighbors = false;
        public bool AllTileHasNeighbors = false;
        public bool ChunkHasDrawn;
        public bool Processing { get; set; } = false;

        [HideInInspector] public BlockType[] ChunkData;

        // Neighbors
        public Chunk Left;
        public Chunk Right;
        public Chunk Front;
        public Chunk Back;



        // NEW
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;

        public int Width;
        public int Height;
        public int Depth;
        [HideInInspector] public Block[,,] Blocks;
        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshFilter = GetComponent<MeshFilter>();
        }

        private async void Start()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            Blocks = new Block[Width, Height, Depth];
            BuildChunk();

            List<MeshData> meshDataList = await GetMeshDataAsync();
            sw.Stop();
            Debug.Log($"{sw.ElapsedMilliseconds / 1000f} s");


            DrawChunk(meshDataList.ToArray());
     
        }


        private async Task<List<MeshData>> GetMeshDataAsync()
        {
            List<MeshData> meshDataList = new List<MeshData>();
            await Task.Run(() =>
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int z = 0; z < Depth; z++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            bool[] solidNeighbors = GetSolidBlockNeighbors(x, y, z); 
                            Blocks[x, y, z] = new Block(ChunkData[IndexOf(x, y, z)], solidNeighbors, new Vector3(x, y, z));
                            if (Blocks[x,y,z].MeshDataArray != null)
                            {
                                meshDataList.AddRange(Blocks[x, y, z].MeshDataArray);
                            }
                        }
                    }
                }
            });

            return meshDataList;
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



        private async void DrawChunk(MeshData[] meshDataArray)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            _meshFilter.mesh = await MeshUtils.MergeMeshAsyncParallel(meshDataArray);

            sw.Stop();
            Debug.Log($"Draw chunk in: {sw.ElapsedMilliseconds / 1000f} s");
        }


        #region Block
        public bool BlockHasSolidNeighbors(int x, int y, int z)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height || z < 0 || z >= Depth)
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

        #endregion

        #region Utilities
        private int IndexOf(int x, int y, int z)
        {
            return x + Width * (y + Height * z);
        }
        #endregion
    }
}
