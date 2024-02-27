using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelMiner.Core;

namespace PixelMiner.Tools
{
    public class OBJConverter : MonoBehaviour
    {
        [SerializeField] private Mesh _mesh;
        public MeshFilter meshFilter;

        List<Vector3> vertices = new List<Vector3>();   
        List<int> tris = new List<int>();
        public ModelData ModelData;
        private void Start()
        {
            meshFilter = GetComponent<MeshFilter>();    
            if(_mesh != null)
            {
                foreach(var v in _mesh.vertices)
                {
                    vertices.Add(v);
                }

                foreach (var tri in _mesh.triangles)
                {
                    tris.Add(tri);
                }
            }

            Mesh m = new Mesh();
            m.SetVertices(vertices);
            m.SetTriangles(tris, 0);

            meshFilter.sharedMesh = m;
            ModelData.Vertices = vertices;
            ModelData.Triangles = tris;
        }
    }
}
