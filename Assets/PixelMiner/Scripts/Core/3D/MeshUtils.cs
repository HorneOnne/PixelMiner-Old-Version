using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using QFSW.QC;
using PixelMiner.Enums;
using System.IO;

//using VertexData = System.Tuple<UnityEngine.Vector3, UnityEngine.Vector3, UnityEngine.Vector2>;
using VertexData = System.Tuple<UnityEngine.Vector3, UnityEngine.Vector3, UnityEngine.Vector2, UnityEngine.Vector2>;
using UnityEngine.UI;
using UnityEngine.Rendering;

namespace PixelMiner.Core
{
    public static class MeshUtils
    {
        public static Vector2[,] BlockUVs =
        {
            /*
             * Order: BOTTOM_LEFT -> BOTTOM_RIGHT -> TOP_LEFT -> TOP_RIGHT.
             */

            /*GRASSTOP*/  
            { new Vector2(0.5f, 0.8125f), new Vector2(0.5625f, 0.8125f),
              new Vector2(0.5f, 0.875f),new Vector2(0.5625f, 0.875f)},

            /*GRASSSIDE*/  
            { new Vector2(0.1875f, 0.9375f), new Vector2(0.25f, 0.9375f),
              new Vector2(0.1875f, 1.0f),new Vector2(0.25f, 1.0f)},

            /*DIRT*/ 
            {new Vector2(0.125f, 0.9375f), new Vector2(0.1875f, 0.9375f),
             new Vector2(0.125f, 1f), new Vector2(0.1875f, 1f)},

            /*STONE*/
            {new Vector2(0.0625f, 0.9375f), new Vector2(0.125f, 0.9375f),
            new Vector2(0.0625f, 1f), new Vector2(0.125f, 1f)},

            /*AIR*/
            {new Vector2(0.4375f, 0.0625f), new Vector2(0.5f, 0.0625f),
            new Vector2(0.4375f, 0.125f), new Vector2(0.5f, 0.125f)},


            /*WATER*/
            {new Vector2(0.8125f, 0.1875f), new Vector2(0.875f, 0.1875f),
            new Vector2(0.8125f, 0.25f), new Vector2(0.875f, 0.25f)},

            /*SAND*/
            {new Vector2(0.125f, 0.875f), new Vector2(0.1875f, 0.875f),
            new Vector2(0.125f, 0.9375f), new Vector2(0.1875f, 0.9375f)},
        };

        public static Vector2[,] BlockUV2s =
        {
            /*
             * Order: BOTTOM_LEFT -> BOTTOM_RIGHT -> TOP_LEFT -> TOP_RIGHT.
             */            
            /*PLAINS*/
            {new Vector2(0.234375f, 0.640625f), new Vector2(0.25f, 0.640625f),
            new Vector2(0.234375f, 0.65625f), new Vector2(0.25f, 0.65625f)},


            /*FOREST*/
            {new Vector2(0.15625f, 0.796875f), new Vector2(0.171875f, 0.796875f),
            new Vector2(0.15625f, 0.8125f), new Vector2(0.171875f, 0.8125f)}, 

            /*JUNGLE*/
            {new Vector2(0.15625f, 0.796875f), new Vector2(0.171875f, 0.796875f),
            new Vector2(0.15625f, 0.8125f), new Vector2(0.171875f, 0.8125f)}, 

            /*DESERT*/
            {new Vector2(0f, 0f), new Vector2(0.015625f, 0f),
            new Vector2(0f, 0.015625f), new Vector2(0.015625f, 0.015625f)}, 

            /*NONE*/
            {new Vector2(0.8125f, 0.8125f), new Vector2(0.828125f, 0.8125f),
            new Vector2(0.8125f, 0.828125f), new Vector2(0.828125f, 0.828125f)},
        };


        [Command("/getUV")]
        private static void GetUV(int u, int v, ushort blockType = 0)
        {
            float tileSize = 1 / 16f;
            Debug.Log($"\n\n/*{((BlockType)blockType).ToString().ToUpper()}*/" +
               $"\n{{new Vector2({tileSize * u}f, {tileSize * v}f), " +
               $"new Vector2({tileSize * u + tileSize}f, {tileSize * v}f)," +
               $"\nnew Vector2({tileSize * u}f, {tileSize * v + tileSize}f), " +
               $"new Vector2({tileSize * u + tileSize}f, {tileSize * v + tileSize}f)}}, ");
        }

