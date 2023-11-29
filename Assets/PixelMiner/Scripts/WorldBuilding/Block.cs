using System.Collections.Generic;
using PixelMiner.Enums;
using UnityEngine;
using PixelMiner.DataStructure;

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

        public Block(BlockType blockType, bool[] solidNeighbors, Vector3 offset = (default))
        {        
            Draw(blockType, solidNeighbors, offset);
        }

        public void Draw(BlockType blockType, bool[] solidNeighbors, Vector3 offset = (default))
        {
            if(blockType != BlockType.Air)
            {
                List<Quad> q = new List<Quad>();
                for (int i = 0; i < solidNeighbors.Length; i++)
                {
                    if (!solidNeighbors[i])
                    {
                        if(blockType == BlockType.GrassSide)
                        {
                            if (i == (byte)BlockSide.Top)
                                q.Add(new Quad((BlockSide)i, BlockType.GrassTop, ColorMapType.Plains, offset));
                            else if (i == (byte)BlockSide.Bottom)
                                q.Add(new Quad((BlockSide)i, BlockType.Dirt, ColorMapType.None, offset));
                            else
                                q.Add(new Quad((BlockSide)i, blockType, ColorMapType.None, offset));
                        }
                        else
                        {
                            q.Add(new Quad((BlockSide)i, blockType, ColorMapType.None, offset));
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
    }
}
