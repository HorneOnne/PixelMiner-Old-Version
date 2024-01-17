using System.Threading.Tasks;
using UnityEngine;
using PixelMiner.Enums;
using System;
using PixelMiner.Lighting;
using PixelMiner.Utilities;
using PixelMiner.World;

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
                       !chunk.IsSolid(b) &&
                       GetBlockLightPropagationForAdjacentFace(chunk, a, voxelFace) == GetBlockLightPropagationForAdjacentFace(chunk, b, voxelFace) &&
                       GetBlockLightPropagationForAdjacentFace(chunk, a, voxelFace) == GetBlockLightPropagationForAdjacentFace(chunk, c, voxelFace) &&
                       GetBlockLightPropagationForAdjacentFace(chunk, a, voxelFace) == GetBlockLightPropagationForAdjacentFace(chunk, d, voxelFace) &&
                       GetBlockLightPropagationForAdjacentFace(chunk, a, voxelFace) == GetBlockLightPropagationForAdjacentFace(chunk, e, voxelFace) &&
                       chunk.IsBlockFaceVisible(b, dimension, isBackFace);
            }


            ChunkMeshBuilder builder = ChunkMeshBuilderPool.Get();
            builder.InitOrLoad(chunk.Dimensions);

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


            await Task.Run(() =>
            {

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
                    //if (voxelFace == 4) continue;    // Don't draw down face (because player cannot see it).

                    bool isBackFace = voxelFace > 2;
                    d = voxelFace % 3;
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

                                // Because at solid block light not exist. We can only get light by the adjacent block and use it as the light as solid voxel face.
                                byte lightValue = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                lightColor = GetLightColor(lightValue, lightAnimCurve);


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



                                if (greedyMeshing)
                                {
                                    Debug.Log("Greedy");
                                    quadSize = new Vector3Int();
                                    // Next step is loop to figure out width and height of the new merged quad.
                                    for (currPos = startPos, currPos[u]++;
                                        currPos[u] < dimensions[u] &&
                                        GreedyCompareLogic(startPos, currPos, d, isBackFace, voxelFace) &&
                                        !builder.Merged[d][currPos[u], currPos[v]];
                                        currPos[u]++)
                                    { }
                                    quadSize[u] = currPos[u] - startPos[u];

                                    for (currPos = startPos, currPos[v]++;
                                        currPos[v] < dimensions[v] &&
                                        GreedyCompareLogic(startPos, currPos, d, isBackFace, voxelFace) &&
                                        !builder.Merged[d][currPos[u], currPos[v]];
                                        currPos[v]++)
                                    {


                                        for (currPos[u] = startPos[u];
                                            currPos[u] < dimensions[u] &&
                                            GreedyCompareLogic(startPos, currPos, d, isBackFace, voxelFace) &&
                                            !builder.Merged[d][currPos[u], currPos[v]];
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


                                // Ambient occlusion
                                // =================
                                bool anisotropy = false;
                                verticesAO[0] = (byte)VoxelAO.ProcessAO(chunk, startPos, 0, voxelFace);
                                verticesAO[1] = (byte)VoxelAO.ProcessAO(chunk, startPos, 1, voxelFace);
                                verticesAO[2] = (byte)VoxelAO.ProcessAO(chunk, startPos, 2, voxelFace);
                                verticesAO[3] = (byte)VoxelAO.ProcessAO(chunk, startPos, 3, voxelFace);

                                //if (voxelFace == 1)
                                //{
                                //    if (verticesAO[1] + verticesAO[3] > verticesAO[2] + verticesAO[0])
                                //    {
                                //        vertices[3] = offsetPos;
                                //        vertices[0] = offsetPos + m;
                                //        vertices[1] = offsetPos + m + n;
                                //        vertices[2] = offsetPos + n;

                                //        var temp0 = verticesAO[0];
                                //        var temp1 = verticesAO[1];
                                //        var temp2 = verticesAO[2];
                                //        var temp3 = verticesAO[3];

                                //        verticesAO[0] = temp1;
                                //        verticesAO[1] = temp2;
                                //        verticesAO[2] = temp3;
                                //        verticesAO[3] = temp0;

                                //        anisotropy = true;
                                //    }
                                //}



                                // BLock light
                                // ===========
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


                                GetBlockUVs(currBlock, voxelFace, quadSize[u], quadSize[v], ref uvs, ref uv2s, ref uv3s);
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
                }
            });

            MeshData meshData = builder.ToMeshData();
            ChunkMeshBuilderPool.Release(builder);
            return meshData;
        }


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
                    if (chunk.ChunkData[i] == BlockType.Grass)
                    {
                        Vector3Int curBlockPos = new Vector3Int(x, y, z);
                        // Generate a random float in the range -0.3 to 0.3
                        float randomFloatX = MapValue(randomNoise.GetNoise(x, z), -1f, 1f, -0.3f, 0.3f);
                        float randomFloatZ = MapValue(randomNoise.GetNoise(x + 1, z), -1f, 1f, -0.3f, 0.3f);
                        Vector3 randomOffset = new Vector3(randomFloatX, 0, randomFloatZ);
                        Vector3 offsetPos = curBlockPos + randomOffset;


                        if (applyRotation)
                        {
                            float rotationAngle = ((float)(randomNoise.GetNoise(x, y + 1) + 1.0f) / 2.0f * (maxRotationAngle - minRotationAngle) + minRotationAngle);
                            float rotationAngleRad = rotationAngle * Mathf.Deg2Rad;
                            Quaternion rotation = Quaternion.Euler(0f, rotationAngle, 0f);
                            vertices[0] = RotatePointAroundPivot(offsetPos, _centerOffset + offsetPos, rotation);
                            vertices[1] = RotatePointAroundPivot(offsetPos + new Vector3Int(1, 0, 1), _centerOffset + offsetPos, rotation);
                            vertices[2] = RotatePointAroundPivot(offsetPos + new Vector3Int(1, 1, 1), _centerOffset + offsetPos, rotation);
                            vertices[3] = RotatePointAroundPivot(offsetPos + new Vector3Int(0, 1, 0), _centerOffset + offsetPos, rotation);
                            GetGrassUVs(BlockType.Grass, ref uvs, ref uv2s);
                            builder.AddQuadFace(vertices, uvs, uv2s);


                            vertices[0] = RotatePointAroundPivot(offsetPos + new Vector3Int(0, 0, 1), _centerOffset + offsetPos, rotation);
                            vertices[1] = RotatePointAroundPivot(offsetPos + new Vector3Int(1, 0, 0), _centerOffset + offsetPos, rotation);
                            vertices[2] = RotatePointAroundPivot(offsetPos + new Vector3Int(1, 1, 0), _centerOffset + offsetPos, rotation);
                            vertices[3] = RotatePointAroundPivot(offsetPos + new Vector3Int(0, 1, 1), _centerOffset + offsetPos, rotation);
                            GetGrassUVs(BlockType.Grass, ref uvs, ref uv2s);
                            builder.AddQuadFace(vertices, uvs, uv2s);
                        }
                        else
                        {
                            vertices[0] = offsetPos;
                            vertices[1] = offsetPos + new Vector3Int(1, 0, 1);
                            vertices[2] = offsetPos + new Vector3Int(1, 1, 1);
                            vertices[3] = offsetPos + new Vector3Int(0, 1, 0);
                            GetGrassUVs(BlockType.Grass, ref uvs, ref uv2s);
                            builder.AddQuadFace(vertices, uvs, uv2s);


                            vertices[0] = offsetPos + new Vector3Int(0, 0, 1);
                            vertices[1] = offsetPos + new Vector3Int(1, 0, 0);
                            vertices[2] = offsetPos + new Vector3Int(1, 1, 0);
                            vertices[3] = offsetPos + new Vector3Int(0, 1, 1);
                            GetGrassUVs(BlockType.Grass, ref uvs, ref uv2s);
                            builder.AddQuadFace(vertices, uvs, uv2s);
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
                            GetGrassUVs(BlockType.TallGrass, ref uvs, ref uv2s, heightFromOrigin);
                            builder.AddQuadFace(vertices, uvs, uv2s);


                            vertices[0] = RotatePointAroundPivot(offsetPos + new Vector3Int(0, 0, 1), _centerOffset + offsetPos, rotation);
                            vertices[1] = RotatePointAroundPivot(offsetPos + new Vector3Int(1, 0, 0), _centerOffset + offsetPos, rotation);
                            vertices[2] = RotatePointAroundPivot(offsetPos + new Vector3Int(1, 1, 0), _centerOffset + offsetPos, rotation);
                            vertices[3] = RotatePointAroundPivot(offsetPos + new Vector3Int(0, 1, 1), _centerOffset + offsetPos, rotation);
                            GetGrassUVs(BlockType.TallGrass, ref uvs, ref uv2s, heightFromOrigin);
                            builder.AddQuadFace(vertices, uvs, uv2s);
                        }
                        else
                        {
                            int heightFromOrigin = WorldGeneration.Instance.GetBlockHeightFromOrigin(chunk, curBlockPos);
                            vertices[0] = offsetPos;
                            vertices[1] = offsetPos + new Vector3Int(1, 0, 1);
                            vertices[2] = offsetPos + new Vector3Int(1, 1, 1);
                            vertices[3] = offsetPos + new Vector3Int(0, 1, 0);
                            GetGrassUVs(BlockType.TallGrass, ref uvs, ref uv2s, heightFromOrigin);
                            builder.AddQuadFace(vertices, uvs, uv2s);


                            vertices[0] = offsetPos + new Vector3Int(0, 0, 1);
                            vertices[1] = offsetPos + new Vector3Int(1, 0, 0);
                            vertices[2] = offsetPos + new Vector3Int(1, 1, 0);
                            vertices[3] = offsetPos + new Vector3Int(0, 1, 1);
                            GetGrassUVs(BlockType.TallGrass, ref uvs, ref uv2s, heightFromOrigin);
                            builder.AddQuadFace(vertices, uvs, uv2s);
                        }




                    }
                }
            });



            MeshData meshData = builder.ToMeshData();
            ChunkMeshBuilderPool.Release(builder);
            return meshData;
        }

        private static void GetGrassUVs(BlockType blockType, ref Vector3[] uvs, ref Vector2[] uv2s, int heightFromOrigin = 0)
        {
            int blockIndex;
            ColorMapType colorMapType;
            colorMapType = ColorMapType.Plains;


            if (blockType == BlockType.Grass)
            {
                blockIndex = (ushort)BlockType.Grass;
                uvs[0] = new Vector3(0, 0, blockIndex);
                uvs[1] = new Vector3(1, 0, blockIndex);
                uvs[2] = new Vector3(1, 1, blockIndex);
                uvs[3] = new Vector3(0, 1, blockIndex);
            }
            else if (blockType == BlockType.TallGrass)
            {
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

            GetColorMapkUVs(colorMapType, ref uv2s);
        }
        private static void GetBlockUVs(BlockType blockType, int face, int width, int height, ref Vector3[] uvs, ref Vector2[] uv2s, ref Vector4[] uv3s)
        {
            int blockIndex;
            ColorMapType colorMapType = ColorMapType.None;
            if (blockType == BlockType.DirtGrass)
            {
                if (face == 1)
                {
                    blockIndex = (ushort)TextureType.GrassTop;
                    colorMapType = ColorMapType.Plains;
                }
                else if (face == 4)
                {
                    blockIndex = (ushort)TextureType.Dirt;
                }
                else
                {
                    blockIndex = (ushort)blockType;
                }
            }
            else if (blockType == BlockType.Leaves)
            {
                blockIndex = (ushort)blockType;
                colorMapType = ColorMapType.Plains;           
            }
            else if(blockType == BlockType.Wood)
            {
                if (face == 1)
                {
                    blockIndex = (ushort)TextureType.HeartWood;
                }
                else if (face == 4)
                {
                    blockIndex = (ushort)TextureType.HeartWood;
                }
                else
                {
                    blockIndex = (ushort)TextureType.BarkWood;
                }
            }
            else
            {
                blockIndex = (ushort)blockType;
            }



            //if (blockType == BlockType.DirtGrass && face == 1)
            //{
            //    colorMapType = ColorMapType.Plains;
            //}
            //else
            //{
            //    colorMapType = ColorMapType.None;
            //}

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

            await Task.Run(() =>
            {
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
                    if (voxelFace == 4) continue;    // Don't draw down face (because player cannot see it).

                    bool isBackFace = voxelFace > 2;
                    d = voxelFace % 3;
                    if (d == 0)
                    {
                        u = 2;
                        v = 1;
                    }
                    else if (d == 1)
                    {
                        u = 0;
                        v = 2;
                    }
                    else
                    {
                        u = 0;
                        v = 1;
                    }

                    startPos = new Vector3Int();
                    currPos = new Vector3Int();

                    for (startPos[d] = 0; startPos[d] < dimensions[d]; startPos[d]++)
                    {
                        Array.Clear(_builder.Merged[d], 0, _builder.Merged[d].Length);

                        // Build the slices of mesh.
                        for (startPos[u] = 0; startPos[u] < dimensions[u]; startPos[u]++)
                        {
                            for (startPos[v] = 0; startPos[v] < dimensions[v]; startPos[v]++)
                            {
                                currBlock = chunk.GetBlock(startPos);


                                // If this block has already been merged, is air, or not visible -> skip it.
                                if (chunk.IsSolid(startPos) == false ||
                                    chunk.IsBlockFaceVisible(startPos, d, isBackFace) == false ||
                                    _builder.Merged[d][startPos[u], startPos[v]])
                                {
                                    continue;
                                }


                                quadSize = new Vector3Int();

                                // Next step is loop to figure out width and height of the new merged quad.
                                for (currPos = startPos, currPos[u]++;
                                    currPos[u] < dimensions[u] &&
                                    GreedyCompareLogic(startPos, currPos, d, isBackFace) &&
                                    !_builder.Merged[d][currPos[u], currPos[v]];
                                    currPos[u]++)
                                { }
                                quadSize[u] = currPos[u] - startPos[u];


                                for (currPos = startPos, currPos[v]++;
                                    currPos[v] < dimensions[v] &&
                                    GreedyCompareLogic(startPos, currPos, d, isBackFace) &&
                                    !_builder.Merged[d][currPos[u], currPos[v]];
                                    currPos[v]++)
                                {


                                    for (currPos[u] = startPos[u];
                                        currPos[u] < dimensions[u] &&
                                        GreedyCompareLogic(startPos, currPos, d, isBackFace) &&
                                        !_builder.Merged[d][currPos[u], currPos[v]];
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

                                vertices[0] = offsetPos;
                                vertices[1] = offsetPos + m;
                                vertices[2] = offsetPos + m + n;
                                vertices[3] = offsetPos + n;

                                _builder.AddQuadFace(vertices, null, null, null, null, voxelFace);

                                // Mark at this position has been merged
                                for (int g = 0; g < quadSize[u]; g++)
                                {
                                    for (int h = 0; h < quadSize[v]; h++)
                                    {
                                        _builder.Merged[d][startPos[u] + g, startPos[v] + h] = true;
                                    }
                                }
                            }
                        }


                    }
                }
            });

            MeshData meshData = _builder.ToMeshData();
            ChunkMeshBuilderPool.Release(_builder);
            return meshData;
        }



    }
}
