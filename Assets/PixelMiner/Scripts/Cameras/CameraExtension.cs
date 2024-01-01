using UnityEngine;
using PixelMiner.World;
using PixelMiner.Core;

namespace PixelMiner.Cam
{
    public class CameraExtension : MonoBehaviour
    {
        private DrawBounds _drawer;
        private Transform _playerTrans;


        private void Start()
        {
            _drawer = GetComponent<DrawBounds>();
            _playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {
            _drawer.Clear();
            DrawChunkBorders();
        }


        private void DrawChunkBorders()
        {

            void AddChunkBounds(Chunk chunk, Color color)
            {
                if (chunk != null)
                {
                    _drawer.AddBounds(chunk.GetBounds(), color);
                }
            }

            if (Main.Instance.TryGetChunk(_playerTrans.position, out Chunk chunk))
            {
                Bounds b = chunk.GetBounds();
                Vector3Int min = new Vector3Int(Mathf.FloorToInt(b.min.x), Mathf.FloorToInt(b.min.y), Mathf.FloorToInt(b.min.z));
                Vector3Int max = new Vector3Int(Mathf.FloorToInt(b.max.x), Mathf.FloorToInt(b.max.y), Mathf.FloorToInt(b.max.z));

                //Debug.Log($"{min}\t{max}");
                _drawer.AddLine(new Vector3(1, 0, 0), new Vector3(1, 32, 0), Color.white);
                for (int x = min.x; x < b.max.x; x++)
                {
                    _drawer.AddLine(new Vector3(x, min.y, min.z), new Vector3(x, max.y, min.z), Color.yellow);
                    _drawer.AddLine(new Vector3(x, min.y, max.z), new Vector3(x, max.y, max.z), Color.yellow);


                    _drawer.AddLine(new Vector3(x, min.y, min.z), new Vector3(x, min.y, max.z), Color.yellow);
                 


                }
                for (int y = min.y; y < b.max.y; y++)
                {
                    _drawer.AddLine(new Vector3(min.x, y, min.z), new Vector3(max.x, y, min.z), Color.white);
                    _drawer.AddLine(new Vector3(min.x, y, max.z), new Vector3(max.x, y, max.z), Color.white);


                    _drawer.AddLine(new Vector3(min.x, y, min.z), new Vector3(min.x, y, max.z), Color.white);
                    _drawer.AddLine(new Vector3(max.x, y, min.z), new Vector3(max.x, y, max.z), Color.white);
                }

                for (int z = min.z; z < b.max.z; z++)
                {
                    _drawer.AddLine(new Vector3(min.x, min.y, z), new Vector3(min.x, max.y, z), Color.yellow);
                    _drawer.AddLine(new Vector3(max.x, min.y, z), new Vector3(max.x, max.y, z), Color.yellow);

                    _drawer.AddLine(new Vector3(min.x, min.y, z), new Vector3(max.x, min.y, z), Color.yellow);
                }





                AddChunkBounds(chunk?.West, Color.cyan);
                AddChunkBounds(chunk?.North, Color.cyan);
                AddChunkBounds(chunk?.East, Color.cyan);
                AddChunkBounds(chunk?.South, Color.cyan);
                AddChunkBounds(chunk?.Northwest, Color.cyan);
                AddChunkBounds(chunk?.Northeast, Color.cyan);
                AddChunkBounds(chunk?.Southwest, Color.cyan);
                AddChunkBounds(chunk?.Southeast, Color.cyan);
                AddChunkBounds(chunk, Color.yellow);
            }
        }
    }
}
