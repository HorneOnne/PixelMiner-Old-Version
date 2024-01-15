using PixelMiner.Lighting;
using System.Collections.Generic;
using UnityEngine;

namespace PixelMiner.WorldBuilding
{
    public class ChunkMeshBuilder
    {
        private List<Vector3> _vertices;
        private List<int> _triangles;
        private List<Vector3> _uvs;
        private List<Vector2> _uv2s;
        private List<Vector4> _uv3s;
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
            _uv3s = new List<Vector4>(100);
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


        public void AddQuadFace(Vector3[] vertices, Vector3[] uvs, Vector2[] uv2s)
        {
            if (vertices.Length != 4)
            {
                throw new System.ArgumentException("A quad requires 4 vertices");
            }

            this._vertices.Add(vertices[0]);
            this._vertices.Add(vertices[1]);
            this._vertices.Add(vertices[2]);
            this._vertices.Add(vertices[3]);


            _triangles.Add(this._vertices.Count - 2);
            _triangles.Add(this._vertices.Count - 3);
            _triangles.Add(this._vertices.Count - 4);

            _triangles.Add(this._vertices.Count - 1);
            _triangles.Add(this._vertices.Count - 2);
            _triangles.Add(this._vertices.Count - 4);


            this._uvs.Add(uvs[0]);
            this._uvs.Add(uvs[1]);
            this._uvs.Add(uvs[2]);
            this._uvs.Add(uvs[3]);

            this._uv2s.Add(uv2s[0]);
            this._uv2s.Add(uv2s[1]);
            this._uv2s.Add(uv2s[2]);
            this._uv2s.Add(uv2s[3]);
        }


        public void AddQuadFace(Vector3[] vertices, Vector3[] uvs, Vector2[] uv2s = null, Vector4[] uv3s = null, Color32[] colors = null, int voxelFace = 0, byte[] vertexAO = null, bool anisotropy = false, byte ambientLight = 0)
        {
            if (vertices.Length != 4)
            {
                throw new System.ArgumentException("A quad requires 4 vertices");
            }

            // Add the 4 vertices, and color for each vertex.
            if (voxelFace == 4)
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
                if (anisotropy)
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

            if (uv2s != null)
            {
                for (int i = 0; i < uv2s.Length; i++)
                {
                    this._uv2s.Add(uv2s[i]);
                }
            }



            // Vertex Light
            if (colors != null)
            {
                for (int i = 0; i < colors.Length; i++)
                {
                    this._colors.Add(colors[i]);
                }
            }

            // Vertex AO
            if (vertexAO != null)
            {
                if (vertexAO.Length != 4)
                {
                    throw new System.ArgumentException("A quad requires 4 vertex color.");
                }

                byte[] indices = new byte[4]; 

                for (int i = 0; i < vertexAO.Length; i++)
                {
                    //this._colors.Add(VertexColorAO(vertexAO[i])); 
                    if (vertexAO[i] == 0)
                    {
                        indices[i] = 208;
                    }
                    else if (vertexAO[i] == 1)
                    {
                        indices[i] = 224;
                    }
                    else if (vertexAO[i] == 2)
                    {
                        indices[i] = 224;
                    }
                    else if (vertexAO[i] == 3)
                    {
                        indices[i] = 240;
                    }
                }

                this._uv3s.Add(new Vector4(indices[0], indices[1], indices[2], indices[3]));
                this._uv3s.Add(new Vector4(indices[0], indices[1], indices[2], indices[3]));
                this._uv3s.Add(new Vector4(indices[0], indices[1], indices[2], indices[3]));
                this._uv3s.Add(new Vector4(indices[0], indices[1], indices[2], indices[3]));
            }

            _triangles.Add(this._vertices.Count - 2);
            _triangles.Add(this._vertices.Count - 3);
            _triangles.Add(this._vertices.Count - 4);

            _triangles.Add(this._vertices.Count - 1);
            _triangles.Add(this._vertices.Count - 2);
            _triangles.Add(this._vertices.Count - 4);


            Color32 VertexColorAO(byte vertexAO)
            {
                Color32 vertexColor;
                switch (vertexAO)
                {
                    case 0:
                        vertexColor = new Color32(151, 156, 157, 255);
                        break;
                    case 1:
                        vertexColor = new Color32(200, 205, 206, 255);
                        break;
                    case 2:
                        vertexColor = new Color32(200, 205, 206, 255);
                        break;
                    default:
                        vertexColor = new Color32(255, 255, 255, 255);
                        break;
                }

                return vertexColor;
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
