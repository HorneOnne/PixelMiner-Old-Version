using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace PixelMiner.Cam
{
    public class DrawChunkBounds : MonoBehaviour
    {
        public Material BorderMat;

        private UnityEngine.Camera _mainCam;
        private List<Bounds> _bounds = new List<Bounds>();
        private List<Color> _colors = new List<Color>();

        private List<Vector3> _lines = new List<Vector3>();
        private List<Color> _lineColors = new List<Color>();

        private Matrix4x4 _matrix;
        private Vector3[] _v = new Vector3[8];


        private void Awake()
        {
            _mainCam = Camera.main;
            _matrix = Matrix4x4.identity;
        }


        private void OnEnable()
        {
            RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
        }

  
        private void OnDisable()
        {
            RenderPipelineManager.endCameraRendering -= RenderPipelineManager_endCameraRendering;
        }

        private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            OnPostRender();
        }


        private void OnPostRender()
        {
            //Debug.Log("OnPostRender");

            for(int i = 0; i < _bounds.Count; i++)
            {
                Bounds bound = _bounds[i];
                Color color = _colors[i];
                Vector3 c = bound.center;
                Vector3 e = bound.extents;

                _v[0] = new Vector3(c.x - e.x, c.y - e.y, c.z - e.z);
                _v[1] = new Vector3(c.x + e.x, c.y - e.y, c.z - e.z);
                _v[2] = new Vector3(c.x - e.x, c.y + e.y, c.z - e.z);
                _v[3] = new Vector3(c.x - e.x, c.y - e.y, c.z + e.z);
                _v[4] = new Vector3(c.x + e.x, c.y + e.y, c.z - e.z);
                _v[5] = new Vector3(c.x + e.x, c.y - e.y, c.z + e.z);
                _v[6] = new Vector3(c.x - e.x, c.y + e.y, c.z + e.z);
                _v[7] = new Vector3(c.x + e.x, c.y + e.y, c.z + e.z);

                GL.PushMatrix();
                GL.MultMatrix(_matrix);

                BorderMat.SetPass(0);


            }

          
        }

        public void AddBounds(Bounds b, Color c)
        {
            _bounds.Add(b);
            _colors.Add(c);
        }


        public void AddLine(Vector3 p1, Vector3 p2, Color c)
        {
            _lines.Add(p1);
            _lines.Add(p2);
            _lineColors.Add(c);
        }

        public void Clear()
        {
            _bounds.Clear();
            _colors.Clear();
            _lines.Clear();
        }

    }
}
