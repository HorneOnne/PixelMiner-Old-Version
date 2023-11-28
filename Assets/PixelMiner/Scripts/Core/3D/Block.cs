using PixelMiner.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace PixelMiner.Core
{
    [System.Serializable]
    public class Block
    {
        public MeshData[] MesheDataArray { get; private set; } = new MeshData[6];

        public Block(Vector3 offset)
        {
            Quad[] q = new Quad[6];
            q[0] = new Quad(BlockSide.Top, BlockType.GrassTop, ColorMapType.Plains, offset);
            q[1] = new Quad(BlockSide.Bottom, BlockType.Dirt, ColorMapType.None, offset);
            q[2] = new Quad(BlockSide.Front, BlockType.GrassSide, ColorMapType.None, offset);
            q[3] = new Quad(BlockSide.Back, BlockType.GrassSide, ColorMapType.None, offset);
            q[4] = new Quad(BlockSide.Left, BlockType.GrassSide, ColorMapType.None, offset);
            q[5] = new Quad(BlockSide.Right, BlockType.GrassSide, ColorMapType.None, offset);

            for (int i = 0; i < MesheDataArray.Length; i++)
            {
                MesheDataArray[i] = new MeshData()
                {
                    Vertices = q[i].MeshData.Vertices,
                    Normals = q[i].MeshData.Normals,
                    Triangles = q[i].MeshData.Triangles,
                    UVs = q[i].MeshData.UVs,
                    UV2s = q[i].MeshData.UV2s,
                };
            }
        }


        public async void InitAsync(Vector3 offset)
        {
           
        }
    }
}
