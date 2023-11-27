using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using QFSW.QC;
using PixelMiner.Enums;

//using VertexData = System.Tuple<UnityEngine.Vector3, UnityEngine.Vector3, UnityEngine.Vector2>;
using VertexData = System.Tuple<UnityEngine.Vector3, UnityEngine.Vector3, UnityEngine.Vector2, UnityEngine.Vector2>;


namespace PixelMiner.Core
{
    public static class MeshUtils
    {
        public static Vector2[,] BlockUVs =
        {
            /*
             * Order: BOTTOM_LEFT -> BOTTOM_RIGHT -> TOP_LEFT -> TOP_RIGHT.
             */

            /*GRASSTOP*/  
            { new Vector2(0.5f, 0.8125f), new Vector2(0.5625f, 0.8125f),
              new Vector2(0.5f, 0.875f),new Vector2(0.5625f, 0.875f)},

            /*GRASSSIDE*/  
            { new Vector2(0.1875f, 0.9375f), new Vector2(0.25f, 0.9375f),
              new Vector2(0.1875f, 1.0f),new Vector2(0.25f, 1.0f)},

            /*DIRT*/ 
            {new Vector2(0.125f, 0.9375f), new Vector2(0.1875f, 0.9375f),
             new Vector2(0.125f, 1f), new Vector2(0.1875f, 1f)},
        };

        public static Vector2[,] BlockUV2s =
        {
            /*
             * Order: BOTTOM_LEFT -> BOTTOM_RIGHT -> TOP_LEFT -> TOP_RIGHT.
             */            
            /*PLAINS*/
            {new Vector2(0.234375f, 0.640625f), new Vector2(0.25f, 0.640625f),
            new Vector2(0.234375f, 0.65625f), new Vector2(0.25f, 0.65625f)},


            /*FOREST*/
            {new Vector2(0.15625f, 0.796875f), new Vector2(0.171875f, 0.796875f),
            new Vector2(0.15625f, 0.8125f), new Vector2(0.171875f, 0.8125f)}, 

            /*JUNGLE*/
            {new Vector2(0.15625f, 0.796875f), new Vector2(0.171875f, 0.796875f),
            new Vector2(0.15625f, 0.8125f), new Vector2(0.171875f, 0.8125f)}, 

            /*DESERT*/
            {new Vector2(0f, 0f), new Vector2(0.015625f, 0f),
            new Vector2(0f, 0.015625f), new Vector2(0.015625f, 0.015625f)}, 

            /*NONE*/
            {new Vector2(0.8125f, 0.8125f), new Vector2(0.828125f, 0.8125f),
            new Vector2(0.8125f, 0.828125f), new Vector2(0.828125f, 0.828125f)},
        };


        [Command("/getUV")]
        private static void GetUV(int u, int v, ushort blockType = 0)
        {
            float tileSize = 1 / 16f;
            Debug.Log($"\n\n/*{((BlockType)blockType).ToString().ToUpper()}*/" +
               $"\n{{new Vector2({tileSize * u}f, {tileSize * v}f), " +
               $"new Vector2({tileSize * u + tileSize}f, {tileSize * v}f)," +
               $"\nnew Vector2({tileSize * u}f, {tileSize * v + tileSize}f), " +
               $"new Vector2({tileSize * u + tileSize}f, {tileSize * v + tileSize}f)}}, ");
        }

        [Command("/getUV2")]
        private static void GetUV2(int u, int v, ushort blockType = 0)
        {
            float tileSize = 1 / 64f;
            Debug.Log($"\n\n/*{((ColorMapType)blockType).ToString().ToUpper()}*/" +
               $"\n{{new Vector2({tileSize * u}f, {tileSize * v}f), " +
               $"new Vector2({tileSize * u + tileSize}f, {tileSize * v}f)," +
               $"\nnew Vector2({tileSize * u}f, {tileSize * v + tileSize}f), " +
               $"new Vector2({tileSize * u + tileSize}f, {tileSize * v + tileSize}f)}}, ");
        }

        public static Mesh MergeMesh(Mesh[] meshes)
        {
            Mesh mesh = new Mesh();
            Dictionary<VertexData, int> pointsOrder = new Dictionary<VertexData, int>();
            List<int> tris = new List<int>();

            int pIndex = 0;
            for (int i = 0; i < meshes.Length; i++)  // Loop through each mesh
            {
                if (meshes[i] == null) continue;

                for (int j = 0; j < meshes[i].vertices.Length; j++)  // Loop through each vertex of the current mesh.
                {
                    Vector3 v = meshes[i].vertices[j];
                    Vector3 n = meshes[i].normals[j];
                    Vector2 uv = meshes[i].uv[j];
                    Vector2 uv2 = meshes[i].uv2[j];
                    VertexData p = new VertexData(v, n, uv, uv2);

                    if (!pointsOrder.ContainsKey(p))
                    {
                        pointsOrder.Add(p, pIndex);
                        pIndex++;
                    }
                }

                for (int t = 0; t < meshes[i].triangles.Length; t++)    // Loop through each trig of the current mesh.
                {
                    int triPoint = meshes[i].triangles[t];
                    Vector3 v = meshes[i].vertices[triPoint];
                    Vector3 n = meshes[i].normals[triPoint];
                    Vector2 uv = meshes[i].uv[triPoint];
                    Vector2 uv2 = meshes[i].uv2[triPoint];
                    VertexData p = new VertexData(v, n, uv, uv2);

                    int index;
                    pointsOrder.TryGetValue(p, out index);
                    tris.Add(index);
                }
                meshes[i] = null;
            }

            ExtractArrays(pointsOrder, mesh);
            mesh.triangles = tris.ToArray();
            mesh.RecalculateNormals();
            return mesh;
        }

        public static void ExtractArrays(Dictionary<VertexData, int> list, Mesh mesh)
        {
            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Vector2> uv2s = new List<Vector2>();

            foreach (VertexData v in list.Keys)
            {
                verts.Add(v.Item1);
                norms.Add(v.Item2);
                uvs.Add(v.Item3);
                uv2s.Add(v.Item4);
            }
            mesh.vertices = verts.ToArray();
            mesh.normals = norms.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.uv2 = uv2s.ToArray();
        }



    }
}
