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

                new Vector3(-0.5f, -0.5f, -0.5f),   // DOWN
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),

                 new Vector3(-0.5f, -0.5f, -0.5f),  // BACK
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
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


            GetBlockUvs(ref _blockUVs, ref _colorUVs);
            mesh.SetUVs(0, _blockUVs);
            mesh.SetUVs(1, _colorUVs);



            _meshFilter.mesh = mesh;

        }


        private void GetBlockUvs(ref Vector3[] uvs, ref Vector3[] uv2s)
        {
            switch (_data.ID)
            {
                default: break;
                case ItemID.DirtGrass:
                    for (int i = 0; i < 24; i++)
                    {
                        int textureIndex = -1;
                        if (i % 4 == 0)
                        {
                            // Default
                            uv2s[i] = new Vector3(1, 1, 1);
                            uv2s[i + 1] = new Vector3(1, 1, 1);
                            uv2s[i + 2] = new Vector3(1, 1, 1);
                            uv2s[i + 3] = new Vector3(1, 1, 1);

                            int face = i / 4;
                            if (face == 1)
                            {
                                textureIndex = (ushort)Enums.TextureType.GrassTop;
                                uv2s[i] = new Vector3(0.2745f, 0.898f, 0.129f);
                                uv2s[i + 1] = new Vector3(0.2745f, 0.898f, 0.129f);
                                uv2s[i + 2] = new Vector3(0.2745f, 0.898f, 0.129f);
                                uv2s[i + 3] = new Vector3(0.2745f, 0.898f, 0.129f);
                            }
                            else if (face == 5)
                            {
                                textureIndex = (ushort)Enums.TextureType.Dirt;
                            }
                            else
                            {
                                textureIndex = (ushort)Enums.TextureType.GrassSide;
                            }


                            uvs[i] = new Vector3(0, 0, textureIndex);
                            uvs[i + 1] = new Vector3(1, 0, textureIndex);
                            uvs[i + 2] = new Vector3(1, 1, textureIndex);
                            uvs[i + 3] = new Vector3(0, 1, textureIndex);
                        }
                    }
                    break;
            }
        }

    }
}
