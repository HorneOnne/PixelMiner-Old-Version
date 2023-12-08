using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using PixelMiner.DataStructure;
using PixelMiner.Utilities;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using System;
using PixelMiner.Enums;

namespace PixelMiner.WorldBuilding
{


    public static class MeshUtils
    {
        // Example method to write Unity Mesh data to a text file

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



        public static async Task<Mesh> MergeMeshAsync(MeshData[] mesheDataArray, IndexFormat format = IndexFormat.UInt16)
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


        public static async Task<Mesh> MergeMeshAsyncParallel(MeshData[] meshes, IndexFormat format = IndexFormat.UInt16)
        {

            Vector3[] verts = null;
            Vector3[] norms = null;
            int[] tris = null; ;
            Vector2[] uvs = null;
            Vector2[] uv2s = null;
            int triCount = 0;
            int vertCount = 0;
            int normalCount = 0;
            int uvCount = 0;

            await Task.Run(() =>
            {
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
            mesh.UploadMeshData(markNoLongerReadable: true);

            return mesh;
        }


        public static async Task<Mesh> MergeLargeMeshDataAsyncParallel(ChunkMeshData largeMeshData, IndexFormat format = IndexFormat.UInt16)
        {
            //await Task.Run(() =>
            //{
            //    largeMeshData.CalculateTriangleIndexes();
            //});

            //Mesh mesh = new Mesh();
            //mesh.indexFormat = format;

            //mesh.SetVertices(largeMeshData.Vertices);
            //mesh.SetNormals(largeMeshData.Normals);
            //mesh.SetTriangles(largeMeshData.Tris, 0);
            //mesh.SetUVs(0, largeMeshData.UVs);
            //mesh.SetUVs(1, largeMeshData.UV2s);

            //mesh.RecalculateNormals();
            //mesh.RecalculateBounds();

            //mesh.UploadMeshData(markNoLongerReadable: true);
            //return mesh;



            await Task.Run(() =>
            {
                largeMeshData.CalculateTriangleIndexes();
            });
            if (largeMeshData.QuadCount == 0)
                return null;

            Mesh mesh = new Mesh();
            mesh.indexFormat = format;

            int numOfQuad = 50;
            int numOfVertices = numOfQuad * 4;
            int numOfTris = numOfQuad * 6;


            largeMeshData.Vertices.RemoveRange(numOfVertices, largeMeshData.Vertices.Count - numOfVertices);
            largeMeshData.Normals.RemoveRange(numOfVertices, largeMeshData.Normals.Count - numOfVertices);
            largeMeshData.UVs.RemoveRange(numOfVertices, largeMeshData.UVs.Count - numOfVertices);
            largeMeshData.UV2s.RemoveRange(numOfVertices, largeMeshData.UV2s.Count - numOfVertices);
            largeMeshData.Tris.RemoveRange(numOfTris, largeMeshData.Tris.Count - numOfTris);



            mesh.SetVertices(largeMeshData.Vertices);
            mesh.SetNormals(largeMeshData.Normals);
            mesh.SetTriangles(largeMeshData.Tris, 0);
            mesh.SetUVs(0, largeMeshData.UVs);
            mesh.SetUVs(1, largeMeshData.UV2s);

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            mesh.UploadMeshData(markNoLongerReadable: true);
            return mesh;

        }

        public static async Task<Mesh> MergeLargeMeshDataAsyncParallel(List<Quad> quads, BlockType[] chunkData, IndexFormat format = IndexFormat.UInt16)
        {
            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Vector2> uv2s = new List<Vector2>();
            List<int> tris = new List<int>();

            int[] dims = new int[] { 3, 3, 3 };
            Debug.Log($"Length: {chunkData.Length}");
            int[] volumes = new int[chunkData.Length];
            for (int i = 0; i < volumes.Length; i++)
            {
                volumes[i] = (int)chunkData[i];
            }
            var greedy = GreedyMeshing(volumes, dims);
            LogUtils.Log(greedy, "Greedy.txt");
   

            await Task.Run(() =>
            {
                foreach(var quad in greedy)
                {
                    verts.AddRange(quad);
                }

                for (int i = 0; i < greedy.Count; i++)
                {
                    for(int j = 0; j < greedy[i].Length; j++)
                    {
                        verts.Add(greedy[i][j]);

                    
                    }

                    int baseIndex = i * 4;  // Each quad has 4 vertices      
                    tris.Add(baseIndex + 0);
                    tris.Add(baseIndex + 3);
                    tris.Add(baseIndex + 1);
                    tris.Add(baseIndex + 1);
                    tris.Add(baseIndex + 3);
                    tris.Add(baseIndex + 2);
                }
            });

            Mesh mesh = new Mesh();
            mesh.indexFormat = format;


            mesh.SetVertices(verts);
            //mesh.SetNormals(norms);
            mesh.SetTriangles(tris, 0);
            //mesh.SetUVs(0, uvs);
            //mesh.SetUVs(1, uv2s);

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();


            LogUtils.WriteMeshToFile(mesh, "MeshData.txt");
            mesh.UploadMeshData(markNoLongerReadable: true);
            return mesh;
        }


        public static List<Vector3[]> GreedyMeshing(int[] volumes, int[] dims)
        {
            // helper function to access volume data.
            int f(int i, int j, int k)
            {
                return volumes[i + dims[0] * (j + dims[1] * k)];
            }

            // Swap over 3 -axes
            List<Vector3[]> quads = new List<Vector3[]>();
            for (int d = 0; d < 3; d++)
            {
                // i, j: used to iterated over the X and Y dimensions of the 3D volume
                // k, l: used as a loop counter for addition iterations within the inner loops
                // w, h: used to represent the width and height of a quad, respectively
                int i, j, k, l, w, h;
                int u = (d + 1) % 3;        // These variables are used to determine the other two dimensions (axes) based on the current dimension
                int v = (d + 2) % 3;        // `d`. They are computed using modular arithmetic to ensure that `u` and `v` are distinct form `d`
                                            // and each other. This is important for selecting the correct neighboring voxel in the subsequent code.

                int[] x = { 0, 0, 0 };  // This array represents the current position in the 3D volume along the X, Y and Z axes. It is used
                                        // to iterate over the entire volume.

                int[] q = { 0, 0, 0 };   // This array is used to specify the direction of the neighboring voxel in the current dimension
                                         // `d`. It is set to {0, 0, 0} initially and later modified to have `1` in the `d`-th dimension.


                int[] mask = new int[dims[u] * dims[v]];    // This array is used to store a binary mask representing whether each voxel in 
                                                            // a 2D slice of the 3D volume is part of the surface. The size of the mask is
                                                            // determined by the dimensions `dims[u]` and `dims[v]` in the plane orthogonal
                                                            // to the current dimentsion `d`.


                q[d] = 1;   // Indicating the direction of the neighboring voxel along the current dimension `d`. This is used to check the
                            // voxel value at the neighboring position in the volume.



                // Check each slide of chunk one at a time
                for (x[d] = -1; x[d] < dims[d];)
                {
                    // Compute mask
                    int n = 0;
                    for (x[v] = 0; x[v] < dims[v]; ++x[v])
                        for (x[u] = 0; x[u] < dims[u]; ++x[u])
                        {
                            // Store whether the voxel is solid (not zero) or not
                            var result = (0 <= x[d] ? f(x[0], x[1], x[2]) : 0) !=
                                        (x[d] < dims[d] - 1 ? f(x[0] + q[0], x[1] + q[1], x[2] + q[2]) : 0);


                            if (result == false)
                                mask[n++] = 0;
                            else
                                mask[n++] = 1;

                            Debug.Log(n);
                        }


                    // Increment x[d]
                    ++x[d];

                    // Generate mesh for mask using lexicographic ordering
                    n = 0;
                    for (j = 0; j < dims[v]; j++)
                    {
                        for (i = 0; i < dims[u];)
                        {
                            if (mask[n] != 0)
                            {
                                // Compute width
                                for (w = 1; mask[n + v] != 0 && i + w < dims[u]; w++)
                                {

                                }


                                // Compute height
                                bool done = false;
                                for (h = 1; j + h < dims[v]; h++)
                                {
                                    for (k = 0; k < w; k++)
                                    {
                                        // Check if the voxel in the height is solid
                                        if (mask[n + k + h * dims[u]] == 0)
                                        {
                                            done = true;
                                            break;
                                        }
                                    }


                                    if (done)
                                    {
                                        break;
                                    }

                                }

                                // Add quad vertices
                                x[u] = i;
                                x[v] = j;
                                int[] du = { 0, 0, 0 };
                                du[u] = w;
                                int[] dv = { 0, 0, 0 };
                                dv[v] = h;
                                quads.Add(new Vector3[]
                                {
                                        new Vector3(x[0], x[1], x[2]),
                                        new Vector3(x[0] + du[0], x[1] + du[1], x[2] + du[2]),
                                        new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]),
                                        new Vector3(x[0] + dv[0], x[1] + dv[1], x[2] + dv[2])
                                });


                                // Zero-out mask
                                for (l = 0; l < h; l++)
                                {
                                    for (k = 0; k < w; k++)
                                    {
                                        mask[n + k + l * dims[u]] = 0;
                                    }
                                }

                                // Increment counter and continue
                                i += w;
                                n += w;
                            }
                            else
                            {
                                // Move to the next voxel
                                i++;
                                n++;
                            }
                        }
                    }
                }

            }

            return quads;
        }

        private static bool IsVoxelSolid(int[] x, int d, Func<int, int, int, int> f, int[] dims, int[] q)
        {
            // Determine the coordinates for the current voxel
            int voxelX = x[0];
            int voxelY = x[1];
            int voxelZ = x[2];


            // Determine the value of the current voxel (at coordinates voxelX, voxelY, voxelZ)
            int currentVoxelValue;
            if (voxelX > 0)
            {
                currentVoxelValue = f(voxelX, voxelY, voxelZ);
            }
            else
            {
                currentVoxelValue = 0;
            }


            // Determine the value of the neighboring voxel along the specified axis `d`
            int neighborVoxelValue;
            if (voxelX < dims[d] - 1)
            {
                neighborVoxelValue = f(voxelX + q[0], voxelY + q[1], voxelZ + q[2]);
            }
            else
            {
                neighborVoxelValue = 0;
            }

            // Check if the voxel is solid (not zero) or not
            return currentVoxelValue != neighborVoxelValue;
        }
    }


}
