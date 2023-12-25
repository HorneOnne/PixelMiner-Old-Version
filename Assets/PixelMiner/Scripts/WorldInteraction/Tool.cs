using UnityEngine;
using System.Collections.Generic;
using PixelMiner.WorldBuilding;
using PixelMiner.Lighting;
using PixelMiner.Enums;
using PixelMiner.Core;

namespace PixelMiner.WorldInteraction
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
        private HashSet<Chunk> chunkNeedUpdate = new HashSet<Chunk>();


        private void Start()
        {
            //_main = Main.Instance;
            _mainCam = Camera.main;

            // Cursor
            _cursor = Instantiate(_cursorPrefab).transform;
            _cursor.gameObject.hideFlags = HideFlags.HideInHierarchy;
        }

        private int ClickCount = 0;

       
        private async void Update()
        {
            if(Time.time - _timer > _time)
            {
                _timer = Time.time;

                _ray = _mainCam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(_ray, out _hit))
                {
                    if (_hit.collider.transform.parent != null && _hit.collider.transform.parent.TryGetComponent<Chunk>(out _chunkHit))
                    {
                        Vector3Int hitGlobalPosition = new Vector3Int(Mathf.FloorToInt(_hit.point.x + 0.001f),
                                                                Mathf.FloorToInt(_hit.point.y + 0.001f),
                                                                Mathf.FloorToInt(_hit.point.z + 0.001f));
                        _cursor.transform.position = hitGlobalPosition;

                        Vector3Int relativePosition = GlobalToRelativeBlockPosition(hitGlobalPosition);
                        OnTarget?.Invoke(hitGlobalPosition, _chunkHit.GetBlock(relativePosition), _chunkHit.GetBlockLight(relativePosition), _chunkHit.GetAmbientLight(relativePosition));
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
                        Vector3Int hitGlobalPosition = new Vector3Int(Mathf.FloorToInt(_hit.point.x + 0.001f),
                                                                Mathf.FloorToInt(_hit.point.y + 0.001f),
                                                                Mathf.FloorToInt(_hit.point.z + 0.001f));

                        _cursor.transform.position = hitGlobalPosition;
                        Vector3Int hitRelativePosition = GlobalToRelativeBlockPosition(hitGlobalPosition);

                        if (Main.Instance.GetChunk((Vector3)hitGlobalPosition).ChunkHasDrawn == false) return;

                        if (_chunkHit.GetBlock(hitRelativePosition) == BlockType.Air || _chunkHit.GetBlock(hitRelativePosition) == BlockType.Water)
                        {
                            _chunkHit.SetBlock(hitRelativePosition, BlockType.Light);
                            _lightBfsQueue.Enqueue(new LightNode() { GlobalPosition = hitGlobalPosition, Intensity = LightUtils.MaxLightIntensity });
                            await LightCalculator.PropagateBlockLightAsync(_lightBfsQueue, chunkNeedUpdate);

                            Debug.Log(chunkNeedUpdate.Count);
                            foreach(var chunk in chunkNeedUpdate)
                            {
                                chunk.ReDrawChunkAsync();
                            }
                            chunkNeedUpdate.Clear();
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
                        Vector3Int hitGlobalPosition = new Vector3Int(Mathf.FloorToInt(_hit.point.x + 0.001f),
                                                                Mathf.FloorToInt(_hit.point.y + 0.001f),
                                                                Mathf.FloorToInt(_hit.point.z + 0.001f));
                        _cursor.transform.position = hitGlobalPosition;

                        Vector3Int hitRelativePosition = GlobalToRelativeBlockPosition(hitGlobalPosition);
                        if (_chunkHit.GetBlock(hitRelativePosition) != BlockType.Air)
                        {
                            _chunkHit.SetBlock(hitRelativePosition, BlockType.Air);
                            _lightRemovalBfsQueue.Enqueue(new LightNode() { GlobalPosition = hitGlobalPosition, Intensity = _chunkHit.GetBlockLight(hitRelativePosition) });
                            await LightCalculator.RemoveBlockLightAsync(_lightRemovalBfsQueue, chunkNeedUpdate);

                            foreach (var chunk in chunkNeedUpdate)
                            {
                                chunk.ReDrawChunkAsync();
                            }
                            chunkNeedUpdate.Clear();
                        }
                      
                    }
                }
            }


            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _ray = _mainCam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(_ray, out _hit))
                {
                    if (_hit.collider.transform.parent != null && _hit.collider.transform.parent.TryGetComponent<Chunk>(out _chunkHit))
                    {
                        Vector3Int hitGlobalPosition = new Vector3Int(Mathf.FloorToInt(_hit.point.x + 0.001f),
                                                                Mathf.FloorToInt(_hit.point.y + 0.001f),
                                                                Mathf.FloorToInt(_hit.point.z + 0.001f));
                        _cursor.transform.position = hitGlobalPosition;
                        Vector3Int hitRelativePosition = GlobalToRelativeBlockPosition(hitGlobalPosition);
                        if (_chunkHit.GetBlock(hitRelativePosition) == BlockType.Air)
                        {
                            _chunkHit.SetBlock(hitRelativePosition, BlockType.Stone);
                            _lightRemovalBfsQueue.Enqueue(new LightNode() { GlobalPosition = hitGlobalPosition, Intensity = _chunkHit.GetBlockLight(hitRelativePosition) });
                            await LightCalculator.RemoveBlockLightAsync(_lightRemovalBfsQueue, chunkNeedUpdate);


                            foreach (var chunk in chunkNeedUpdate)
                            {
                                chunk.ReDrawChunkAsync();
                            }
                            chunkNeedUpdate.Clear();
                        }
                    }
                }
            }


        }

        public Vector3Int GlobalToRelativeBlockPosition(Vector3 globalPosition)
        {
            // Assuming chunk volume is (32, 10, 32)
            int chunkWidth = 32;
            int chunkHeight = 10;
            int chunkDepth = 32;

            // Calculate the relative position within the chunk
            int relativeX = Mathf.FloorToInt(globalPosition.x) % chunkWidth;
            int relativeY = Mathf.FloorToInt(globalPosition.y) % chunkHeight;
            int relativeZ = Mathf.FloorToInt(globalPosition.z) % chunkDepth;

            // Ensure that the result is within the chunk's dimensions
            if (relativeX < 0) relativeX += chunkWidth;
            if (relativeY < 0) relativeY += chunkHeight;
            if (relativeZ < 0) relativeZ += chunkDepth;

            return new Vector3Int(relativeX, relativeY, relativeZ);
        }
    }
}

