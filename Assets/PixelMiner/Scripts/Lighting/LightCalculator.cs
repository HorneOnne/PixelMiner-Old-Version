using PixelMiner.Enums;
using PixelMiner.WorldBuilding;
using System.Collections.Generic;
using UnityEngine;
using PixelMiner.Core;
using System.Threading.Tasks;
using System.Collections.Concurrent;


namespace PixelMiner.Lighting
{
    public struct LightNode
    {
        public Vector3Int GlobalPosition;
        public byte Intensity;

        public LightNode(Vector3Int globalPosition, byte intensity)
        {
            this.GlobalPosition = globalPosition;
            this.Intensity = intensity;
        }
    }



    public struct Light
    {
        public ushort Torque;
        public byte Sun;
    }



    /// <summary>
    /// To calculate lighting i use global position for LightNode.Position for easily calcualting light propagate cross chunk.
    /// </summary>
    public class LightCalculator
    {             
        private static Vector3Int[] _neighborsPosition = new Vector3Int[6];
        private static Vector3Int[] _diagonalNeighborsPosition = new Vector3Int[4];
        private static Vector3Int CHUNK_VOLUME = new Vector3Int(32, 10, 32);

        //public GameObject TextPrefab;
        //private ConcurrentDictionary<Vector3Int, int> Dict = new ConcurrentDictionary<Vector3Int, int>();
        //private WaitForSeconds _wait = new WaitForSeconds(0.001f);

        public static async Task PropagateBlockLightAsync(Queue<LightNode> lightBfsQueue, HashSet<Chunk> chunkNeedUpdate)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            //Debug.Log("Propagate light");
            int attempts = 0;
            Main.Instance.SetBlockLight(lightBfsQueue.Peek().GlobalPosition, lightBfsQueue.Peek().Intensity);


            await Task.Run(() =>
            {
                
                while (lightBfsQueue.Count > 0)
                {
                    LightNode currentNode = lightBfsQueue.Dequeue();

                    var neighbors = GetVoxelNeighborPosition(currentNode.GlobalPosition);
                    for (int i = 0; i < neighbors.Length; i++)
                    {

                        if (neighbors[i].y > CHUNK_VOLUME[1] - 1) continue;

                        if (Main.Instance.TryGetChunk(neighbors[i], out Chunk chunk))
                        {
                            if (!chunkNeedUpdate.Contains(chunk))
                            {
                                chunkNeedUpdate.Add(chunk);
                            }
                        }

                        BlockType currentBlock = Main.Instance.GetBlock(neighbors[i]);
                        byte blockOpacity = LightUtils.GetOpacity(currentBlock);

                        if (Main.Instance.GetBlockLight(neighbors[i]) + blockOpacity < currentNode.Intensity && currentNode.Intensity > 0)
                        {
                            LightNode neighborNode = new LightNode(neighbors[i], (byte)(currentNode.Intensity - blockOpacity));
                            lightBfsQueue.Enqueue(neighborNode);
                            Main.Instance.SetBlockLight(neighborNode.GlobalPosition, neighborNode.Intensity);
                        }
                    }


                    //var diagonalNeighbors = GetVoxelNeighborDiagonalPositions(currentNode.GlobalPosition);
                    //for (int i = 0; i < diagonalNeighbors.Length; i++)
                    //{

                    //    if (diagonalNeighbors[i].y > CHUNK_VOLUME[1] - 1) continue;

                    //    if (Main.Instance.TryGetChunk(diagonalNeighbors[i], out Chunk chunk))
                    //    {
                    //        if (!chunkNeedUpdate.Contains(chunk))
                    //        {
                    //            chunkNeedUpdate.Add(chunk);
                    //        }
                    //    }

                    //    BlockType currentBlock = Main.Instance.GetBlock(diagonalNeighbors[i]);
                    //    byte blockOpacity = (byte)(LightUtils.GetOpacity(currentBlock) * 1.4);

                    //    if (Main.Instance.GetBlockLight(diagonalNeighbors[i]) + blockOpacity < currentNode.Intensity && currentNode.Intensity > 0)
                    //    {
                    //        LightNode neighborNode = new LightNode(diagonalNeighbors[i], (byte)(currentNode.Intensity - blockOpacity));
                    //        lightBfsQueue.Enqueue(neighborNode);
                    //        Main.Instance.SetBlockLight(neighborNode.GlobalPosition, neighborNode.Intensity);
                    //    }
                    //}


                    attempts++;
                    if (attempts > 10000)
                    {
                        Debug.LogWarning("Infinite loop");
                        break;
                    }
                }

                Debug.Log($"Propagate Attempts: {attempts}");
             
            });

