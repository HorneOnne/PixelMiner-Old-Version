using System.Collections.Generic;
using UnityEngine;

namespace PixelMiner.WorldBuilding
{
    public class ChunkMeshBuilder
    {
        private readonly List<Vector3> _vertices;
        private readonly List<int> _triangles;

        public ChunkMeshBuilder()
        {
            _vertices = new List<Vector3>();
            _triangles = new List<int>();
        }

        public void AddQuadFace(Vector3[] vertices, bool isBackFace)
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

            if (!isBackFace)
            {
                _triangles.Add(this._vertices.Count - 4);
                _triangles.Add(this._vertices.Count - 3);
                _triangles.Add(this._vertices.Count - 2);

                _triangles.Add(this._vertices.Count - 4);
                _triangles.Add(this._vertices.Count - 2);
                _triangles.Add(this._vertices.Count - 1);
            }
            else
            {
                _triangles.Add(this._vertices.Count - 2);
                _triangles.Add(this._vertices.Count - 3);
                _triangles.Add(this._vertices.Count - 4);

                _triangles.Add(this._vertices.Count - 1);
                _triangles.Add(this._vertices.Count - 2);
                _triangles.Add(this._vertices.Count - 4);
            }
        }

        public MeshData ToMeshData()
        {
            MeshData data = new MeshData(_vertices.ToArray(), _triangles.ToArray());
            _vertices.Clear();
            _triangles.Clear();

            return data;
        }
    }
}
