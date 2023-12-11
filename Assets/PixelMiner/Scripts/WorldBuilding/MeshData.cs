using UnityEngine;

namespace PixelMiner.WorldBuilding
{
    public class MeshData
    {
        public Vector3[] Vertices { get; }
        public int[] Triangles { get; }
        public Vector3[] UVs { get; }


        public MeshData(Vector3[] vertices, int[] triangles, Vector3[] uvs)
        {
            Vertices = vertices;
            Triangles = triangles;
            UVs = uvs;
        }
    }
}