        [Command("/getUV2")]
        private static void GetUV2(int u, int v, ushort blockType = 0)
        {
            float tileSize = 1 / 64f;
            Debug.Log($"\n\n/*{((ColorMapType)blockType).ToString().ToUpper()}*/" +
               $"\n{{new Vector2({tileSize * u}f, {tileSize * v}f), " +
               $"new Vector2({tileSize * u + tileSize}f, {tileSize * v}f)," +
               $"\nnew Vector2({tileSize * u}f, {tileSize * v + tileSize}f), " +
               $"new Vector2({tileSize * u + tileSize}f, {tileSize * v + tileSize}f)}}, ");
        }

        public static Mesh MergeMesh(Mesh[] meshes)
        {
            Mesh mesh = new Mesh();
            Dictionary<VertexData, int> pointsOrder = new Dictionary<VertexData, int>();
            List<int> tris = new List<int>();

            int pIndex = 0;
            for (int i = 0; i < meshes.Length; i++)  // Loop through each mesh
            {
                if (meshes[i] == null) continue;

                for (int j = 0; j < meshes[i].vertices.Length; j++)  // Loop through each vertex of the current mesh.
                {
                    Vector3 v = meshes[i].vertices[j];
                    Vector3 n = meshes[i].normals[j];
                    Vector2 uv = meshes[i].uv[j];
                    Vector2 uv2 = meshes[i].uv2[j];
                    VertexData p = new VertexData(v, n, uv, uv2);

                    if (!pointsOrder.ContainsKey(p))
                    {
                        pointsOrder.Add(p, pIndex);
                        pIndex++;
                    }
                }

                for (int t = 0; t < meshes[i].triangles.Length; t++)    // Loop through each trig of the current mesh.
                {
                    int triPoint = meshes[i].triangles[t];
                    Vector3 v = meshes[i].vertices[triPoint];
                    Vector3 n = meshes[i].normals[triPoint];
                    Vector2 uv = meshes[i].uv[triPoint];
                    Vector2 uv2 = meshes[i].uv2[triPoint];
                    VertexData p = new VertexData(v, n, uv, uv2);

                    int index;
                    pointsOrder.TryGetValue(p, out index);
                    tris.Add(index);
                }
                meshes[i] = null;
            }

            ExtractArrays(pointsOrder, mesh);
            mesh.triangles = tris.ToArray();
            mesh.RecalculateNormals();
            return mesh;
        }

        public static void ExtractArrays(Dictionary<VertexData, int> list, Mesh mesh)
        {
            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Vector2> uv2s = new List<Vector2>();

            foreach (VertexData v in list.Keys)
            {
                verts.Add(v.Item1);
                norms.Add(v.Item2);
                uvs.Add(v.Item3);
                uv2s.Add(v.Item4);
            }
            mesh.vertices = verts.ToArray();
            mesh.normals = norms.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.uv2 = uv2s.ToArray();
        }



        public static Mesh MergeMesh(MeshData[] meshes)
        {
            Mesh mesh = new Mesh();
            Dictionary<VertexData, int> pointsOrder = new Dictionary<VertexData, int>();
            List<int> tris = new List<int>();

            int pIndex = 0;
            for (int i = 0; i < meshes.Length; i++)  // Loop through each mesh
            {
                for (int j = 0; j < meshes[i].Vertices.Length; j++)  // Loop through each vertex of the current mesh.
                {
                    Vector3 v = meshes[i].Vertices[j];
                    Vector3 n = meshes[i].Normals[j];
                    Vector2 uv = meshes[i].UVs[j];
                    Vector2 uv2 = meshes[i].UV2s[j];
                    VertexData p = new VertexData(v, n, uv, uv2);

                    if (!pointsOrder.ContainsKey(p))
                    {
                        pointsOrder.Add(p, pIndex);
                        pIndex++;
                    }
                }

                for (int t = 0; t < meshes[i].Triangles.Length; t++)    // Loop through each trig of the current mesh.
                {
                    int triPoint = meshes[i].Triangles[t];
                    Vector3 v = meshes[i].Vertices[triPoint];
                    Vector3 n = meshes[i].Normals[triPoint];
                    Vector2 uv = meshes[i].UVs[triPoint];
                    Vector2 uv2 = meshes[i].UV2s[triPoint];
                    VertexData p = new VertexData(v, n, uv, uv2);

                    int index;
                    pointsOrder.TryGetValue(p, out index);
                    tris.Add(index);
                }
            }
            ExtractArrays(pointsOrder, mesh);
            mesh.triangles = tris.ToArray();
            mesh.indexFormat = IndexFormat.UInt32;
            mesh.RecalculateNormals();
            return mesh;
        }




