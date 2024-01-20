using System.Threading.Tasks;
using UnityEngine;
using PixelMiner.Enums;
using System;
using PixelMiner.Lighting;
using PixelMiner.Utilities;
using PixelMiner.World;
using System.Collections.Generic;

namespace PixelMiner.WorldBuilding
{
    /* Voxel Face Index
        * 0: Right
        * 1: Up
        * 2: Front
        * 3: Left
        * 4: Down 
        * 5: Back
        */

    public static class MeshUtils
    {
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


        public static Vector2[,] LightMapUVs =
        {
             {new Vector2(0.875f, 1f), new Vector2(0.9375f, 1f),
             new Vector2(0.875f, 0.9375f), new Vector2(0.9375f, 0.9375f)},

            {new Vector2(0.875f, 0.9375f), new Vector2(0.9375f, 0.9375f),
             new Vector2(0.875f, 0.875f), new Vector2(0.9375f, 0.875f)},

            {new Vector2(0.875f, 0.875f), new Vector2(0.9375f, 0.875f),
             new Vector2(0.875f, 0.8125f), new Vector2(0.9375f, 0.8125f)},

            {new Vector2(0.875f, 0.8125f), new Vector2(0.9375f, 0.8125f),
             new Vector2(0.875f, 0.75f), new Vector2(0.9375f, 0.75f)},

            {new Vector2(0.875f, 0.75f), new Vector2(0.9375f, 0.75f),
             new Vector2(0.875f, 0.6875f), new Vector2(0.9375f, 0.6875f)},

            {new Vector2(0.875f, 0.6875f), new Vector2(0.9375f, 0.6875f),
             new Vector2(0.875f, 0.625f), new Vector2(0.9375f, 0.625f)},

            {new Vector2(0.875f, 0.625f), new Vector2(0.9375f, 0.625f),
             new Vector2(0.875f, 0.5625f), new Vector2(0.9375f, 0.5625f)},

            {new Vector2(0.875f, 0.5625f), new Vector2(0.9375f, 0.5625f),
             new Vector2(0.875f, 0.5f), new Vector2(0.9375f, 0.5f)},

            {new Vector2(0.875f, 0.5f), new Vector2(0.9375f, 0.5f),
             new Vector2(0.875f, 0.4375f), new Vector2(0.9375f, 0.4375f)},

            {new Vector2(0.875f, 0.4375f), new Vector2(0.9375f, 0.4375f),
             new Vector2(0.875f, 0.375f), new Vector2(0.9375f, 0.375f)},

            {new Vector2(0.875f, 0.375f), new Vector2(0.9375f, 0.375f),
             new Vector2(0.875f, 0.3125f), new Vector2(0.9375f, 0.3125f)},

            {new Vector2(0.875f, 0.3125f), new Vector2(0.9375f, 0.3125f),
             new Vector2(0.875f, 0.25f), new Vector2(0.9375f, 0.25f)},

            {new Vector2(0.875f, 0.25f), new Vector2(0.9375f, 0.25f),
             new Vector2(0.875f, 0.1875f), new Vector2(0.9375f, 0.1875f)},

            {new Vector2(0.875f, 0.1875f), new Vector2(0.9375f, 0.1875f),
             new Vector2(0.875f, 0.125f), new Vector2(0.9375f, 0.125f)},

            {new Vector2(0.875f, 0.125f), new Vector2(0.9375f, 0.125f),
             new Vector2(0.875f, 0.0625f), new Vector2(0.9375f, 0.0625f)},

            {new Vector2(0.875f, 0.0625f), new Vector2(0.9375f, 0.0625f),
             new Vector2(0.875f, 0f), new Vector2(0.9375f, 0f)},
        };


        public static void GetColorMap(ref Vector2[] colorUVs, float heatValue, bool clear = false)
        {
            float tileSize = 1 / 256f;
            float u = 0f;
            //float v = (heatValue) * 256;
            float v = (0.76f) * 256;


            if (clear)
            {
                colorUVs[0] = new Vector2(0.8125f, 0.8125f);
                colorUVs[1] = new Vector2(0.828125f, 0.8125f);
                colorUVs[2] = new Vector2(0.8125f, 0.828125f);
                colorUVs[3] = new Vector2(0.828125f, 0.828125f);
            }
            else
            {
                //colorUVs[0] = new Vector2(tileSize * u, tileSize * v);
                //colorUVs[1] = new Vector2(tileSize * u + tileSize, tileSize * v);
                //colorUVs[2] = new Vector2(tileSize * u, tileSize * v + tileSize);
                //colorUVs[3] = new Vector2(tileSize * u + tileSize, tileSize * v + tileSize);


                colorUVs[0] = new Vector2(0.15625f, 0.796875f);
                colorUVs[1] = new Vector2(0.171875f, 0.796875f);
                colorUVs[2] = new Vector2(0.15625f, 0.8125f);
                colorUVs[3] = new Vector2(0.171875f, 0.8125f);
            }
        }

        public static Vector2[] GetDepthUVs(float depth)
        {
            float depthValue = MathHelper.Map(depth, 0.2f, 0.45f, 0.0f, 0.75f);
            float offset = 0.05f;

            return new Vector2[]
            {
                new Vector2(0, depthValue),
                new Vector2(1, depthValue),
                 new Vector2(0f, Mathf.Clamp01(depthValue + offset)),
                new Vector2(1, Mathf.Clamp01(depthValue + offset))
            };
        }

        private static float MapValue(float value, float originalMin, float originalMax, float targetMin, float targetMax)
        {
            // Ensure the value is within the original range
            float clampedValue = Mathf.Clamp(value, originalMin, originalMax);

            // Perform the mapping
            float mappedValue = targetMin + (clampedValue - originalMin) / (originalMax - originalMin) * (targetMax - targetMin);

            return mappedValue;
        }

        public static Color32 GetLightColor(byte light, AnimationCurve lightAnimCurve)
        {
            float maxLight = 150.0f;
            float channelValue = lightAnimCurve.Evaluate(light / maxLight);
            byte lightValue = (byte)Mathf.Clamp(channelValue * 255, 0, 255);
            return new Color32(lightValue, lightValue, lightValue, 255);
        }


        private static byte GetBlockLightPropagationForAdjacentFace(Chunk chunk, Vector3Int blockPosition, int voxelFace)
        {
            Vector3Int offset = Vector3Int.up;
            switch (voxelFace)
            {
                case 0:
                    offset = Vector3Int.right;
                    break;
                case 1:
                    offset = Vector3Int.up;
                    break;
                case 2:
                    offset = Vector3Int.forward;
                    break;
                case 3:
                    offset = Vector3Int.left;
                    break;
                case 4:
                    offset = Vector3Int.down;
                    break;
                case 5:
                    offset = Vector3Int.back;
                    break;
                default:
                    offset = Vector3Int.zero;
                    break;
            }

            Vector3Int blockOffsetPosition = blockPosition + offset;
            return chunk.GetBlockLight(blockOffsetPosition);
        }
        private static byte GetAmbientLightPropagationForAdjacentFace(Chunk chunk, Vector3Int blockPosition, int face)
        {
            Vector3Int offset;
            switch (face)
            {
                case 0:
                    offset = Vector3Int.right;
                    break;
                case 1:
                    offset = Vector3Int.up;
                    break;
                case 2:
                    offset = Vector3Int.forward;
                    break;
                case 3:
                    offset = Vector3Int.left;
                    break;
                case 4:
                    offset = Vector3Int.down;
                    break;
                case 5:
                    offset = Vector3Int.back;
                    break;
                default:
                    offset = Vector3Int.zero;
                    break;

            }

            Vector3Int blockOffsetPosition = blockPosition + offset;
            blockOffsetPosition = new Vector3Int(Mathf.Clamp(blockOffsetPosition.x, 0, chunk._width),
                                           Mathf.Clamp(blockOffsetPosition.y, 0, chunk._height),
                                           Mathf.Clamp(blockOffsetPosition.z, 0, chunk._depth));
            return chunk.GetAmbientLight(blockOffsetPosition);
        }


