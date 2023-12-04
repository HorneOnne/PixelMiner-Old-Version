using System.Collections.Generic;
using UnityEngine;


namespace PixelMiner.WorldBuilding
{
    public class ChunkMeshData
    {
        public List<Vector3> Vertices;
        public List<Vector3> Normals;
        public List<Vector2> UVs;
        public List<Vector2> UV2s;
        public List<int> Tris;
        public int QuadCount;
     

        public ChunkMeshData()
        {
            Vertices = new List<Vector3>();
            Normals = new List<Vector3>();
            UVs = new List<Vector2>();
            UV2s = new List<Vector2>();
            Tris = new List<int>();
            QuadCount = 0;
        }

        public void Add(Block block)
        {
            Vertices.AddRange(block._vertices);
            Normals.AddRange(block._normals);
            UVs.AddRange(block._uvs);
            UV2s.AddRange(block._uv2s);
            QuadCount += block.QuadCount;
        }



        public void CalculateTriangleIndexes()
        {
            for (int i = 0; i < QuadCount; i++)
            {
                int baseIndex = i * 4;  // Each quad has 4 vertices      
                int offset = i * 6;  // Each quad has 6 indices (2 triangles)
                Tris.Add(baseIndex + 0);
                Tris.Add(baseIndex + 3);
                Tris.Add(baseIndex + 1);
                Tris.Add(baseIndex + 1);
                Tris.Add(baseIndex + 3);
                Tris.Add(baseIndex + 2);
            }
        }


        public void Reset()
        {
            Vertices.Clear();
            Normals.Clear();
            UVs.Clear();
            UV2s.Clear();
            Tris.Clear();
            QuadCount = 0;

        }
    }
}
