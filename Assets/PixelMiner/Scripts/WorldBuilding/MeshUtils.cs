using System.Threading.Tasks;
using UnityEngine;
using PixelMiner.Utilities;
using PixelMiner.Enums;
using System;


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


            //{new Vector2(0.875f, 0f), new Vector2(0.9375f, 0f),
            //new Vector2(0.875f, 0.0625f), new Vector2(0.9375f, 0.0625f)},

            //{new Vector2(0.875f, 0.0625f), new Vector2(0.9375f, 0.0625f),
            //new Vector2(0.875f, 0.125f), new Vector2(0.9375f, 0.125f)},

            //{new Vector2(0.875f, 0.125f), new Vector2(0.9375f, 0.125f),
            //new Vector2(0.875f, 0.1875f), new Vector2(0.9375f, 0.1875f)},

            //{new Vector2(0.875f, 0.1875f), new Vector2(0.9375f, 0.1875f),
            //new Vector2(0.875f, 0.25f), new Vector2(0.9375f, 0.25f)},

            //{new Vector2(0.875f, 0.25f), new Vector2(0.9375f, 0.25f),
            //new Vector2(0.875f, 0.3125f), new Vector2(0.9375f, 0.3125f)},

            //{new Vector2(0.875f, 0.3125f), new Vector2(0.9375f, 0.3125f),
            //new Vector2(0.875f, 0.375f), new Vector2(0.9375f, 0.375f)},

            //{new Vector2(0.875f, 0.375f), new Vector2(0.9375f, 0.375f),
            //new Vector2(0.875f, 0.4375f), new Vector2(0.9375f, 0.4375f)},

            //{new Vector2(0.875f, 0.4375f), new Vector2(0.9375f, 0.4375f),
            //new Vector2(0.875f, 0.5f), new Vector2(0.9375f, 0.5f)},

            //{new Vector2(0.875f, 0.5f), new Vector2(0.9375f, 0.5f),
            //new Vector2(0.875f, 0.5625f), new Vector2(0.9375f, 0.5625f)},

            //{new Vector2(0.875f, 0.5625f), new Vector2(0.9375f, 0.5625f),
            //new Vector2(0.875f, 0.625f), new Vector2(0.9375f, 0.625f)},

            //{new Vector2(0.875f, 0.625f), new Vector2(0.9375f, 0.625f),
            //new Vector2(0.875f, 0.6875f), new Vector2(0.9375f, 0.6875f)},

            //{new Vector2(0.875f, 0.6875f), new Vector2(0.9375f, 0.6875f),
            //new Vector2(0.875f, 0.75f), new Vector2(0.9375f, 0.75f)},

            //{new Vector2(0.875f, 0.75f), new Vector2(0.9375f, 0.75f),
            //new Vector2(0.875f, 0.8125f), new Vector2(0.9375f, 0.8125f)},

            //{new Vector2(0.875f, 0.8125f), new Vector2(0.9375f, 0.8125f),
            //new Vector2(0.875f, 0.875f), new Vector2(0.9375f, 0.875f)},

            //{new Vector2(0.875f, 0.875f), new Vector2(0.9375f, 0.875f),
            //new Vector2(0.875f, 0.9375f), new Vector2(0.9375f, 0.9375f)},

            //{new Vector2(0.875f, 0.9375f), new Vector2(0.9375f, 0.9375f),
            //new Vector2(0.875f, 1f), new Vector2(0.9375f, 1f)},
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

        public static Color32 GetLightColor(byte light)
        {
            float maxLight = 15.0f;
            //float channelValue = light / maxLight;
            //return new Color(channelValue, channelValue, channelValue, 1.0f);

            // Apply square function for a darker appearance
            float channelValue = Mathf.Pow(light / maxLight, 2);
            byte lightValue = (byte)(channelValue * 255);
            return new Color32(lightValue, lightValue, lightValue, 255);
        }

        public static async Task<MeshData> SolidGreedyMeshingAsync(Chunk chunk)
        {
            bool GreedyCompareLogic(Vector3Int a, Vector3Int b, int dimension, bool isBackFace)
            {
                BlockType blockA = chunk.GetBlock(a);
                BlockType blockB = chunk.GetBlock(b);

                Vector3Int c = b + new Vector3Int(1, 0, 1);
                Vector3Int d = b + new Vector3Int(-1, 0, 1);
                Vector3Int e = b + new Vector3Int(1, 0, -1);
                return blockA == blockB &&
                       chunk.IsSolid(b) &&
                       GetBlockLightPropagationForAdjacentFace(a, 1) == GetBlockLightPropagationForAdjacentFace(b, 1) &&
                        GetBlockLightPropagationForAdjacentFace(a, 1) == GetBlockLightPropagationForAdjacentFace(c, 1) &&
                        GetBlockLightPropagationForAdjacentFace(a, 1) == GetBlockLightPropagationForAdjacentFace(d, 1) &&
                        GetBlockLightPropagationForAdjacentFace(a, 1) == GetBlockLightPropagationForAdjacentFace(e, 1) &&
                       chunk.IsBlockFaceVisible(b, dimension, isBackFace);
            }
            byte GetBlockLightPropagationForAdjacentFace(Vector3Int blockPosition, int face)
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
                return chunk.GetBlockLight((blockPosition + offset));
            }
            byte GetAmbientLightPropagationForAdjacentFace(Vector3Int blockPosition, int face)
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
                return chunk.GetAmbientLight(blockPosition + offset);
            }


            ChunkMeshBuilder builder = ChunkMeshBuilderPool.Get();
            builder.InitOrLoad(chunk.Dimensions);

            Vector3Int startPos, currPos, quadSize, m, n, offsetPos;
            Vector3[] vertices = new Vector3[4];
            Vector3[] uvs = new Vector3[4];
            Vector2[] uv2s = new Vector2[4];
            Vector2[] uv3s = new Vector2[4];
            Color32[] colors = new Color32[4];

            BlockType currBlock;
            int d, u, v;
            Vector3Int dimensions = chunk.Dimensions;
            Color lightColor;

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
                                byte lightValue = GetBlockLightPropagationForAdjacentFace(startPos, voxelFace);
                                lightColor = GetLightColor(lightValue);

                                // If this block has already been merged, is air, or not visible -> skip it.
                                if (chunk.IsSolid(startPos) == false ||
                                    chunk.IsBlockFaceVisible(startPos, d, isBackFace) == false ||
                                    builder.Merged[d][startPos[u], startPos[v]])
                                {
                                    continue;
                                }


                                quadSize = new Vector3Int();

                                // Next step is loop to figure out width and height of the new merged quad.
                                for (currPos = startPos, currPos[u]++;
                                    currPos[u] < dimensions[u] &&
                                    GreedyCompareLogic(startPos, currPos, d, isBackFace) &&
                                    !builder.Merged[d][currPos[u], currPos[v]];
                                    currPos[u]++)
                                { }
                                quadSize[u] = currPos[u] - startPos[u];


                                for (currPos = startPos, currPos[v]++;
                                    currPos[v] < dimensions[v] &&
                                    GreedyCompareLogic(startPos, currPos, d, isBackFace) &&
                                    !builder.Merged[d][currPos[u], currPos[v]];
                                    currPos[v]++)
                                {


                                    for (currPos[u] = startPos[u];
                                        currPos[u] < dimensions[u] &&
                                        GreedyCompareLogic(startPos, currPos, d, isBackFace) &&
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

                                colors[0] = GetLightColor(GetBlockLightPropagationForAdjacentFace(startPos, voxelFace));
                                colors[1] = GetLightColor(GetBlockLightPropagationForAdjacentFace(startPos + m, voxelFace));
                                colors[2] = GetLightColor(GetBlockLightPropagationForAdjacentFace(startPos + m + n, voxelFace));
                                colors[3] = GetLightColor(GetBlockLightPropagationForAdjacentFace(startPos + n, voxelFace));



                                //if (lightValue == 0)
                                //{
                                //    colors[0] = lightColor;
                                //    colors[1] = lightColor;
                                //    colors[2] = lightColor;
                                //    colors[3] = lightColor;
                                //}
                                //else
                                //{
                                //    colors[0] = LightUtils.GetLightColor(GetLightPropagationForAdjacentFace(startPos, voxelFace));
                                //    colors[1] = LightUtils.GetLightColor(GetLightPropagationForAdjacentFace(startPos + m, voxelFace));
                                //    colors[2] = LightUtils.GetLightColor(GetLightPropagationForAdjacentFace(startPos + m + n, voxelFace));
                                //    colors[3] = LightUtils.GetLightColor(GetLightPropagationForAdjacentFace(startPos + n, voxelFace));
                                //}

                                uv3s[0] = LightMapUVs[GetAmbientLightPropagationForAdjacentFace(startPos, voxelFace), 0];
                                uv3s[1] = LightMapUVs[GetAmbientLightPropagationForAdjacentFace(startPos, voxelFace), 1];
                                uv3s[2] = LightMapUVs[GetAmbientLightPropagationForAdjacentFace(startPos, voxelFace), 2];
                                uv3s[3] = LightMapUVs[GetAmbientLightPropagationForAdjacentFace(startPos, voxelFace), 3];
                     

                                GetBlockUVs(currBlock, voxelFace, quadSize[u], quadSize[v], ref uvs, ref uv2s, ref uv3s);
                                builder.AddQuadFace(vertices, uvs, uv2s, uv3s, colors, voxelFace);


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
        private static void GetBlockUVs(BlockType blockType, int face, int width, int height, ref Vector3[] uvs, ref Vector2[] uv2s, ref Vector2[] uv3s)
        {
            int blockIndex;
            ColorMapType colorMapType;
            if (blockType == BlockType.GrassSide)
            {
                if (face == 1)
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
