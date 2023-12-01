using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using PixelMiner.DataStructure;
using PixelMiner.Enums;

namespace PixelMiner.Utilities
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

            
            /*GLASS*/
            {new Vector2(0.0625f, 0.75f), new Vector2(0.125f, 0.75f),
            new Vector2(0.0625f, 0.8125f), new Vector2(0.125f, 0.8125f)},
        };

        public static Vector2[,] ColorMapUVs =
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


        public static Vector2[] GetDepthUVs(float depth)
        {
            float depthValue = MathHelper.Map(depth, 0.2f, 0.45f, 0.0f, 0.75f);
            float offset = 0.05f;
           // Debug.Log(depthValue);
            return new Vector2[]
            {
                new Vector2(0, depthValue),
                new Vector2(1, depthValue),
                 new Vector2(0f, Mathf.Clamp01(depthValue + offset)),
                new Vector2(1, Mathf.Clamp01(depthValue + offset))
            };
        }


        public static Vector2[] GetBlockUV(BlockType blockType)
        {
            return new Vector2[]
            {
                BlockUVs[(ushort)blockType, 0] ,
                BlockUVs[(ushort)blockType, 1] ,
                BlockUVs[(ushort)blockType, 2],
                BlockUVs[(ushort)blockType, 3]};
        }

        public static Vector2[] GetColorMapUV(ColorMapType colorMapType)
        {
            return new Vector2[]
            {
                ColorMapUVs[(ushort)colorMapType, 0] ,
                ColorMapUVs[(ushort)colorMapType, 1] ,
                ColorMapUVs[(ushort)colorMapType, 2],
                ColorMapUVs[(ushort)colorMapType, 3]};
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
