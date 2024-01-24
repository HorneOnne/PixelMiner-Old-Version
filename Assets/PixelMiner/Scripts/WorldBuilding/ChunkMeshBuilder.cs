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
        private List<Vector4> _uv4s;
        private List<Color32> _colors;
        private List<byte> _vertexAO;
        //public bool[][][,] Merged;
        public bool[][,] Merged;
        private bool _isInit = false;

 
        public int VerticesCount { get => _vertices.Count; }

        public ChunkMeshBuilder()
        {
            Debug.Log("Create ChunkMeshBuilder");
            _isInit = false;
        }

        ~ChunkMeshBuilder()
        {
        
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
            _uv4s = new List<Vector4>(100);
            _colors = new List<Color32>(100);
            _vertexAO = new List<byte>(10);
          

            Merged = new bool[][,]
            {
                new bool[dimensions[2], dimensions[1]],
                new bool[dimensions[0], dimensions[2]],
                new bool[dimensions[0], dimensions[1]]
            };

            _isInit = true;
        }


        public void AddQuadFace(Vector3[] vertices)
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
        }
        public void AddQuadFace(Vector3[] vertices, Vector3[] uvs)
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
        }



        public void AddQuadFace(Vector3[] vertices, Vector3[] uvs, Vector2[] uv2s, Color32[] colors)
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


            // Vertex Light
            if (colors != null)
            {
                for (int i = 0; i < colors.Length; i++)
                {
                    this._colors.Add(colors[i]);
                }
            }
        }


        public void AddQuadFace(int voxelFace, Vector3[] vertices, Color32[] colors = null, Vector3[] uvs = null, Vector2[] uv2s = null, Vector4[] uv3s = null, Vector4[] uv4s = null)
        {
            // uv: Block texture
            // uv2: Color map
            // uv3: Ambient occlusion
            // uv4: Ambient light
            // Vertex Color: Block light

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

            //this._vertices.Add(vertices[0]);
            //this._vertices.Add(vertices[1]);
            //this._vertices.Add(vertices[2]);
            //this._vertices.Add(vertices[3]);


            if (uvs != null)
            {
                this._uvs.Add(uvs[0]);
                this._uvs.Add(uvs[1]);
                this._uvs.Add(uvs[2]);
                this._uvs.Add(uvs[3]);

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


            this._uv3s.Add(uv3s[0]);
            this._uv3s.Add(uv3s[1]);
            this._uv3s.Add(uv3s[2]);
            this._uv3s.Add(uv3s[3]);


            this._uv4s.Add(uv4s[0]);
            this._uv4s.Add(uv4s[1]);
            this._uv4s.Add(uv4s[2]);
            this._uv4s.Add(uv4s[3]);


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


        public void Add(ChunkMeshBuilder otherBuilder)
        { 
            int currentVertexCount = this.VerticesCount;
            this._vertices.AddRange(otherBuilder._vertices);


            for (int i = 0; i < otherBuilder._triangles.Count; i++)
            {
                _triangles.Add(otherBuilder._triangles[i] + currentVertexCount);
            }

            this._uvs.AddRange(otherBuilder._uvs);
            this._uv2s.AddRange(otherBuilder._uv2s);
            this._uv3s.AddRange(otherBuilder._uv3s);
            this._uv4s.AddRange(otherBuilder._uv4s);
            this._colors.AddRange(otherBuilder._colors);
            this._vertexAO.AddRange(otherBuilder._vertexAO);
        }

        public MeshData ToMeshData()
        {
            MeshData data = MeshDataPool.Get();
            data.Init(_vertices, _triangles, _uvs, _uv2s, _uv3s, _uv4s, _colors);
            return data;
        }

        public void Reset()
        {
            _vertices.Clear();
            _triangles.Clear();
            _uvs.Clear();
            _uv2s.Clear();
            _uv3s.Clear();
            _uv4s.Clear();
            _colors.Clear();
        }
    }
}
