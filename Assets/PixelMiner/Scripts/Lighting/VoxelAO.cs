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

        public static int CalculateVertexAO(bool side1, bool side2, bool corner)
        {
            if (side1 && side2)
                return 0;

            return 3 - (System.Convert.ToInt32(side1) + System.Convert.ToInt32(side2) + System.Convert.ToInt32(corner));
        }

        public static int[] ProcessAO(Vector3Int globalPosition, int vertex, int voxelFace)
        {
            int[] vertexAO = new int[4];
            switch(voxelFace)
            {
                case 5:
                    if(vertex == 0)
                    {
                        BlockType side1 = Main.Instance.GetBlock(globalPosition + new Vector3Int(0, -1, -1));
                        BlockType side2 = Main.Instance.GetBlock(globalPosition + new Vector3Int(-1, 0, -1));
                        BlockType corner = Main.Instance.GetBlock(globalPosition + new Vector3Int(-1, -1, -1));

                        vertexAO[0] = CalculateVertexAO(side1 != BlockType.Air, side2 != BlockType.Air, corner != BlockType.Air);
                    }
                    else if(vertex == 1)
                    {
                        BlockType side1 = Main.Instance.GetBlock(globalPosition + new Vector3Int(0, -1, -1));
                        BlockType side2 = Main.Instance.GetBlock(globalPosition + new Vector3Int(1, 0, -1));
                        BlockType corner = Main.Instance.GetBlock(globalPosition + new Vector3Int(1, -1, -1));

                        vertexAO[1] = CalculateVertexAO(side1 != BlockType.Air, side2 != BlockType.Air, corner != BlockType.Air);
                    }
                    else if(vertex == 2)
                    {
                        BlockType side1 = Main.Instance.GetBlock(globalPosition + new Vector3Int(0, 1, -1));
                        BlockType side2 = Main.Instance.GetBlock(globalPosition + new Vector3Int(1, 0, -1));
                        BlockType corner = Main.Instance.GetBlock(globalPosition + new Vector3Int(1, 1, -1));

                        vertexAO[0] = CalculateVertexAO(side1 != BlockType.Air, side2 != BlockType.Air, corner != BlockType.Air);
                    }
                    else if(vertex == 3)
                    {
                        BlockType side1 = Main.Instance.GetBlock(globalPosition + new Vector3Int(0, 1, -1));
                        BlockType side2 = Main.Instance.GetBlock(globalPosition + new Vector3Int(-1, 0, -1));
                        BlockType corner = Main.Instance.GetBlock(globalPosition + new Vector3Int(-1, 1, 0));

                        vertexAO[0] = CalculateVertexAO(side1 != BlockType.Air, side2 != BlockType.Air, corner != BlockType.Air);
                    }
                    break;
                default:
                    break;



            }

            return vertexAO;
        }
    }
}
