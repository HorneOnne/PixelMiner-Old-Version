using UnityEngine;

namespace PixelMiner.WorldBuilding
{
    public class MeshData
    {
        public Vector3[] Vertices { get; }
        public int[] Triangles { get; }
        public Vector2[] UVs { get; }


        public MeshData(Vector3[] vertices, int[] triangles, Vector2[] uvs)
        {
            Vertices = vertices;
            Triangles = triangles;
            UVs = uvs;
        }
    }
}