            sw.Stop();
            //Debug.Log($"Propagte light in : {sw.ElapsedMilliseconds / 1000f} s");
        }

        public static async Task RemoveBlockLightAsync(Queue<LightNode> removeLightBfsQueue, HashSet<Chunk> chunkNeedUpdate)
        {
            Debug.Log("Remove Light");

            Main.Instance.SetBlockLight(removeLightBfsQueue.Peek().GlobalPosition, 0);
            Queue<LightNode> spreadLightBfsQueue = new Queue<LightNode>();

            await Task.Run(() =>
            {
                int attempts = 0;
                while (removeLightBfsQueue.Count > 0)
                {
                    LightNode currentNode = removeLightBfsQueue.Dequeue();


                    var neighbors = GetVoxelNeighborPosition(currentNode.GlobalPosition);
                    for (int i = 0; i < neighbors.Length; i++)
                    {
                        //if (IsValidPosition(neighbors[i]) == false) continue;

                        if (Main.Instance.GetBlockLight(neighbors[i]) != 0)
                        {
                            if (Main.Instance.TryGetChunk(neighbors[i], out Chunk chunk))
                            {
                                if (!chunkNeedUpdate.Contains(chunk))
                                {
                                    chunkNeedUpdate.Add(chunk);
                                }
                            }

                            BlockType currentBlock = Main.Instance.GetBlock(neighbors[i]);
                            byte blockOpacity = LightUtils.GetOpacity(currentBlock);

                            if (Main.Instance.GetBlockLight(neighbors[i]) < currentNode.Intensity)
                            {
                                LightNode neighborNode = new LightNode(neighbors[i], (byte)(currentNode.Intensity - blockOpacity));
                                removeLightBfsQueue.Enqueue(neighborNode);
                                Main.Instance.SetBlockLight(neighbors[i], 0);
                            }
                            else
                            {
                                LightNode neighborNode = new LightNode(neighbors[i], Main.Instance.GetBlockLight(neighbors[i]));
                                spreadLightBfsQueue.Enqueue(neighborNode);
                            }
                        }
                    }


                    //var diagonalNeighbors = GetVoxelNeighborDiagonalPositions(currentNode.GlobalPosition);
                    //for (int i = 0; i < diagonalNeighbors.Length; i++)
                    //{
                    //    if (Main.Instance.GetBlockLight(diagonalNeighbors[i]) != 0)
                    //    {
                    //        if (Main.Instance.TryGetChunk(diagonalNeighbors[i], out Chunk chunk))
                    //        {
                    //            if (!chunkNeedUpdate.Contains(chunk))
                    //            {
                    //                chunkNeedUpdate.Add(chunk);
                    //            }
                    //        }

                    //        BlockType currentBlock = Main.Instance.GetBlock(diagonalNeighbors[i]);
                    //        byte blockOpacity = (byte)(LightUtils.GetOpacity(currentBlock) * 1.4);


                    //        if (Main.Instance.GetBlockLight(diagonalNeighbors[i]) < currentNode.Intensity)
                    //        {
                    //            LightNode neighborNode = new LightNode(diagonalNeighbors[i], (byte)(currentNode.Intensity - blockOpacity));
                    //            removeLightBfsQueue.Enqueue(neighborNode);
                    //            Main.Instance.SetBlockLight(diagonalNeighbors[i], 0);
                    //        }
                    //        else
                    //        {
                    //            LightNode neighborNode = new LightNode(diagonalNeighbors[i], Main.Instance.GetBlockLight(diagonalNeighbors[i]));
                    //            spreadLightBfsQueue.Enqueue(neighborNode);
                    //        }
                    //    }
                    //}


                    attempts++;
                    if (attempts > 10000)
                    {
                        Debug.LogWarning("Infinite loop");
                        break;
                    }
                }

                Debug.Log($"Remove block Attempts: {attempts}");
            });


            if (spreadLightBfsQueue.Count > 0)
            {
                await PropagateBlockLightAsync(spreadLightBfsQueue, chunkNeedUpdate);
            }
        }


