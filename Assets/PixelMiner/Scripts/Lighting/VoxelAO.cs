using JetBrains.Annotations;
using PixelMiner.WorldBuilding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelMiner.Enums;
using PixelMiner.Core;


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

        public static int ProcessAO(Chunk chunk, Vector3Int relativePosition, int vertex, int voxelFace)
        {
            int vertexAO = 3;
            switch (voxelFace)
            {
                case 0:
                    if (vertex == 0)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(1, -1, 0));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(1, 0, -1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(1, -1, -1));

                        vertexAO = CalculateVertexAO(side1.IsSolid(), side2.IsSolid(), corner.IsSolid());
                    }
                    else if (vertex == 1)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(1, -1, 0));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(1, 0, 1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(1, -1, 1));

                        vertexAO = CalculateVertexAO(side1.IsSolid(), side2.IsSolid(), corner.IsSolid());
                    }
                    else if (vertex == 2)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(1, 1, 0));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(0, 1, 1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(1, 1, 1));

                        vertexAO = CalculateVertexAO(side1.IsSolid(), side2.IsSolid(), corner.IsSolid());
                    }
                    else if (vertex == 3)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(1, 1, 0));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(0, 1, -1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(1, 1, -1));

                        vertexAO = CalculateVertexAO(side1.IsSolid(), side2.IsSolid(), corner.IsSolid());
                    }
                    break;
                case 1:
                    if (vertex == 0)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(-1, 1, 0));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(0, 1, -1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(-1, 1, -1));

                        vertexAO = CalculateVertexAO(side1.IsSolid(), side2.IsSolid(), corner.IsSolid());
                    }
                    else if (vertex == 1)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(1, 1, 0));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(0, 1, -1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(1, 1, -1));

                        vertexAO = CalculateVertexAO(side1.IsSolid(), side2.IsSolid(), corner.IsSolid());
                    }
                    else if (vertex == 2)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(1, 1, 0));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(0, 1, 1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(1, 1, 1));

                        vertexAO = CalculateVertexAO(side1.IsSolid(), side2.IsSolid(), corner.IsSolid());
                    }
                    else if (vertex == 3)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(-1, 1, 0));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(0, 1, 1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(-1, 1, 1));

                        vertexAO = CalculateVertexAO(side1.IsSolid(), side2.IsSolid(), corner.IsSolid());
                    }
                    break;
                case 5:
                    if (vertex == 0)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(0, -1, -1));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(-1, 0, -1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(-1, -1, -1));

                        vertexAO = CalculateVertexAO(side1.IsSolid(), side2.IsSolid(), corner.IsSolid());
                    }
                    else if (vertex == 1)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(0, -1, -1));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(1, 0, -1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(1, -1, -1));

                        vertexAO = CalculateVertexAO(side1.IsSolid(), side2.IsSolid(), corner.IsSolid());
                    }
                    else if (vertex == 2)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(0, 1, -1));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(1, 0, -1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(1, 1, -1));

                        vertexAO = CalculateVertexAO(side1.IsSolid(), side2.IsSolid(), corner.IsSolid());
                    }
                    else if (vertex == 3)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(0, 1, -1));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(-1, 0, -1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(-1, 1, 0));

                        vertexAO = CalculateVertexAO(side1.IsSolid(), side2.IsSolid(), corner.IsSolid());
                    }
                    break;
                case 2:
                    if (vertex == 0)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(0, -1, 1));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(1, 0, 1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(1, -1, 1));

                        vertexAO = CalculateVertexAO(side1.IsSolid(), side2.IsSolid(), corner.IsSolid());
                    }
                    else if (vertex == 1)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(0, -1, 1));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(-1, 0, 1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(-1, -1, 1));

                        vertexAO = CalculateVertexAO(side1.IsSolid(), side2.IsSolid(), corner.IsSolid());
                    }
                    else if (vertex == 2)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(-1, 1, 0));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(0, 1, 1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(-1, 1, 1));

                        vertexAO = CalculateVertexAO(side1.IsSolid(), side2.IsSolid(), corner.IsSolid());
                    }
                    else if (vertex == 3)
                    {
                        BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(1, 1, 0));
                        BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(0, 1, 1));
                        BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(1, 1, 1));

                        vertexAO = CalculateVertexAO(side1.IsSolid(), side2.IsSolid(), corner.IsSolid());
                    }
                    break;
                case 3:
                    if(vertex == 0)
                    {
                        return 0;
                    }
                    else if(vertex == 1)
                    {
                        return 3;
                    }
                    else if(vertex == 2)
                    {
                        return 1;
                    }
                    else if(vertex == 3)
                    {
                        return 3;
                    }

                    //if (vertex == 0)
                    //{
                    //    BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(-1, -1, 0));
                    //    BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(-1, 0, 1));
                    //    BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(-1, -1, 1));

                    //    vertexAO = CalculateVertexAO(side1.IsSolid(), side2.IsSolid(), corner.IsSolid());
                    //}
                    //else if (vertex == 1)
                    //{
                    //    BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(-1, -1, 0));
                    //    BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(-1, 0, -1));
                    //    BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(-1, -1, -1));

                    //    vertexAO = CalculateVertexAO(side1.IsSolid(), side2.IsSolid(), corner.IsSolid());
                    //}
                    //else if (vertex == 2)
                    //{
                    //    BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(-1, 1, 0));
                    //    BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(-1, 0, -1));
                    //    BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(-1, 1, -1));

                    //    vertexAO = CalculateVertexAO(side1.IsSolid(), side2.IsSolid(), corner.IsSolid());
                    //}
                    //else if (vertex == 3)
                    //{
                    //    BlockType side1 = chunk.GetBlock(relativePosition + new Vector3Int(-1, 1, 0));
                    //    BlockType side2 = chunk.GetBlock(relativePosition + new Vector3Int(-1, 0, 1));
                    //    BlockType corner = chunk.GetBlock(relativePosition + new Vector3Int(-1, 1, 1));

                    //    vertexAO = CalculateVertexAO(side1.IsSolid(), side2.IsSolid(), corner.IsSolid());
                    //}
                    break;
                default:
                    break;

            }
                    

            return vertexAO;
        }

    }
}
