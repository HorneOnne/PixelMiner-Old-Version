using UnityEngine;
using PixelMiner.Enums;

namespace PixelMiner.Core
{
    public class Quad
    {
        public Mesh Mesh{get; private set;}



        public Quad(BlockSide side, BlockType blockType, Vector3 offset = (default))
        {
            Mesh = new Mesh();

            Vector3[] vertices = new Vector3[4];
            Vector3[] normals = new Vector3[4];
            Vector2[] uvs = new Vector2[4];
            Vector2[] uv2s = new Vector2[4];
            int[] triangles = new int[6] { 3, 1, 0, 3, 2, 1 };

            Vector2 uv00 = MeshUtils.BlockUVs[(ushort)blockType, 0];   // Bottom left
            Vector2 uv10 = MeshUtils.BlockUVs[(ushort)blockType, 1];   // Bottom right
            Vector2 uv01 = MeshUtils.BlockUVs[(ushort)blockType, 2];   // Top left
            Vector2 uv11 = MeshUtils.BlockUVs[(ushort)blockType, 3];   // Top Right

            //Vector2 uv00 = Vector2.zero;   // Bottom left
            //Vector2 uv10 = Vector2.right;   // Bottom right
            //Vector2 uv01 = Vector2.up;   // Top left
            //Vector2 uv11 = new Vector2(1f,1f);   // Top Right


            Vector2 uv2_00 = new Vector2(0.5f, 0.8125f);   // Bottom left
            Vector2 uv2_10 = new Vector2(0.5625f, 0.8125f);   // Bottom right
            Vector2 uv2_01 = new Vector2(0.5f, 0.875f);   // Top left
            Vector2 uv2_11 = new Vector2(0.5625f, 0.875f);   // Top Right

            if (!(side == BlockSide.Top))
            {
                uv2_00 = new Vector2(0.9375f, 0f);
                uv2_10 = new Vector2(1f, 0f);
                uv2_01 = new Vector2(0.9375f, 0.0625f);
                uv2_11 = new Vector2(1f, 0.0625f);
            }




            Vector3 p0 = new Vector3(-0.5f, -0.5f, 0.5f) + offset;
            Vector3 p1 = new Vector3(0.5f, -0.5f, 0.5f) + offset;
            Vector3 p2 = new Vector3(0.5f, -0.5f, -0.5f) + offset;
            Vector3 p3 = new Vector3(-0.5f, -0.5f, -0.5f) + offset;
            Vector3 p4 = new Vector3(-0.5f, 0.5f, 0.5f) + offset;
            Vector3 p5 = new Vector3(0.5f, 0.5f, 0.5f) + offset;
            Vector3 p6 = new Vector3(0.5f, 0.5f, -0.5f) + offset;
            Vector3 p7 = new Vector3(-0.5f, 0.5f, -0.5f) + offset;

            switch (side)
            {
                case BlockSide.Bottom:
                    vertices = new Vector3[] { p0, p1, p2, p3 };
                    normals = new Vector3[] {Vector3.down, Vector3.down, Vector3.down, Vector3.down};
                    uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                    uv2s = new Vector2[] { uv2_11, uv2_01, uv2_00, uv2_10 };
                    break;
                case BlockSide.Top:
                    vertices = new Vector3[] { p7, p6, p5, p4 };
                    normals = new Vector3[] {Vector3.up, Vector3.up, Vector3.up, Vector3.up};
                    uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                    uv2s = new Vector2[] { uv2_11, uv2_01, uv2_00, uv2_10 };
                 
                    break;
                case BlockSide.Left:
                    vertices = new Vector3[] { p7, p4, p0, p3 };
                    normals = new Vector3[] {Vector3.left, Vector3.left, Vector3.left, Vector3.left};
                    uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                    uv2s = new Vector2[] { uv2_11, uv2_01, uv2_00, uv2_10 };
                    break;
                case BlockSide.Right:
                    vertices = new Vector3[] { p5, p6, p2, p1 };
                    normals = new Vector3[] {Vector3.right, Vector3.right, Vector3.right, Vector3.right};
                    uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                    uv2s = new Vector2[] { uv2_11, uv2_01, uv2_00, uv2_10 };
                    break;
                case BlockSide.Front:
                    vertices = new Vector3[] { p4, p5, p1, p0 };
                    normals = new Vector3[] {Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward};
                    uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                    uv2s = new Vector2[] { uv2_11, uv2_01, uv2_00, uv2_10 };
                    break;
                case BlockSide.Back:
                    vertices = new Vector3[] { p6, p7, p3, p2 };
                    normals = new Vector3[] {Vector3.back, Vector3.back, Vector3.back, Vector3.back};
                    uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                    uv2s = new Vector2[] { uv2_11, uv2_01, uv2_00, uv2_10 };
                    break;
            }

           


            Mesh.vertices = vertices;
            Mesh.triangles = triangles;
            Mesh.normals = normals;
            Mesh.SetUVs(0, uvs);
            Mesh.SetUVs(1, uv2s);
      
           

            Mesh.RecalculateBounds();
        }
    }
}