        private static async Task<ChunkMeshBuilder> RenderChunkFace(Chunk chunk, int voxelFace, AnimationCurve lightAnimCurve, bool isTransparentMesh = false)
        {
            ChunkMeshBuilder builder = ChunkMeshBuilderPool.Get();
            builder.InitOrLoad(chunk.Dimensions);

            Vector3Int dimensions = chunk.Dimensions;
            bool isBackFace = voxelFace > 2;
            int d = voxelFace % 3;
            int u, v;

            Vector3Int startPos, currPos, quadSize = Vector3Int.one, m, n, offsetPos;
            Vector3[] vertices = new Vector3[4];
            Vector3[] uvs = new Vector3[4];
            Vector2[] uv2s = new Vector2[4];
            Vector4[] uv3s = new Vector4[4];
            Color32[] colors = new Color32[4];
            byte[] verticesAO = new byte[4];
            byte[] vertexColorIntensity = new byte[4];
            Vector3Int[] faceNeighbors = new Vector3Int[6];
            BlockType currBlock;
            Color lightColor;
            bool smoothLight = true;
            bool greedyMeshing = false;


            await Task.Run(() =>
            {
                switch (d)
                {
                    case 0:
                        u = 2;
                        v = 1;
                        break;
                    case 1:
                        u = 0;
                        v = 2;
                        break;
                    default:
                        u = 0;
                        v = 1;
                        break;
                }


                startPos = new Vector3Int();
                currPos = new Vector3Int();


                for (startPos[d] = 0; startPos[d] < dimensions[d]; startPos[d]++)
                {
                    Array.Clear(builder.Merged[d], 0, builder.Merged[d].Length);

                    // Build the slices of mesh.
                    for (startPos[u] = 0; startPos[u] < dimensions[u]; startPos[u]++)
                    {
                        for (startPos[v] = 0; startPos[v] < dimensions[v]; startPos[v]++)
                        {
                            currBlock = chunk.GetBlock(startPos);
                            if (currBlock == BlockType.Air) continue;



                            // If this block has already been merged, is air, or not visible -> skip it.
                            //if (chunk.IsSolid(startPos) == false ||
                            //    chunk.IsBlockFaceVisible(startPos, d, isBackFace) == false ||
                            //    builder.Merged[d][startPos[u], startPos[v]])
                            //{
                            //    continue;
                            //}

                            if (builder.Merged[d][startPos[u], startPos[v]])
                            {
                                continue;
                            }


                            if (isTransparentMesh)
                            {
                                // TRANSPARENT SOLID
                                if (chunk.GetBlock(startPos).IsTransparentSolidBlock() == false)
                                {
                                    continue;
                                }

                                if (chunk.IsBlockFaceVisible(startPos, d, isBackFace) == false)
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                // OPAQUE SOLID
                                if (chunk.GetBlock(startPos).IsSolid() == false ||
                                    chunk.IsBlockFaceVisible(startPos, d, isBackFace) == false)
                                {
                                    continue;
                                }

                                if (chunk.GetBlock(startPos).IsTransparentSolidBlock() == true)
                                {
                                    continue;
                                }
                            }



                            // Ambient occlusion
                            // =================
                            bool anisotropy = false;

                            verticesAO[0] = (byte)VoxelAO.ProcessAO(chunk, startPos, 0, voxelFace, ref faceNeighbors);
                            verticesAO[1] = (byte)VoxelAO.ProcessAO(chunk, startPos, 1, voxelFace, ref faceNeighbors);
                            verticesAO[2] = (byte)VoxelAO.ProcessAO(chunk, startPos, 2, voxelFace, ref faceNeighbors);
                            verticesAO[3] = (byte)VoxelAO.ProcessAO(chunk, startPos, 3, voxelFace, ref faceNeighbors);



                            if (greedyMeshing)
                            {
                                //quadSize = new Vector3Int();
                                //// Next step is loop to figure out width and height of the new merged quad.
                                //for (currPos = startPos, currPos[u]++;
                                //    currPos[u] < dimensions[u] &&
                                //    GreedyCompareLogic(startPos, currPos, d, isBackFace, voxelFace) &&
                                //    GreeyAO(chunk, startPos, voxelFace, chunk, currPos, voxelFace) &&
                                //    !builder.Merged[d][currPos[u], currPos[v]];
                                //    currPos[u]++)
                                //{ }
                                //quadSize[u] = currPos[u] - startPos[u];

                                //for (currPos = startPos, currPos[v]++;
                                //    currPos[v] < dimensions[v] &&
                                //    GreedyCompareLogic(startPos, currPos, d, isBackFace, voxelFace) &&
                                //     GreeyAO(chunk, startPos, voxelFace, chunk, currPos, voxelFace) &&
                                //    !builder.Merged[d][currPos[u], currPos[v]];
                                //    currPos[v]++)
                                //{


                                //    for (currPos[u] = startPos[u];
                                //        currPos[u] < dimensions[u] &&
                                //        GreedyCompareLogic(startPos, currPos, d, isBackFace, voxelFace) &&
                                //         GreeyAO(chunk, startPos, voxelFace, chunk, currPos, voxelFace) &&
                                //        !builder.Merged[d][currPos[u], currPos[v]];
                                //        currPos[u]++)
                                //    { }


                                //    if (currPos[u] - startPos[u] < quadSize[u])
                                //    {
                                //        break;
                                //    }
                                //    else
                                //    {
                                //        currPos[u] = startPos[u];
                                //    }
                                //}

                                //quadSize[v] = currPos[v] - startPos[v];
                            }
                            else
                            {
                                quadSize = Vector3Int.one;
                            }



                            // Add new quad to mesh data.
                            m = new Vector3Int();
                            n = new Vector3Int();

                            m[u] = quadSize[u];
                            n[v] = quadSize[v];

                            offsetPos = startPos;
                            offsetPos[d] += isBackFace ? 0 : 1;


                            vertices[0] = offsetPos;
                            vertices[1] = offsetPos + m;
                            vertices[2] = offsetPos + m + n;
                            vertices[3] = offsetPos + n;






                            // BLock light
                            // ===========
                            // Because at solid block light not exist. We can only get light by the adjacent block and use it as the light as solid voxel face.
                            byte lightValue = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                            lightColor = GetLightColor(lightValue, lightAnimCurve);

                            vertexColorIntensity[0] = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                            vertexColorIntensity[1] = GetBlockLightPropagationForAdjacentFace(chunk, startPos + m, voxelFace);
                            vertexColorIntensity[2] = GetBlockLightPropagationForAdjacentFace(chunk, startPos + m + n, voxelFace);
                            vertexColorIntensity[3] = GetBlockLightPropagationForAdjacentFace(chunk, startPos + n, voxelFace);

                            if (smoothLight)
                            {
                                if (voxelFace == 1 || voxelFace == 5 || voxelFace == 0)
                                {
                                    if (vertexColorIntensity[0] == 0)
                                    {
                                        colors[0] = lightColor;
                                    }
                                    else
                                    {
                                        colors[0] = GetLightColor(vertexColorIntensity[0], lightAnimCurve);
                                    }
                                    if (vertexColorIntensity[1] == 0)
                                    {
                                        colors[1] = lightColor;
                                    }
                                    else
                                    {
                                        colors[1] = GetLightColor(vertexColorIntensity[1], lightAnimCurve);
                                    }
                                    if (vertexColorIntensity[2] == 0)
                                    {
                                        colors[2] = lightColor;
                                    }
                                    else
                                    {
                                        colors[2] = GetLightColor(vertexColorIntensity[2], lightAnimCurve);
                                    }
                                    if (vertexColorIntensity[3] == 0)
                                    {
                                        colors[3] = lightColor;
                                    }
                                    else
                                    {
                                        colors[3] = GetLightColor(vertexColorIntensity[3], lightAnimCurve);
                                    }
                                }
                                else if (voxelFace == 2 || voxelFace == 3)
                                {
                                    if (vertexColorIntensity[1] == 0)
                                    {
                                        colors[0] = lightColor;
                                    }
                                    else
                                    {
                                        colors[0] = GetLightColor(vertexColorIntensity[1], lightAnimCurve);
                                    }
                                    if (vertexColorIntensity[0] == 0)
                                    {
                                        colors[1] = lightColor;
                                    }
                                    else
                                    {
                                        colors[1] = GetLightColor(vertexColorIntensity[0], lightAnimCurve);
                                    }
                                    if (vertexColorIntensity[3] == 0)
                                    {
                                        colors[2] = lightColor;
                                    }
                                    else
                                    {
                                        colors[2] = GetLightColor(vertexColorIntensity[3], lightAnimCurve);
                                    }
                                    if (vertexColorIntensity[2] == 0)
                                    {
                                        colors[3] = lightColor;
                                    }
                                    else
                                    {
                                        colors[3] = GetLightColor(vertexColorIntensity[2], lightAnimCurve);
                                    }
                                }
                                else
                                {
                                    colors[0] = GetLightColor(vertexColorIntensity[0], lightAnimCurve);
                                    colors[1] = GetLightColor(vertexColorIntensity[0], lightAnimCurve);
                                    colors[2] = GetLightColor(vertexColorIntensity[0], lightAnimCurve);
                                    colors[3] = GetLightColor(vertexColorIntensity[0], lightAnimCurve);
                                }
                            }
                            else
                            {
                                colors[0] = GetLightColor(vertexColorIntensity[0], lightAnimCurve);
                                colors[1] = GetLightColor(vertexColorIntensity[0], lightAnimCurve);
                                colors[2] = GetLightColor(vertexColorIntensity[0], lightAnimCurve);
                                colors[3] = GetLightColor(vertexColorIntensity[0], lightAnimCurve);
                            }


                            // Ambient Lights
                            byte ambientLight = chunk.GetAmbientLight(startPos);


                            GetBlockUVs(currBlock, voxelFace, quadSize[u], quadSize[v], chunk.GetHeat(currPos), ref uvs, ref uv2s, ref uv3s);
                            builder.AddQuadFace(vertices, uvs, uv2s, uv3s, colors, voxelFace, verticesAO, anisotropy, ambientLight);


                            // Mark at this position has been merged
                            for (int g = 0; g < quadSize[u]; g++)
                            {
                                for (int h = 0; h < quadSize[v]; h++)
                                {
                                    builder.Merged[d][startPos[u] + g, startPos[v] + h] = true;
                                }
                            }
                        }
                    }
                }
            });


            return builder;

            //MeshData meshData = builder.ToMeshData();
            //ChunkMeshBuilderPool.Release(builder);
            //return meshData;
        }