        #region Ambient Light
        public static async void PropagateAmbientLight(Chunk chunk, List<LightNode> ambientLightList)
        {
            Debug.Log("PropagateAmbientLight");
            ConcurrentQueue<LightNode> lightSpreadQueue = new ConcurrentQueue<LightNode>();


            await Task.Run(() =>
            {
                Parallel.For(0, ambientLightList.Count, (i) =>
                {
                    int attempts = 0;
                    chunk.SetAmbientLight(ambientLightList[i].GlobalPosition, ambientLightList[i].Intensity);
                    LightNode currentNode = ambientLightList[i];

                    //if(Dict.ContainsKey(currentNode.position) == false)
                    //{
                    //    Dict.TryAdd(currentNode.position, currentNode.val);
                    //}

                    while (true)
                    {
                        Vector3Int dLPos = new Vector3Int(currentNode.GlobalPosition.x, currentNode.GlobalPosition.y - 1, currentNode.GlobalPosition.z);

                        if (chunk.IsValidRelativePosition(dLPos) == false)
                        {
                            break;
                        }

                        if (chunk.GetBlock(dLPos) == BlockType.Air || chunk.GetBlock(dLPos) == BlockType.Water)
                        {
                            if (chunk.GetAmbientLight(dLPos) + 1 < currentNode.Intensity && currentNode.Intensity > 0)
                            {
                                chunk.SetAmbientLight(dLPos, currentNode.Intensity);
                                //if (Dict.ContainsKey(dLPos) == false)
                                //{
                                //    Dict.TryAdd(dLPos, currentNode.val);
                                //}

                                currentNode = new LightNode(dLPos, currentNode.Intensity);


                            }
                            else
                            {
                                lightSpreadQueue.Enqueue(currentNode);
                                break;
                            }

                        }
                        else
                        {
                            lightSpreadQueue.Enqueue(currentNode);
                            break;
                        }


                        attempts++;
                        if (attempts > 100)
                        {
                            Debug.LogWarning("Infinite loop");
                            break;
                        }
                    }
                });
            });
        }

        #endregion

        #region Neighbors
        public static Vector3Int[] GetVoxelNeighborPosition(Vector3Int position)
        {
            _neighborsPosition[0] = position + new Vector3Int(1, 0, 0);
            _neighborsPosition[1] = position + new Vector3Int(-1, 0, 0);
            _neighborsPosition[2] = position + new Vector3Int(0, 0, 1);
            _neighborsPosition[3] = position + new Vector3Int(0, 0, -1);
            _neighborsPosition[4] = position + new Vector3Int(0, 1, 0);
            _neighborsPosition[5] = position + new Vector3Int(0, -1, 0);

            return _neighborsPosition;
        }

        public static Vector3Int[] GetVoxelNeighborDiagonalPositions(Vector3Int position)
        {
            //return new Vector3Int[]
            //{
            //    position + new Vector3Int(-1, 0, 1),
            //    position + new Vector3Int(1, 0, 1),
            //     position + new Vector3Int(-1, 0, -1),
            //    position + new Vector3Int(1, 0, -1),
            //};

            _diagonalNeighborsPosition[0] = position + new Vector3Int(-1, 0, 1);
            _diagonalNeighborsPosition[1] = position + new Vector3Int(1, 0, 1);
            _diagonalNeighborsPosition[2] = position + new Vector3Int(-1, 0, -1);
            _diagonalNeighborsPosition[3] = position + new Vector3Int(1, 0, -1);

            return _diagonalNeighborsPosition;

        }
        #endregion
    }
}
