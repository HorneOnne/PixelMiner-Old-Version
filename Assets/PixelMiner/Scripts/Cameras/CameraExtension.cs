using UnityEngine;
using PixelMiner.World;
using PixelMiner.Core;
using PixelMiner.Miscellaneous;

namespace PixelMiner.Cam
{
    public class CameraExtension : MonoBehaviour
    {
        public static CameraExtension Instance { get; private set; }

        private DrawBounds _drawer;
        private Transform _playerTrans;
        private const float _ythreshold = 0.02f;
        private Vector3 _threshold = new Vector3(0.02f, -0.02f, 0.02f);
        private Vector3 _blockOffsetOrigin = new Vector3(0.5f, 0.5f, 0.5f);

        private Vector3 _lastDir;
        private Camera _mainCam;
        private Ray _ray;
        public RaycastVoxelHit VoxelHit { get; private set; }
        private RayCasting _rayCasting;

        private void Awake()
        {
            Instance = this;    
        }

        private void Start()
        {
            _mainCam = Camera.main;
            _drawer = GetComponent<DrawBounds>();
            _playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
            _rayCasting = GameObject.FindAnyObjectByType<RayCasting>();
        }


        private void Update()
        {
            _drawer.Clear();

            _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayDirection = _ray.direction;
            if (_rayCasting.DDAVoxelRayCast(_mainCam.transform.position, rayDirection, out RaycastVoxelHit hitVoxel, out RaycastVoxelHit preHitVoxel))
            {
                VoxelHit = hitVoxel;
                Vector3Int hitGlobalPosition = new Vector3Int(Mathf.FloorToInt(hitVoxel.point.x + 0.001f),
                                                                  Mathf.FloorToInt(hitVoxel.point.y + 0.001f),
                                                                  Mathf.FloorToInt(hitVoxel.point.z + 0.001f));
                Vector3 hitCenter = hitGlobalPosition + _blockOffsetOrigin;

                _drawer.AddBounds(new Bounds(hitCenter, new Vector3(1.01f, 1.01f, 1.01f)), Color.white);
            }
            else
            {
                VoxelHit = default;
            }

            //if (VoxelHit.point == default(Vector3Int))
            //{
            //    Debug.Log("not hit");
            //}
            //else
            //{
            //    Debug.Log("hit");
            //}

        }
        private void LateUpdate()
        {
            //_lastDir = Camera.main.ScreenPointToRay(Input.mousePosition).direction;       
            //DrawChunkBorders();
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

                for (int x = min.x + 1; x < b.max.x; x++)
                {
                    _drawer.AddLine(new Vector3(x, min.y, min.z) + _threshold, new Vector3(x, max.y, min.z) + _threshold, Color.yellow);
                    _drawer.AddLine(new Vector3(x, min.y, max.z) + _threshold, new Vector3(x, max.y, max.z) + _threshold, Color.yellow);
                    _drawer.AddLine(new Vector3(x, min.y, min.z) + _threshold, new Vector3(x, min.y, max.z) + _threshold, Color.yellow);
                }
                for (int y = min.y + 1; y < b.max.y; y++)
                {
                    _drawer.AddLine(new Vector3(min.x, y, min.z) + _threshold, new Vector3(max.x, y , min.z) + _threshold, Color.yellow);
                    _drawer.AddLine(new Vector3(min.x, y, max.z) + _threshold, new Vector3(max.x, y, max.z) + _threshold, Color.yellow);

                    _drawer.AddLine(new Vector3(min.x, y, min.z) + _threshold, new Vector3(min.x, y, max.z) + _threshold, Color.yellow);
                    _drawer.AddLine(new Vector3(max.x, y, min.z) + _threshold, new Vector3(max.x, y, max.z) + _threshold, Color.yellow);
                }

                for (int z = min.z + 1; z < b.max.z; z++)
                {
                    _drawer.AddLine(new Vector3(min.x, min.y, z) + _threshold, new Vector3(min.x, max.y, z) + _threshold, Color.yellow);
                    _drawer.AddLine(new Vector3(max.x, min.y, z) + _threshold, new Vector3(max.x, max.y, z) + _threshold, Color.yellow);

                    _drawer.AddLine(new Vector3(min.x, min.y, z) + _threshold, new Vector3(max.x, min.y, z) + _threshold, Color.yellow);
                }





                AddChunkBounds(chunk?.West, Color.red);
                AddChunkBounds(chunk?.North, Color.red);
                AddChunkBounds(chunk?.East, Color.red);
                AddChunkBounds(chunk?.South, Color.red);
                AddChunkBounds(chunk?.Northwest, Color.red);
                AddChunkBounds(chunk?.Northeast, Color.red);
                AddChunkBounds(chunk?.Southwest, Color.red);
                AddChunkBounds(chunk?.Southeast, Color.red);
                AddChunkBounds(chunk, Color.blue);
            }
        } 
    }
}
