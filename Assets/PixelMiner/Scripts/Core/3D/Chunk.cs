using UnityEngine;

namespace PixelMiner.Core
{
    public class Chunk : MonoBehaviour
    {
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;

        public int Width;
        public int Height;
        public int Depth;
        public Block[,,] Blocks;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshFilter = GetComponent<MeshFilter>();

            Blocks = new Block[Width, Height, Depth];
            Mesh[] meshes = new Mesh[Width * Height * Depth];


            for(int x= 0 ; x < Width; x++)
            {
                for (int z = 0; z < Depth; z++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        Blocks[x, y, z] = new Block(new Vector3(x, y, z));

                        int index = x + Width * (y + Height * z);
                        meshes[index] = Blocks[x, y, z].Mesh;
                    }
                }
            }

            _meshFilter.mesh = MeshUtils.MergeMesh(meshes);
        }
    }
}
