using UnityEngine;

namespace PixelMiner.WorldBuilding
{
    public static class MeshFilterExtensions
    {
        public static void ApplyMeshData(this MeshFilter meshFilter, MeshData meshData)
        {
            meshFilter.mesh.Clear();
            meshFilter.mesh.vertices = meshData.Vertices;
            meshFilter.mesh.triangles = meshData.Triangles;
            meshFilter.mesh.uv = meshData.UVs;

            // Color mesh and calculate normals
            meshFilter.mesh.RecalculateNormals();

            LogUtils.WriteMeshToFile(meshFilter.mesh, "MeshData.txt");
        }
    }
}
