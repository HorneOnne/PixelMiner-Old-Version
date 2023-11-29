using UnityEngine;
using PixelMiner.Enums;
using PixelMiner.Utilities;
using PixelMiner.DataStructure;

namespace PixelMiner.WorldBuilding
{
    public class Quad
    {
        /*
               3--------2
               |\       |
               | \      |
               |  \     |
               |   \    |
               |    \   |
               |     \  |
               |      \ |
               |       \|
               0--------1
        */

        public MeshData MeshData { get; private set; }

        public Quad(BlockSide side, BlockType blockType, ColorMapType colorMap, Vector3 offset = (default))
        {
            Vector3[] vertices;
            Vector3[] normals;
            Vector2[] uvs;
            Vector2[] uv2s;
            int[] triangles = new int[6] { 0, 3, 1, 1, 3, 2 };

            Vector2 uv00 = MeshUtils.BlockUVs[(ushort)blockType, 0];   // Bottom left
            Vector2 uv10 = MeshUtils.BlockUVs[(ushort)blockType, 1];   // Bottom right
            Vector2 uv01 = MeshUtils.BlockUVs[(ushort)blockType, 2];   // Top left
            Vector2 uv11 = MeshUtils.BlockUVs[(ushort)blockType, 3];   // Top Right

            Vector2 uv2_00 = MeshUtils.BlockUV2s[(ushort)colorMap, 0];    // Bottom left
            Vector2 uv2_10 = MeshUtils.BlockUV2s[(ushort)colorMap, 1];   // Bottom right
            Vector2 uv2_01 = MeshUtils.BlockUV2s[(ushort)colorMap, 2];   // Top left
            Vector2 uv2_11 = MeshUtils.BlockUV2s[(ushort)colorMap, 3];   // Top Right


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
                default:
                case BlockSide.Bottom:
                    vertices = new Vector3[] { p0, p1, p2, p3 };
                    normals = new Vector3[] { Vector3.down, Vector3.down, Vector3.down, Vector3.down };
                    uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                    uv2s = new Vector2[] { uv2_11, uv2_01, uv2_00, uv2_10 };
                    break;
                case BlockSide.Top:
                    vertices = new Vector3[] { p7, p6, p5, p4 };
                    normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };
                    uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                    uv2s = new Vector2[] { uv2_11, uv2_01, uv2_00, uv2_10 };

                    break;
                case BlockSide.Left:
                    vertices = new Vector3[] { p7, p4, p0, p3 };
                    normals = new Vector3[] { Vector3.left, Vector3.left, Vector3.left, Vector3.left };
                    uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                    uv2s = new Vector2[] { uv2_11, uv2_01, uv2_00, uv2_10 };
                    break;
                case BlockSide.Right:
                    vertices = new Vector3[] { p5, p6, p2, p1 };
                    normals = new Vector3[] { Vector3.right, Vector3.right, Vector3.right, Vector3.right };
                    uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                    uv2s = new Vector2[] { uv2_11, uv2_01, uv2_00, uv2_10 };
                    break;
                case BlockSide.Front:
                    vertices = new Vector3[] { p4, p5, p1, p0 };
                    normals = new Vector3[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
                    uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                    uv2s = new Vector2[] { uv2_11, uv2_01, uv2_00, uv2_10 };
                    break;
                case BlockSide.Back:
                    vertices = new Vector3[] { p6, p7, p3, p2 };
                    normals = new Vector3[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back };
                    uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                    uv2s = new Vector2[] { uv2_11, uv2_01, uv2_00, uv2_10 };
                    break;
            }

            MeshData = new MeshData()
            {
                Vertices = vertices,
                Normals = normals,
                Triangles = triangles,
                UVs = uvs,
                UV2s = uv2s,
            };
        }
    }
}
