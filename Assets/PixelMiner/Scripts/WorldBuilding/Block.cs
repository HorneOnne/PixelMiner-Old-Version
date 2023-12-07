using System.Collections.Generic;
using PixelMiner.Enums;
using UnityEngine;

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


        Vector2[] _blockUVs;
        Vector2[] _colormapUVs;
        //public List<Vector3> _vertices;
        //public List<Vector3> _normals;
        //public List<Vector2> _uvs;
        //public List<Vector2> _uv2s;
        //public int QuadCount;
        public List<Quad> Quads = new List<Quad>();


        public Block()
        {
            Debug.Log("Create new Block.cs");
            _blockUVs = new Vector2[4];
            _colormapUVs = new Vector2[4];
            //_vertices = new List<Vector3>();
            //_normals = new List<Vector3>(); 
            //_uvs = new List<Vector2>();
            //_uv2s = new List<Vector2>();
            //QuadCount = 0;
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
                                AddQuadData(q);
                               // QuadPool.Release(q);
         
                            }
                            else if (i == (byte)BlockSide.Bottom)
                            {
                                GetBlockUVs(BlockType.Dirt, ref _blockUVs);
                                Quad q = QuadPool.Get();
                                q.Init((BlockSide)i, offset, uvs: _blockUVs, uv2s: _colormapUVs);
                                AddQuadData(q);
                                //QuadPool.Release(q);
                            }
                            else
                            {
                                Quad q = QuadPool.Get();
                                q.Init((BlockSide)i, offset, uvs: _blockUVs, uv2s: _colormapUVs);
                                AddQuadData(q);
                                //QuadPool.Release(q);
                            }
                        }
                        else
                        {
                            Quad q = QuadPool.Get();
                            q.Init((BlockSide)i, offset, uvs: _blockUVs, uv2s: _colormapUVs);
                            AddQuadData(q);
                            //QuadPool.Release(q);
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
                                AddQuadData(q);
                                QuadPool.Release(q);

                            }
                            else if (i == (byte)BlockSide.Bottom)
                            {
                                GetBlockUVs(BlockType.Dirt, ref _blockUVs);
                                Quad q = QuadPool.Get();
                                q.Init((BlockSide)i, offset, uvs: _blockUVs, uv2s: _colormapUVs);
                                AddQuadData(q);
                                QuadPool.Release(q);
                            }
                            else
                            {
                                Quad q = QuadPool.Get();
                                q.Init((BlockSide)i, offset, uvs: _blockUVs, uv2s: _colormapUVs);
                                AddQuadData(q);
                                QuadPool.Release(q);
                            }
                        }
                        else
                        {
                            Quad q = QuadPool.Get();
                            q.Init((BlockSide)i, offset, uvs: _blockUVs, uv2s: _colormapUVs);
                            AddQuadData(q);
                            QuadPool.Release(q);
                        }
                    }
                }
            }
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


        private void AddQuadData(Quad quad)
        {
            Quads.Add(quad);

            //_vertices.AddRange(quad._vertices);
            //_normals.AddRange(quad._normals);
            //_uvs.AddRange(quad._uvs);
            //_uv2s.AddRange(quad._uv2s);
            //QuadCount++;
        }

        public void Reset()
        {
            //_vertices.Clear();
            //_normals.Clear();
            //_uvs.Clear();
            //_uv2s.Clear();
            //QuadCount = 0;

            Quads.Clear();
        }
    }
}
