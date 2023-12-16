using UnityEngine;
using PixelMiner.WorldGen;
using PixelMiner.Enums;
using PixelMiner.WorldBuilding;

namespace PixelMiner
{
    public class Tool : MonoBehaviour
    {
        [SerializeField] private GameObject _cursorPrefab;
        private Main _main;

        private Transform _cursor;

        private float _timer;
        private float _time = 0.02f;
        private Camera _mainCam;
        private Ray _ray;
        private RaycastHit _hit;

        private void Start()
        {
            _main = Main.Instance;
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
                    if (_hit.collider != null)
                    {
                        Vector3Int hitPosition = new Vector3Int(Mathf.FloorToInt(_hit.point.x),
                                                                Mathf.FloorToInt(_hit.point.y),
                                                                Mathf.FloorToInt(_hit.point.z));
                        _cursor.transform.position = hitPosition;
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

