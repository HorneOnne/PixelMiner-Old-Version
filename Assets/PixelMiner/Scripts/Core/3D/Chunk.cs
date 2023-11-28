using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Unity.Burst;
using System.Threading.Tasks;

namespace PixelMiner.Core
{
    public class Chunk : MonoBehaviour
    {
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;

        public int Width;
        public int Height;
        public int Depth;
        public Block[,,] Blocks;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshFilter = GetComponent<MeshFilter>();
        }

        private void Start()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            Blocks = new Block[Width, Height, Depth];

            List<MeshData> meshDataList = new List<MeshData>();
            for (int x = 0; x < Width; x++)
            {
                for (int z = 0; z < Depth; z++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        Blocks[x, y, z] = new Block(new Vector3(x, y, z));
                        meshDataList.AddRange(Blocks[x, y, z].MesheDataArray);
                    }
                }
            }
            sw.Stop();
            Debug.Log($"{sw.ElapsedMilliseconds / 1000f} s");
            sw.Restart();

          
            _meshFilter.mesh = MeshUtils.MergeMesh(meshDataList.ToArray());
            sw.Stop();
            Debug.Log($"{sw.ElapsedMilliseconds / 1000f} s");
        }
    }
}
