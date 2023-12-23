using UnityEngine;
using System.Collections.Generic;
using PixelMiner.WorldBuilding;
using PixelMiner.Lighting;
using PixelMiner.Enums;

namespace PixelMiner
{
    public class Tool : MonoBehaviour
    {
        public static event System.Action<Vector3Int, BlockType, byte, byte> OnTarget;
        [SerializeField] private GameObject _cursorPrefab;
        //private Main _main;

        private Transform _cursor;

        private float _timer;
        private float _time = 0.015f;
        private Camera _mainCam;
        private Ray _ray;
        private RaycastHit _hit;

        private Chunk _chunkHit;
        private Queue<LightNode> _lightBfsQueue = new Queue<LightNode>();
        private Queue<LightNode> _lightRemovalBfsQueue = new Queue<LightNode>();


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

                        OnTarget?.Invoke(hitPosition, _chunkHit.GetBlock(hitPosition), _chunkHit.GetBlockLight(hitPosition), _chunkHit.GetAmbientLight(hitPosition));
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

                        //if(ClickCount == 0)
                        //{
                        //    ClickCount++;
                        //    hitPosition = new Vector3Int(16, 4, 16);
                        //}
                        //else
                        //{
                        //    hitPosition = new Vector3Int(16, 4, 20);
                        //}


                        if(_chunkHit.GetBlock(hitPosition) == BlockType.Air)
                        {
                            _chunkHit.SetBlock(hitPosition, BlockType.Light);
                            _lightBfsQueue.Enqueue(new LightNode() { position = hitPosition, val = 16 });
                            LightCalculator.PropagateLight(_lightBfsQueue);
  
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
                            _lightRemovalBfsQueue.Enqueue(new LightNode() { position = hitPosition, val = 16 });
                            LightCalculator.RemoveLight(_lightRemovalBfsQueue);
                            _chunkHit.ReDrawChunkAsync();
                        }
                      
                    }
                }
            }


        }
    }
}