        public static async Task<MeshData> RenderSolidMesh(Chunk chunk, AnimationCurve lightAnimCurve, bool isTransparentMesh = false)
        {
            bool GreedyCompareLogic(Vector3Int a, Vector3Int b, int dimension, bool isBackFace, int voxelFace)
            {
                BlockType blockA = chunk.GetBlock(a);
                BlockType blockB = chunk.GetBlock(b);

                Vector3Int c = b + new Vector3Int(1, 0, 1);
                Vector3Int d = b + new Vector3Int(-1, 0, 1);
                Vector3Int e = b + new Vector3Int(1, 0, -1);

                return blockA == blockB &&
                       chunk.GetBlock(b).IsSolid() == false &&
                       //chunk.GetBlock(b).IsTransparentSolidBlock() == true &&
                       GetBlockLightPropagationForAdjacentFace(chunk, a, voxelFace) == GetBlockLightPropagationForAdjacentFace(chunk, b, voxelFace) &&
                       GetBlockLightPropagationForAdjacentFace(chunk, a, voxelFace) == GetBlockLightPropagationForAdjacentFace(chunk, c, voxelFace) &&
                       GetBlockLightPropagationForAdjacentFace(chunk, a, voxelFace) == GetBlockLightPropagationForAdjacentFace(chunk, d, voxelFace) &&
                       GetBlockLightPropagationForAdjacentFace(chunk, a, voxelFace) == GetBlockLightPropagationForAdjacentFace(chunk, e, voxelFace) &&
                       chunk.IsBlockFaceVisible(b, dimension, isBackFace);
                //chunk.IsTransparentBlockFaceVisible(b, dimension, isBackFace);
            }

            bool GreeyAO(Chunk chunkA, Vector3Int relativePosA, int voxelFaceA, Chunk chunkB, Vector3Int relativePosB, int voxelFaceB)
            {
                //byte a0 = (byte)VoxelAO.ProcessAO(chunkA, relativePosB, 0, voxelFaceA);
                //byte a1 = (byte)VoxelAO.ProcessAO(chunkA, relativePosB, 1, voxelFaceA);
                //byte a2 = (byte)VoxelAO.ProcessAO(chunkA, relativePosB, 2, voxelFaceA);
                //byte a3 = (byte)VoxelAO.ProcessAO(chunkA, relativePosB, 3, voxelFaceA);

                //byte b0 = (byte)VoxelAO.ProcessAO(chunkB, relativePosB, 0, voxelFaceB);
                //byte b1 = (byte)VoxelAO.ProcessAO(chunkB, relativePosB, 1, voxelFaceB);
                //byte b2 = (byte)VoxelAO.ProcessAO(chunkB, relativePosB, 2, voxelFaceB);
                //byte b3 = (byte)VoxelAO.ProcessAO(chunkB, relativePosB, 3, voxelFaceB);

                //bool allEqual = (a0 == b0) && (a1 == b1) && (a2 == b2) && (a3 == b3);

                //if (allEqual)
                //{
                //    Console.WriteLine("All values are equal.");
                //    return true;
                //}
                //else
                //{

                //    Console.WriteLine("Values are not equal.");
                //    return false;
                //}

                return false;
            }



            //ChunkMeshBuilder builder = ChunkMeshBuilderPool.Get();
            //builder.InitOrLoad(chunk.Dimensions);

            ChunkMeshBuilder[] builders = new ChunkMeshBuilder[]
            {
                ChunkMeshBuilderPool.Get(),
                ChunkMeshBuilderPool.Get(),
                ChunkMeshBuilderPool.Get(),
                ChunkMeshBuilderPool.Get(),
                ChunkMeshBuilderPool.Get(),
                ChunkMeshBuilderPool.Get(),
            };

            for (int i = 0; i < builders.Length; i++)
            {
                builders[i].InitOrLoad(chunk.Dimensions);
            }



            Vector3Int startPos, currPos, quadSize, m, n, offsetPos;
            Vector3[] vertices = new Vector3[4];
            Vector3[] uvs = new Vector3[4];
            Vector2[] uv2s = new Vector2[4];
            Vector4[] uv3s = new Vector4[4];
            Color32[] colors = new Color32[4];
            byte[] verticesAO = new byte[4];
            byte[] vertexColorIntensity = new byte[4];
            BlockType currBlock;
            int d, u, v;
            Vector3Int dimensions = chunk.Dimensions;
            Color lightColor;
            bool smoothLight = true;
            bool greedyMeshing = false;

            List<Task<ChunkMeshBuilder>> buildMeshTaskList = new List<Task<ChunkMeshBuilder>>();
            ChunkMeshBuilder finalBuilder = ChunkMeshBuilderPool.Get();
            finalBuilder.InitOrLoad(chunk.Dimensions);

            await Task.Run(async () =>
            {

                //builders[0] = await RenderChunkFace(chunk, 0, lightAnimCurve, isTransparentMesh);
                //builders[1] = await RenderChunkFace(chunk, 1, lightAnimCurve, isTransparentMesh);
                //builders[2] = await RenderChunkFace(chunk, 2, lightAnimCurve, isTransparentMesh);
                //builders[3] = await RenderChunkFace(chunk, 3, lightAnimCurve, isTransparentMesh);
                //builders[4] = await RenderChunkFace(chunk, 4, lightAnimCurve, isTransparentMesh);
                //builders[5] = await RenderChunkFace(chunk, 5, lightAnimCurve, isTransparentMesh);

 
   


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
              
                    buildMeshTaskList.Add(RenderChunkFace(chunk, voxelFace, lightAnimCurve, isTransparentMesh));

                }
            });

            await Task.WhenAll(buildMeshTaskList);
            for(int i = 0; i < buildMeshTaskList.Count; i++)
            {
                finalBuilder.Add(buildMeshTaskList[i].Result);
            }

            //builders[1].Add(builders[2]);
            //builders[1].Add(builders[0]);
            //builders[1].Add(builders[3]);
            //builders[1].Add(builders[4]);
            //builders[1].Add(builders[5]);


            MeshData meshData = finalBuilder.ToMeshData();

            ChunkMeshBuilderPool.Release(finalBuilder);
            for (int i = 0; i < builders.Length; i++)
            {
                ChunkMeshBuilderPool.Release(builders[i]);
            }

            return meshData;
        }


        //public static async Task<MeshData> RenderSolidMesh2(Chunk chunk, AnimationCurve lightAnimCurve, bool isTransparentMesh = false)
        //{
        //    bool GreedyCompareLogic(Vector3Int a, Vector3Int b, int dimension, bool isBackFace, int voxelFace)
        //    {
        //        BlockType blockA = chunk.GetBlock(a);
        //        BlockType blockB = chunk.GetBlock(b);

        //        Vector3Int c = b + new Vector3Int(1, 0, 1);
        //        Vector3Int d = b + new Vector3Int(-1, 0, 1);
        //        Vector3Int e = b + new Vector3Int(1, 0, -1);

        //        return blockA == blockB &&
        //               chunk.GetBlock(b).IsSolid() == false &&
        //               //chunk.GetBlock(b).IsTransparentSolidBlock() == true &&
        //               GetBlockLightPropagationForAdjacentFace(chunk, a, voxelFace) == GetBlockLightPropagationForAdjacentFace(chunk, b, voxelFace) &&
        //               GetBlockLightPropagationForAdjacentFace(chunk, a, voxelFace) == GetBlockLightPropagationForAdjacentFace(chunk, c, voxelFace) &&
        //               GetBlockLightPropagationForAdjacentFace(chunk, a, voxelFace) == GetBlockLightPropagationForAdjacentFace(chunk, d, voxelFace) &&
        //               GetBlockLightPropagationForAdjacentFace(chunk, a, voxelFace) == GetBlockLightPropagationForAdjacentFace(chunk, e, voxelFace) &&
        //               chunk.IsBlockFaceVisible(b, dimension, isBackFace);
        //        //chunk.IsTransparentBlockFaceVisible(b, dimension, isBackFace);
        //    }

        //    bool GreeyAO(Chunk chunkA, Vector3Int relativePosA, int voxelFaceA, Chunk chunkB, Vector3Int relativePosB, int voxelFaceB)
        //    {
        //        byte a0 = (byte)VoxelAO.ProcessAO(chunkA, relativePosB, 0, voxelFaceA);
        //        byte a1 = (byte)VoxelAO.ProcessAO(chunkA, relativePosB, 1, voxelFaceA);
        //        byte a2 = (byte)VoxelAO.ProcessAO(chunkA, relativePosB, 2, voxelFaceA);
        //        byte a3 = (byte)VoxelAO.ProcessAO(chunkA, relativePosB, 3, voxelFaceA);

        //        byte b0 = (byte)VoxelAO.ProcessAO(chunkB, relativePosB, 0, voxelFaceB);
        //        byte b1 = (byte)VoxelAO.ProcessAO(chunkB, relativePosB, 1, voxelFaceB);
        //        byte b2 = (byte)VoxelAO.ProcessAO(chunkB, relativePosB, 2, voxelFaceB);
        //        byte b3 = (byte)VoxelAO.ProcessAO(chunkB, relativePosB, 3, voxelFaceB);

        //        bool allEqual = (a0 == b0) && (a1 == b1) && (a2 == b2) && (a3 == b3);

        //        if (allEqual)
        //        {
        //            Console.WriteLine("All values are equal.");
        //            return true;
        //        }
        //        else
        //        {

        //            Console.WriteLine("Values are not equal.");
        //            return false;
        //        }
        //    }



        //    ChunkMeshBuilder[] builders = new ChunkMeshBuilder[]
        //    {
        //         ChunkMeshBuilderPool.Get(),
        //         ChunkMeshBuilderPool.Get(),
        //         ChunkMeshBuilderPool.Get(),
        //         ChunkMeshBuilderPool.Get(),
        //         ChunkMeshBuilderPool.Get(),
        //         ChunkMeshBuilderPool.Get(),
        //    };
        //    for (int i = 0; i < builders.Length; i++)
        //    {
        //        builders[i].InitOrLoad(chunk.Dimensions);
        //    }




        //    Vector3Int quadSize, m, n, offsetPos;

        //    await Task.Run(() =>
        //    {
        //        Parallel.For(0, 6, (voxelFace) =>
        //        {

        //            //Vector3[] vertices = new Vector3[4];
        //            //Vector3[] uvs = new Vector3[4];
        //            //Vector2[] uv2s = new Vector2[4];
        //            //Vector4[] uv3s = new Vector4[4];
        //            //Color32[] colors = new Color32[4];
        //            //byte[] verticesAO = new byte[4];
        //            //byte[] vertexColorIntensity = new byte[4];

        //            int d, u, v;
        //            Vector3Int dimensions = chunk.Dimensions;
        //            Color lightColor;
        //            bool smoothLight = true;
        //            bool greedyMeshing = false;


        //            /* Voxel Face Index
        //            * 0: Right
        //            * 1: Up
        //            * 2: Front
        //            * 3: Left
        //            * 4: Down 
        //            * 5: Back
        //            * 
        //            * BackFace -> Face that drawn in clockwise direction. (Need detect which face is clockwise in order to draw it on 
        //            * Unity scene).
        //            */
        //            //if (voxelFace == 4) continue;    // Don't draw down face (because player cannot see it).

        //            bool isBackFace = voxelFace > 2;
        //            d = voxelFace % 3;
        //            switch (d)
        //            {
        //                case 0:
        //                    u = 2;
        //                    v = 1;
        //                    break;
        //                case 1:
        //                    u = 0;
        //                    v = 2;
        //                    break;
        //                default:
        //                    u = 0;
        //                    v = 1;
        //                    break;
        //            }

