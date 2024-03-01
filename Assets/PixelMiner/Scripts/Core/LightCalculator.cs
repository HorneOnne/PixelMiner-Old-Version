using PixelMiner.Enums;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace PixelMiner.Core
{
    /// <summary>
    /// To calculate lighting i use global position for LightNode.Position for easily calcualting light propagate cross chunk.
    /// </summary>
    public class LightCalculator : MonoBehaviour
    {
        public static LightCalculator Instance { get; private set; }


        //public GameObject TextPrefab;
        //private Dictionary<Vector3Int, GameObject> Dict = new Dictionary<Vector3Int, GameObject>();
        //private WaitForSeconds _wait = new WaitForSeconds(0.2f);
        //public bool ShowDiagonal = false;
        //public float RemoveTime;
        //public float PropagateTime;

        private void Awake()
        {
            Instance = this;
        }


        public static async Task PropagateBlockLightAsync(Queue<LightNode> lightBfsQueue, HashSet<Chunk> chunksNeedUpdate)
        {
            //Debug.Log("Propagate light");
            int attempts = 0;
            Main main = Main.Instance;
            Vector3Int[] neighbors = new Vector3Int[6];

            LightNode startNode = lightBfsQueue.Peek();
            main.SetBlockLight(startNode.GlobalPosition, startNode.Intensity);

            Chunk currentTargetChunk;
            main.TryGetChunk(startNode.GlobalPosition, out currentTargetChunk);
            chunksNeedUpdate.Add(currentTargetChunk);

            await Task.Run(() =>
            {
                while (lightBfsQueue.Count > 0)
                {
                    LightNode currentNode = lightBfsQueue.Dequeue();
                    GetVoxelNeighborPosition(currentNode.GlobalPosition, ref neighbors);
                    for (int i = 0; i < neighbors.Length; i++)
                    {
                        //if (i < 6)
                        //{
                        //   blockOpacity = LightUtils.GetOpacity(currentBlock); 
                        //}
                        //else
                        //{
                        //    blockOpacity = (byte)(LightUtils.GetOpacity(currentBlock) * 1.4);
                        //}

                        if (!main.InSideChunkBound(currentTargetChunk, neighbors[i]))
                        {
                            if (main.TryGetChunk(neighbors[i], out currentTargetChunk))
                            {
                                if (!chunksNeedUpdate.Contains(currentTargetChunk))
                                {
                                    chunksNeedUpdate.Add(currentTargetChunk);
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }

                        Vector3Int relativePos = currentTargetChunk.GetRelativePosition(neighbors[i]);
                        BlockType currentBlock = currentTargetChunk.GetBlock(relativePos);
                        byte blockOpacity = LightUtils.BlocksLightResistance[(byte)currentBlock];

                        if (currentTargetChunk.GetBlockLight(relativePos) + blockOpacity < currentNode.Intensity && currentNode.Intensity > 0)
                        {
                            LightNode neighborNode = new LightNode(neighbors[i], (byte)(currentNode.Intensity - blockOpacity));
                            lightBfsQueue.Enqueue(neighborNode);
                            currentTargetChunk.SetBlockLight(relativePos, neighborNode.Intensity);
                        }
                    }

                    attempts++;
                    if (attempts > 10000)
                    {
                        Debug.LogWarning("Infinite loop");
                        break;
                    }
                }
            });
        }
        public static async Task RemoveBlockLightAsync(Queue<LightNode> removeLightBfsQueue, HashSet<Chunk> chunksNeedUpdate)
        {
            //Debug.Log("Remove Light");
            Main main = Main.Instance;
            main.SetBlockLight(removeLightBfsQueue.Peek().GlobalPosition, 0);
            Queue<LightNode> spreadLightBfsQueue = new Queue<LightNode>();
            Dictionary<Vector3Int, byte> spreadLightDict = new Dictionary<Vector3Int, byte>();
            int attempts = 0;
            Vector3Int[] neighbors = new Vector3Int[6];


            Chunk currentTargetChunk;
            main.TryGetChunk(removeLightBfsQueue.Peek().GlobalPosition, out currentTargetChunk);
            chunksNeedUpdate.Add(currentTargetChunk);


            await Task.Run(() =>
            {
                while (removeLightBfsQueue.Count > 0)
                {
                    LightNode currentNode = removeLightBfsQueue.Dequeue();

                    GetVoxelNeighborPosition(currentNode.GlobalPosition, ref neighbors);
                    for (int i = 0; i < neighbors.Length; i++)
                    {
                        if (!main.InSideChunkBound(currentTargetChunk, neighbors[i]))
                        {
                            if (main.TryGetChunk(neighbors[i], out currentTargetChunk))
                            {
                                if (!chunksNeedUpdate.Contains(currentTargetChunk))
                                {
                                    chunksNeedUpdate.Add(currentTargetChunk);
                                }
                            }
                        }
                        Vector3Int relativePos = currentTargetChunk.GetRelativePosition(neighbors[i]);
                        if (currentTargetChunk.GetBlockLight(relativePos) == 0)
                        {
                            continue;
                        }


                        BlockType currentBlock = currentTargetChunk.GetBlock(relativePos);
                        byte blockOpacity = LightUtils.BlocksLightResistance[(byte)currentBlock];

                        if (currentTargetChunk.GetBlockLight(relativePos) + blockOpacity <= currentNode.Intensity)
                        {
                            LightNode neighborNode = new LightNode(neighbors[i], (byte)(currentNode.Intensity - blockOpacity));
                            removeLightBfsQueue.Enqueue(neighborNode);
                            currentTargetChunk.SetBlockLight(relativePos, 0);

                            if (spreadLightDict.ContainsKey(neighbors[i]))
                            {
                                spreadLightDict.Remove(neighbors[i]);
                            }
                        }
                        else
                        {
                            LightNode neighborNode = new LightNode(neighbors[i], currentTargetChunk.GetBlockLight(relativePos));


                            if (spreadLightDict.ContainsKey(neighborNode.GlobalPosition) == false)
                            {
                                spreadLightDict.Add(neighborNode.GlobalPosition, neighborNode.Intensity);
                            }
                            else
                            {
                                spreadLightDict[neighborNode.GlobalPosition] = neighborNode.Intensity;
                            }
                        }
                    }



                    attempts++;
                    if (attempts > 10000)
                    {
                        Debug.LogWarning("Infinite loop");
                        break;
                    }
                }
            });



            //main.TryGetChunk(removeLightBfsQueue.Peek().GlobalPosition, out Chunk chunk);
            //chunksNeedUpdate.Add(chunk);
            //await Task.Run(() =>
            //{
            //    while (removeLightBfsQueue.Count > 0)
            //    {
            //        LightNode currentNode = removeLightBfsQueue.Dequeue();

            //        GetVoxelNeighborPosition(currentNode.GlobalPosition, ref neighbors);
            //        for (int i = 0; i < neighbors.Length; i++)
            //        {         
            //            if (!main.InSideChunkBound(chunk, neighbors[i]))
            //            {
            //                Chunk chunkEffected = main.GetChunkPerformance(chunk, neighbors[i]);
            //                if (!chunksNeedUpdate.Contains(chunkEffected))
            //                {
            //                    chunksNeedUpdate.Add(chunkEffected);
            //                }
            //            }


            //            if (main.GetBlockLightPerformance(chunk, neighbors[i]) == 0)
            //            {
            //                continue;
            //            }



            //            BlockType currentBlock = main.GetBlockPerformance(chunk, neighbors[i]);
            //            byte blockOpacity = LightUtils.BlocksOpaque[(byte)currentBlock];

            //            if (main.GetBlockLightPerformance(chunk, neighbors[i]) + blockOpacity <= currentNode.Intensity)
            //            {
            //                LightNode neighborNode = new LightNode(neighbors[i], (byte)(currentNode.Intensity - blockOpacity));
            //                removeLightBfsQueue.Enqueue(neighborNode);
            //                main.SetBlockLightPerformance(chunk, neighbors[i], 0);

            //                if (spreadLightDict.ContainsKey(neighbors[i]))
            //                {
            //                    spreadLightDict.Remove(neighbors[i]);
            //                }
            //            }
            //            else
            //            {
            //                LightNode neighborNode = new LightNode(neighbors[i], main.GetBlockLightPerformance(chunk, neighbors[i]));

            //                if (spreadLightDict.ContainsKey(neighborNode.GlobalPosition) == false)
            //                {
            //                    spreadLightDict.Add(neighborNode.GlobalPosition, neighborNode.Intensity);
            //                }
            //                else
            //                {
            //                    spreadLightDict[neighborNode.GlobalPosition] = neighborNode.Intensity;
            //                }
            //            }
            //        }



            //        attempts++;
            //        if (attempts > 10000)
            //        {
            //            Debug.LogWarning("Infinite loop");
            //            break;
            //        }
            //    }
            //});



            foreach (var node in spreadLightDict)
            {
                spreadLightBfsQueue.Enqueue(new LightNode(node.Key, node.Value));
            }
            if (spreadLightBfsQueue.Count > 0)
            {
                await PropagateBlockLightAsync(spreadLightBfsQueue, chunksNeedUpdate);
            }
        }


        #region Ambient Light   
        public async Task SpreadAmbientLightTask(Chunk chunk)
        {
            Debug.Log("Propagate light");
            int attempts = 0;
            Main main = Main.Instance;
            Vector3Int[] neighbors = new Vector3Int[6];

            await Task.Run(() =>
            {
                while (chunk.AmbientLightBfsQueue.Count > 0)
                {
                    if (chunk.AmbientLightBfsQueue.TryDequeue(out LightNode currentNode))
                    {
                        GetVoxelNeighborPosition(currentNode.GlobalPosition, ref neighbors);
                        for (int i = 0; i < neighbors.Length; i++)
                        {
                            if (main.InSideChunkBound(chunk, neighbors[i]))
                            {
                                Vector3Int relativePos = chunk.GetRelativePosition(neighbors[i]);
                                BlockType currentBlock = chunk.GetBlock(relativePos);
                                byte blockOpacity;
                                if (currentBlock == BlockType.Air && i == 5)
                                {
                                    blockOpacity = 0;
                                }
                                else
                                {
                                    blockOpacity = LightUtils.BlocksLightResistance[(byte)currentBlock];
                                }


                                if (chunk.GetAmbientLight(relativePos) + blockOpacity < currentNode.Intensity && currentNode.Intensity > 0)
                                {
                                    LightNode neighborNode = new LightNode(neighbors[i], (byte)(currentNode.Intensity - blockOpacity));
                                    chunk.AmbientLightBfsQueue.Enqueue(neighborNode);
                                    chunk.SetAmbientLight(relativePos, neighborNode.Intensity);
                                }

                                //if (main.GetAmbientLight(neighbors[i]) + blockOpacity < currentNode.Intensity && currentNode.Intensity > 0)
                                //{
                                //    LightNode neighborNode = new LightNode(neighbors[i], (byte)(currentNode.Intensity - blockOpacity));
                                //    chunk.AmbientLightBfsQueue.Enqueue(neighborNode);
                                //    main.SetAmbientLight(neighbors[i], neighborNode.Intensity);
                                //}
                            }
                            else
                            {
                                BlockType currentBlock = Main.Instance.GetBlock(neighbors[i]);
                                byte blockOpacity;
                                if (currentBlock == BlockType.Air && i == 5)
                                {
                                    blockOpacity = 0;
                                }
                                else
                                {
                                    blockOpacity = LightUtils.BlocksLightResistance[(byte)currentBlock];
                                }

                                if (main.GetAmbientLight(neighbors[i]) + blockOpacity < currentNode.Intensity && currentNode.Intensity > 0)
                                {
                                    if (main.TryGetChunk(neighbors[i], out Chunk neighborChunk))
                                    {
                                        LightNode neighborNode = new LightNode(neighbors[i], (byte)(currentNode.Intensity - blockOpacity));
                                        neighborChunk.SetAmbientLight(neighborChunk.GetRelativePosition(neighbors[i]), neighborNode.Intensity);
                                    }
                                }
                            }
                        }
                    }


                    attempts++;
                    if (attempts > 50000)
                    {
                        Debug.LogWarning("Infinite loop");
                        break;
                    }
                }
            });


            //int attempts = 0;
            //Main main = Main.Instance;
            //Vector3Int[] neighbors = new Vector3Int[6];
            //await Task.Run(() =>
            //{
            //    while (chunk.AmbientLightBfsQueue.Count > 0)
            //    {
            //        if (chunk.AmbientLightBfsQueue.TryDequeue(out LightNode currentNode))
            //        {
            //            GetVoxelNeighborPosition(currentNode.GlobalPosition, ref neighbors);
            //            for (int i = 0; i < neighbors.Length; i++)
            //            {
            //                if (main.InSideChunkBound(chunk, neighbors[i]))
            //                {
            //                    BlockType currentBlock = main.GetBlock(neighbors[i]);
            //                    byte blockOpacity;
            //                    if (currentBlock == BlockType.Air && i == 5)
            //                    {
            //                        blockOpacity = 0;
            //                    }
            //                    else
            //                    {
            //                        blockOpacity = LightUtils.BlocksLightResistance[(byte)currentBlock];
            //                    }


            //                    if (main.GetAmbientLight(neighbors[i]) + blockOpacity < currentNode.Intensity && currentNode.Intensity > 0)
            //                    {
            //                        LightNode neighborNode = new LightNode(neighbors[i], (byte)(currentNode.Intensity - blockOpacity));
            //                        chunk.AmbientLightBfsQueue.Enqueue(neighborNode);
            //                        main.SetAmbientLight(neighbors[i], neighborNode.Intensity);
            //                    }
            //                }
            //                else
            //                {
            //                    BlockType currentBlock = Main.Instance.GetBlock(neighbors[i]);
            //                    byte blockOpacity;
            //                    if (currentBlock == BlockType.Air && i == 5)
            //                    {
            //                        blockOpacity = 0;
            //                    }
            //                    else
            //                    {
            //                        blockOpacity = LightUtils.BlocksLightResistance[(byte)currentBlock];
            //                    }

            //                    if (main.GetAmbientLight(neighbors[i]) + blockOpacity < currentNode.Intensity && currentNode.Intensity > 0)
            //                    {
            //                        if (main.TryGetChunk(neighbors[i], out Chunk neighborChunk))
            //                        {
            //                            LightNode neighborNode = new LightNode(neighbors[i], (byte)(currentNode.Intensity - blockOpacity));
            //                            neighborChunk.SetAmbientLight(neighborChunk.GetRelativePosition(neighbors[i]), neighborNode.Intensity);
            //                        }
            //                    }
            //                }
            //            }
            //        }


            //        attempts++;
            //        if (attempts > 50000)
            //        {
            //            Debug.LogWarning("Infinite loop");
            //            break;
            //        }
            //    }
            //});
        }

        public static async Task PropagateAmbientLightAsync(Queue<LightNode> lightBfsQueue, HashSet<Chunk> chunksNeedUpdate)
        {
            //Debug.Log("Propagate ambient light");
            int attempts = 0;
            Main main = Main.Instance;
            Vector3Int[] neighbors = new Vector3Int[6];

            LightNode startNode = lightBfsQueue.Peek();
            main.SetAmbientLight(startNode.GlobalPosition, startNode.Intensity);

            Chunk currentTargetChunk;
            main.TryGetChunk(startNode.GlobalPosition, out currentTargetChunk);
            chunksNeedUpdate.Add(currentTargetChunk);

            await Task.Run(() =>
            {
                while (lightBfsQueue.Count > 0)
                {
                    LightNode currentNode = lightBfsQueue.Dequeue();
                    GetVoxelNeighborPosition(currentNode.GlobalPosition, ref neighbors);
                    for (int i = 0; i < neighbors.Length; i++)
                    {
                        if (!main.InSideChunkBound(currentTargetChunk, neighbors[i]))
                        {
                            if (main.TryGetChunk(neighbors[i], out currentTargetChunk))
                            {
                                if (!chunksNeedUpdate.Contains(currentTargetChunk))
                                {
                                    chunksNeedUpdate.Add(currentTargetChunk);
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }

                        Vector3Int relativePos = currentTargetChunk.GetRelativePosition(neighbors[i]);
                        BlockType currentBlock = currentTargetChunk.GetBlock(relativePos);
                        byte blockOpacity;
                        if (currentBlock == BlockType.Air && i == 5)
                        {
                            blockOpacity = 0;
                        }
                        else
                        {
                            blockOpacity = LightUtils.BlocksLightResistance[(byte)currentBlock];
                        }
                        //byte blockOpacity = LightUtils.BlocksLightResistance[(byte)currentBlock];

                        if (currentTargetChunk.GetAmbientLight(relativePos) + blockOpacity < currentNode.Intensity && currentNode.Intensity > 0)
                        {
                            LightNode neighborNode = new LightNode(neighbors[i], (byte)(currentNode.Intensity - blockOpacity));
                            lightBfsQueue.Enqueue(neighborNode);
                            currentTargetChunk.SetAmbientLight(relativePos, neighborNode.Intensity);
                        }
                    }

                    attempts++;
                    if (attempts > 10000)
                    {
                        Debug.LogWarning("Infinite loop");
                        break;
                    }
                }
            });
        }
        public static async Task RemoveAmbientLightAsync(Queue<LightNode> removeLightBfsQueue, HashSet<Chunk> chunksNeedUpdate)
        {
            Main main = Main.Instance;
            main.SetAmbientLight(removeLightBfsQueue.Peek().GlobalPosition, 0);
            Queue<LightNode> spreadLightBfsQueue = new Queue<LightNode>();
            Dictionary<Vector3Int, byte> spreadLightDict = new Dictionary<Vector3Int, byte>();
            int attempts = 0;
            Vector3Int[] neighbors = new Vector3Int[6];


            Chunk currentTargetChunk;
            main.TryGetChunk(removeLightBfsQueue.Peek().GlobalPosition, out currentTargetChunk);
            chunksNeedUpdate.Add(currentTargetChunk);


            await Task.Run(() =>
            {
                while (removeLightBfsQueue.Count > 0)
                {
                    LightNode currentNode = removeLightBfsQueue.Dequeue();

                    GetVoxelNeighborPosition(currentNode.GlobalPosition, ref neighbors);
                    for (int i = 0; i < neighbors.Length; i++)
                    {
                        if (!main.InSideChunkBound(currentTargetChunk, neighbors[i]))
                        {
                            if (main.TryGetChunk(neighbors[i], out currentTargetChunk))
                            {
                                if (!chunksNeedUpdate.Contains(currentTargetChunk))
                                {
                                    chunksNeedUpdate.Add(currentTargetChunk);
                                }
                            }
                        }
                        Vector3Int relativePos = currentTargetChunk.GetRelativePosition(neighbors[i]);
                        if (currentTargetChunk.GetAmbientLight(relativePos) == 0)
                        {
                            continue;
                        }


                        BlockType currentBlock = currentTargetChunk.GetBlock(relativePos);
                        byte blockOpacity = LightUtils.BlocksLightResistance[(byte)currentBlock];

                        if (currentTargetChunk.GetAmbientLight(relativePos) + blockOpacity <= currentNode.Intensity)
                        {
                            LightNode neighborNode = new LightNode(neighbors[i], (byte)(currentNode.Intensity - blockOpacity));
                            removeLightBfsQueue.Enqueue(neighborNode);
                            currentTargetChunk.SetAmbientLight(relativePos, 0);

                            if (spreadLightDict.ContainsKey(neighbors[i]))
                            {
                                spreadLightDict.Remove(neighbors[i]);
                            }
                        }
                        else
                        {
                            LightNode neighborNode = new LightNode(neighbors[i], currentTargetChunk.GetAmbientLight(relativePos));

                            if (spreadLightDict.ContainsKey(neighborNode.GlobalPosition) == false)
                            {
                                spreadLightDict.Add(neighborNode.GlobalPosition, neighborNode.Intensity);
                            }
                            else
                            {
                                spreadLightDict[neighborNode.GlobalPosition] = neighborNode.Intensity;
                            }
                        }
                    }



                    attempts++;
                    if (attempts > 10000)
                    {
                        Debug.LogWarning("Infinite loop");
                        break;
                    }
                }
            });


            foreach (var node in spreadLightDict)
            {
                spreadLightBfsQueue.Enqueue(new LightNode(node.Key, node.Value));
            }
            if (spreadLightBfsQueue.Count > 0)
            {
                await PropagateAmbientLightAsync(spreadLightBfsQueue, chunksNeedUpdate);
            }
        }
        #endregion

        #region Neighbors
        public static void GetVoxelNeighborPosition(Vector3Int position, ref Vector3Int[] neighborPosition)
        {
            neighborPosition[0] = position + new Vector3Int(1, 0, 0);
            neighborPosition[1] = position + new Vector3Int(-1, 0, 0);
            neighborPosition[2] = position + new Vector3Int(0, 0, 1);
            neighborPosition[3] = position + new Vector3Int(0, 0, -1);
            neighborPosition[4] = position + new Vector3Int(0, 1, 0);
            neighborPosition[5] = position + new Vector3Int(0, -1, 0);


            //_neighborsPosition[6] = position + new Vector3Int(-1, 0, 1);
            //_neighborsPosition[7] = position + new Vector3Int(1, 0, 1);
            //_neighborsPosition[8] = position + new Vector3Int(-1, 0, -1);
            //_neighborsPosition[9] = position + new Vector3Int(1, 0, -1);


            //return _neighborsPosition;
        }
        #endregion
    }
}
