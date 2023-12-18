using UnityEngine;
using System.Collections.Generic;

namespace PixelMiner.WorldBuilding
{
    public class MeshData
    {
        public List<Vector3> Vertices { get; private set; }
        public List<int> Triangles { get; private set; }
        public List<Vector3> UVs { get; private set; }
        public List<Vector2> UV2s { get; private set; }
        public List<Color32> Colors { get; private set; }


        public MeshData() 
        { 
            Debug.Log("Create MeshData.cs");
            Vertices = new List<Vector3>(4);
            Triangles = new List<int>(6);
            UVs = new List<Vector3>(4);
            UV2s = new List<Vector2>(4);
            Colors = new List<Color32>(4);
        }

        public void Init(List<Vector3> vertices, List<int> triangles, List<Vector3> uvs, List<Vector2> uv2s, List<Color32> colors)
        {
            Vertices.AddRange(vertices);
            Triangles.AddRange(triangles);
            UVs.AddRange(uvs);
            UV2s.AddRange(uv2s);
            Colors.AddRange(colors);
        }


        public void Reset()
        {
            Vertices.Clear();
            Triangles.Clear();
            UVs.Clear();
            UV2s.Clear();
            Colors.Clear();
        }
    }
}
