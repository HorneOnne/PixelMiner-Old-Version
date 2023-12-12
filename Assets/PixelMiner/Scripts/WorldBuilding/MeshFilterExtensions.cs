using UnityEngine;

namespace PixelMiner.WorldBuilding
{
    public static class MeshFilterExtensions
    {
        public static void ApplyMeshData(this MeshFilter meshFilter, MeshData meshData)
        {
            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;
            mesh.SetVertices(meshData.Vertices);
            mesh.SetTriangles(meshData.Triangles, 0);
            mesh.SetUVs(0, meshData.UVs);
            mesh.SetUVs(1, meshData.UV2s);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
         
    

            meshFilter.sharedMesh = mesh;

            //LogUtils.WriteMeshToFile(meshFilter.mesh, "MeshData.txt");

            MeshDataPool.Release(meshData);
        }
    }
}