        //            Vector3Int startPos = new Vector3Int();
        //            Vector3Int currPos = new Vector3Int();

        //            for (startPos[d] = 0; startPos[d] < dimensions[d]; startPos[d]++)
        //            {
        //                Array.Clear(builders[voxelFace].Merged[voxelFace][d], 0, builders[voxelFace].Merged[voxelFace][d].Length);

        //                // Build the slices of mesh.
        //                for (startPos[u] = 0; startPos[u] < dimensions[u]; startPos[u]++)
        //                {
        //                    for (startPos[v] = 0; startPos[v] < dimensions[v]; startPos[v]++)
        //                    {
        //                        BlockType currBlock = chunk.GetBlock(startPos);
        //                        if (currBlock == BlockType.Air) continue;


        //                        // If this block has already been merged, is air, or not visible -> skip it.
        //                        //if (chunk.IsSolid(startPos) == false ||
        //                        //    chunk.IsBlockFaceVisible(startPos, d, isBackFace) == false ||
        //                        //    builder.Merged[d][startPos[u], startPos[v]])
        //                        //{
        //                        //    continue;
        //                        //}

        //                        if (builders[voxelFace].Merged[voxelFace][d][startPos[u], startPos[v]])
        //                        {
        //                            continue;
        //                        }


        //                        if (isTransparentMesh)
        //                        {
        //                            // TRANSPARENT SOLID
        //                            if (chunk.GetBlock(startPos).IsTransparentSolidBlock() == false)
        //                            {
        //                                continue;
        //                            }

        //                            if (chunk.IsBlockFaceVisible(startPos, d, isBackFace) == false)
        //                            {
        //                                continue;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            // OPAQUE SOLID
        //                            if (chunk.GetBlock(startPos).IsSolid() == false ||
        //                                chunk.IsBlockFaceVisible(startPos, d, isBackFace) == false)
        //                            {
        //                                continue;
        //                            }

        //                            if (chunk.GetBlock(startPos).IsTransparentSolidBlock() == true)
        //                            {
        //                                continue;
        //                            }
        //                        }



        //                        // Ambient occlusion
        //                        // =================
        //                        bool anisotropy = false;

        //                        //try
        //                        //{
        //                        //    builders[voxelFace].VerticesAOCached[0] = (byte)VoxelAO.ProcessAO(chunk, startPos, 0, voxelFace);
        //                        //    builders[voxelFace].VerticesAOCached[1] = (byte)VoxelAO.ProcessAO(chunk, startPos, 1, voxelFace);
        //                        //    builders[voxelFace].VerticesAOCached[2] = (byte)VoxelAO.ProcessAO(chunk, startPos, 2, voxelFace);
        //                        //    builders[voxelFace].VerticesAOCached[3] = (byte)VoxelAO.ProcessAO(chunk, startPos, 3, voxelFace);

        //                        //}
        //                        //catch (Exception ex)
        //                        //{
        //                        //    Debug.Log(startPos);
        //                        //}





        //                        //if (greedyMeshing)
        //                        //{
        //                        //    quadSize = new Vector3Int();
        //                        //    // Next step is loop to figure out width and height of the new merged quad.
        //                        //    for (currPos = startPos, currPos[u]++;
        //                        //        currPos[u] < dimensions[u] &&
        //                        //        GreedyCompareLogic(startPos, currPos, d, isBackFace, voxelFace) &&
        //                        //        GreeyAO(chunk, startPos, voxelFace, chunk, currPos, voxelFace) &&
        //                        //        !builders[voxelFace].Merged[voxelFace][d][currPos[u], currPos[v]];
        //                        //        currPos[u]++)
        //                        //    { }
        //                        //    quadSize[u] = currPos[u] - startPos[u];

        //                        //    for (currPos = startPos, currPos[v]++;
        //                        //        currPos[v] < dimensions[v] &&
        //                        //        GreedyCompareLogic(startPos, currPos, d, isBackFace, voxelFace) &&
        //                        //         GreeyAO(chunk, startPos, voxelFace, chunk, currPos, voxelFace) &&
        //                        //        !builders[voxelFace].Merged[voxelFace][d][currPos[u], currPos[v]];
        //                        //        currPos[v]++)
        //                        //    {


        //                        //        for (currPos[u] = startPos[u];
        //                        //            currPos[u] < dimensions[u] &&
        //                        //            GreedyCompareLogic(startPos, currPos, d, isBackFace, voxelFace) &&
        //                        //             GreeyAO(chunk, startPos, voxelFace, chunk, currPos, voxelFace) &&
        //                        //            !builders[voxelFace].Merged[voxelFace][d][currPos[u], currPos[v]];
        //                        //            currPos[u]++)
        //                        //        { }


        //                        //        if (currPos[u] - startPos[u] < quadSize[u])
        //                        //        {
        //                        //            break;
        //                        //        }
        //                        //        else
        //                        //        {
        //                        //            currPos[u] = startPos[u];
        //                        //        }
        //                        //    }

        //                        //    quadSize[v] = currPos[v] - startPos[v];
        //                        //}
        //                        //else
        //                        //{
        //                        //    quadSize = Vector3Int.one;
        //                        //}


        //                        quadSize = Vector3Int.one;
        //                        // Add new quad to mesh data.
        //                        m = new Vector3Int();
        //                        n = new Vector3Int();

        //                        m[u] = quadSize[u];
        //                        n[v] = quadSize[v];

        //                        offsetPos = startPos;
        //                        offsetPos[d] += isBackFace ? 0 : 1;


        //                        builders[voxelFace].VerticesCached[0] = offsetPos;
        //                        builders[voxelFace].VerticesCached[1] = offsetPos + m;
        //                        builders[voxelFace].VerticesCached[2] = offsetPos + m + n;
        //                        builders[voxelFace].VerticesCached[3] = offsetPos + n;




        //                        // BLock light
        //                        // ===========
        //                        // Because at solid block light not exist. We can only get light by the adjacent block and use it as the light as solid voxel face.
        //                        byte lightValue = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
        //                        lightColor = GetLightColor(lightValue, lightAnimCurve);

        //                        builders[voxelFace].VertexColorIntensityCached[0] = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
        //                        builders[voxelFace].VertexColorIntensityCached[1] = GetBlockLightPropagationForAdjacentFace(chunk, startPos + m, voxelFace);
        //                        builders[voxelFace].VertexColorIntensityCached[2] = GetBlockLightPropagationForAdjacentFace(chunk, startPos + m + n, voxelFace);
        //                        builders[voxelFace].VertexColorIntensityCached[3] = GetBlockLightPropagationForAdjacentFace(chunk, startPos + n, voxelFace);

        //                        if (smoothLight)
        //                        {
        //                            if (voxelFace == 1 || voxelFace == 5 || voxelFace == 0)
        //                            {
        //                                if (builders[voxelFace].VertexColorIntensityCached[0] == 0)
        //                                {
        //                                    builders[voxelFace].ColorsCached[0] = lightColor;
        //                                }
        //                                else
        //                                {
        //                                    builders[voxelFace].ColorsCached[0] = GetLightColor(builders[voxelFace].VertexColorIntensityCached[0], lightAnimCurve);
        //                                }
        //                                if (builders[voxelFace].VertexColorIntensityCached[1] == 0)
        //                                {
        //                                    builders[voxelFace].ColorsCached[1] = lightColor;
        //                                }
        //                                else
        //                                {
        //                                    builders[voxelFace].ColorsCached[1] = GetLightColor(builders[voxelFace].VertexColorIntensityCached[1], lightAnimCurve);
        //                                }
        //                                if (builders[voxelFace].VertexColorIntensityCached[2] == 0)
        //                                {
        //                                    builders[voxelFace].ColorsCached[2] = lightColor;
        //                                }
        //                                else
        //                                {
        //                                    builders[voxelFace].ColorsCached[2] = GetLightColor(builders[voxelFace].VertexColorIntensityCached[2], lightAnimCurve);
        //                                }
        //                                if (builders[voxelFace].VertexColorIntensityCached[3] == 0)
        //                                {
        //                                    builders[voxelFace].ColorsCached[3] = lightColor;
        //                                }
        //                                else
        //                                {
        //                                    builders[voxelFace].ColorsCached[3] = GetLightColor(builders[voxelFace].VertexColorIntensityCached[3], lightAnimCurve);
        //                                }
        //                            }
        //                            else if (voxelFace == 2 || voxelFace == 3)
        //                            {
        //                                if (builders[voxelFace].VertexColorIntensityCached[1] == 0)
        //                                {
        //                                    builders[voxelFace].ColorsCached[0] = lightColor;
        //                                }
        //                                else
        //                                {
        //                                    builders[voxelFace].ColorsCached[0] = GetLightColor(builders[voxelFace].VertexColorIntensityCached[1], lightAnimCurve);
        //                                }
        //                                if (builders[voxelFace].VertexColorIntensityCached[0] == 0)
        //                                {
        //                                    builders[voxelFace].ColorsCached[1] = lightColor;
        //                                }
        //                                else
        //                                {
        //                                    builders[voxelFace].ColorsCached[1] = GetLightColor(builders[voxelFace].VertexColorIntensityCached[0], lightAnimCurve);
        //                                }
        //                                if (builders[voxelFace].VertexColorIntensityCached[3] == 0)
        //                                {
        //                                    builders[voxelFace].ColorsCached[2] = lightColor;
        //                                }
        //                                else
        //                                {
        //                                    builders[voxelFace].ColorsCached[2] = GetLightColor(builders[voxelFace].VertexColorIntensityCached[3], lightAnimCurve);
        //                                }
        //                                if (builders[voxelFace].VertexColorIntensityCached[2] == 0)
        //                                {
        //                                    builders[voxelFace].ColorsCached[3] = lightColor;
        //                                }
        //                                else
        //                                {
        //                                    builders[voxelFace].ColorsCached[3] = GetLightColor(builders[voxelFace].VertexColorIntensityCached[2], lightAnimCurve);
        //                                }
        //                            }
        //                            else
        //                            {
        //                                builders[voxelFace].ColorsCached[0] = GetLightColor(builders[voxelFace].VertexColorIntensityCached[0], lightAnimCurve);
        //                                builders[voxelFace].ColorsCached[1] = GetLightColor(builders[voxelFace].VertexColorIntensityCached[0], lightAnimCurve);
        //                                builders[voxelFace].ColorsCached[2] = GetLightColor(builders[voxelFace].VertexColorIntensityCached[0], lightAnimCurve);
        //                                builders[voxelFace].ColorsCached[3] = GetLightColor(builders[voxelFace].VertexColorIntensityCached[0], lightAnimCurve);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            builders[voxelFace].ColorsCached[0] = GetLightColor(builders[voxelFace].VertexColorIntensityCached[0], lightAnimCurve);
        //                            builders[voxelFace].ColorsCached[1] = GetLightColor(builders[voxelFace].VertexColorIntensityCached[0], lightAnimCurve);
        //                            builders[voxelFace].ColorsCached[2] = GetLightColor(builders[voxelFace].VertexColorIntensityCached[0], lightAnimCurve);
        //                            builders[voxelFace].ColorsCached[3] = GetLightColor(builders[voxelFace].VertexColorIntensityCached[0], lightAnimCurve);
        //                        }


