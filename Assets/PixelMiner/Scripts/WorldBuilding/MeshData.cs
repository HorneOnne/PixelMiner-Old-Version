using UnityEngine;

namespace PixelMiner.WorldBuilding
{
    public class MeshData
    {
        public Vector3[] Vertices { get; }
        public int[] Triangles { get; }


        public MeshData(Vector3[] vertices, int[] triangles)
        {
            Vertices = vertices;
            Triangles = triangles;
        }
    }
}
