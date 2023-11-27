using PixelMiner.Enums;
using UnityEngine;

namespace PixelMiner.Core
{
    public class Block
    {
        public Mesh Mesh { get; private set; }  
        public Block(Vector3 offset = (default))
        {
            Quad[] q = new Quad[6];
            q[0] = new Quad(BlockSide.Top, BlockType.GrassTop, ColorMapType.Plains, offset);
            q[1] = new Quad(BlockSide.Bottom, BlockType.Dirt, ColorMapType.None, offset);
            q[2] = new Quad(BlockSide.Front, BlockType.GrassSide, ColorMapType.None, offset);
            q[3] = new Quad(BlockSide.Back, BlockType.GrassSide, ColorMapType.None, offset);
            q[4] = new Quad(BlockSide.Left, BlockType.GrassSide, ColorMapType.None, offset);
            q[5] = new Quad(BlockSide.Right, BlockType.GrassSide, ColorMapType.None, offset);
            Mesh[] meshes = new Mesh[6];
            for (int i = 0; i < meshes.Length; i++)
            {
                meshes[i] = q[i].Mesh;
            }

            Mesh = MeshUtils.MergeMesh(meshes);
        }
    }
}