        //                        // Ambient Lights
        //                        byte ambientLight = chunk.GetAmbientLight(startPos);


        //                        GetBlockUVs(currBlock, voxelFace, quadSize[u], quadSize[v], chunk.GetHeat(currPos), ref builders[voxelFace].UvsCached, ref builders[voxelFace].Uv2sCached, ref builders[voxelFace].Uv3sCached);
        //                        builders[voxelFace].AddQuadFace(builders[voxelFace].VerticesCached, builders[voxelFace].UvsCached, builders[voxelFace].Uv2sCached, builders[voxelFace].Uv3sCached, builders[voxelFace].ColorsCached, voxelFace, builders[voxelFace].VerticesAOCached, anisotropy, ambientLight);

        //                        // Mark at this position has been merged
        //                        for (int g = 0; g < quadSize[u]; g++)
        //                        {
        //                            for (int h = 0; h < quadSize[v]; h++)
        //                            {
        //                                builders[voxelFace].Merged[voxelFace][d][startPos[u] + g, startPos[v] + h] = true;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        });

        //        //for(int voxelFace = 0; voxelFace < 6; voxelFace++)
        //        //{
        //        //    Vector3Int startPos, currPos, quadSize, m, n, offsetPos;
        //        //    Vector3[] vertices = new Vector3[4];
        //        //    Vector3[] uvs = new Vector3[4];
        //        //    Vector2[] uv2s = new Vector2[4];
        //        //    Vector4[] uv3s = new Vector4[4];
        //        //    Color32[] colors = new Color32[4];
        //        //    byte[] verticesAO = new byte[4];
        //        //    byte[] vertexColorIntensity = new byte[4];
        //        //    BlockType currBlock;
        //        //    int d, u, v;
        //        //    Vector3Int dimensions = chunk.Dimensions;
        //        //    Color lightColor;
        //        //    bool smoothLight = true;
        //        //    bool greedyMeshing = false;


        //        //    /* Voxel Face Index
        //        //    * 0: Right
        //        //    * 1: Up
        //        //    * 2: Front
        //        //    * 3: Left
        //        //    * 4: Down 
        //        //    * 5: Back
        //        //    * 
        //        //    * BackFace -> Face that drawn in clockwise direction. (Need detect which face is clockwise in order to draw it on 
        //        //    * Unity scene).
        //        //    */
        //        //    //if (voxelFace == 4) continue;    // Don't draw down face (because player cannot see it).

        //        //    bool isBackFace = voxelFace > 2;
        //        //    d = voxelFace % 3;
        //        //    switch (d)
        //        //    {
        //        //        case 0:
        //        //            u = 2;
        //        //            v = 1;
        //        //            break;
        //        //        case 1:
        //        //            u = 0;
        //        //            v = 2;
        //        //            break;
        //        //        default:
        //        //            u = 0;
        //        //            v = 1;
        //        //            break;
        //        //    }

        //        //    //if (voxelFace != 1) continue;



        //        //    startPos = new Vector3Int();
        //        //    currPos = new Vector3Int();

        //        //    for (startPos[d] = 0; startPos[d] < dimensions[d]; startPos[d]++)
        //        //    {
        //        //        Array.Clear(builders[voxelFace].Merged[voxelFace][d], 0, builders[voxelFace].Merged[voxelFace][d].Length);

        //        //        // Build the slices of mesh.
        //        //        for (startPos[u] = 0; startPos[u] < dimensions[u]; startPos[u]++)
        //        //        {
        //        //            for (startPos[v] = 0; startPos[v] < dimensions[v]; startPos[v]++)
        //        //            {
        //        //                currBlock = chunk.GetBlock(startPos);
        //        //                if (currBlock == BlockType.Air) continue;


        //        //                // If this block has already been merged, is air, or not visible -> skip it.
        //        //                //if (chunk.IsSolid(startPos) == false ||
        //        //                //    chunk.IsBlockFaceVisible(startPos, d, isBackFace) == false ||
        //        //                //    builder.Merged[d][startPos[u], startPos[v]])
        //        //                //{
        //        //                //    continue;
        //        //                //}

        //        //                if (builders[voxelFace].Merged[voxelFace][d][startPos[u], startPos[v]])
        //        //                {
        //        //                    continue;
        //        //                }


        //        //                if (isTransparentMesh)
        //        //                {
        //        //                    // TRANSPARENT SOLID
        //        //                    if (chunk.GetBlock(startPos).IsTransparentSolidBlock() == false)
        //        //                    {
        //        //                        continue;
        //        //                    }

        //        //                    if (chunk.IsBlockFaceVisible(startPos, d, isBackFace) == false)
        //        //                    {
        //        //                        continue;
        //        //                    }
        //        //                }
        //        //                else
        //        //                {
        //        //                    // OPAQUE SOLID
        //        //                    if (chunk.GetBlock(startPos).IsSolid() == false ||
        //        //                        chunk.IsBlockFaceVisible(startPos, d, isBackFace) == false)
        //        //                    {
        //        //                        continue;
        //        //                    }

        //        //                    if (chunk.GetBlock(startPos).IsTransparentSolidBlock() == true)
        //        //                    {
        //        //                        continue;
        //        //                    }
        //        //                }



        //        //                // Ambient occlusion
        //        //                // =================
        //        //                bool anisotropy = false;

        //        //                verticesAO[0] = (byte)VoxelAO.ProcessAO(chunk, startPos, 0, voxelFace);
        //        //                verticesAO[1] = (byte)VoxelAO.ProcessAO(chunk, startPos, 1, voxelFace);
        //        //                verticesAO[2] = (byte)VoxelAO.ProcessAO(chunk, startPos, 2, voxelFace);
        //        //                verticesAO[3] = (byte)VoxelAO.ProcessAO(chunk, startPos, 3, voxelFace);



        //        //                if (greedyMeshing)
        //        //                {
        //        //                    quadSize = new Vector3Int();
        //        //                    // Next step is loop to figure out width and height of the new merged quad.
        //        //                    for (currPos = startPos, currPos[u]++;
        //        //                        currPos[u] < dimensions[u] &&
        //        //                        GreedyCompareLogic(startPos, currPos, d, isBackFace, voxelFace) &&
        //        //                        GreeyAO(chunk, startPos, voxelFace, chunk, currPos, voxelFace) &&
        //        //                        !builders[voxelFace].Merged[voxelFace][d][currPos[u], currPos[v]];
        //        //                        currPos[u]++)
        //        //                    { }
        //        //                    quadSize[u] = currPos[u] - startPos[u];

        //        //                    for (currPos = startPos, currPos[v]++;
        //        //                        currPos[v] < dimensions[v] &&
        //        //                        GreedyCompareLogic(startPos, currPos, d, isBackFace, voxelFace) &&
        //        //                         GreeyAO(chunk, startPos, voxelFace, chunk, currPos, voxelFace) &&
        //        //                        !builders[voxelFace].Merged[voxelFace][d][currPos[u], currPos[v]];
        //        //                        currPos[v]++)
        //        //                    {


        //        //                        for (currPos[u] = startPos[u];
        //        //                            currPos[u] < dimensions[u] &&
        //        //                            GreedyCompareLogic(startPos, currPos, d, isBackFace, voxelFace) &&
        //        //                             GreeyAO(chunk, startPos, voxelFace, chunk, currPos, voxelFace) &&
        //        //                            !builders[voxelFace].Merged[voxelFace][d][currPos[u], currPos[v]];
        //        //                            currPos[u]++)
        //        //                        { }


        //        //                        if (currPos[u] - startPos[u] < quadSize[u])
        //        //                        {
        //        //                            break;
        //        //                        }
        //        //                        else
        //        //                        {
        //        //                            currPos[u] = startPos[u];
        //        //                        }
        //        //                    }

        //        //                    quadSize[v] = currPos[v] - startPos[v];
        //        //                }
        //        //                else
        //        //                {
        //        //                    quadSize = Vector3Int.one;
        //        //                }



        //        //                // Add new quad to mesh data.
        //        //                m = new Vector3Int();
        //        //                n = new Vector3Int();

        //        //                m[u] = quadSize[u];
        //        //                n[v] = quadSize[v];

        //        //                offsetPos = startPos;
        //        //                offsetPos[d] += isBackFace ? 0 : 1;


        //        //                vertices[0] = offsetPos;
        //        //                vertices[1] = offsetPos + m;
        //        //                vertices[2] = offsetPos + m + n;
        //        //                vertices[3] = offsetPos + n;






        //        //                // BLock light
        //        //                // ===========
        //        //                // Because at solid block light not exist. We can only get light by the adjacent block and use it as the light as solid voxel face.
        //        //                byte lightValue = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
        //        //                lightColor = GetLightColor(lightValue, lightAnimCurve);

