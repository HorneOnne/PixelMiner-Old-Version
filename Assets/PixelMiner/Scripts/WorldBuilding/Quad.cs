using UnityEngine;
using PixelMiner.Enums;
using System.Collections.Generic;

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

        //public MeshData MeshData { get; private set; }
        public static readonly int[] Triangles = new int[6] { 0,3,1,1,3,2};
        public List<Vector3> _vertices;
        public List<Vector3> _normals;
        public List<Vector2> _uvs;
        public List<Vector2> _uv2s;
        Vector3 p0;
        Vector3 p1;
        Vector3 p2;
        Vector3 p3;
        Vector3 p4;
        Vector3 p5;
        Vector3 p6;
        Vector3 p7;

        public Quad()
        {
            _vertices = new List<Vector3>();
            _normals = new List<Vector3>();
            _uvs = new List<Vector2>();
            _uv2s = new List<Vector2>();

            //Debug.Log("Create new Quad.cs");
        }



        public void Init(BlockSide side, Vector3 offset = (default), Vector2[] uvs = null, Vector2[] uv2s = null)
        {
            p0 = new Vector3(-0.5f, -0.5f, 0.5f) + offset;
            p1 = new Vector3(0.5f, -0.5f, 0.5f) + offset;
            p2 = new Vector3(0.5f, -0.5f, -0.5f) + offset;
            p3 = new Vector3(-0.5f, -0.5f, -0.5f) + offset;
            p4 = new Vector3(-0.5f, 0.5f, 0.5f) + offset;
            p5 = new Vector3(0.5f, 0.5f, 0.5f) + offset;
            p6 = new Vector3(0.5f, 0.5f, -0.5f) + offset;
            p7 = new Vector3(-0.5f, 0.5f, -0.5f) + offset;


            switch (side)
            {
                default:
                case BlockSide.Bottom:
                    SetVertices(p0, p1, p2, p3);
                    SetNormals(Vector3.down, Vector3.down, Vector3.down, Vector3.down);
                    SetUVs(uvs[3], uvs[2], uvs[0], uvs[1]);
                    SetUV2s(uv2s[3], uv2s[2], uv2s[0], uv2s[1]);
                    break;
                case BlockSide.Top:
                    SetVertices(p7, p6, p5, p4);
                    SetNormals(Vector3.up, Vector3.up, Vector3.up, Vector3.up);
                    SetUVs(uvs[3], uvs[2], uvs[0], uvs[1]);
                    SetUV2s(uv2s[3], uv2s[2], uv2s[0], uv2s[1]);

                    break;
                case BlockSide.Left:
                    SetVertices(p7, p4, p0, p3);
                    SetNormals(Vector3.left, Vector3.left, Vector3.left, Vector3.left);
                    SetUVs(uvs[3], uvs[2], uvs[0], uvs[1]);
                    SetUV2s(uv2s[3], uv2s[2], uv2s[0], uv2s[1]);
                    break;
                case BlockSide.Right:
                    SetVertices(p5, p6, p2, p1);
                    SetNormals(Vector3.right, Vector3.right, Vector3.right, Vector3.right);
                    SetUVs(uvs[3], uvs[2], uvs[0], uvs[1]);
                    SetUV2s(uv2s[3], uv2s[2], uv2s[0], uv2s[1]);
                    break;
                case BlockSide.Front:
                    SetVertices(p4, p5, p1, p0);
                    SetNormals(Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward);
                    SetUVs(uvs[3], uvs[2], uvs[0], uvs[1]);
                    SetUV2s(uv2s[3], uv2s[2], uv2s[0], uv2s[1]);
                    break;
                case BlockSide.Back:
                    SetVertices(p6, p7, p3, p2);
                    SetNormals(Vector3.back, Vector3.back, Vector3.back, Vector3.back);
                    SetUVs(uvs[3], uvs[2], uvs[0], uvs[1]);
                    SetUV2s(uv2s[3], uv2s[2], uv2s[0], uv2s[1]);
                    break;
            }
        }


        public void SetVertices(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            _vertices.Add(v0);
            _vertices.Add(v1);
            _vertices.Add(v2);
            _vertices.Add(v3);
        }

        public void SetNormals(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            _normals.Add(v0);
            _normals.Add(v1);
            _normals.Add(v2);
            _normals.Add(v3);
        }

        public void SetUVs(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            _uvs.Add(v0);
            _uvs.Add(v1);
            _uvs.Add(v2);
            _uvs.Add(v3);
        }

        public void SetUV2s(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            _uv2s.Add(v0);
            _uv2s.Add(v1);
            _uv2s.Add(v2);
            _uv2s.Add(v3);
        }


        public void Reset()
        {
            _vertices.Clear();
            _normals.Clear();
            _uvs.Clear();
            _uv2s.Clear();
        }
    }
}
