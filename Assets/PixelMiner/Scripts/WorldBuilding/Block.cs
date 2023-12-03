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
        public MeshData[] MeshDataArray { get; private set; }

        public Block() { }

        public void DrawSolid(BlockType blockType, bool[] neighbors, Vector3 offset = (default))
        {
            if(blockType != BlockType.Air)
            {
                List<Quad> q = new List<Quad>();
                for (int i = 0; i < neighbors.Length; i++)
                {
                    if (!neighbors[i])
                    {
                        Vector2[] blockUV = MeshUtils.GetBlockUV(blockType);
                     
                        if (blockType == BlockType.GrassSide)
                        {
                            if (i == (byte)BlockSide.Top)
                                q.Add(new Quad((BlockSide)i, offset, uvs: MeshUtils.GetBlockUV(BlockType.GrassTop), uv2s: MeshUtils.GetColorMapUV(ColorMapType.Plains)));
                            else if (i == (byte)BlockSide.Bottom)
                                q.Add(new Quad((BlockSide)i, offset, uvs: MeshUtils.GetBlockUV(BlockType.Dirt), uv2s: MeshUtils.GetColorMapUV(ColorMapType.None)));
                            else
                                q.Add(new Quad((BlockSide)i, offset, uvs: blockUV, uv2s: MeshUtils.GetColorMapUV(ColorMapType.None)));
                        }
                        else
                        {
                            q.Add(new Quad((BlockSide)i, offset, uvs: blockUV, uv2s: MeshUtils.GetColorMapUV(ColorMapType.None)));
                        }
                    }
                }
                if (q.Count == 0) return;
                MeshDataArray = new MeshData[q.Count];
                for (int i = 0; i < MeshDataArray.Length; i++)
                {
                    MeshDataArray[i] = new MeshData()
                    {
                        Vertices = q[i].MeshData.Vertices,
                        Normals = q[i].MeshData.Normals,
                        Triangles = q[i].MeshData.Triangles,      
                        UVs = q[i].MeshData.UVs,
                        UV2s = q[i].MeshData.UV2s,
                    };
                }
            }
        }

        public void DrawFluid(BlockType blockType, bool[] neighbors, float height, Vector3 offset = (default))
        {
            if (blockType != BlockType.Air)
            {
                List<Quad> q = new List<Quad>();
                for (int i = 0; i < neighbors.Length; i++)
                {
                    if (!neighbors[i])
                    {
                        q.Add(new Quad((BlockSide)i, offset, 
                            uvs: MeshUtils.GetBlockUV(blockType), 
                            uv2s: MeshUtils.GetDepthUVs(height)));
                    }
                }
                if (q.Count == 0) return;
                MeshDataArray = new MeshData[q.Count];
                for (int i = 0; i < MeshDataArray.Length; i++)
                {
                    MeshDataArray[i] = new MeshData()
                    {
                        Vertices = q[i].MeshData.Vertices,
                        Normals = q[i].MeshData.Normals,
                        Triangles = q[i].MeshData.Triangles,
                        UVs = q[i].MeshData.UVs,
                        UV2s = q[i].MeshData.UV2s,
                    };
                }
            }
        }
    }
}