        //        //                vertexColorIntensity[0] = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
        //        //                vertexColorIntensity[1] = GetBlockLightPropagationForAdjacentFace(chunk, startPos + m, voxelFace);
        //        //                vertexColorIntensity[2] = GetBlockLightPropagationForAdjacentFace(chunk, startPos + m + n, voxelFace);
        //        //                vertexColorIntensity[3] = GetBlockLightPropagationForAdjacentFace(chunk, startPos + n, voxelFace);

        //        //                if (smoothLight)
        //        //                {
        //        //                    if (voxelFace == 1 || voxelFace == 5 || voxelFace == 0)
        //        //                    {
        //        //                        if (vertexColorIntensity[0] == 0)
        //        //                        {
        //        //                            colors[0] = lightColor;
        //        //                        }
        //        //                        else
        //        //                        {
        //        //                            colors[0] = GetLightColor(vertexColorIntensity[0], lightAnimCurve);
        //        //                        }
        //        //                        if (vertexColorIntensity[1] == 0)
        //        //                        {
        //        //                            colors[1] = lightColor;
        //        //                        }
        //        //                        else
        //        //                        {
        //        //                            colors[1] = GetLightColor(vertexColorIntensity[1], lightAnimCurve);
        //        //                        }
        //        //                        if (vertexColorIntensity[2] == 0)
        //        //                        {
        //        //                            colors[2] = lightColor;
        //        //                        }
        //        //                        else
        //        //                        {
        //        //                            colors[2] = GetLightColor(vertexColorIntensity[2], lightAnimCurve);
        //        //                        }
        //        //                        if (vertexColorIntensity[3] == 0)
        //        //                        {
        //        //                            colors[3] = lightColor;
        //        //                        }
        //        //                        else
        //        //                        {
        //        //                            colors[3] = GetLightColor(vertexColorIntensity[3], lightAnimCurve);
        //        //                        }
        //        //                    }
        //        //                    else if (voxelFace == 2 || voxelFace == 3)
        //        //                    {
        //        //                        if (vertexColorIntensity[1] == 0)
        //        //                        {
        //        //                            colors[0] = lightColor;
        //        //                        }
        //        //                        else
        //        //                        {
        //        //                            colors[0] = GetLightColor(vertexColorIntensity[1], lightAnimCurve);
        //        //                        }
        //        //                        if (vertexColorIntensity[0] == 0)
        //        //                        {
        //        //                            colors[1] = lightColor;
        //        //                        }
        //        //                        else
        //        //                        {
        //        //                            colors[1] = GetLightColor(vertexColorIntensity[0], lightAnimCurve);
        //        //                        }
        //        //                        if (vertexColorIntensity[3] == 0)
        //        //                        {
        //        //                            colors[2] = lightColor;
        //        //                        }
        //        //                        else
        //        //                        {
        //        //                            colors[2] = GetLightColor(vertexColorIntensity[3], lightAnimCurve);
        //        //                        }
        //        //                        if (vertexColorIntensity[2] == 0)
        //        //                        {
        //        //                            colors[3] = lightColor;
        //        //                        }
        //        //                        else
        //        //                        {
        //        //                            colors[3] = GetLightColor(vertexColorIntensity[2], lightAnimCurve);
        //        //                        }
        //        //                    }
        //        //                    else
        //        //                    {
        //        //                        colors[0] = GetLightColor(vertexColorIntensity[0], lightAnimCurve);
        //        //                        colors[1] = GetLightColor(vertexColorIntensity[0], lightAnimCurve);
        //        //                        colors[2] = GetLightColor(vertexColorIntensity[0], lightAnimCurve);
        //        //                        colors[3] = GetLightColor(vertexColorIntensity[0], lightAnimCurve);
        //        //                    }
        //        //                }
        //        //                else
        //        //                {
        //        //                    colors[0] = GetLightColor(vertexColorIntensity[0], lightAnimCurve);
        //        //                    colors[1] = GetLightColor(vertexColorIntensity[0], lightAnimCurve);
        //        //                    colors[2] = GetLightColor(vertexColorIntensity[0], lightAnimCurve);
        //        //                    colors[3] = GetLightColor(vertexColorIntensity[0], lightAnimCurve);
        //        //                }


        //        //                // Ambient Lights
        //        //                byte ambientLight = chunk.GetAmbientLight(startPos);


        //        //                GetBlockUVs(currBlock, voxelFace, quadSize[u], quadSize[v], chunk.GetHeat(currPos), ref uvs, ref uv2s, ref uv3s);
        //        //                builders[voxelFace].AddQuadFace(vertices, uvs, uv2s, uv3s, colors, voxelFace, verticesAO, anisotropy, ambientLight);

        //        //                // Mark at this position has been merged
        //        //                for (int g = 0; g < quadSize[u]; g++)
        //        //                {
        //        //                    for (int h = 0; h < quadSize[v]; h++)
        //        //                    {
        //        //                        builders[voxelFace].Merged[voxelFace][d][startPos[u] + g, startPos[v] + h] = true;
        //        //                    }
        //        //                }
        //        //            }
        //        //        }
        //        //    }
        //        //}
        //    });


        //    ChunkMeshBuilder resultBuilder = ChunkMeshBuilderPool.Get();
        //    resultBuilder.InitOrLoad(chunk.Dimensions);
        //    for(int i = 0; i < builders.Length; i++)
        //    {
        //        resultBuilder.Add(builders[i]);
        //    }
        //    MeshData resultMeshData = resultBuilder.ToMeshData();


        //    // Release pool data
        //    ChunkMeshBuilderPool.Release(resultBuilder);
        //    for (int i = 0; i < builders.Length; i++)
        //    {
        //        ChunkMeshBuilderPool.Release(builders[i]);
        //    }
        //    return resultMeshData;
        //}


        //public static async Task<MeshData> WaterGreedyMeshingAsync(Chunk chunk)
        //{
        //    bool GreedyCompareLogic(Vector3Int a, Vector3Int b, int dimension, bool isBackFace)
        //    {
        //        BlockType blockA = chunk.GetBlock(a);
        //        BlockType blockB = chunk.GetBlock(b);

        //        return blockA == blockB && chunk.IsWater(b) && chunk.IsBlockFaceVisible(b, dimension, isBackFace);
        //    }

        //    ChunkMeshBuilder _builder = ChunkMeshBuilderPool.Get();
        //    _builder.InitOrLoad(chunk.Dimensions);

        //    Vector3Int startPos, currPos, quadSize, m, n, offsetPos;
        //    Vector3[] vertices = new Vector3[4];
        //    Vector3[] uvs = new Vector3[4];
        //    Vector2[] uv2s = new Vector2[4];
        //    BlockType currBlock;
        //    int d, u, v;
        //    Vector3Int dimensions = chunk.Dimensions;

        //    await Task.Run(() =>
        //    {
        //        // Iterate over each aface of the blocks.
        //        for (int voxelFace = 0; voxelFace < 6; voxelFace++)
        //        {
        //            /* Voxel Face Index
        //            * 0: Right
        //            * 1: Up
        //            * 2: Front
        //            * 3: Left
        //            * 4: Down 
        //            * 5: Back
        //            * 
        //            * BackFace -> Face that drawn in clockwise direction. (Need detect which face is clockwise in order to draw it on 
        //            * Unity scene).
        //            */
        //            //if(voxelFace == 4) continue;
        //            if (voxelFace != 1)
        //                continue;

        //            bool isBackFace = voxelFace > 2;
        //            d = voxelFace % 3;
        //            if (d == 0)
        //            {
        //                u = 2;
        //                v = 1;
        //            }
        //            else if (d == 1)
        //            {
        //                u = 0;
        //                v = 2;
        //            }
        //            else
        //            {
        //                u = 0;
        //                v = 1;
        //            }

        //            startPos = new Vector3Int();
        //            currPos = new Vector3Int();

        //            for (startPos[d] = 0; startPos[d] < dimensions[d]; startPos[d]++)
        //            {
        //                Array.Clear(_builder.Merged[d], 0, _builder.Merged[d].Length);

        //                // Build the slices of mesh.
        //                for (startPos[u] = 0; startPos[u] < dimensions[u]; startPos[u]++)
        //                {
        //                    for (startPos[v] = 0; startPos[v] < dimensions[v]; startPos[v]++)
        //                    {
        //                        currBlock = chunk.GetBlock(startPos);

        //                        // If this block has already been merged, is air, or not visible -> skip it.
        //                        if (chunk.IsWater(startPos) == false ||
        //                            chunk.IsWaterFaceVisible(startPos, d, isBackFace) == false ||
        //                            _builder.Merged[d][startPos[u], startPos[v]])
        //                        {
        //                            continue;
        //                        }


        //                        quadSize = new Vector3Int();

        //                        // Next step is loop to figure out width and height of the new merged quad.
        //                        for (currPos = startPos, currPos[u]++;
        //                            currPos[u] < dimensions[u] &&
        //                            GreedyCompareLogic(startPos, currPos, d, isBackFace) &&
        //                            !_builder.Merged[d][currPos[u], currPos[v]];
        //                            currPos[u]++)
        //                        { }
        //                        quadSize[u] = currPos[u] - startPos[u];


        //                        for (currPos = startPos, currPos[v]++;
        //                            currPos[v] < dimensions[v] &&
        //                            GreedyCompareLogic(startPos, currPos, d, isBackFace) &&
        //                            !_builder.Merged[d][currPos[u], currPos[v]];
        //                            currPos[v]++)
        //                        {


        //                            for (currPos[u] = startPos[u];
        //                                currPos[u] < dimensions[u] &&
        //                                GreedyCompareLogic(startPos, currPos, d, isBackFace) &&
        //                                !_builder.Merged[d][currPos[u], currPos[v]];
        //                                currPos[u]++)
        //                            { }


        //                            if (currPos[u] - startPos[u] < quadSize[u])
        //                            {
        //                                break;
        //                            }
        //                            else
        //                            {
        //                                currPos[u] = startPos[u];
        //                            }
        //                        }

        //                        quadSize[v] = currPos[v] - startPos[v];


        //                        // Add new quad to mesh data.
        //                        m = new Vector3Int();
        //                        n = new Vector3Int();

