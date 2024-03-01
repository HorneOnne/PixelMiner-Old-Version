using UnityEngine;
using PixelMiner.Enums;
namespace PixelMiner
{
    [RequireComponent(typeof(Block))]
    public class BlockVisualize : MonoBehaviour
    {
        private Block _blockItem;
        private ItemData _data;
        private MeshRenderer _renderer;
        private MeshFilter _meshFilter;
        private Vector3[] _blockUVs;
        private Vector3[] _colorUVs;




        private void Start()
        {
            _blockItem = GetComponent<Block>();
            _data = _blockItem.Data;
            _renderer = GetComponent<MeshRenderer>();
            _meshFilter = GetComponent<MeshFilter>();
            _blockUVs = new Vector3[24];
            _colorUVs = new Vector3[24];        // (r,g,b)

            Vector3[] _vertices = new Vector3[]
            {
                new Vector3(0.5f, -0.5f, -0.5f),    // RIGHT
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),

                new Vector3(-0.5f, 0.5f, -0.5f),    // UP
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f),

                new Vector3(0.5f, -0.5f, 0.5f),     // FRONT
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),

                new Vector3(-0.5f, -0.5f, 0.5f),    // LEFT
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f),


                new Vector3(-0.5f, -0.5f, -0.5f),  // DOWN
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),

                new Vector3(-0.5f, -0.5f, -0.5f),   // BACK
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),

            };

            int[] _tris = new int[]
            {
            2, 1, 0,
            3, 2, 0,

            6, 5, 4,
            7, 6, 4,

            10, 9, 8,
            11, 10, 8,

            14, 13, 12,
            15, 14, 12,

            18, 17, 16,
            19, 18, 16,

            22, 21,20,
            23, 22, 20
            };


            Mesh mesh = new Mesh();
            mesh.SetVertices(_vertices);
            mesh.SetTriangles(_tris, 0);

            BlockType blockType = (BlockType)_data.ID;
            GetBlockUvs(blockType, ref _blockUVs, ref _colorUVs);
            mesh.SetUVs(0, _blockUVs);
            mesh.SetUVs(1, _colorUVs);



            _meshFilter.mesh = mesh;

        }


        private void GetBlockUvs(BlockType blockType, ref Vector3[] uvs, ref Vector3[] uv2s)
        {
            for(int face = 0; face < 6; face++)
            {
                uv2s[face * 4] = new Vector3(1, 1, 1);
                uv2s[face * 4 + 1] = new Vector3(1, 1, 1);
                uv2s[face * 4 + 2] = new Vector3(1, 1, 1);
                uv2s[face * 4 + 3] = new Vector3(1, 1, 1);
            }
            
            switch (blockType)
            {
                default:
                    for (int face = 0; face < 6; face++)
                    {
                        GetBlockUVs(blockType, face, ref uvs);
                    }      
                    break;
                case BlockType.DirtGrass:
                    for (int face = 0; face < 6; face++)
                    {
                        GetBlockUVs(blockType, face, ref uvs);
                        if (face == 1)
                        {
                            uv2s[face * 4] = new Vector3(0.2745f, 0.898f, 0.129f);
                            uv2s[face * 4 + 1] = new Vector3(0.2745f, 0.898f, 0.129f);
                            uv2s[face * 4 + 2] = new Vector3(0.2745f, 0.898f, 0.129f);
                            uv2s[face * 4 + 3] = new Vector3(0.2745f, 0.898f, 0.129f);
                        }
                    }
                    break;
            }
        }


        private void GetBlockUVs(BlockType blockType, int face, ref Vector3[] uvs)
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

            uvs[face * 4] = new Vector3(0, 0, blockIndex);
            uvs[face * 4 + 1] = new Vector3(1, 0, blockIndex);
            uvs[face * 4 + 2] = new Vector3(1, 1, blockIndex);
            uvs[face * 4 + 3] = new Vector3(0, 1, blockIndex);
        }
    }
}
