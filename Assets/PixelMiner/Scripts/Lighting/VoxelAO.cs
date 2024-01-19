using PixelMiner.World;
using UnityEngine;
using PixelMiner.Enums;

namespace PixelMiner.Lighting
{
    public class VoxelAO 
    {
        /* Voxel Face Index
        * 0: Right
        * 1: Up
        * 2: Front
        * 3: Left
        * 4: Down 
        * 5: Back
        */

        private static int CalculateVertexAO(bool side1, bool side2, bool corner)
        {
            if (side1 && side2)
                return 0;

            return 3 - (System.Convert.ToInt32(side1) + System.Convert.ToInt32(side2) + System.Convert.ToInt32(corner));
        }

        public static int ProcessAO(Chunk chunk, Vector3Int relativePosition, int vertex, int voxelFace, ref Vector3Int[] faceNeighbors)
        {
            int vertexAO = 3;

            // For transparent block like LEAVES i only calculate AO for voxel that face with AIR block.
            if(chunk.GetBlock(relativePosition).IsTransparentSolidBlock())
            {
                if (chunk.IsNeighborHasAirBlock(relativePosition, ref faceNeighbors) == false)
                {
                    return 0;
                }

            }


            switch (voxelFace)
            {
                case 0:
                    if (vertex == 0)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(1, -1, 0));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(1, 0, -1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(1, -1, -1));

                        vertexAO = CalculateVertexAO(side1.IsSolid() || side1.IsTransparentSolidBlock(), side2.IsSolid() || side2.IsTransparentSolidBlock(), corner.IsSolid() || corner.IsTransparentSolidBlock());
                    }
                    else if (vertex == 1)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(1, -1, 0));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(1, 0, 1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(1, -1, 1));

