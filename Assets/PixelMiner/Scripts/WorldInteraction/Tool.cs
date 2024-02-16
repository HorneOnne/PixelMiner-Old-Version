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
            if (Time.time - _timer > _time)
            {
                _timer = Time.time;

                //_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //Vector3 rayDirection = _ray.direction;
                //if (_rayCasting.DDAVoxelRayCast(_mainCam.transform.position, rayDirection, out RaycastVoxelHit hit, out RaycastVoxelHit preHit))
                //{
                //    Vector3Int hitGlobalPosition = hit.point;
                //    Vector3Int relativePosition = GlobalToRelativeBlockPosition(hitGlobalPosition);

            
                //    OnTarget?.Invoke(hitGlobalPosition, Main.Instance.GetBlock(hitGlobalPosition), Main.Instance.GetBlockLight(preHit.point), Main.Instance.GetAmbientLight(preHit.point));
                //}
            }

            if (Input.GetMouseButtonDown(0))
            {
             
                _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 rayDirection = _ray.direction;
                if (_rayCasting.DDAVoxelRayCast(_mainCam.transform.position, rayDirection, out RaycastVoxelHit hit, out RaycastVoxelHit preHit))
                {
                    Vector3Int hitGlobalPosition = preHit.point;

                    if (Main.Instance.GetChunk((Vector3)hitGlobalPosition).HasDrawnFirstTime == false) return;

                    if (Main.Instance.GetBlock(hitGlobalPosition).IsSolid() == false &&
                        Main.Instance.GetBlock(hitGlobalPosition).IsTransparentSolidBlock() == false)
                    {
                        Main.Instance.SetBlock(hitGlobalPosition, BlockType.Light);
                        _lightBfsQueue.Enqueue(new LightNode() { GlobalPosition = hitGlobalPosition, Intensity = LightUtils.MaxLightIntensity });                   
                        await LightCalculator.PropagateBlockLightAsync(_lightBfsQueue, chunksNeedUpdate);              
                        DrawChunksAtOnce(chunksNeedUpdate);
                    }                
                }
             

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
                        DrawChunksAtOnce(chunksNeedUpdate);
                    }
                }
         
            }


            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 rayDirection = _ray.direction;
                if (_rayCasting.DDAVoxelRayCast(_mainCam.transform.position, rayDirection, out RaycastVoxelHit hit, out RaycastVoxelHit preHit))
                {

                    Vector3Int hitGlobalPosition = preHit.point;

                    if (Main.Instance.GetBlock(hitGlobalPosition).IsSolid() == false &&
                        Main.Instance.GetBlock(hitGlobalPosition).IsTransparentSolidBlock() == false)
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
            // Calculate the relative position within the chunk
            int relativeX = Mathf.FloorToInt(globalPosition.x) % Main.Instance.ChunkDimension[0];
            int relativeY = Mathf.FloorToInt(globalPosition.y) % Main.Instance.ChunkDimension[1];
            int relativeZ = Mathf.FloorToInt(globalPosition.z) % Main.Instance.ChunkDimension[2];

            // Ensure that the result is within the chunk's dimensions
            if (relativeX < 0) relativeX += Main.Instance.ChunkDimension[0];
            if (relativeY < 0) relativeY += Main.Instance.ChunkDimension[1];
            if (relativeZ < 0) relativeZ += Main.Instance.ChunkDimension[2];

            return new Vector3Int(relativeX, relativeY, relativeZ);
        }

        private void PlaceBlock(Vector3 globalPosition, TextureType blockType)
        {

        }

        private async void DrawChunksAtOnce(HashSet<Chunk> chunks)
        {
            List<Task> drawChunkTasks = new List<Task>();
            //Debug.Log($"Draw at once: {chunks.Count}");
            foreach (var chunk in chunks)
            {
                drawChunkTasks.Add(WorldGeneration.Instance.ReDrawChunkTask(chunk));
                //await WorldGeneration.Instance.ReDrawChunkTask(chunk);
            }
            await Task.WhenAll(drawChunkTasks);
            chunks.Clear();
        }
    }
}

