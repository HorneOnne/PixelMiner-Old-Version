using System.Collections.Generic;
using log4net.Appender;
using UnityEngine;

namespace PixelMiner.WorldBuilding
{
    public class ChunkMeshBuilder
    {
        private List<Vector3> _vertices;
        private List<int> _triangles;
        private List<Vector3> _uvs;
        private List<Vector2> _uv2s;
        private List<Color32> _colors;
        public bool[][,] Merged;
        private bool _isInit = false;

        public ChunkMeshBuilder()
        {
            Debug.Log("Create ChunkMeshBuilder");
            _isInit = false;
        }

        public void InitOrLoad(Vector3Int dimensions)
        {
            if (_isInit) return;
            Debug.Log("Init ChunkMeshBuilder");

            _vertices = new List<Vector3>(100);
            _triangles = new List<int>(100);
            _uvs = new List<Vector3>(100);
            _uv2s = new List<Vector2>(100);
            _colors = new List<Color32>(100);
            Merged = new bool[][,]
            {
                 new bool[dimensions[2], dimensions[1]],
                 new bool[dimensions[0], dimensions[2]],
                 new bool[dimensions[0], dimensions[1]]
            };

            _isInit = true;
        }

        public void AddQuadFace(Vector3[] vertices, Vector3[] uvs, Vector2[] uv2s, Color32[] colors, int voxelFace = 0)
        {
            if (vertices.Length != 4)
            {
                throw new System.ArgumentException("A quad requires 4 vertices");
            }

            // Add the 4 vertices, and color for each vertex.
            for (int i = 0; i < vertices.Length; i++)
            {
                this._vertices.Add(vertices[i]);
            }

            if(uvs != null)
            {
                for (int i = 0; i < uvs.Length; i++)
                {
                    this._uvs.Add(uvs[i]);
                }
            }
           
            
            
            if(uv2s != null)
            {
                for (int i = 0; i < uv2s.Length; i++)
                {
                    this._uv2s.Add(uv2s[i]);
                }
            }

            
            if(colors != null)
            {
                for (int i = 0; i < colors.Length; i++)
                {
                    _colors.Add(colors[i]);
                }
            }


            switch (voxelFace)
            {
                case 1:
                    _triangles.Add(this._vertices.Count - 2);
                    _triangles.Add(this._vertices.Count - 3);
                    _triangles.Add(this._vertices.Count - 4);

                    _triangles.Add(this._vertices.Count - 1);
                    _triangles.Add(this._vertices.Count - 2);
                    _triangles.Add(this._vertices.Count - 4);
                    break;
                case 4:
                    _triangles.Add(this._vertices.Count - 4);
                    _triangles.Add(this._vertices.Count - 3);
                    _triangles.Add(this._vertices.Count - 2);


                    _triangles.Add(this._vertices.Count - 2);
                    _triangles.Add(this._vertices.Count - 1);
                    _triangles.Add(this._vertices.Count - 4);
                    break;
                case 3:
                    _triangles.Add(this._vertices.Count - 4);
                    _triangles.Add(this._vertices.Count - 3);
                    _triangles.Add(this._vertices.Count - 2);

                    _triangles.Add(this._vertices.Count - 4);
                    _triangles.Add(this._vertices.Count - 2);
                    _triangles.Add(this._vertices.Count - 1);
                    break;
                case 0:
                    _triangles.Add(this._vertices.Count - 2);
                    _triangles.Add(this._vertices.Count - 3);
                    _triangles.Add(this._vertices.Count - 4);

                    _triangles.Add(this._vertices.Count - 1);
                    _triangles.Add(this._vertices.Count - 2);
                    _triangles.Add(this._vertices.Count - 4);
                    break;
                case 2:
                    _triangles.Add(this._vertices.Count - 4);
                    _triangles.Add(this._vertices.Count - 3);
                    _triangles.Add(this._vertices.Count - 2);
                    
                    _triangles.Add(this._vertices.Count - 2);
                    _triangles.Add(this._vertices.Count - 1);
                    _triangles.Add(this._vertices.Count - 4);
                    break;
                default:
                    _triangles.Add(this._vertices.Count - 2);
                    _triangles.Add(this._vertices.Count - 3);
                    _triangles.Add(this._vertices.Count - 4);

                    _triangles.Add(this._vertices.Count - 1);
                    _triangles.Add(this._vertices.Count - 2);
                    _triangles.Add(this._vertices.Count - 4);
                    break;
            }

        }


        public MeshData ToMeshData()
        {
            MeshData data = MeshDataPool.Get();
            data.Init(_vertices, _triangles, _uvs, _uv2s, _colors);
            return data;
        }

        public void Reset()
        {
            _vertices.Clear();
            _triangles.Clear();
            _uvs.Clear();
            _uv2s.Clear();
            _colors.Clear();
        }
    }
}
