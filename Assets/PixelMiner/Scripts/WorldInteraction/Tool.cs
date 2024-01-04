using UnityEngine;
using System.Collections.Generic;
using PixelMiner.WorldBuilding;
using PixelMiner.Lighting;
using PixelMiner.Enums;
using PixelMiner.Core;
using System.Threading.Tasks;
using PixelMiner.World;
using PixelMiner.Cam;

namespace PixelMiner.WorldInteraction
{
    public class Tool : MonoBehaviour
    {
        public static event System.Action<Vector3Int, BlockType, byte, byte> OnTarget;

        private float _timer;
        private float _time = 0.015f;
        private Camera _mainCam;
        private Ray _ray;
        private RaycastHit _hit;
        private RayCasting _rayCasting;

        //private Chunk _chunkHit;
        private Queue<LightNode> _lightBfsQueue = new Queue<LightNode>();
        private Queue<LightNode> _lightRemovalBfsQueue = new Queue<LightNode>();
        private HashSet<Chunk> chunksNeedUpdate = new HashSet<Chunk>();


        private void Start()
        {
            //_main = Main.Instance;
            _mainCam = Camera.main;


            _rayCasting = GameObject.FindAnyObjectByType<RayCasting>();
        }

        private int ClickCount = 0;


        private async void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Break();
            }


            if (Time.time - _timer > _time)
            {
                _timer = Time.time;

                _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 rayDirection = _ray.direction;
                if (_rayCasting.DDAVoxelRayCast(_mainCam.transform.position, rayDirection, out RaycastVoxelHit hit, out RaycastVoxelHit preHit))
                {
                    Vector3Int hitGlobalPosition = hit.point;
                    Vector3Int relativePosition = GlobalToRelativeBlockPosition(hitGlobalPosition);

                    OnTarget?.Invoke(hitGlobalPosition, Main.Instance.GetBlock(hitGlobalPosition), Main.Instance.GetBlockLight(hitGlobalPosition), Main.Instance.GetAmbientLight(hitGlobalPosition));
                }


                //_ray = _mainCam.ScreenPointToRay(Input.mousePosition);
                //if (Physics.Raycast(_ray, out _hit))
                //{
                //    if (_hit.collider.transform.parent != null && _hit.collider.transform.parent.TryGetComponent<Chunk>(out _chunkHit))
                //    {
                //        Vector3Int hitGlobalPosition = new Vector3Int(Mathf.FloorToInt(_hit.point.x + 0.001f),
                //                                                Mathf.FloorToInt(_hit.point.y + 0.001f),
                //                                                Mathf.FloorToInt(_hit.point.z + 0.001f));
                //        _cursor.transform.position = hitGlobalPosition;
                //        Vector3Int relativePosition = GlobalToRelativeBlockPosition(hitGlobalPosition);
                //        OnTarget?.Invoke(hitGlobalPosition, _chunkHit.GetBlock(relativePosition), _chunkHit.GetBlockLight(relativePosition), _chunkHit.GetAmbientLight(relativePosition));
                //    }
                //}
            }

            if (Input.GetMouseButtonDown(0))
            {
                _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 rayDirection = _ray.direction;
                if (_rayCasting.DDAVoxelRayCast(_mainCam.transform.position, rayDirection, out RaycastVoxelHit hit, out RaycastVoxelHit preHit))
                {
                    Vector3Int hitGlobalPosition = preHit.point;

                    if (Main.Instance.GetChunk((Vector3)hitGlobalPosition).ChunkHasDrawn == false) return;

                    if (!Main.Instance.GetBlock(hitGlobalPosition).IsSolid())
                    {
                        Main.Instance.SetBlock(hitGlobalPosition, BlockType.Light);
                        _lightBfsQueue.Enqueue(new LightNode() { GlobalPosition = hitGlobalPosition, Intensity = LightUtils.MaxLightIntensity });
                        await LightCalculator.PropagateBlockLightAsync(_lightBfsQueue, chunksNeedUpdate);
                        //StartCoroutine(FindAnyObjectByType<LightCalculator>().PropagateBlockLightAsync(_lightBfsQueue, chunksNeedUpdate));

                        DrawChunksAtOnce(chunksNeedUpdate);
                    }                
                }

                //_ray = _mainCam.ScreenPointToRay(Input.mousePosition);
                //if (Physics.Raycast(_ray, out _hit))
                //{
                //    if (_hit.collider.transform.parent != null && _hit.collider.transform.parent.TryGetComponent<Chunk>(out _chunkHit))
                //    {
                //        Vector3Int hitGlobalPosition = new Vector3Int(Mathf.FloorToInt(_hit.point.x + 0.001f),
                //                                                Mathf.FloorToInt(_hit.point.y + 0.001f),
                //                                                Mathf.FloorToInt(_hit.point.z + 0.001f));


                //        Vector3Int hitRelativePosition = GlobalToRelativeBlockPosition(hitGlobalPosition);

                //        if (Main.Instance.GetChunk((Vector3)hitGlobalPosition).ChunkHasDrawn == false) return;

                //        if (_chunkHit.GetBlock(hitRelativePosition) == BlockType.Air || _chunkHit.GetBlock(hitRelativePosition) == BlockType.Water)
                //        {
                //            _chunkHit.SetBlock(hitRelativePosition, BlockType.Light);
                //            _lightBfsQueue.Enqueue(new LightNode() { GlobalPosition = hitGlobalPosition, Intensity = LightUtils.MaxLightIntensity });
                //            await LightCalculator.PropagateBlockLightAsync(_lightBfsQueue, chunksNeedUpdate);
                //            //StartCoroutine(FindAnyObjectByType<LightCalculator>().PropagateBlockLightAsync(_lightBfsQueue, chunksNeedUpdate));


                //            DrawChunksAtOnce(chunksNeedUpdate);
                //        }
                //    }
                //}
            }

