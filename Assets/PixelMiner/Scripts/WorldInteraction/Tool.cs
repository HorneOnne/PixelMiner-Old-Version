using UnityEngine;
using PixelMiner.WorldBuilding;
using PixelMiner.Lighting;
using PixelMiner.Enums;
using PlasticGui.Help.Conditions;

namespace PixelMiner
{
    public class Tool : MonoBehaviour
    {
        public static event System.Action<Vector3Int, BlockType, byte> OnTarget;
        public LightCalculator LightCalculator;
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

        private int ClickCount = 0;

       
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
                        Vector3Int hitPosition = new Vector3Int(Mathf.FloorToInt(_hit.point.x + 0.001f),
                                                                Mathf.FloorToInt(_hit.point.y + 0.001f),
                                                                Mathf.FloorToInt(_hit.point.z + 0.001f));
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
                        Vector3Int hitPosition = new Vector3Int(Mathf.FloorToInt(_hit.point.x + 0.001f),
                                                                Mathf.FloorToInt(_hit.point.y + 0.001f),
                                                                Mathf.FloorToInt(_hit.point.z + 0.001f));
                        _cursor.transform.position = hitPosition;

                        if(ClickCount == 0)
                        {
                            ClickCount++;
                            hitPosition = new Vector3Int(16, 4, 16);
                        }
                        else
                        {
                            hitPosition = new Vector3Int(16, 4, 20);
                        }


                        if(_chunkHit.GetBlock(hitPosition) == BlockType.Air)
                        {
                            _chunkHit.SetBlock(hitPosition, BlockType.Light);
                            LightCalculator.ProcessLight(hitPosition, _chunkHit);
                            //StartCoroutine(LightCalculator.ProcessLight(hitPosition, _chunkHit));

                            _chunkHit.ReDrawChunkAsync();
                        }                
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                _ray = _mainCam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(_ray, out _hit))
                {
                    if (_hit.collider.transform.parent != null && _hit.collider.transform.parent.TryGetComponent<Chunk>(out _chunkHit))
                    {
                        Vector3Int hitPosition = new Vector3Int(Mathf.FloorToInt(_hit.point.x + 0.001f),
                                                                Mathf.FloorToInt(_hit.point.y + 0.001f),
                                                                Mathf.FloorToInt(_hit.point.z + 0.001f));
                        _cursor.transform.position = hitPosition;


                        if(_chunkHit.GetBlock(hitPosition) != BlockType.Air)
                        {
                            _chunkHit.SetBlock(hitPosition, BlockType.Air);
                            //LightCalculator.RemoveLight(hitPosition, _chunkHit);
                            StartCoroutine(LightCalculator.RemoveLight(hitPosition, _chunkHit));


                            _chunkHit.ReDrawChunkAsync();
                        }
                      
                    }
                }
            }


        }
    }
}

