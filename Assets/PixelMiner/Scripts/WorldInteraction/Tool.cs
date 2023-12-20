using UnityEngine;
using PixelMiner.WorldBuilding;
using PixelMiner.Lighting;
using PixelMiner.Enums;

namespace PixelMiner
{
    public class Tool : MonoBehaviour
    {
        public static event System.Action<Vector3Int, BlockType, byte> OnTarget;

        [SerializeField] private GameObject _cursorPrefab;
        //private Main _main;

        private Transform _cursor;

        private float _timer;
        private float _time = 0.015f;
        private Camera _mainCam;
        private Ray _ray;
        private RaycastHit _hit;

        private Chunk _chunkHit;

        private void Start()
        {
            //_main = Main.Instance;
            _mainCam = Camera.main;

            // Cursor
            _cursor = Instantiate(_cursorPrefab).transform;
            _cursor.gameObject.hideFlags = HideFlags.HideInHierarchy;
        }


       
        private void Update()
        {
            if(Time.time - _timer > _time)
            {
                _timer = Time.time;

                _ray = _mainCam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(_ray, out _hit))
                {
                    if (_hit.collider.transform.parent != null && _hit.collider.transform.parent.TryGetComponent<Chunk>(out _chunkHit))
                    {
                        Vector3Int hitPosition = new Vector3Int(Mathf.FloorToInt(_hit.point.x),
                                                                Mathf.FloorToInt(_hit.point.y - 1),
                                                                Mathf.FloorToInt(_hit.point.z));
                        _cursor.transform.position = hitPosition;

                        OnTarget?.Invoke(hitPosition, _chunkHit.GetBlock(hitPosition), _chunkHit.GetLight(hitPosition));
                    }
                }
            }

            if(Input.GetMouseButtonDown(0))
            {
                _ray = _mainCam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(_ray, out _hit))
                {
                    if (_hit.collider.transform.parent != null && _hit.collider.transform.parent.TryGetComponent<Chunk>(out _chunkHit))
                    {
                        Vector3Int hitPosition = new Vector3Int(Mathf.FloorToInt(_hit.point.x),
                                                                Mathf.FloorToInt(_hit.point.y - 1),
                                                                Mathf.FloorToInt(_hit.point.z));
                        _cursor.transform.position = hitPosition;
                 

                        LightCalculator.ProcessLight(hitPosition, _chunkHit);


                        _chunkHit.RedrawLightAsync();
                        //Debug.Log($"hit chunk: {_chunkHit.name}\t{_chunkHit.GetBlock(hitPosition)}");
                    }
                }
            }
 

            //Tile tile = Main.Instance.GetTile(mousePosition, out Chunk2D chunk);
            //Vector2 tileWorldPos = _main.GetTileWorldPosition(mousePosition);
            //_cursor.position = tileWorldPos;


            //if (Input.GetMouseButton(0))
            //{
            //    if (chunk != null)
            //    {
            //        chunk.SetTile(tile.FrameX, tile.FrameY, _main.GetTileBase(TileType.DirtGrass));
            //    }

            //}

            //if(Input.GetMouseButton(1))
            //{
            //    if (chunk != null)
            //    {
            //        chunk.SetTile(tile.FrameX, tile.FrameY, null);
            //    }
            //}

         
        }
    }
}

