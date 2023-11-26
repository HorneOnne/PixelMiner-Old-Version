using PixelMiner.Enums;
using UnityEngine;

namespace PixelMiner.Core
{
    public class Block : MonoBehaviour
    {
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;



        private void Start()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
  
            CreateCube();
        }

        private void CreateCube()
        {
            Quad[] q = new Quad[6];
            q[0] = new Quad(BlockSide.Top, BlockType.Dirt);
            q[1] = new Quad(BlockSide.Bottom, BlockType.GrassSide);
            q[2] = new Quad(BlockSide.Front, BlockType.GrassSide);
            q[3] = new Quad(BlockSide.Back, BlockType.GrassSide);
            q[4] = new Quad(BlockSide.Left, BlockType.GrassSide);
            q[5] = new Quad(BlockSide.Right, BlockType.GrassSide);
            Mesh[] meshes = new Mesh[6];
            for (int i = 0; i < meshes.Length; i++)
            {
                meshes[i] = q[i].Mesh;
            }

            _meshFilter.mesh = MeshUtils.MergeMesh(meshes);
            _meshFilter.mesh.name = "Cube_0_0_0";
        }
    }
}
