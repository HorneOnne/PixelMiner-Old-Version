using System.Collections.Generic;
using PixelMiner.Enums;
using UnityEngine;
using PixelMiner.DataStructure;
using PixelMiner.Utilities;
using System.Linq;

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
        private List<Quad> _listQuad;
        Vector2[] _blockUVs;
        Vector2[] _colormapUVs;

        public Block()
        {
            Debug.Log("Create new Block.cs");
            MeshDataList = new List<MeshData>();
            _listQuad = new List<Quad>();
            _blockUVs = new Vector2[4];
            _colormapUVs = new Vector2[4];
        }

        public void DrawSolid(BlockType blockType, bool[] neighbors, Vector3 offset = (default))
        {
            if (blockType != BlockType.Air)
            {
                for (int i = 0; i < neighbors.Length; i++)
                {
                    if (!neighbors[i])
                    {
                        GetBlockUVs(blockType, ref _blockUVs);
                        GetColorMapkUVs(ColorMapType.None, ref _colormapUVs);

                        if (blockType == BlockType.GrassSide)
                        {
                            if (i == (byte)BlockSide.Top)
                            {
                                GetBlockUVs(BlockType.GrassTop, ref _blockUVs);
                                GetColorMapkUVs(ColorMapType.Plains, ref _colormapUVs);
                                Quad q = QuadPool.Get();
                                q.Init((BlockSide)i, offset, uvs: _blockUVs, uv2s: _colormapUVs);
                                AddData(q);
                                QuadPool.Release(q);
         
                            }
                            else if (i == (byte)BlockSide.Bottom)
                            {
                                GetBlockUVs(BlockType.Dirt, ref _blockUVs);
                                Quad q = QuadPool.Get();
                                q.Init((BlockSide)i, offset, uvs: _blockUVs, uv2s: _colormapUVs);
                                AddData(q);
                                QuadPool.Release(q);
                            }
                            else
                            {
                                 Quad q = QuadPool.Get();
                               // Quad q = new Quad();
                                q.Init((BlockSide)i, offset, uvs: _blockUVs, uv2s: _colormapUVs);
                                AddData(q);
                                QuadPool.Release(q);
                            }
                        }
                        else
                        {
                            Quad q = QuadPool.Get();
                            q.Init((BlockSide)i, offset, uvs: _blockUVs, uv2s: _colormapUVs);
                            AddData(q);
                            QuadPool.Release(q);
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


        private void GetColorMapkUVs(ColorMapType colormapType, ref Vector2[] colormapUVs)
        {
            colormapUVs[0] = MeshUtils.ColorMapUVs[(ushort)colormapType, 0];
            colormapUVs[1] = MeshUtils.ColorMapUVs[(ushort)colormapType, 1];
            colormapUVs[2] = MeshUtils.ColorMapUVs[(ushort)colormapType, 2];
            colormapUVs[3] = MeshUtils.ColorMapUVs[(ushort)colormapType, 3];
        }


        private void AddData(Quad quad)
        {
            MeshDataList.Add(new MeshData()
            {
                //Vertices = quad.MeshData.Vertices,
                //Normals = quad.MeshData.Normals,
                //Triangles = quad.MeshData.Triangles,
                //UVs = quad.MeshData.UVs,
                //UV2s = quad.MeshData.UV2s,

                Vertices = quad._vertices.ToArray(),
                Normals = quad._normals.ToArray(),
                Triangles = quad._triangles.ToArray(),
                UVs = quad._uvs.ToArray(),
                UV2s = quad._uv2s.ToArray(),
            });
       

            quad.IsProcessing = false;
        }

        public void Reset()
        {
            MeshDataList.Clear();
            _listQuad.Clear();
        }
    }
}
