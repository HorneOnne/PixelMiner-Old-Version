using System.Collections.Generic;
using UnityEngine;

namespace PixelMiner.Utilities
{
    public class ChunkMeshBuilder
    {
        private List<Vector3> _vertices;
        private List<int> _triangles;
        private List<Vector3> _uvs;
        private List<Vector2> _uv2s;
        private List<Vector2> _uv3s;
        private List<Color32> _colors;
        private List<byte> _vertexAO;
        public bool[][,] Merged;
        private bool _isInit = false;

        Color32[] _vertexAOColor;

        public ChunkMeshBuilder()
        {
            //Debug.Log("Create ChunkMeshBuilder");
            _isInit = false;
        }

        public void InitOrLoad(Vector3Int dimensions)
        {
            if (_isInit) return;
            //Debug.Log("Init ChunkMeshBuilder");

            _vertices = new List<Vector3>(100);
            _triangles = new List<int>(100);
            _uvs = new List<Vector3>(100);
            _uv2s = new List<Vector2>(100);
            _uv3s = new List<Vector2>(100);
            _colors = new List<Color32>(100);
            _vertexAO = new List<byte>(10);
            _vertexAOColor = new Color32[4];

            Merged = new bool[][,]
            {
                 new bool[dimensions[2], dimensions[1]],
                 new bool[dimensions[0], dimensions[2]],
                 new bool[dimensions[0], dimensions[1]]
            };

            _isInit = true;
        }