        public static async Task<Mesh> MergeMeshAsync(MeshData[] mesheDataArray, IndexFormat format = IndexFormat.UInt32)
        {
            Vector3[] verts = new Vector3[0];
            Vector3[] norms = new Vector3[0];
            int[] tris = new int[0]; ;
            Vector2[] uvs = new Vector2[0];
            Vector2[] uv2s = new Vector2[0];
            await Task.Run(() =>
            {
                int triCount = 0;
                int vertCount = 0;
                int normalCount = 0;
                int uvCount = 0;

                for (int i = 0; i < mesheDataArray.Length; i++)
                {
                    if (mesheDataArray[i].Equals(default(MeshData)))
                    {
                        Debug.Log("Mesh data array is emtpy");
                        continue;
                    }

                    triCount += mesheDataArray[i].Triangles.Length;
                    vertCount += mesheDataArray[i].Vertices.Length;
                    normalCount += mesheDataArray[i].Normals.Length;
                    uvCount += mesheDataArray[i].UVs.Length;
                }
                verts = new Vector3[vertCount];
                norms = new Vector3[normalCount];
                tris = new int[triCount];
                uvs = new Vector2[uvCount];
                uv2s = new Vector2[uvCount];

                for (int i = 0; i < mesheDataArray.Length; i++)  // Loop through each mesh
                {
                    if (mesheDataArray[i].Equals(default(MeshData)))
                    {
                        Debug.Log("Mesh data array is emtpy");
                        continue;
                    }

                    for (int j = 0; j < mesheDataArray[i].Triangles.Length; j++)
                    {
                        int triPoint = (i * mesheDataArray[i].Vertices.Length + mesheDataArray[i].Triangles[j]);
                        tris[j + i * mesheDataArray[i].Triangles.Length] = triPoint;
                    }


                    for (int v = 0; v < mesheDataArray[i].Vertices.Length; v++)
                    {
                        int vertexIndex = v + i * mesheDataArray[i].Vertices.Length;
                        verts[vertexIndex] = mesheDataArray[i].Vertices[v];
                        norms[vertexIndex] = mesheDataArray[i].Normals[v];
                        uvs[vertexIndex] = mesheDataArray[i].UVs[v];
                        uv2s[vertexIndex] = mesheDataArray[i].UV2s[v];
                    }
                }
            });

            Mesh mesh = new Mesh();
            mesh.indexFormat = format;
            mesh.vertices = verts;
            mesh.normals = norms;
            mesh.uv = uvs;
            mesh.uv2 = uv2s;
            mesh.triangles = tris;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }


        public static async Task<Mesh> MergeMeshAsyncParallel(MeshData[] meshes, IndexFormat format = IndexFormat.UInt32)
        {
            Vector3[] verts = new Vector3[0];
            Vector3[] norms = new Vector3[0];
            int[] tris = new int[0]; ;
            Vector2[] uvs = new Vector2[0];
            Vector2[] uv2s = new Vector2[0];
            await Task.Run(() =>
            {
                int triCount = 0;
                int vertCount = 0;
                int normalCount = 0;
                int uvCount = 0;

                for (int i = 0; i < meshes.Length; i++)
                {
                    triCount += meshes[i].Triangles.Length;
                    vertCount += meshes[i].Vertices.Length;
                    normalCount += meshes[i].Normals.Length;
                    uvCount += meshes[i].UVs.Length;
                }
                verts = new Vector3[vertCount];
                norms = new Vector3[normalCount];
                tris = new int[triCount];
                uvs = new Vector2[uvCount];
                uv2s = new Vector2[uvCount];

                Parallel.For(0, meshes.Length, i =>
                {
                    for (int j = 0; j < meshes[i].Triangles.Length; j++)
                    {
                        int triPoint = (i * meshes[i].Vertices.Length + meshes[i].Triangles[j]);
                        tris[j + i * meshes[i].Triangles.Length] = triPoint;
                    }


                    for (int v = 0; v < meshes[i].Vertices.Length; v++)
                    {
                        int vertexIndex = v + i * meshes[i].Vertices.Length;
                        verts[vertexIndex] = meshes[i].Vertices[v];
                        norms[vertexIndex] = meshes[i].Normals[v];
                        uvs[vertexIndex] = meshes[i].UVs[v];
                        uv2s[vertexIndex] = meshes[i].UV2s[v];
                    }
                });
            });

            Mesh mesh = new Mesh();
            mesh.indexFormat = format;
            mesh.vertices = verts;
            mesh.normals = norms;
            mesh.uv = uvs;
            mesh.uv2 = uv2s;
            mesh.triangles = tris;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

    }
}