                        vertexAO = CalculateVertexAO(side1.IsSolid() || side1.IsTransparentSolidBlock(), side2.IsSolid() || side2.IsTransparentSolidBlock(), corner.IsSolid() || corner.IsTransparentSolidBlock());
                    }
                    else if (vertex == 2)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(1, 1, 0));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(1, 0, 1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(1, 1, 1));

                        vertexAO = CalculateVertexAO(side1.IsSolid() || side1.IsTransparentSolidBlock(), side2.IsSolid() || side2.IsTransparentSolidBlock(), corner.IsSolid() || corner.IsTransparentSolidBlock());
                    }
                    else if (vertex == 3)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(1, 1, 0));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(1, 0, -1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(1, 1, -1));

                        vertexAO = CalculateVertexAO(side1.IsSolid() || side1.IsTransparentSolidBlock(), side2.IsSolid() || side2.IsTransparentSolidBlock(), corner.IsSolid() || corner.IsTransparentSolidBlock());
                    }
                    break;
                case 1:
                    if (vertex == 0)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(-1, 1, 0));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(0, 1, -1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(-1, 1, -1));

                        vertexAO = CalculateVertexAO(side1.IsSolid() || side1.IsTransparentSolidBlock(), side2.IsSolid() || side2.IsTransparentSolidBlock(), corner.IsSolid() || corner.IsTransparentSolidBlock());
                    }
                    else if (vertex == 1)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(1, 1, 0));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(0, 1, -1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(1, 1, -1));

                        vertexAO = CalculateVertexAO(side1.IsSolid() || side1.IsTransparentSolidBlock(), side2.IsSolid() || side2.IsTransparentSolidBlock(), corner.IsSolid() || corner.IsTransparentSolidBlock());
                    }
                    else if (vertex == 2)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(1, 1, 0));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(0, 1, 1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(1, 1, 1));

                        vertexAO = CalculateVertexAO(side1.IsSolid() || side1.IsTransparentSolidBlock(), side2.IsSolid() || side2.IsTransparentSolidBlock(), corner.IsSolid() || corner.IsTransparentSolidBlock());
                    }
                    else if (vertex == 3)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(-1, 1, 0));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(0, 1, 1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(-1, 1, 1));

                        vertexAO = CalculateVertexAO(side1.IsSolid() || side1.IsTransparentSolidBlock(), side2.IsSolid() || side2.IsTransparentSolidBlock(), corner.IsSolid() || corner.IsTransparentSolidBlock());
                    }
                    break;
                case 2:
                    if (vertex == 0)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(0, -1, 1));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(1, 0, 1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(1, -1, 1));

                        vertexAO = CalculateVertexAO(side1.IsSolid() || side1.IsTransparentSolidBlock(), side2.IsSolid() || side2.IsTransparentSolidBlock(), corner.IsSolid() || corner.IsTransparentSolidBlock());
                    }
                    else if (vertex == 1)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(0, -1, 1));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(-1, 0, 1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(-1, -1, 1));

                        vertexAO = CalculateVertexAO(side1.IsSolid() || side1.IsTransparentSolidBlock(), side2.IsSolid() || side2.IsTransparentSolidBlock(), corner.IsSolid() || corner.IsTransparentSolidBlock());
                    }
                    else if (vertex == 2)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(-1, 0, 1));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(0, 1, 1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(-1, 1, 1));

                        vertexAO = CalculateVertexAO(side1.IsSolid() || side1.IsTransparentSolidBlock(), side2.IsSolid() || side2.IsTransparentSolidBlock(), corner.IsSolid() || corner.IsTransparentSolidBlock());
                    }
                    else if (vertex == 3)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(1, 0, 1));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(0, 1, 1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(1, 1, 1));

                        vertexAO = CalculateVertexAO(side1.IsSolid() || side1.IsTransparentSolidBlock(), side2.IsSolid() || side2.IsTransparentSolidBlock(), corner.IsSolid() || corner.IsTransparentSolidBlock());
                    }
                    break;                           
                case 3:
                    if (vertex == 0)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(-1, -1, 0));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(-1, 0, 1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(-1, -1, 1));

                        vertexAO = CalculateVertexAO(side1.IsSolid() || side1.IsTransparentSolidBlock(), side2.IsSolid() || side2.IsTransparentSolidBlock(), corner.IsSolid() || corner.IsTransparentSolidBlock());
                    }
                    else if (vertex == 1)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(-1, -1, 0));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(-1, 0, -1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(-1, -1, -1));

                        vertexAO = CalculateVertexAO(side1.IsSolid() || side1.IsTransparentSolidBlock(), side2.IsSolid() || side2.IsTransparentSolidBlock(), corner.IsSolid() || corner.IsTransparentSolidBlock());
                    }
                    else if (vertex == 2)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(-1, 1, 0));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(-1, 0, -1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(-1, 1, -1));

                        vertexAO = CalculateVertexAO(side1.IsSolid() || side1.IsTransparentSolidBlock(), side2.IsSolid() || side2.IsTransparentSolidBlock(), corner.IsSolid() || corner.IsTransparentSolidBlock());
                    }
                    else if (vertex == 3)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(-1, 1, 0));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(-1, 0, 1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(-1, 1, 1));

                        vertexAO = CalculateVertexAO(side1.IsSolid() || side1.IsTransparentSolidBlock(), side2.IsSolid() || side2.IsTransparentSolidBlock(), corner.IsSolid() || corner.IsTransparentSolidBlock());
                    }
                    break;
                case 4:                  
                    if (vertex == 0)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(-1, -1, 0));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(0, -1, 1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(-1, -1, 1));

                        vertexAO = CalculateVertexAO(side1.IsSolid() || side1.IsTransparentSolidBlock(), side2.IsSolid() || side2.IsTransparentSolidBlock(), corner.IsSolid() || corner.IsTransparentSolidBlock());
                    }
                    else if (vertex == 1)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(1, -1, 0));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(0, -1, 1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(1, -1, -1));

                        vertexAO = CalculateVertexAO(side1.IsSolid() || side1.IsTransparentSolidBlock(), side2.IsSolid() || side2.IsTransparentSolidBlock(), corner.IsSolid() || corner.IsTransparentSolidBlock());
                    }
                    else if (vertex == 2)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(0, -1, -1));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(1, -1, 0));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(1, -1, -1));

                        vertexAO = CalculateVertexAO(side1.IsSolid() || side1.IsTransparentSolidBlock(), side2.IsSolid() || side2.IsTransparentSolidBlock(), corner.IsSolid() || corner.IsTransparentSolidBlock());
                    }
                    else if (vertex == 3)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(0, -1, -1));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(-1, -1, 0));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(-1, -1, -1));

                        vertexAO = CalculateVertexAO(side1.IsSolid() || side1.IsTransparentSolidBlock(), side2.IsSolid() || side2.IsTransparentSolidBlock(), corner.IsSolid() || corner.IsTransparentSolidBlock());
                    }
                    break;
                case 5:
                    if (vertex == 0)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(0, -1, -1));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(-1, 0, -1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(-1, -1, -1));

                        vertexAO = CalculateVertexAO(side1.IsSolid() || side1.IsTransparentSolidBlock(), side2.IsSolid() || side2.IsTransparentSolidBlock(), corner.IsSolid() || corner.IsTransparentSolidBlock());
                    }
                    else if (vertex == 1)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(0, -1, -1));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(1, 0, -1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(1, -1, -1));

                        vertexAO = CalculateVertexAO(side1.IsSolid() || side1.IsTransparentSolidBlock(), side2.IsSolid() || side2.IsTransparentSolidBlock(), corner.IsSolid() || corner.IsTransparentSolidBlock());
                    }
                    else if (vertex == 2)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(0, 1, -1));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(1, 0, -1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(1, 1, -1));

                        vertexAO = CalculateVertexAO(side1.IsSolid() || side1.IsTransparentSolidBlock(), side2.IsSolid() || side2.IsTransparentSolidBlock(), corner.IsSolid() || corner.IsTransparentSolidBlock());
                    }
                    else if (vertex == 3)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(0, 1, -1));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(-1, 0, -1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(-1, 1, -1));

                        vertexAO = CalculateVertexAO(side1.IsSolid() || side1.IsTransparentSolidBlock(), side2.IsSolid() || side2.IsTransparentSolidBlock(), corner.IsSolid() || corner.IsTransparentSolidBlock());
                    }
                    break;
                default:
                    break;
            }
                    
            return vertexAO;
        }

    }
}