        public void AddQuadFace(Vector3[] vertices, Vector3[] uvs, Vector2[] uv2s = null, Vector2[] uv3s = null, Color32[] colors = null, int voxelFace = 0, byte[] vertexAO = null, bool anisotropy = false)
        {
            if (vertices.Length != 4)
            {
                throw new System.ArgumentException("A quad requires 4 vertices");
            }

            // Add the 4 vertices, and color for each vertex.
            if(voxelFace == 4)
            {
                this._vertices.Add(vertices[3]);
                this._vertices.Add(vertices[2]);
                this._vertices.Add(vertices[1]);
                this._vertices.Add(vertices[0]);
            }
            else if (voxelFace == 2 || voxelFace == 3)
            {
                this._vertices.Add(vertices[1]);
                this._vertices.Add(vertices[0]);
                this._vertices.Add(vertices[3]);
                this._vertices.Add(vertices[2]);
            }
            else
            {
                this._vertices.Add(vertices[0]);
                this._vertices.Add(vertices[1]);
                this._vertices.Add(vertices[2]);
                this._vertices.Add(vertices[3]);
            }

           

            if (uvs != null)
            {
                if(anisotropy)
                {
                    this._uvs.Add(uvs[1]);
                    this._uvs.Add(uvs[2]);
                    this._uvs.Add(uvs[3]);
                    this._uvs.Add(uvs[0]);
                }
                else
                {
                    this._uvs.Add(uvs[0]);
                    this._uvs.Add(uvs[1]);
                    this._uvs.Add(uvs[2]);
                    this._uvs.Add(uvs[3]);
                }
               
            }
                           
            if(uv2s != null)
            {
                for (int i = 0; i < uv2s.Length; i++)
                {
                    this._uv2s.Add(uv2s[i]);
                }
            }

            if (uv3s != null)
            {
                for (int i = 0; i < uv3s.Length; i++)
                {
                    this._uv3s.Add(uv3s[i]);
                }
            }

   
           
            // Vertex Light
            if (colors != null)
            {
                for (int i = 0; i < colors.Length; i++)
                {
                    //this._colors.Add(colors[i]);
                }
            }

            // Vertex AO
            if (vertexAO != null)
            {
                if (vertexAO.Length != 4)
                {
                    throw new System.ArgumentException("A quad requires 4 vertex color.");
                }

                for (int i = 0; i < vertexAO.Length; i++)
                {
                    _vertexAOColor[i] = VertexColorAO(vertexAO[i]);
                    this._colors.Add(_vertexAOColor[i]);
                }

                //if (voxelFace == 2 || voxelFace == 3 || voxelFace == 4)
                //{
                //    this._colors.Add(VertexColorAO(vertexAO[1]));
                //    this._colors.Add(VertexColorAO(vertexAO[0]));
                //    this._colors.Add(VertexColorAO(vertexAO[3]));
                //    this._colors.Add(VertexColorAO(vertexAO[2]));
                //}
                //else
                //{               
                //    this._colors.Add(VertexColorAO(vertexAO[0]));
                //    this._colors.Add(VertexColorAO(vertexAO[1]));
                //    this._colors.Add(VertexColorAO(vertexAO[2]));
                //    this._colors.Add(VertexColorAO(vertexAO[3]));
                //}

            }

            _triangles.Add(this._vertices.Count - 2);
            _triangles.Add(this._vertices.Count - 3);
            _triangles.Add(this._vertices.Count - 4);

            _triangles.Add(this._vertices.Count - 1);
            _triangles.Add(this._vertices.Count - 2);
            _triangles.Add(this._vertices.Count - 4);


            //switch (voxelFace)
            //{
            //    case 0:
            //        _triangles.Add(this._vertices.Count - 2);
            //        _triangles.Add(this._vertices.Count - 3);
            //        _triangles.Add(this._vertices.Count - 4);

            //        _triangles.Add(this._vertices.Count - 1);
            //        _triangles.Add(this._vertices.Count - 2);
            //        _triangles.Add(this._vertices.Count - 4);
            //        break;
            //    case 1:
            //        _triangles.Add(this._vertices.Count - 2);
            //        _triangles.Add(this._vertices.Count - 3);
            //        _triangles.Add(this._vertices.Count - 4);

            //        _triangles.Add(this._vertices.Count - 1);
            //        _triangles.Add(this._vertices.Count - 2);
            //        _triangles.Add(this._vertices.Count - 4);
            //        break;
            //    case 2:
            //        _triangles.Add(this._vertices.Count - 4);
            //        _triangles.Add(this._vertices.Count - 3);
            //        _triangles.Add(this._vertices.Count - 2);

            //        _triangles.Add(this._vertices.Count - 2);
            //        _triangles.Add(this._vertices.Count - 1);
            //        _triangles.Add(this._vertices.Count - 4);
            //        break;
            //    case 3:
            //        _triangles.Add(this._vertices.Count - 4);
            //        _triangles.Add(this._vertices.Count - 3);
            //        _triangles.Add(this._vertices.Count - 2);

            //        _triangles.Add(this._vertices.Count - 4);
            //        _triangles.Add(this._vertices.Count - 2);
            //        _triangles.Add(this._vertices.Count - 1);
            //        break;
            //    case 4:
            //        _triangles.Add(this._vertices.Count - 4);
            //        _triangles.Add(this._vertices.Count - 3);
            //        _triangles.Add(this._vertices.Count - 2);


            //        _triangles.Add(this._vertices.Count - 2);
            //        _triangles.Add(this._vertices.Count - 1);
            //        _triangles.Add(this._vertices.Count - 4);
            //        break;
            //    default:
            //        _triangles.Add(this._vertices.Count - 2);
            //        _triangles.Add(this._vertices.Count - 3);
            //        _triangles.Add(this._vertices.Count - 4);

            //        _triangles.Add(this._vertices.Count - 1);
            //        _triangles.Add(this._vertices.Count - 2);
            //        _triangles.Add(this._vertices.Count - 4);
            //        break;
            //}


            Color32 VertexColorAO(byte vertexAO)
            {
                Color32 vertexColor;
                switch(vertexAO)
                {
                    case 0:
                        vertexColor = new Color32(255, 0, 0, 255);
                        break;
                    case 1:
                        vertexColor = new Color32(0, 255, 0, 255);
                        break;
                    case 2:
                        vertexColor = new Color32(0, 0, 255, 255);
                        break;
                    default:
                        vertexColor = new Color32(255,255,255,255);
                        break;
                }

                return vertexColor;
            }

  
            // Function to blend block light and ambient occlusion colors
            Color32 BlendColors(Color32 blockLightColor, Color32 ambientOcclusionColor, float blendingFactor)
            {
                // Normalize blending factor to the range [0, 1]
                blendingFactor = Mathf.Clamp01(blendingFactor);

                if (blockLightColor.r == 0 && blockLightColor.g == 0 && blockLightColor.b == 0)
                {
                    // If block light color is zero, use ambient occlusion color directly
                    return ambientOcclusionColor;
                }

                byte resultR = (byte)Mathf.RoundToInt(blockLightColor.r * (1 - blendingFactor) + ambientOcclusionColor.r * blendingFactor);
                byte resultG = (byte)Mathf.RoundToInt(blockLightColor.g * (1 - blendingFactor) + ambientOcclusionColor.g * blendingFactor);
                byte resultB = (byte)Mathf.RoundToInt(blockLightColor.b * (1 - blendingFactor) + ambientOcclusionColor.b * blendingFactor);
                byte resultA = (byte)Mathf.RoundToInt(blockLightColor.a * (1 - blendingFactor) + ambientOcclusionColor.a * blendingFactor);

                return new Color32(resultR, resultG, resultB, resultA);
            }

        }


        public MeshData ToMeshData()
        {
            MeshData data = MeshDataPool.Get();
            data.Init(_vertices, _triangles, _uvs, _uv2s, _uv3s, _colors);
            return data;
        }

        public void Reset()
        {
            _vertices.Clear();
            _triangles.Clear();
            _uvs.Clear();
            _uv2s.Clear();
            _uv3s.Clear();
            _colors.Clear();
        }
    }
}
