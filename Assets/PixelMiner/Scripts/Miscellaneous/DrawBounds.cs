using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using PixelMiner.DataStructure;
using System;

namespace PixelMiner.Miscellaneous
{
    public class DrawBounds : MonoBehaviour
    {
        public static DrawBounds Instance { get; private set; }

        private CommandBuffer _commandBuffer;
        public Material LineMat;

        private List<Bounds> _bounds = new List<Bounds>();
        private List<Color> _colors = new List<Color>();

        private List<Vector3> _lines = new List<Vector3>();
        private List<Color> _lineColors = new List<Color>();

        private Matrix4x4 _matrix;
        private Vector3[] _v = new Vector3[8];



        private void Awake()
        {
            Instance = this;
            _matrix = Matrix4x4.identity;
        }




        //private void OnEnable()
        //{
        //    RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
        //}

    

        //private void OnDisable()
        //{
        //    RenderPipelineManager.endCameraRendering -= RenderPipelineManager_endCameraRendering;
        //}

        //private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext context, Camera camera)
        //{

        //}

 

        private void Update()
        {
            Clear();
        }

        private void OnRenderObject()
        {
            for (int bc = 0; bc < _bounds.Count; ++bc)
            {
                Bounds b = _bounds[bc];
                Color col = _colors[bc];

                Vector3 c = b.center;
                Vector3 e = b.extents;
                float d = 0.00f;

                _v[0] = new Vector3(c.x - e.x - d, c.y - e.y - d, c.z - e.z - d);
                _v[1] = new Vector3(c.x + e.x + d, c.y - e.y - d, c.z - e.z - d);
                _v[2] = new Vector3(c.x - e.x - d, c.y + e.y + d, c.z - e.z - d);
                _v[3] = new Vector3(c.x + e.x + d, c.y + e.y + d, c.z - e.z - d);
                _v[4] = new Vector3(c.x - e.x - d, c.y - e.y - d, c.z + e.z + d);
                _v[5] = new Vector3(c.x + e.x + d, c.y - e.y - d, c.z + e.z + d);
                _v[6] = new Vector3(c.x - e.x - d, c.y + e.y + d, c.z + e.z + d);
                _v[7] = new Vector3(c.x + e.x + d, c.y + e.y + d, c.z + e.z + d);


                LineMat.SetPass(0);
                GL.PushMatrix();
                GL.MultMatrix(_matrix);

                GL.Begin(GL.LINES);
                GL.Color(col);

                for (int i = 0; i < 4; ++i)
                {
                    // forward lines
                    GL.Vertex(_v[i]);
                    GL.Vertex(_v[i + 4]);

                    // right lines
                    GL.Vertex(_v[i * 2]);
                    GL.Vertex(_v[i * 2 + 1]);

                    // up lines
                    int u = i < 2 ? 0 : 2;
                    GL.Vertex(_v[i + u]);
                    GL.Vertex(_v[i + u + 2]);
                }

                GL.End();
                GL.PopMatrix();
            }

            // Draw all lines
            GL.PushMatrix();
            GL.MultMatrix(_matrix);

            LineMat.SetPass(0);

            GL.Begin(GL.LINES);

            for (int l = 0; l < _lines.Count / 2; l++)
            {
                GL.Color(_lineColors[l]);
                GL.Vertex(_lines[l * 2]);
                GL.Vertex(_lines[l * 2 + 1]);
            }

            GL.End();
            GL.PopMatrix();
        }



        public void AddBounds(Bounds b, Color c)
        {
            _bounds.Add(b);
            _colors.Add(c);
        }

        public void AddPhysicBounds(AABB b, Color c)
        {
            AddLine(new Vector3(b.x, b.y, b.z), new Vector3(b.x + b.w, b.y, b.z), c);
            AddLine(new Vector3(b.x + b.w, b.y, b.z), new Vector3(b.x + b.w, b.y, b.z + b.d), c);
            AddLine(new Vector3(b.x + b.w, b.y, b.z + b.d), new Vector3(b.x, b.y, b.z + b.d), c);
            AddLine(new Vector3(b.x, b.y, b.z + b.d), new Vector3(b.x, b.y, b.z), c);

            // Draw the top face
            AddLine(new Vector3(b.x, b.y + b.h, b.z), new Vector3(b.x + b.w, b.y + b.h, b.z),c);
            AddLine(new Vector3(b.x + b.w, b.y + b.h, b.z), new Vector3(b.x + b.w, b.y + b.h, b.z + b.d),c);
            AddLine(new Vector3(b.x + b.w, b.y + b.h, b.z + b.d), new Vector3(b.x, b.y + b.h, b.z + b.d),c);
            AddLine(new Vector3(b.x, b.y + b.h, b.z + b.d), new Vector3(b.x, b.y + b.h, b.z),c);

            // Connect the corresponding points between the top and bottom faces
            AddLine(new Vector3(b.x, b.y, b.z), new Vector3(b.x, b.y + b.h, b.z),c);
            AddLine(new Vector3(b.x + b.w, b.y, b.z), new Vector3(b.x + b.w, b.y + b.h, b.z),c);
            AddLine(new Vector3(b.x + b.w, b.y, b.z + b.d), new Vector3(b.x + b.w, b.y + b.h, b.z + b.d),c);
            AddLine(new Vector3(b.x, b.y, b.z + b.d), new Vector3(b.x, b.y + b.h, b.z + b.d),c);
        }


        public void AddLine(Vector3 p1, Vector3 p2, Color c)
        {
            _lines.Add(p1);
            _lines.Add(p2);
            _lineColors.Add(c);
        }

        public void AddRay(Vector3 origin, Vector3 dir, Color c, float maxLength = 10)
        {
            Vector3 endpoint = origin + dir.normalized * maxLength;
            AddLine(origin, endpoint, c);
        }

        public void Clear()
        {
            _bounds.Clear();
            _colors.Clear();
            _lines.Clear();
            _lineColors.Clear();
        }


  

        private void OnApplicationQuit()
        {
#if UNITY_EDITOR
            Clear();
#endif
        }
    }

}
