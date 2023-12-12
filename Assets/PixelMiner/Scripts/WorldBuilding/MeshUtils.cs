using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using PixelMiner.Utilities;
using System.Collections.Generic;
using PixelMiner.Enums;
using System;

namespace PixelMiner.WorldBuilding
{
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



        public static async Task<MeshData> SolidGreedyMeshingAsync(Chunk chunk)
        {
            bool GreedyCompareLogic(Vector3Int a, Vector3Int b, int dimension, bool isBackFace)
            {
                BlockType blockA = chunk.GetBlock(a);
                BlockType blockB = chunk.GetBlock(b);

                return blockA == blockB && chunk.IsSolid(b) && chunk.IsBlockFaceVisible(b, dimension, isBackFace);
            }

            ChunkMeshBuilder _builder = ChunkMeshBuilderPool.Get();
            _builder.InitOrLoad(chunk.Dimensions);

            Vector3Int startPos, currPos, quadSize, m, n, offsetPos;
            Vector3[] vertices = new Vector3[4];
            Vector3[] uvs = new Vector3[4];
            Vector2[] uv2s = new Vector2[4];
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
                    //if(voxelFace == 4) continue;

                    bool isBackFace = voxelFace > 2;
                    d = voxelFace % 3;
                    u = (d + 1) % 3;
                    v = (d + 2) % 3;

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
                  
                                GetBlockUVs(currBlock, voxelFace, quadSize[u], quadSize[v], ref uvs, ref uv2s);
                                _builder.AddQuadFace(vertices, uvs, uv2s, isBackFace);


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

        public static async Task<MeshData> WaterGreedyMeshingAsync(Chunk chunk)
        {
            bool GreedyCompareLogic(Vector3Int a, Vector3Int b, int dimension, bool isBackFace)
            {
                BlockType blockA = chunk.GetBlock(a);
                BlockType blockB = chunk.GetBlock(b);

                return blockA == blockB && chunk.IsWater(b) && chunk.IsBlockFaceVisible(b, dimension, isBackFace);
            }

            ChunkMeshBuilder _builder = ChunkMeshBuilderPool.Get();
            _builder.InitOrLoad(chunk.Dimensions);

            Vector3Int startPos, currPos, quadSize, m, n, offsetPos;
            Vector3[] vertices = new Vector3[4];
            Vector3[] uvs = new Vector3[4];
            Vector2[] uv2s = new Vector2[4];
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
                    //if(voxelFace == 4) continue;
                    if (voxelFace != 1)
                        continue;

                    bool isBackFace = voxelFace > 2;
                    d = voxelFace % 3;
                    u = (d + 1) % 3;
                    v = (d + 2) % 3;

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
                                if (chunk.IsWater(startPos) == false ||
                                    chunk.IsWaterFaceVisible(startPos, d, isBackFace) == false ||
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

                                GetBlockUVs(currBlock, voxelFace, quadSize[u], quadSize[v], ref uvs, ref uv2s);
                                _builder.AddQuadFace(vertices, uvs, uv2s, isBackFace);


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
        private static void GetBlockUVs(BlockType blockType, int face, int width, int height, ref Vector3[] uvs, ref Vector2[] uv2s)
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
    }
}
