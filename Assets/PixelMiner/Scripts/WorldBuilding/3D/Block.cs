using PixelMiner.Enums;
using UnityEngine;

namespace PixelMiner.WorldBuilding
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
            q[0] = new Quad(BlockSide.Front);
            q[1] = new Quad(BlockSide.Back);
            q[2] = new Quad(BlockSide.Top);
            q[3] = new Quad(BlockSide.Bottom);
            q[4] = new Quad(BlockSide.Left);
            q[5] = new Quad(BlockSide.Right);
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
