using UnityEngine;

namespace PixelMiner.Core
{
    public class ChunkTesting : MonoBehaviour
    {
        public int ChunkWidth;
        public int ChunkHeight;
        public Block BlockPrefab;
    


        private void Start()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            for (int x = 0; x < ChunkWidth; x++)
            {
                for (int y = 0; y < ChunkHeight; y++)
                {
                    for (int z = 0; z < ChunkWidth; z++)
                    {
                        //Instantiate(BlockPrefab, new Vector3Int(x, y, z), Quaternion.identity);
                    }
                }
            }
            sw.Stop();
            Debug.Log($"{sw.ElapsedMilliseconds / 1000f}");
        }
    }
}
