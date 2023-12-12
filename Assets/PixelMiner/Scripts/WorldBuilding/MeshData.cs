using UnityEngine;

namespace PixelMiner.WorldBuilding
{
    public class MeshData
    {
        public Vector3[] Vertices { get; }
        public int[] Triangles { get; }
        public Vector3[] UVs { get; }
        public Vector2[] UV2s { get; }


        public MeshData(Vector3[] vertices, int[] triangles, Vector3[] uvs, Vector2[] uv2s)
        {
            Vertices = vertices;
            Triangles = triangles;
            UVs = uvs;
            UV2s = uv2s;
        }
    }
}