        //                        m[u] = quadSize[u];
        //                        n[v] = quadSize[v];

        //                        offsetPos = startPos;
        //                        offsetPos[d] += isBackFace ? 0 : 1;

        //                        vertices[0] = offsetPos;
        //                        vertices[1] = offsetPos + m;
        //                        vertices[2] = offsetPos + m + n;
        //                        vertices[3] = offsetPos + n;

        //                        GetBlockUVs(currBlock, voxelFace, quadSize[u], quadSize[v], ref uvs, ref uv2s);
        //                        _builder.AddQuadFace(vertices, uvs, uv2s, voxelFace);


        //                        // Mark at this position has been merged
        //                        for (int g = 0; g < quadSize[u]; g++)
        //                        {
        //                            for (int h = 0; h < quadSize[v]; h++)
        //                            {
        //                                _builder.Merged[d][startPos[u] + g, startPos[v] + h] = true;
        //                            }
        //                        }
        //                    }
        //                }


        //            }
        //        }
        //    });

        //    MeshData meshData = _builder.ToMeshData();
        //    ChunkMeshBuilderPool.Release(_builder);
        //    return meshData;
        //}


        public static async Task<MeshData> GetChunkGrassMeshData(Chunk chunk, AnimationCurve lightAnimCurve, FastNoiseLite randomNoise)
        {
            ChunkMeshBuilder builder = ChunkMeshBuilderPool.Get();
            builder.InitOrLoad(chunk.Dimensions);

            Vector3[] vertices = new Vector3[4];
            Vector3[] uvs = new Vector3[4];
            Vector2[] uv2s = new Vector2[4];
            Vector4[] uv3s = new Vector4[4];
            Color32[] colors = new Color32[4];
            byte[] verticesAO = new byte[4];
            byte[] vertexColorIntensity = new byte[4];
            Vector3 _centerOffset = new Vector3(0.5f, 0.5f, 0.5f);
            bool applyRotation = true;
            float minRotationAngle = 25f;
            float maxRotationAngle = 75f;
            Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
            {
                return rotation * (point - pivot) + pivot;
            }

            await Task.Run(() =>
            {

                for (int i = 0; i < chunk.ChunkData.Length; i++)
                {
                    int x = i % chunk._width;
                    int y = (i / chunk._width) % chunk._height;
                    int z = i / (chunk._width * chunk._height);

                    BlockType currBlock = chunk.ChunkData[i];
                    if (currBlock == BlockType.Grass || currBlock == BlockType.Shrub)
                    {
                        Vector3Int curBlockPos = new Vector3Int(x, y, z);
                        // Generate a random float in the range -0.3 to 0.3
                        float randomFloatX = MapValue(randomNoise.GetNoise(x, z), -1f, 1f, -0.3f, 0.3f);
                        float randomFloatZ = MapValue(randomNoise.GetNoise(x + 1, z), -1f, 1f, -0.3f, 0.3f);
                        Vector3 randomOffset = new Vector3(randomFloatX, 0, randomFloatZ);
                        Vector3 offsetPos = curBlockPos + randomOffset;

                        Color32 lightColor = GetLightColor(chunk.GetBlockLight(curBlockPos), lightAnimCurve);
                        //lightColor = new Color32(255,0,0,255);
                        colors[0] = lightColor;
                        colors[1] = lightColor;
                        colors[2] = lightColor;
                        colors[3] = lightColor;

                        if (applyRotation)
                        {
                            float rotationAngle = ((float)(randomNoise.GetNoise(x, y + 1) + 1.0f) / 2.0f * (maxRotationAngle - minRotationAngle) + minRotationAngle);
                            float rotationAngleRad = rotationAngle * Mathf.Deg2Rad;
                            Quaternion rotation = Quaternion.Euler(0f, rotationAngle, 0f);
                            vertices[0] = RotatePointAroundPivot(offsetPos, _centerOffset + offsetPos, rotation);
                            vertices[1] = RotatePointAroundPivot(offsetPos + new Vector3Int(1, 0, 1), _centerOffset + offsetPos, rotation);
                            vertices[2] = RotatePointAroundPivot(offsetPos + new Vector3Int(1, 1, 1), _centerOffset + offsetPos, rotation);
                            vertices[3] = RotatePointAroundPivot(offsetPos + new Vector3Int(0, 1, 0), _centerOffset + offsetPos, rotation);
                            GetGrassUVs(currBlock, chunk.GetHeat(curBlockPos), ref uvs, ref uv2s);
                            builder.AddQuadFace(vertices, uvs, uv2s, colors);


                            vertices[0] = RotatePointAroundPivot(offsetPos + new Vector3Int(0, 0, 1), _centerOffset + offsetPos, rotation);
                            vertices[1] = RotatePointAroundPivot(offsetPos + new Vector3Int(1, 0, 0), _centerOffset + offsetPos, rotation);
                            vertices[2] = RotatePointAroundPivot(offsetPos + new Vector3Int(1, 1, 0), _centerOffset + offsetPos, rotation);
                            vertices[3] = RotatePointAroundPivot(offsetPos + new Vector3Int(0, 1, 1), _centerOffset + offsetPos, rotation);
                            GetGrassUVs(currBlock, chunk.GetHeat(curBlockPos), ref uvs, ref uv2s);
                            builder.AddQuadFace(vertices, uvs, uv2s, colors);
                        }
                        else
                        {
                            vertices[0] = offsetPos;
                            vertices[1] = offsetPos + new Vector3Int(1, 0, 1);
                            vertices[2] = offsetPos + new Vector3Int(1, 1, 1);
                            vertices[3] = offsetPos + new Vector3Int(0, 1, 0);
                            GetGrassUVs(currBlock, chunk.GetHeat(curBlockPos), ref uvs, ref uv2s);
                            builder.AddQuadFace(vertices, uvs, uv2s, colors);


                            vertices[0] = offsetPos + new Vector3Int(0, 0, 1);
                            vertices[1] = offsetPos + new Vector3Int(1, 0, 0);
                            vertices[2] = offsetPos + new Vector3Int(1, 1, 0);
                            vertices[3] = offsetPos + new Vector3Int(0, 1, 1);
                            GetGrassUVs(currBlock, chunk.GetHeat(curBlockPos), ref uvs, ref uv2s);
                            builder.AddQuadFace(vertices, uvs, uv2s, colors);
                        }

                    }
                    else if (chunk.ChunkData[i] == BlockType.TallGrass)
                    {
                        Vector3Int curBlockPos = new Vector3Int(x, y, z);
                        // Generate a random float in the range -0.3 to 0.3
                        float randomFloatX = MapValue(randomNoise.GetNoise(x, z), -1f, 1f, -0.3f, 0.3f);
                        float randomFloatZ = MapValue(randomNoise.GetNoise(x + 1, z), -1f, 1f, -0.3f, 0.3f);
                        Vector3 randomOffset = new Vector3(randomFloatX, 0, randomFloatZ);
                        Vector3 offsetPos = curBlockPos + randomOffset;

                        Color32 lightColor = GetLightColor(chunk.GetBlockLight(curBlockPos), lightAnimCurve);
                        //lightColor = new Color32(255,0,0,255);
                        colors[0] = lightColor;
                        colors[1] = lightColor;
                        colors[2] = lightColor;
                        colors[3] = lightColor;

                        if (applyRotation)
                        {
                            float rotationAngle = ((float)(randomNoise.GetNoise(x + 1, y + 1) + 1.0f) / 2.0f * (maxRotationAngle - minRotationAngle) + minRotationAngle);
                            float rotationAngleRad = rotationAngle * Mathf.Deg2Rad;
                            Quaternion rotation = Quaternion.Euler(0f, rotationAngle, 0f);
                            int heightFromOrigin = WorldGeneration.Instance.GetBlockHeightFromOrigin(chunk, curBlockPos);
                            vertices[0] = RotatePointAroundPivot(offsetPos, _centerOffset + offsetPos, rotation);
                            vertices[1] = RotatePointAroundPivot(offsetPos + new Vector3Int(1, 0, 1), _centerOffset + offsetPos, rotation);
                            vertices[2] = RotatePointAroundPivot(offsetPos + new Vector3Int(1, 1, 1), _centerOffset + offsetPos, rotation);
                            vertices[3] = RotatePointAroundPivot(offsetPos + new Vector3Int(0, 1, 0), _centerOffset + offsetPos, rotation);
                            GetGrassUVs(BlockType.TallGrass, chunk.GetHeat(curBlockPos), ref uvs, ref uv2s, heightFromOrigin);
                            builder.AddQuadFace(vertices, uvs, uv2s, colors);


                            vertices[0] = RotatePointAroundPivot(offsetPos + new Vector3Int(0, 0, 1), _centerOffset + offsetPos, rotation);
                            vertices[1] = RotatePointAroundPivot(offsetPos + new Vector3Int(1, 0, 0), _centerOffset + offsetPos, rotation);
                            vertices[2] = RotatePointAroundPivot(offsetPos + new Vector3Int(1, 1, 0), _centerOffset + offsetPos, rotation);
                            vertices[3] = RotatePointAroundPivot(offsetPos + new Vector3Int(0, 1, 1), _centerOffset + offsetPos, rotation);
                            GetGrassUVs(BlockType.TallGrass, chunk.GetHeat(curBlockPos), ref uvs, ref uv2s, heightFromOrigin);
                            builder.AddQuadFace(vertices, uvs, uv2s, colors);
                        }
                        else
                        {
                            int heightFromOrigin = WorldGeneration.Instance.GetBlockHeightFromOrigin(chunk, curBlockPos);
                            vertices[0] = offsetPos;
                            vertices[1] = offsetPos + new Vector3Int(1, 0, 1);
                            vertices[2] = offsetPos + new Vector3Int(1, 1, 1);
                            vertices[3] = offsetPos + new Vector3Int(0, 1, 0);
                            GetGrassUVs(BlockType.TallGrass, chunk.GetHeat(curBlockPos), ref uvs, ref uv2s, heightFromOrigin);
                            builder.AddQuadFace(vertices, uvs, uv2s, colors);


                            vertices[0] = offsetPos + new Vector3Int(0, 0, 1);
                            vertices[1] = offsetPos + new Vector3Int(1, 0, 0);
                            vertices[2] = offsetPos + new Vector3Int(1, 1, 0);
                            vertices[3] = offsetPos + new Vector3Int(0, 1, 1);
                            GetGrassUVs(BlockType.TallGrass, chunk.GetHeat(curBlockPos), ref uvs, ref uv2s, heightFromOrigin);
                            builder.AddQuadFace(vertices, uvs, uv2s, colors);
                        }



                       
                    }
                }
            });



            MeshData meshData = builder.ToMeshData();
            ChunkMeshBuilderPool.Release(builder);
            return meshData;
        }

