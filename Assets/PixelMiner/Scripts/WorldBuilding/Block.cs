using System.Collections.Generic;
using PixelMiner.Enums;
using UnityEngine;
using PixelMiner.DataStructure;
using PixelMiner.Utilities;

namespace PixelMiner.WorldBuilding
{
    [System.Serializable]
    public class Block
    {
        /*
                4--------5
               /|       /|
              / |      / |
             /  |     /  |
            7--------6   |
            |  0-----|---1
            | /      |  /
            |/       | /
            3--------2
         */
        public List<MeshData> MeshDataList { get; private set; }
        private List<Quad> listQuad;
        Vector2[] blockUVs;

        public Block()
        {
            Debug.Log("A");
            MeshDataList = new List<MeshData>();
            listQuad = new List<Quad>();
            blockUVs = new Vector2[4];
        }

        public void DrawSolid(BlockType blockType, bool[] neighbors, Vector3 offset = (default))
        {
            bool useObjectPool = true;

            if (useObjectPool)
            {
                if (blockType != BlockType.Air)
                {
                    for (int i = 0; i < neighbors.Length; i++)
                    {
                        if (!neighbors[i])
                        {
                            GetBlockUVs(blockType, ref blockUVs);
                            if (blockType == BlockType.GrassSide)
                            {
                                if (i == (byte)BlockSide.Top)
                                {
                                    GetBlockUVs(BlockType.GrassTop, ref blockUVs);
                                    Quad q = QuadPool.Get();
                                    q.Init((BlockSide)i, offset, uvs: blockUVs, uv2s: MeshUtils.GetColorMapUV(ColorMapType.Plains));
                                    listQuad.Add(q);
                                    QuadPool.Release(q);
                                }
                                else if (i == (byte)BlockSide.Bottom)
                                {
                                    GetBlockUVs(BlockType.Dirt, ref blockUVs);
                                    Quad q = QuadPool.Get();
                                    q.Init((BlockSide)i, offset, uvs: blockUVs, uv2s: MeshUtils.GetColorMapUV(ColorMapType.None));
                                    listQuad.Add(q);
                                    QuadPool.Release(q);
                                }
                                else
                                {                             
                                    Quad q = QuadPool.Get();
                                    q.Init((BlockSide)i, offset, uvs: blockUVs, uv2s: MeshUtils.GetColorMapUV(ColorMapType.None));
                                    listQuad.Add(q);
                                    QuadPool.Release(q);
                                }
                            }
                            else
                            {
                                Quad q = QuadPool.Get();
                                q.Init((BlockSide)i, offset, uvs: blockUVs, uv2s: MeshUtils.GetColorMapUV(ColorMapType.None));
                                listQuad.Add(q);
                                QuadPool.Release(q);
                            }
                        }
                    }

                    if (listQuad.Count != 0)
                    {
                        for (int i = 0; i < listQuad.Count; i++)
                        {
                            MeshDataList.Add(new MeshData()
                            {
                                Vertices = listQuad[i].MeshData.Vertices,
                                Normals = listQuad[i].MeshData.Normals,
                                Triangles = listQuad[i].MeshData.Triangles,
                                UVs = listQuad[i].MeshData.UVs,
                                UV2s = listQuad[i].MeshData.UV2s,
                            });
                        }
                    }

                    listQuad.Clear();
                }
            }
            else
            {
                if (blockType != BlockType.Air)
                {
                    List<Quad> listQuad = new List<Quad>();
                    for (int i = 0; i < neighbors.Length; i++)
                    {
                        if (!neighbors[i])
                        {
                            Vector2[] blockUV = MeshUtils.GetBlockUV(blockType);

                            if (blockType == BlockType.GrassSide)
                            {
                                if (i == (byte)BlockSide.Top)
                                {
                                    listQuad.Add(new Quad((BlockSide)i, offset, uvs: MeshUtils.GetBlockUV(BlockType.GrassTop), uv2s: MeshUtils.GetColorMapUV(ColorMapType.Plains)));
                                }
                                else if (i == (byte)BlockSide.Bottom)
                                {
                                    listQuad.Add(new Quad((BlockSide)i, offset, uvs: MeshUtils.GetBlockUV(BlockType.Dirt), uv2s: MeshUtils.GetColorMapUV(ColorMapType.None)));
                                }
                                else
                                {
                                    listQuad.Add(new Quad((BlockSide)i, offset, uvs: blockUV, uv2s: MeshUtils.GetColorMapUV(ColorMapType.None)));
                                }
                            }
                            else
                            {
                                listQuad.Add(new Quad((BlockSide)i, offset, uvs: blockUV, uv2s: MeshUtils.GetColorMapUV(ColorMapType.None)));
                            }
                        }
                    }

                    if (listQuad.Count != 0)
                    {
                        for (int i = 0; i < listQuad.Count; i++)
                        {
                            MeshDataList.Add(new MeshData()
                            {
                                Vertices = listQuad[i].MeshData.Vertices,
                                Normals = listQuad[i].MeshData.Normals,
                                Triangles = listQuad[i].MeshData.Triangles,
                                UVs = listQuad[i].MeshData.UVs,
                                UV2s = listQuad[i].MeshData.UV2s,
                            });
                        }
                    }
                }
            }

        }

        public void DrawFluid(BlockType blockType, bool[] neighbors, float height, Vector3 offset = (default))
        {
            //if (blockType != BlockType.Air)
            //{
            //    List<Quad> q = new List<Quad>();
            //    for (int i = 0; i < neighbors.Length; i++)
            //    {
            //        if (!neighbors[i])
            //        {
            //            q.Add(new Quad((BlockSide)i, offset,
            //                uvs: MeshUtils.GetBlockUV(blockType),
            //                uv2s: MeshUtils.GetDepthUVs(height)));
            //        }
            //    }
            //    if (q.Count == 0) return;
            //    MeshDataArray = new MeshData[q.Count];
            //    for (int i = 0; i < MeshDataArray.Length; i++)
            //    {
            //        MeshDataArray[i] = new MeshData()
            //        {
            //            Vertices = q[i].MeshData.Vertices,
            //            Normals = q[i].MeshData.Normals,
            //            Triangles = q[i].MeshData.Triangles,
            //            UVs = q[i].MeshData.UVs,
            //            UV2s = q[i].MeshData.UV2s,
            //        };
            //    }
            //}
        }

        private void GetBlockUVs(BlockType blockType, ref Vector2[] blockUVs)
        {
            blockUVs[0] = MeshUtils.BlockUVs[(ushort)blockType, 0];
            blockUVs[1] = MeshUtils.BlockUVs[(ushort)blockType, 1];
            blockUVs[2] = MeshUtils.BlockUVs[(ushort)blockType, 2];
            blockUVs[3] = MeshUtils.BlockUVs[(ushort)blockType, 3];
        }



        public void Reset()
        {
            MeshDataList.Clear();
            listQuad.Clear();
        }
    }
}
