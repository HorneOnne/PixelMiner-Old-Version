using System.Threading.Tasks;
using UnityEngine;
using PixelMiner.Enums;
using System;
using PixelMiner.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace PixelMiner.Core
{
    /* Voxel Face Index
        * 0: Right
        * 1: Up
        * 2: Front
        * 3: Left
        * 4: Down 
        * 5: Back
        */

    public class MeshUtils : MonoBehaviour
    {
        public static MeshUtils Instance { get; private set; }
        public ModelData TorchModel;
        private Main _main;
        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            _main = Main.Instance;  
        }

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

        public static void GetAOUVs(byte[] vertexAO, ref Vector4[] uv3s)
        {
            if (vertexAO != null)
            {
                if (vertexAO.Length != 4)
                {
                    throw new System.ArgumentException("A quad requires 4 vertex color.");
                }

                for (int i = 0; i < vertexAO.Length; i++)
                {
                    if (vertexAO[i] == 0)
                    {
                        vertexAO[i] = 208;
                    }
                    else if (vertexAO[i] == 1 || vertexAO[i] == 2)
                    {
                        vertexAO[i] = 224;
                    }
                    else if (vertexAO[i] == 3)
                    {
                        vertexAO[i] = 240;
                    }
                }
                uv3s[0] = new Vector4(vertexAO[0], vertexAO[1], vertexAO[2], vertexAO[3]);
                uv3s[1] = new Vector4(vertexAO[0], vertexAO[1], vertexAO[2], vertexAO[3]);
                uv3s[2] = new Vector4(vertexAO[0], vertexAO[1], vertexAO[2], vertexAO[3]);
                uv3s[3] = new Vector4(vertexAO[0], vertexAO[1], vertexAO[2], vertexAO[3]);
            }
        }
        public static void GetAmbientLightUVs(byte[] ambientLight, ref Vector4[] uv4s)
        {
            float i0 = ambientLight[0] / (float)Main.MAX_LIGHT_INTENSITY;
            float i1 = ambientLight[1] / (float)Main.MAX_LIGHT_INTENSITY;
            float i2 = ambientLight[2] / (float)Main.MAX_LIGHT_INTENSITY;
            float i3 = ambientLight[3] / (float)Main.MAX_LIGHT_INTENSITY;

            uv4s[0] = new Vector4(i0, i0, i0, i0);
            uv4s[1] = new Vector4(i1, i1, i1, i1);
            uv4s[2] = new Vector4(i2, i2, i2, i2);
            uv4s[3] = new Vector4(i3, i3, i3, i3);
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
        private static void GetBlockUVs(BlockType blockType, int face, int width, int height, ref Vector3[] uvs)
        {
            int blockIndex;
            switch (blockType)
            {
                default:
                    blockIndex = (ushort)blockType;
                    break;
                case BlockType.DirtGrass:
                    if (face == 1)
                    {
                        blockIndex = (ushort)TextureType.GrassTop;
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
                    break;
                case BlockType.PineLeaves:
                    blockIndex = (ushort)blockType;
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

        private static void GetNonblockUVs(BlockType blockType, ref Vector3[] uvs)
        {
            int blockIndex = (ushort)blockType;
            switch (blockType)
            {
                case BlockType.Torch:
                    uvs[0] = new Vector3(0, 0, blockIndex);
                    uvs[1] = new Vector3(1, 0, blockIndex);
                    uvs[2] = new Vector3(1, 1, blockIndex);
                    uvs[3] = new Vector3(0, 1, blockIndex);
                    break;
                default: 
                    break;
            }
        }

        public static void GetColorMapUVs(BlockType blockType, int face, float heatValue, ref Vector2[] uv2s)
        {
            GetColorMap(ref uv2s, heatValue, clear: true);
            switch (blockType)
            {
                default:
                    break;
                case BlockType.DirtGrass:
                    if (face == 1)
                    {
                        GetColorMap(ref uv2s, heatValue);
                    }
                    break;
                case BlockType.Leaves:
                    GetColorMap(ref uv2s, heatValue);
                    break;
                case BlockType.PineLeaves:
                    GetColorMap(ref uv2s, heatValue);
                    break;
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
            float channelValue = lightAnimCurve.Evaluate(light / (float)Main.MAX_LIGHT_INTENSITY);
            byte lightValue = (byte)Mathf.Clamp(channelValue * 255, 0, 255);
            return new Color32(lightValue, lightValue, lightValue, 255);
        }

        public static float GetLightIntensityNormalize(byte lightIntensity, AnimationCurve lightAnimCurve)
        {
            return lightAnimCurve.Evaluate(lightIntensity / Main.MAX_LIGHT_INTENSITY);
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

            //if (face == 1)
            //{
            //    offset = Vector3Int.up;
            //}
            //else if (face == 5)
            //{
            //    offset = new Vector3Int(0,-1,-2);
            //}
            //else
            //{
            //    offset = Vector3Int.zero;
            //}

            //Vector3Int blockOffsetPosition = blockPosition + offset;
            //blockOffsetPosition = new Vector3Int(Mathf.Clamp(blockOffsetPosition.x, 0, chunk._width),
            //                               Mathf.Clamp(blockOffsetPosition.y, 0, chunk._height),
            //                               Mathf.Clamp(blockOffsetPosition.z, 0, chunk._depth));
            //return chunk.GetAmbientLight(blockOffsetPosition);

            Vector3Int blockOffsetPosition = blockPosition + offset;
            return chunk.GetAmbientLight(blockOffsetPosition);
        }
        private static BlockType GetBlockTypeLightAdjacentFace(Chunk chunk, Vector3Int blockPosition, int face)
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
            return chunk.GetBlock(blockOffsetPosition);
        }






        private async Task<ChunkMeshBuilder> RenderChunkFace(Chunk chunk, int voxelFace, AnimationCurve lightAnimCurve, bool isTransparentMesh = false)
        {
            bool GreedyCompareLogic(Vector3Int a, Vector3Int b, int dimension, bool isBackFace, int voxelFace)
            {
                BlockType blockA = chunk.GetBlock(a);
                BlockType blockB = chunk.GetBlock(b);

                return blockA == blockB && chunk.IsTransparentBlockFaceVisible(b, dimension, isBackFace)
                    && BlockLightCompare(a, b)
                    && SkyLightCompare(a, b);
            }

            bool AOCompare(Vector3Int a, Vector3Int b, int voxelFace, ref Vector3Int[] faceNeighbors)
            {
                byte a0 = (byte)VoxelAO.ProcessAO(chunk, a, 0, voxelFace, ref faceNeighbors);
                byte a1 = (byte)VoxelAO.ProcessAO(chunk, a, 1, voxelFace, ref faceNeighbors);
                byte a2 = (byte)VoxelAO.ProcessAO(chunk, a, 2, voxelFace, ref faceNeighbors);
                byte a3 = (byte)VoxelAO.ProcessAO(chunk, a, 3, voxelFace, ref faceNeighbors);

                byte b0 = (byte)VoxelAO.ProcessAO(chunk, b, 0, voxelFace, ref faceNeighbors);
                byte b1 = (byte)VoxelAO.ProcessAO(chunk, b, 1, voxelFace, ref faceNeighbors);
                byte b2 = (byte)VoxelAO.ProcessAO(chunk, b, 2, voxelFace, ref faceNeighbors);
                byte b3 = (byte)VoxelAO.ProcessAO(chunk, b, 3, voxelFace, ref faceNeighbors);


                return (a0 == b0 && a1 == b1 && a2 == b2 && a3 == b3);
            }
            bool BlockLightCompare(Vector3Int a, Vector3Int b)
            {
                return chunk.GetBlockLight(a) == chunk.GetBlockLight(b);
            }
            bool SkyLightCompare(Vector3Int a, Vector3Int b)
            {
                return chunk.GetAmbientLight(a) == chunk.GetAmbientLight(b);
            }

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
            Vector4[] uv4s = new Vector4[4];
            Color32[] colors = new Color32[4];
            byte[] verticesAO = new byte[4];   // AO (0 -> 3)
            byte[] blockColorIntensity = new byte[4];
            byte[] ambientColorIntensity = new byte[4];
            Vector3Int[] faceNeighbors = new Vector3Int[6];
            BlockType currBlock;
            Color lightColor;
            bool smoothLight = true;
            bool greedyMeshing = true;

            BlockType[] slaf = new BlockType[4];   // Smooth light adjacent face neighbors
            bool[] slafSolid = new bool[4];   // Smooth light adjacent face neighbors

            // Greedy compare
            //Vector3Int[] faceNeighborsA = new Vector3Int[6];
            //Vector3Int[] faceNeighborsB = new Vector3Int[6];


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
                            if (startPos.y == 0 && voxelFace != 1) continue;
     

                            if (builder.Merged[d][startPos[u], startPos[v]])
                            {
                                continue;
                            }
                            


                            currBlock = chunk.GetBlock(startPos);
                            if (currBlock == BlockType.Air) continue;
                          


                            // If this block has already been merged, is air, or not visible -> skip it.
                            //if (chunk.IsSolid(startPos) == false ||
                            //    chunk.IsBlockFaceVisible(startPos, d, isBackFace) == false ||
                            //    builder.Merged[d][startPos[u], startPos[v]])
                            //{
                            //    continue;
                            //}



                            if (isTransparentMesh)
                            {
                                // TRANSPARENT SOLID
                                if (chunk.GetBlock(startPos).IsTransparentVoxel() == false)
                                {
                                    continue;
                                }

                                if (chunk.IsBlockFaceVisible(startPos, d, isBackFace) == false)
                                {
                                    continue;
                                }

                                if (chunk.IsTransparentBlockFaceVisible(startPos, d, isBackFace) == false)
                                {
                                    continue;
                                }

                            }
                            else
                            {
                                // OPAQUE SOLID
                                if (chunk.GetBlock(startPos).IsSolidVoxel() == false ||
                                    chunk.IsBlockFaceVisible(startPos, d, isBackFace) == false)
                                {
                                    continue;
                                }

                                if (chunk.GetBlock(startPos).IsTransparentVoxel() == true)
                                {
                                    continue;
                                }
                            }



                            // Ambient occlusion
                            // =================                
                            verticesAO[0] = (byte)VoxelAO.ProcessAO(chunk, startPos, 0, voxelFace, ref faceNeighbors);
                            verticesAO[1] = (byte)VoxelAO.ProcessAO(chunk, startPos, 1, voxelFace, ref faceNeighbors);
                            verticesAO[2] = (byte)VoxelAO.ProcessAO(chunk, startPos, 2, voxelFace, ref faceNeighbors);
                            verticesAO[3] = (byte)VoxelAO.ProcessAO(chunk, startPos, 3, voxelFace, ref faceNeighbors);

                            if (greedyMeshing && isTransparentMesh)
                            {
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


                            if (voxelFace == 3 || voxelFace == 2)
                            {
                                m = -m;
                            }


                            // Vertex AO Colors
                            //for (int i = 0; i < verticesAO.Length; i++)
                            //{
                            //    if (verticesAO[i] == 0)
                            //    {
                            //        colors[i] = new Color32(255, 0, 0, 255);
                            //    }
                            //    else if (verticesAO[i] == 1)
                            //    {
                            //        colors[i] = new Color32(0, 255, 0, 255);
                            //    }
                            //    else if (verticesAO[i] == 2)
                            //    {
                            //        colors[i] = new Color32(0, 0, 255, 255);
                            //    }
                            //    else
                            //    {
                            //        colors[i] = new Color32(255, 255, 255, 255);
                            //    }
                            //}


                            slaf[0] = GetBlockTypeLightAdjacentFace(chunk, startPos, voxelFace);
                            slaf[1] = GetBlockTypeLightAdjacentFace(chunk, startPos + m, voxelFace);
                            slaf[2] = GetBlockTypeLightAdjacentFace(chunk, startPos + m + n, voxelFace);
                            slaf[3] = GetBlockTypeLightAdjacentFace(chunk, startPos + n, voxelFace);

                            slafSolid[0] = slaf[0].IsSolidVoxel();
                            slafSolid[1] = slaf[1].IsSolidVoxel();
                            slafSolid[2] = slaf[2].IsSolidVoxel();
                            slafSolid[3] = slaf[3].IsSolidVoxel();

                            // BLock light
                            // ===========
                            // Because at solid block light not exist. We can only get light by the adjacent block and use it as the light as solid voxel face.
                            byte lightValue = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                            lightColor = GetLightColor(lightValue, lightAnimCurve);
                            if (smoothLight)
                            {
                                if (!slafSolid[0] && !slafSolid[1] && !slafSolid[2] && !slafSolid[3])
                                {
                                    blockColorIntensity[0] = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                    blockColorIntensity[1] = GetBlockLightPropagationForAdjacentFace(chunk, startPos + m, voxelFace);
                                    blockColorIntensity[2] = GetBlockLightPropagationForAdjacentFace(chunk, startPos + m + n, voxelFace);
                                    blockColorIntensity[3] = GetBlockLightPropagationForAdjacentFace(chunk, startPos + n, voxelFace);
                                }
                                else if (!slafSolid[0] && !slafSolid[1] && slafSolid[2] && slafSolid[3])
                                {
                                    blockColorIntensity[0] = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                    blockColorIntensity[1] = GetBlockLightPropagationForAdjacentFace(chunk, startPos + m, voxelFace);
                                    blockColorIntensity[2] = GetBlockLightPropagationForAdjacentFace(chunk, startPos + m, voxelFace);
                                    blockColorIntensity[3] = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                }
                                else if (!slafSolid[0] && slafSolid[1] && slafSolid[2] && !slafSolid[3])
                                {
                                    blockColorIntensity[0] = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                    blockColorIntensity[1] = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                    blockColorIntensity[2] = GetBlockLightPropagationForAdjacentFace(chunk, startPos + n, voxelFace);
                                    blockColorIntensity[3] = GetBlockLightPropagationForAdjacentFace(chunk, startPos + n, voxelFace);
                                }
                                else if (!slafSolid[0] && slafSolid[1] && !slafSolid[2] && !slafSolid[3])
                                {
                                    blockColorIntensity[0] = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                    blockColorIntensity[1] = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                    blockColorIntensity[2] = GetBlockLightPropagationForAdjacentFace(chunk, startPos + m + n, voxelFace);
                                    blockColorIntensity[3] = GetBlockLightPropagationForAdjacentFace(chunk, startPos + n, voxelFace);
                                }
                                else if (!slafSolid[0] && slafSolid[1] && slafSolid[2] && slafSolid[3])
                                {
                                    blockColorIntensity[0] = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                    blockColorIntensity[1] = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                    blockColorIntensity[2] = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                    blockColorIntensity[3] = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                }
                                else if (!slafSolid[0] && !slafSolid[1] && !slafSolid[2] && slafSolid[3])
                                {
                                    blockColorIntensity[0] = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                    blockColorIntensity[1] = GetBlockLightPropagationForAdjacentFace(chunk, startPos + m, voxelFace);
                                    blockColorIntensity[2] = GetBlockLightPropagationForAdjacentFace(chunk, startPos + m + n, voxelFace);
                                    blockColorIntensity[3] = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                }
                                else if (!slafSolid[0] && !slafSolid[1] && slafSolid[2] && !slafSolid[3])
                                {
                                    blockColorIntensity[0] = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                    blockColorIntensity[1] = GetBlockLightPropagationForAdjacentFace(chunk, startPos + m, voxelFace);
                                    blockColorIntensity[2] = GetBlockLightPropagationForAdjacentFace(chunk, startPos + m, voxelFace);
                                    blockColorIntensity[3] = GetBlockLightPropagationForAdjacentFace(chunk, startPos + n, voxelFace);
                                }
                                else
                                {
                                    blockColorIntensity[0] = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                    blockColorIntensity[0] = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                    blockColorIntensity[0] = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                    blockColorIntensity[0] = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                }
                            }
                            else
                            {
                                colors[0] = GetLightColor(blockColorIntensity[0], lightAnimCurve);
                                colors[1] = GetLightColor(blockColorIntensity[0], lightAnimCurve);
                                colors[2] = GetLightColor(blockColorIntensity[0], lightAnimCurve);
                                colors[3] = GetLightColor(blockColorIntensity[0], lightAnimCurve);
                            }

                            colors[0] = GetLightColor(blockColorIntensity[0], lightAnimCurve);
                            colors[1] = GetLightColor(blockColorIntensity[1], lightAnimCurve);
                            colors[2] = GetLightColor(blockColorIntensity[2], lightAnimCurve);
                            colors[3] = GetLightColor(blockColorIntensity[3], lightAnimCurve);




                            // Ambient Lights
                            byte ambientLight = chunk.GetAmbientLight(startPos);
                            ambientColorIntensity[0] = 0;
                            ambientColorIntensity[1] = 0;
                            ambientColorIntensity[2] = 0;
                            ambientColorIntensity[3] = 0;

                            if (!slafSolid[0] && !slafSolid[1] && !slafSolid[2] && !slafSolid[3])
                            {
                                ambientColorIntensity[0] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                ambientColorIntensity[1] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos + m, voxelFace);
                                ambientColorIntensity[2] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos + m + n, voxelFace);
                                ambientColorIntensity[3] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos + n, voxelFace);
                            }
                            else if (!slafSolid[0] && !slafSolid[1] && slafSolid[2] && slafSolid[3])
                            {
                                ambientColorIntensity[0] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                ambientColorIntensity[1] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos + m, voxelFace);
                                ambientColorIntensity[2] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos + m, voxelFace);
                                ambientColorIntensity[3] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                            }
                            else if (!slafSolid[0] && slafSolid[1] && slafSolid[2] && !slafSolid[3])
                            {
                                ambientColorIntensity[0] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                ambientColorIntensity[1] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                ambientColorIntensity[2] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos + n, voxelFace);
                                ambientColorIntensity[3] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos + n, voxelFace);
                            }
                            else if (!slafSolid[0] && slafSolid[1] && !slafSolid[2] && !slafSolid[3])
                            {
                                ambientColorIntensity[0] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                ambientColorIntensity[1] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                ambientColorIntensity[2] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos + m + n, voxelFace);
                                ambientColorIntensity[3] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos + n, voxelFace);
                            }
                            else if (!slafSolid[0] && slafSolid[1] && slafSolid[2] && slafSolid[3])
                            {
                                ambientColorIntensity[0] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                ambientColorIntensity[1] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                ambientColorIntensity[2] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                ambientColorIntensity[3] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                            }
                            else if (!slafSolid[0] && !slafSolid[1] && !slafSolid[2] && slafSolid[3])
                            {
                                ambientColorIntensity[0] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                ambientColorIntensity[1] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos + m, voxelFace);
                                ambientColorIntensity[2] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos + m + n, voxelFace);
                                ambientColorIntensity[3] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                            }
                            else if (!slafSolid[0] && !slafSolid[1] && slafSolid[2] && !slafSolid[3])
                            {
                                ambientColorIntensity[0] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                ambientColorIntensity[1] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos + m, voxelFace);
                                ambientColorIntensity[2] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos + m, voxelFace);
                                ambientColorIntensity[3] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos + n, voxelFace);
                            }

                            GetBlockUVs(currBlock, voxelFace, quadSize[u], quadSize[v], ref uvs);
                            GetColorMapUVs(currBlock, voxelFace, chunk.GetHeat(currPos), ref uv2s);
                            GetAOUVs(verticesAO, ref uv3s);
                            GetAmbientLightUVs(ambientColorIntensity, ref uv4s);
                            builder.AddQuadFace(voxelFace, vertices, colors, uvs, uv2s, uv3s, uv4s);


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
        public async Task<MeshData> RenderSolidMesh(Chunk chunk, AnimationCurve lightAnimCurve, bool isTransparentMesh = false)
        {



            bool GreeyAO(Chunk chunkA, Vector3Int relativePosA, int voxelFaceA, Chunk chunkB, Vector3Int relativePosB, int voxelFaceB)
            {
                //byte a0 = (byte)VoxelAO.ProcessAO(chunkA, relativePosB, 0, voxelFaceA);
                //byte a1 = (byte)VoxelAO.ProcessAO(chunkA, relativePosB, 1, voxelFaceA);
                //byte a2 = (byte)VoxelAO.ProcessAO(chunkA, relativePosB, 2, voxelFaceA);
                //byte a3 = (byte)VoxelAO.ProcessAO(chunkA, relativePosB, 3, voxelFaceA);

                //byte slaf[0] = (byte)VoxelAO.ProcessAO(chunkB, relativePosB, 0, voxelFaceB);
                //byte slaf[1] = (byte)VoxelAO.ProcessAO(chunkB, relativePosB, 1, voxelFaceB);
                //byte slaf[2] = (byte)VoxelAO.ProcessAO(chunkB, relativePosB, 2, voxelFaceB);
                //byte slaf[3] = (byte)VoxelAO.ProcessAO(chunkB, relativePosB, 3, voxelFaceB);

                //bool allEqual = (a0 == slaf[0]) && (a1 == slaf[1]) && (a2 == slaf[2]) && (a3 == slaf[3]);

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

                    buildMeshTaskList.Add(RenderChunkFace(chunk, voxelFace, lightAnimCurve, isTransparentMesh));

                }
            });

            await Task.WhenAll(buildMeshTaskList);
            for (int i = 0; i < buildMeshTaskList.Count; i++)
            {
                finalBuilder.Add(buildMeshTaskList[i].Result);
            }


            MeshData meshData = finalBuilder.ToMeshData();
            ChunkMeshBuilderPool.Release(finalBuilder);
            for (int i = 0; i < buildMeshTaskList.Count; i++)
            {
                ChunkMeshBuilderPool.Release(buildMeshTaskList[i].Result);
            }


            return meshData;
        }
        public async Task<MeshData> GetChunkGrassMeshData(Chunk chunk, AnimationCurve lightAnimCurve, FastNoiseLite randomNoise)
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
                        colors[0] = lightColor;
                        colors[1] = lightColor;
                        colors[2] = lightColor;
                        colors[3] = lightColor;

                        if (applyRotation)
                        {
                            float rotationAngle = ((float)(randomNoise.GetNoise(x + 1, y + 1) + 1.0f) / 2.0f * (maxRotationAngle - minRotationAngle) + minRotationAngle);
                            float rotationAngleRad = rotationAngle * Mathf.Deg2Rad;
                            Quaternion rotation = Quaternion.Euler(0f, rotationAngle, 0f);
                            int heightFromOrigin = _main.GetBlockHeightFromOrigin(chunk, curBlockPos);
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
                            int heightFromOrigin = _main.GetBlockHeightFromOrigin(chunk, curBlockPos);
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
        public async Task<MeshData> RenderSolidNonvoxelMesh(Chunk chunk, AnimationCurve lightAnimCurve)
        {
            ChunkMeshBuilder builder = ChunkMeshBuilderPool.Get();
            builder.InitOrLoad(chunk.Dimensions);

            Vector3[] vertices = new Vector3[4];
            Vector3[] uvs = new Vector3[4];
            Vector2[] uv2s = new Vector2[4];
            Vector4[] uv3s = new Vector4[4];
            Color32[] colors = new Color32[4];
            int[] tris = new int[6];
            byte[] verticesAO = new byte[4];
            byte[] vertexColorIntensity = new byte[4];
            Vector3 _centerOffset = new Vector3(0.5f, 0.5f, 0.5f);

            await Task.Run(() =>
            {

                for (int i = 0; i < chunk.ChunkData.Length; i++)
                {
                    int x = i % chunk._width;
                    int y = (i / chunk._width) % chunk._height;
                    int z = i / (chunk._width * chunk._height);

                    BlockType currBlock = chunk.ChunkData[i];
                    if (currBlock == BlockType.Torch)
                    {
                        Vector3Int curBlockPos = new Vector3Int(x, y, z);
                        Vector3 offsetTorchPos = curBlockPos + new Vector3(0.5f, 0.5f, 0.5f);
                        for (int j = 0; j < TorchModel.Vertices.Count; j++)
                        {
                            if (j % 4 == 0)
                            {
                                vertices[0] = offsetTorchPos + TorchModel.Vertices[j];
                                vertices[1] = offsetTorchPos + TorchModel.Vertices[j + 1];
                                vertices[2] = offsetTorchPos + TorchModel.Vertices[j + 2];
                                vertices[3] = offsetTorchPos + TorchModel.Vertices[j + 3];

                                tris[0] = TorchModel.Triangles[j / 4 * 6 + 0];
                                tris[1] = TorchModel.Triangles[j / 4 * 6 + 1];
                                tris[2] = TorchModel.Triangles[j / 4 * 6 + 2];
                                tris[3] = TorchModel.Triangles[j / 4 * 6 + 3];
                                tris[4] = TorchModel.Triangles[j / 4 * 6 + 4];
                                tris[5] = TorchModel.Triangles[j / 4 * 6 + 5];


                                GetNonblockUVs(BlockType.Torch, ref uvs);
                                builder.AddQuadFace(vertices, tris, uvs);
                            }
                        }

                      

                    }
                }
            });



            MeshData meshData = builder.ToMeshData();
            ChunkMeshBuilderPool.Release(builder);
            return meshData;
        }

        public async Task<MeshData> SolidGreedyMeshingForColliderAsync(Chunk chunk)
        {
            bool GreedyCompareLogic(Vector3Int a, Vector3Int b, int dimension, bool isBackFace)
            {
                return chunk.GetBlock(b).IsSolidVoxel() && chunk.IsBlockFaceVisible(b, dimension, isBackFace);
            }

            ChunkMeshBuilder builder = ChunkMeshBuilderPool.Get();
            builder.InitOrLoad(chunk.Dimensions);

            Vector3Int startPos, currPos, quadSize, m, n, offsetPos;
            Vector3[] vertices = new Vector3[4];
            Vector3[] uvs = new Vector3[4];
            BlockType currBlock;
            int d, u, v;
            Vector3Int dimensions = chunk.Dimensions;

            await Task.Run(() =>
            {
                for (int voxelFace = 0; voxelFace < 6; voxelFace++)
                {
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
                                if (startPos.y == 0 && voxelFace != 1) continue;
                                if (builder.Merged[d][startPos[u], startPos[v]])
                                {
                                    continue;
                                }

                                currBlock = chunk.GetBlock(startPos);
                                if (currBlock.IsSolidVoxel() == false) continue;

                                // If this block has already been merged, is air, or not visible -> skip it.
                                if (currBlock.IsSolidVoxel() == false ||
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



                                if (voxelFace == 2 || voxelFace == 3)
                                {
                                    vertices[1] = offsetPos;
                                    vertices[0] = offsetPos + m;
                                    vertices[3] = offsetPos + m + n;
                                    vertices[2] = offsetPos + n;
                                }
                                else
                                {
                                    vertices[0] = offsetPos;
                                    vertices[1] = offsetPos + m;
                                    vertices[2] = offsetPos + m + n;
                                    vertices[3] = offsetPos + n;
                                }



                                builder.AddQuadFace(vertices);

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

        public async Task<MeshData> WaterGreedyMeshingAsync(Chunk chunk, AnimationCurve lightAnimCurve)
        {
            bool GreedyCompareLogic(Vector3Int a, Vector3Int b, int dimension, bool isBackFace)
            {
                BlockType blockA = chunk.GetBlock(a);
                BlockType blockB = chunk.GetBlock(b);

                return blockA == blockB && chunk.IsWater(b) && chunk.IsWaterFaceVisible(b, dimension, isBackFace);
            }

            ChunkMeshBuilder builder = ChunkMeshBuilderPool.Get();
            builder.InitOrLoad(chunk.Dimensions);

            Vector3Int startPos, currPos, quadSize = Vector3Int.one, m, n, offsetPos;
            Vector3[] vertices = new Vector3[4];
            Vector3[] uvs = new Vector3[4];
            Vector2[] uv2s = new Vector2[4];
            Vector4[] uv3s = new Vector4[4];
            Vector4[] uv4s = new Vector4[4];
            Color32[] colors = new Color32[4];
            byte[] verticesAO = new byte[4];   // AO (0 -> 3)
            byte[] blockColorIntensity = new byte[4];
            byte[] ambientColorIntensity = new byte[4];
            Vector3Int[] faceNeighbors = new Vector3Int[6];
            BlockType currBlock;
            Color lightColor;
            bool smoothLight = true;
            bool greedyMeshing = true;
            int d, u, v;
            bool isBackFace;
            Vector3Int dimensions = chunk.Dimensions;
            await Task.Run(() =>
            {
                for (int voxelFace = 0; voxelFace < 6; voxelFace++)
                {
                    //if (voxelFace != 1) continue;

                    isBackFace = voxelFace > 2;
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
                                if (startPos.y == 0 && voxelFace != 1) continue;
                                if (builder.Merged[d][startPos[u], startPos[v]])
                                {
                                    continue;
                                }

                                currBlock = chunk.GetBlock(startPos);

                                // If this block has already been merged, is air, or not visible -> skip it.
                                if (currBlock != BlockType.Water ||
                                    chunk.IsWaterFaceVisible(startPos, d, isBackFace) == false ||
                                    builder.Merged[d][startPos[u], startPos[v]])
                                {
                                    continue;
                                }



                                // Ambient occlusion
                                // =================
                                verticesAO[0] = (byte)VoxelAO.ProcessAO(chunk, startPos, 0, voxelFace, ref faceNeighbors);
                                verticesAO[1] = (byte)VoxelAO.ProcessAO(chunk, startPos, 1, voxelFace, ref faceNeighbors);
                                verticesAO[2] = (byte)VoxelAO.ProcessAO(chunk, startPos, 2, voxelFace, ref faceNeighbors);
                                verticesAO[3] = (byte)VoxelAO.ProcessAO(chunk, startPos, 3, voxelFace, ref faceNeighbors);


                                quadSize = Vector3Int.one;
                                if (greedyMeshing)
                                {
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


                                if (voxelFace == 3 || voxelFace == 2)
                                {
                                    m = -m;
                                }


                                blockColorIntensity[0] = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                blockColorIntensity[0] = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                blockColorIntensity[0] = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                blockColorIntensity[0] = GetBlockLightPropagationForAdjacentFace(chunk, startPos, voxelFace);

                                colors[0] = GetLightColor(blockColorIntensity[0], lightAnimCurve);
                                colors[1] = GetLightColor(blockColorIntensity[1], lightAnimCurve);
                                colors[2] = GetLightColor(blockColorIntensity[2], lightAnimCurve);
                                colors[3] = GetLightColor(blockColorIntensity[3], lightAnimCurve);




                                // Ambient Lights
                                byte ambientLight = chunk.GetAmbientLight(startPos);

                                ambientColorIntensity[0] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                ambientColorIntensity[1] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                ambientColorIntensity[2] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos, voxelFace);
                                ambientColorIntensity[3] = GetAmbientLightPropagationForAdjacentFace(chunk, startPos, voxelFace);


                                GetBlockUVs(currBlock, voxelFace, quadSize[u], quadSize[v], ref uvs);
                                GetColorMapUVs(currBlock, voxelFace, chunk.GetHeat(currPos), ref uv2s);
                                GetAOUVs(verticesAO, ref uv3s);
                                GetAmbientLightUVs(ambientColorIntensity, ref uv4s);
                                builder.AddQuadFace(voxelFace, vertices, colors, uvs, uv2s, uv3s, uv4s);



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

    }
}
