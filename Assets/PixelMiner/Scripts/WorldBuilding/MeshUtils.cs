using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using PixelMiner.Utilities;
using System.Collections.Generic;
using PixelMiner.Enums;


namespace PixelMiner.WorldBuilding
{
    public static class MeshUtils
    {
        private static ChunkMeshBuilder _builder = new ChunkMeshBuilder();

        public static Vector2[,] BlockUVs =
        {
            /*
             * Order: BOTTOM_LEFT -> BOTTOM_RIGHT -> TOP_LEFT -> TOP_RIGHT.
             */
            /*AIR*/
            {new Vector2(0.4375f, 0.0625f), new Vector2(0.5f, 0.0625f),
            new Vector2(0.4375f, 0.125f), new Vector2(0.5f, 0.125f)},

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



        //public static async Task<Mesh> MergeMeshAsync(MeshData[] mesheDataArray, IndexFormat format = IndexFormat.UInt16)
        //{
        //    Vector3[] verts = new Vector3[0];
        //    Vector3[] norms = new Vector3[0];
        //    int[] tris = new int[0]; ;
        //    Vector2[] uvs = new Vector2[0];
        //    Vector2[] uv2s = new Vector2[0];

        //    await Task.Run(() =>
        //    {
        //        int triCount = 0;
        //        int vertCount = 0;
        //        int normalCount = 0;
        //        int uvCount = 0;

        //        for (int i = 0; i < mesheDataArray.Length; i++)
        //        {
        //            if (mesheDataArray[i].Equals(default(MeshData)))
        //            {
        //                Debug.Log("Mesh data array is emtpy");
        //                continue;
        //            }

        //            triCount += mesheDataArray[i].Triangles.Length;
        //            vertCount += mesheDataArray[i].Vertices.Length;
        //            normalCount += mesheDataArray[i].Normals.Length;
        //            uvCount += mesheDataArray[i].UVs.Length;
        //        }
        //        verts = new Vector3[vertCount];
        //        norms = new Vector3[normalCount];
        //        tris = new int[triCount];
        //        uvs = new Vector2[uvCount];
        //        uv2s = new Vector2[uvCount];

        //        for (int i = 0; i < mesheDataArray.Length; i++)  // Loop through each mesh
        //        {
        //            if (mesheDataArray[i].Equals(default(MeshData)))
        //            {
        //                Debug.Log("Mesh data array is emtpy");
        //                continue;
        //            }

        //            for (int j = 0; j < mesheDataArray[i].Triangles.Length; j++)
        //            {
        //                int triPoint = (i * mesheDataArray[i].Vertices.Length + mesheDataArray[i].Triangles[j]);
        //                tris[j + i * mesheDataArray[i].Triangles.Length] = triPoint;
        //            }


        //            for (int v = 0; v < mesheDataArray[i].Vertices.Length; v++)
        //            {
        //                int vertexIndex = v + i * mesheDataArray[i].Vertices.Length;
        //                verts[vertexIndex] = mesheDataArray[i].Vertices[v];
        //                norms[vertexIndex] = mesheDataArray[i].Normals[v];
        //                uvs[vertexIndex] = mesheDataArray[i].UVs[v];
        //                uv2s[vertexIndex] = mesheDataArray[i].UV2s[v];
        //            }
        //        }
        //    });

        //    Mesh mesh = new Mesh();
        //    mesh.indexFormat = format;
        //    mesh.vertices = verts;
        //    mesh.normals = norms;
        //    mesh.uv = uvs;
        //    mesh.uv2 = uv2s;
        //    mesh.triangles = tris;

        //    mesh.RecalculateNormals();
        //    mesh.RecalculateBounds();

        //    return mesh;
        //}


        //public static async Task<Mesh> MergeMeshAsyncParallel(MeshData[] meshes, IndexFormat format = IndexFormat.UInt16)
        //{

            //Vector3[] verts = null;
            //Vector3[] norms = null;
            //int[] tris = null; ;
            //Vector2[] uvs = null;
            //Vector2[] uv2s = null;
            //int triCount = 0;
            //int vertCount = 0;
            //int normalCount = 0;
            //int uvCount = 0;

            //await Task.Run(() =>
            //{
            //    for (int i = 0; i < meshes.Length; i++)
            //    {
            //        triCount += meshes[i].Triangles.Length;
            //        vertCount += meshes[i].Vertices.Length;
            //        normalCount += meshes[i].Normals.Length;
            //        uvCount += meshes[i].UVs.Length;
            //    }
            //    verts = new Vector3[vertCount];
            //    norms = new Vector3[normalCount];
            //    tris = new int[triCount];
            //    uvs = new Vector2[uvCount];
            //    uv2s = new Vector2[uvCount];

            //    Parallel.For(0, meshes.Length, i =>
            //    {
            //        for (int j = 0; j < meshes[i].Triangles.Length; j++)
            //        {
            //            int triPoint = (i * meshes[i].Vertices.Length + meshes[i].Triangles[j]);
            //            tris[j + i * meshes[i].Triangles.Length] = triPoint;
            //        }


            //        for (int v = 0; v < meshes[i].Vertices.Length; v++)
            //        {
            //            int vertexIndex = v + i * meshes[i].Vertices.Length;
            //            verts[vertexIndex] = meshes[i].Vertices[v];
            //            norms[vertexIndex] = meshes[i].Normals[v];
            //            uvs[vertexIndex] = meshes[i].UVs[v];
            //            uv2s[vertexIndex] = meshes[i].UV2s[v];
            //        }
            //    });
            //});


            //Mesh mesh = new Mesh();
            //mesh.indexFormat = format;

            //mesh.vertices = verts;
            //mesh.normals = norms;
            //mesh.uv = uvs;
            //mesh.uv2 = uv2s;
            //mesh.triangles = tris;
            //mesh.RecalculateNormals();
            //mesh.RecalculateBounds();
            //mesh.UploadMeshData(markNoLongerReadable: true);

            //return mesh;
        //}


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

            //int[] dims = new int[] { 1, 3, 2 };
            //Debug.Log($"Length: {chunkData.Length}");
            //int[] volumes = new int[chunkData.Length];
            //for (int i = 0; i < volumes.Length; i++)
            //{
            //    volumes[i] = 1;
            //}
            //var greedy = GreedyMeshing2(volumes, dims,1,3,2);
            //LogUtils.Log(greedy, "Greedy.txt");


            //await Task.Run(() =>
            //{
            //    foreach (var quad in greedy)
            //    {
            //        verts.AddRange(quad);
            //    }

            //    for (int i = 0; i < greedy.Count; i++)
            //    {
            //        for (int j = 0; j < greedy[i].Length; j++)
            //        {
            //            verts.Add(greedy[i][j]);


            //        }

            //        int baseIndex = i * 4;  // Each quad has 4 vertices      
            //        tris.Add(baseIndex + 0);
            //        tris.Add(baseIndex + 3);
            //        tris.Add(baseIndex + 1);
            //        tris.Add(baseIndex + 1);
            //        tris.Add(baseIndex + 3);
            //        tris.Add(baseIndex + 2);
            //    }
            //});

            await Task.Run(() =>
            {


                for (int i = 0; i < quads.Count; i++)
                {

                    verts.AddRange(quads[i]._vertices);


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


            //LogUtils.WriteMeshToFile(mesh, "MeshData.txt");
            mesh.UploadMeshData(markNoLongerReadable: true);
            return mesh;
        }



        public static MeshData GreedyMeshing(Chunk chunk)
        {
            bool GreedyCompareLogic(Vector3Int a, Vector3Int b, int dimension, bool isBackFace)
            {
                BlockType blockA = chunk.GetBlock(a);
                BlockType blockB = chunk.GetBlock(b);

                return blockA == blockB && chunk.IsSolid(b) && chunk.IsBlockFaceVisible(b, dimension, isBackFace);
            }

            //ChunkMeshBuilder _builder = new ChunkMeshBuilder();

            bool[,] merged;

            Vector3Int startPos, currPos, quadSize, m, n, offsetPos;
            Vector3[] vertices;
            Vector3[] uvs = new Vector3[4];
            Vector2[] uv2s = new Vector2[4];
            BlockType currBlock;
            int d, u, v;
            Vector3Int dimensions = chunk.Dimensions;

          
            // Iterate over each aface of the blocks.
            for (int voxelFace = 0; voxelFace < 6; voxelFace++)
            {
                /* Voxel Face Index
                * 0: Right
                * 1: Up
                * 2: Front
                * 3: Left
                * 4: Down 
                * 5: Back
                * 
                * BackFace -> Face that drawn in clockwise direction. (Need detect which face is clockwise in order to draw it on 
                * Unity scene).
                */
                //if(voxelFace == 4) continue;

                bool isBackFace = voxelFace > 2;
                d = voxelFace % 3;
                u = (d + 1) % 3;
                v = (d + 2) % 3;

                startPos = new Vector3Int();
                currPos = new Vector3Int();

                for (startPos[d] = 0; startPos[d] < dimensions[d]; startPos[d]++)
                {
                    merged = new bool[dimensions[u], dimensions[v]];

                    // Build the slices of mesh.
                    for (startPos[u] = 0; startPos[u] < dimensions[u]; startPos[u]++)
                    {
                        for (startPos[v] = 0; startPos[v] < dimensions[v]; startPos[v]++)
                        {
                            currBlock = chunk.GetBlock(startPos);

                            // If this block has already been merged, is air, or not visible -> skip it.
                            if (chunk.IsSolid(startPos) == false ||
                                chunk.IsBlockFaceVisible(startPos, d, isBackFace) == false ||
                                merged[startPos[u], startPos[v]])
                            {
                                continue;
                            }
                   

                            quadSize = new Vector3Int();

                            // Next step is loop to figure out width and height of the new merged quad.
                            for (currPos = startPos, currPos[u]++;
                                currPos[u] < dimensions[u] &&
                                GreedyCompareLogic(startPos, currPos, d, isBackFace) &&
                                !merged[currPos[u], currPos[v]];
                                currPos[u]++)
                            { }
                            quadSize[u] = currPos[u] - startPos[u];


                            for (currPos = startPos, currPos[v]++;
                                currPos[v] < dimensions[v] &&
                                GreedyCompareLogic(startPos, currPos, d, isBackFace) &&
                                !merged[currPos[u], currPos[v]];
                                currPos[v]++)
                            {
                                for (currPos[u] = startPos[u];
                                    currPos[u] < dimensions[u] &&
                                    GreedyCompareLogic(startPos, currPos, d, isBackFace) &&
                                    !merged[currPos[u], currPos[v]];
                                    currPos[u]++)
                                { }


                                if (currPos[u] - startPos[u] < quadSize[u])
                                {
                                    break;
                                }
                                else
                                {
                                    currPos[u] = startPos[u];
                                }
                            }

                            quadSize[v] = currPos[v] - startPos[v];


                            // Add new quad to mesh data.
                            m = new Vector3Int();
                            n = new Vector3Int();

                            m[u] = quadSize[u];
                            n[v] = quadSize[v];

                            offsetPos = startPos;
                            offsetPos[d] += isBackFace ? 0 : 1;

                            vertices = new Vector3[]
                            {
                                offsetPos,
                                offsetPos + m,
                                offsetPos + m + n,
                                offsetPos + n
                            };

                            GetBlockUVs(currBlock, voxelFace, quadSize[u], quadSize[v], ref uvs, ref uv2s);
                            _builder.AddQuadFace(vertices, uvs, uv2s, isBackFace);


                            // Mark at this position has been merged
                            for (int g = 0; g < quadSize[u]; g++)
                            {
                                for (int h = 0; h < quadSize[v]; h++)
                                {
                                    merged[startPos[u] + g, startPos[v] + h] = true;
                                }
                            }

                        }
                    }


                }
            }
            return _builder.ToMeshData();
        }


        private static void GetBlockUVs(BlockType blockType, int face, int width, int height, ref Vector3[] uvs, ref Vector2[] uv2s)
        {
            int blockIndex;
            ColorMapType colorMapType;
            if (blockType == BlockType.GrassSide)
            {
                if(face == 1)
                {
                    blockIndex = (ushort)BlockType.GrassTop;
                    colorMapType = ColorMapType.Plains;
                }
                else if (face == 4)
                {
                    blockIndex = (ushort)BlockType.Dirt;
                }
                else
                {
                    blockIndex = (ushort)blockType;
                }
            }
            else
            {
                blockIndex = (ushort)blockType;
            }

            if (blockType == BlockType.GrassSide && face == 1)
            {
                colorMapType = ColorMapType.Plains;
            }
            else
            {
                colorMapType = ColorMapType.None;
            }

            uvs[0] = new Vector3(0, 0, blockIndex);
            uvs[1] = new Vector3(width, 0, blockIndex);
            uvs[2] = new Vector3(width, height, blockIndex);
            uvs[3] = new Vector3(0, height, blockIndex);

            GetColorMapkUVs(colorMapType, ref uv2s);
        }
        private static void GetColorMapkUVs(ColorMapType colormapType, ref Vector2[] colormapUVs)
        {
            colormapUVs[0] = ColorMapUVs[(ushort)colormapType, 0];
            colormapUVs[1] = ColorMapUVs[(ushort)colormapType, 1];
            colormapUVs[2] = ColorMapUVs[(ushort)colormapType, 2];
            colormapUVs[3] = ColorMapUVs[(ushort)colormapType, 3];
        }
    }
}