        private static void GetGrassUVs(BlockType blockType, float heatValue, ref Vector3[] uvs, ref Vector2[] uv2s, int heightFromOrigin = 0)
        {
            //Debug.Log(heatValue);
            int blockIndex;
            GetColorMap(ref uv2s, heatValue, clear: true);

            if (blockType == BlockType.Grass)
            {
                GetColorMap(ref uv2s, heatValue);
                blockIndex = (ushort)BlockType.Grass;
                uvs[0] = new Vector3(0, 0, blockIndex);
                uvs[1] = new Vector3(1, 0, blockIndex);
                uvs[2] = new Vector3(1, 1, blockIndex);
                uvs[3] = new Vector3(0, 1, blockIndex);
            }
            else if (blockType == BlockType.TallGrass)
            {
                GetColorMap(ref uv2s, heatValue);
                switch (heightFromOrigin)
                {
                    default:
                    case 0:
                        blockIndex = (ushort)TextureType.BelowTallGrass;
                        break;
                    case 1:
                        blockIndex = (ushort)TextureType.AboveTallGrass;
                        break;
                }

                uvs[0] = new Vector3(0, 0, blockIndex);
                uvs[1] = new Vector3(1, 0, blockIndex);
                uvs[2] = new Vector3(1, 1, blockIndex);
                uvs[3] = new Vector3(0, 1, blockIndex);
            }
            else if (blockType == BlockType.Shrub)
            {
                blockIndex = (ushort)TextureType.Shrub;
                uvs[0] = new Vector3(0, 0, blockIndex);
                uvs[1] = new Vector3(1, 0, blockIndex);
                uvs[2] = new Vector3(1, 1, blockIndex);
                uvs[3] = new Vector3(0, 1, blockIndex);
            }
        }
        private static void GetBlockUVs(BlockType blockType, int face, int width, int height, float heatValue, ref Vector3[] uvs, ref Vector2[] uv2s, ref Vector4[] uv3s)
        {
            int blockIndex;
            GetColorMap(ref uv2s, heatValue, clear: true);
            switch (blockType)
            {
                default:
                    blockIndex = (ushort)blockType;
                    break;
                case BlockType.DirtGrass:
                    if (face == 1)
                    {
                        blockIndex = (ushort)TextureType.GrassTop;
                        GetColorMap(ref uv2s, heatValue);
                    }
                    else if (face == 4)
                    {
                        blockIndex = (ushort)TextureType.Dirt;
                    }
                    else
                    {
                        blockIndex = (ushort)blockType;
                    }
                    break;
                case BlockType.SnowDritGrass:
                    if (face == 1)
                    {
                        blockIndex = (ushort)TextureType.SnowGrassTop;
                    }
                    else if (face == 4)
                    {
                        blockIndex = (ushort)TextureType.Dirt;
                    }
                    else
                    {
                        blockIndex = (ushort)TextureType.SnowGrassSide;
                    }
                    break;
                case BlockType.Leaves:
                    blockIndex = (ushort)blockType;
                    GetColorMap(ref uv2s, heatValue);
                    break;
                case BlockType.PineLeaves:
                    blockIndex = (ushort)blockType;
                    GetColorMap(ref uv2s, heatValue);
                    break;
                case BlockType.Wood:
                    if (face == 1 || face == 4)
                    {
                        blockIndex = (ushort)TextureType.HeartWood;
                    }
                    else
                    {
                        blockIndex = (ushort)TextureType.BarkWood;
                    }
                    break;
                case BlockType.PineWood:
                    if (face == 1 || face == 4)
                    {
                        blockIndex = (ushort)TextureType.HeartPineWood;
                    }
                    else
                    {
                        blockIndex = (ushort)TextureType.BarkPineWood;
                    }
                    break;
                case BlockType.Cactus:
                    if (face == 1)
                    {
                        blockIndex = (ushort)TextureType.Cactus_Upper;
                    }
                    else
                    {
                        blockIndex = (ushort)TextureType.Cactus_Middle;
                    }
                    break;
            }



            uvs[0] = new Vector3(0, 0, blockIndex);
            uvs[1] = new Vector3(width, 0, blockIndex);
            uvs[2] = new Vector3(width, height, blockIndex);
            uvs[3] = new Vector3(0, height, blockIndex);
        }



        public static async Task<MeshData> SolidGreedyMeshingForColliderAsync(Chunk chunk)
        {
            bool GreedyCompareLogic(Vector3Int a, Vector3Int b, int dimension, bool isBackFace)
            {
                return chunk.IsSolid(b) && chunk.IsBlockFaceVisible(b, dimension, isBackFace);
            }

            ChunkMeshBuilder _builder = ChunkMeshBuilderPool.Get();
            _builder.InitOrLoad(chunk.Dimensions);

            Vector3Int startPos, currPos, quadSize, m, n, offsetPos;
            Vector3[] vertices = new Vector3[4];
            BlockType currBlock;
            int d, u, v;
            Vector3Int dimensions = chunk.Dimensions;

            //await Task.Run(() =>
            //{
            //    // Iterate over each aface of the blocks.
            //    for (int voxelFace = 0; voxelFace < 6; voxelFace++)
            //    {
            //        /* Voxel Face Index
            //        * 0: Right
            //        * 1: Up
            //        * 2: Front
            //        * 3: Left
            //        * 4: Down 
            //        * 5: Back
            //        * 
            //        * BackFace -> Face that drawn in clockwise direction. (Need detect which face is clockwise in order to draw it on 
            //        * Unity scene).
            //        */
            //        if (voxelFace == 4) continue;    // Don't draw down face (because player cannot see it).

            //        bool isBackFace = voxelFace > 2;
            //        d = voxelFace % 3;
            //        if (d == 0)
            //        {
            //            u = 2;
            //            v = 1;
            //        }
            //        else if (d == 1)
            //        {
            //            u = 0;
            //            v = 2;
            //        }
            //        else
            //        {
            //            u = 0;
            //            v = 1;
            //        }

            //        startPos = new Vector3Int();
            //        currPos = new Vector3Int();

            //        for (startPos[d] = 0; startPos[d] < dimensions[d]; startPos[d]++)
            //        {
            //            Array.Clear(_builder.Merged[d], 0, _builder.Merged[d].Length);

            //            // Build the slices of mesh.
            //            for (startPos[u] = 0; startPos[u] < dimensions[u]; startPos[u]++)
            //            {
            //                for (startPos[v] = 0; startPos[v] < dimensions[v]; startPos[v]++)
            //                {
            //                    currBlock = chunk.GetBlock(startPos);


            //                    // If this block has already been merged, is air, or not visible -> skip it.
            //                    if (chunk.IsSolid(startPos) == false ||
            //                        chunk.IsBlockFaceVisible(startPos, d, isBackFace) == false ||
            //                        _builder.Merged[d][startPos[u], startPos[v]])
            //                    {
            //                        continue;
            //                    }


            //                    quadSize = new Vector3Int();

            //                    // Next step is loop to figure out width and height of the new merged quad.
            //                    for (currPos = startPos, currPos[u]++;
            //                        currPos[u] < dimensions[u] &&
            //                        GreedyCompareLogic(startPos, currPos, d, isBackFace) &&
            //                        !_builder.Merged[d][currPos[u], currPos[v]];
            //                        currPos[u]++)
            //                    { }
            //                    quadSize[u] = currPos[u] - startPos[u];


            //                    for (currPos = startPos, currPos[v]++;
            //                        currPos[v] < dimensions[v] &&
            //                        GreedyCompareLogic(startPos, currPos, d, isBackFace) &&
            //                        !_builder.Merged[d][currPos[u], currPos[v]];
            //                        currPos[v]++)
            //                    {


            //                        for (currPos[u] = startPos[u];
            //                            currPos[u] < dimensions[u] &&
            //                            GreedyCompareLogic(startPos, currPos, d, isBackFace) &&
            //                            !_builder.Merged[d][currPos[u], currPos[v]];
            //                            currPos[u]++)
            //                        { }


            //                        if (currPos[u] - startPos[u] < quadSize[u])
            //                        {
            //                            break;
            //                        }
            //                        else
            //                        {
            //                            currPos[u] = startPos[u];
            //                        }
            //                    }

            //                    quadSize[v] = currPos[v] - startPos[v];


            //                    // Add new quad to mesh data.
            //                    m = new Vector3Int();
            //                    n = new Vector3Int();

            //                    m[u] = quadSize[u];
            //                    n[v] = quadSize[v];

            //                    offsetPos = startPos;
            //                    offsetPos[d] += isBackFace ? 0 : 1;

            //                    vertices[0] = offsetPos;
            //                    vertices[1] = offsetPos + m;
            //                    vertices[2] = offsetPos + m + n;
            //                    vertices[3] = offsetPos + n;

            //                    _builder.AddQuadFace(vertices, null, null, null, null, voxelFace);

            //                    // Mark at this position has been merged
            //                    for (int g = 0; g < quadSize[u]; g++)
            //                    {
            //                        for (int h = 0; h < quadSize[v]; h++)
            //                        {
            //                            _builder.Merged[d][startPos[u] + g, startPos[v] + h] = true;
            //                        }
            //                    }
            //                }
            //            }


            //        }
            //    }
            //});

            MeshData meshData = _builder.ToMeshData();
            ChunkMeshBuilderPool.Release(_builder);
            return meshData;
        }



    }
}