            if (Input.GetMouseButtonDown(1))
            {
                _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 rayDirection = _ray.direction;
                if (_rayCasting.DDAVoxelRayCast(_mainCam.transform.position, rayDirection, out RaycastVoxelHit hit, out RaycastVoxelHit preHit))
                {
                    Vector3Int hitGlobalPosition = hit.point;
                    Vector3Int relativePosition = GlobalToRelativeBlockPosition(hitGlobalPosition);

                    if (Main.Instance.GetBlock(hitGlobalPosition) != BlockType.Air)
                    {
                        Main.Instance.SetBlock(hitGlobalPosition, BlockType.Air);
                        _lightRemovalBfsQueue.Enqueue(new LightNode() { GlobalPosition = hitGlobalPosition, Intensity = Main.Instance.GetBlockLight(hitGlobalPosition) });
                        await LightCalculator.RemoveBlockLightAsync(_lightRemovalBfsQueue, chunksNeedUpdate);
                        //StartCoroutine(FindAnyObjectByType<LightCalculator>().RemoveBlockLightAsync(_lightRemovalBfsQueue, chunksNeedUpdate));

                        DrawChunksAtOnce(chunksNeedUpdate);
                    }
                }


                //_ray = _mainCam.ScreenPointToRay(Input.mousePosition);
                //if (Physics.Raycast(_ray, out _hit))
                //{
                //    if (_hit.collider.transform.parent != null && _hit.collider.transform.parent.TryGetComponent<Chunk>(out _chunkHit))
                //    {
                //        Vector3Int hitGlobalPosition = new Vector3Int(Mathf.FloorToInt(_hit.point.x + 0.001f),
                //                                                Mathf.FloorToInt(_hit.point.y + 0.001f),
                //                                                Mathf.FloorToInt(_hit.point.z + 0.001f));
      
                //        Vector3Int hitRelativePosition = GlobalToRelativeBlockPosition(hitGlobalPosition);
                //        if (_chunkHit.GetBlock(hitRelativePosition) != BlockType.Air)
                //        {
                //            _chunkHit.SetBlock(hitRelativePosition, BlockType.Air);
                //            _lightRemovalBfsQueue.Enqueue(new LightNode() { GlobalPosition = hitGlobalPosition, Intensity = _chunkHit.GetBlockLight(hitRelativePosition) });
                //            await LightCalculator.RemoveBlockLightAsync(_lightRemovalBfsQueue, chunksNeedUpdate);
                //            //StartCoroutine(FindAnyObjectByType<LightCalculator>().RemoveBlockLightAsync(_lightRemovalBfsQueue, chunksNeedUpdate));

                //            DrawChunksAtOnce(chunksNeedUpdate);
                //        }

                //    }
                //}
            }


            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 rayDirection = _ray.direction;
                if (_rayCasting.DDAVoxelRayCast(_mainCam.transform.position, rayDirection, out RaycastVoxelHit hit, out RaycastVoxelHit preHit))
                {

                    Vector3Int hitGlobalPosition = preHit.point;

                    if (Main.Instance.GetBlock(hitGlobalPosition).IsSolid() == false)
                    {
                        Main.Instance.SetBlock(hitGlobalPosition, BlockType.Stone);
                        _lightRemovalBfsQueue.Enqueue(new LightNode() { GlobalPosition = hitGlobalPosition, Intensity = Main.Instance.GetBlockLight(hitGlobalPosition) });
                        await LightCalculator.RemoveBlockLightAsync(_lightRemovalBfsQueue, chunksNeedUpdate);

                        DrawChunksAtOnce(chunksNeedUpdate);

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

        private void PlaceBlock(Vector3 globalPosition, BlockType blockType)
        {

        }

        private async void DrawChunksAtOnce(HashSet<Chunk> chunks)
        {
            List<Task> drawChunkTasks = new List<Task>();

            foreach (var chunk in chunks)
            {
                drawChunkTasks.Add(WorldGeneration.Instance.ReDrawChunkTask(chunk));
            }

            await Task.WhenAll(drawChunkTasks);
            chunks.Clear();
        }
    }
}

