using PixelMiner.Enums;
using PixelMiner.World;
using System.Collections.Generic;
using UnityEngine;
using PixelMiner.Core;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace PixelMiner.Lighting
{
    /// <summary>
    /// To calculate lighting i use global position for LightNode.Position for easily calcualting light propagate cross chunk.
    /// </summary>
    public class LightCalculator 
    {
        private static Vector3Int[] _neighborsPosition = new Vector3Int[6];

        //public GameObject TextPrefab;
        //private Dictionary<Vector3Int, GameObject> Dict = new Dictionary<Vector3Int, GameObject>();
        //private WaitForSeconds _wait = new WaitForSeconds(0.2f);
        //public bool ShowDiagonal = false;
        //public float RemoveTime;
        //public float PropagateTime;

        public static async Task PropagateBlockLightAsync(Queue<LightNode> lightBfsQueue, HashSet<Chunk> chunksNeedUpdate)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

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
                        byte blockOpacity = LightUtils.BlocksOpaque[(byte)currentBlock];

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


            //Debug.Log($"Attempts: {attempts}");


            sw.Stop();
            Debug.Log($"Propagate block light: {sw.ElapsedMilliseconds / 1000f} s");
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
                            else
                            {
                                continue;
                            }
                        }
                        Vector3Int relativePos = currentTargetChunk.GetRelativePosition(neighbors[i]);
                        if (currentTargetChunk.GetBlockLight(relativePos) == 0)
                        {
                            continue;
                        }


                        BlockType currentBlock = currentTargetChunk.GetBlock(relativePos);
                        byte blockOpacity = LightUtils.BlocksOpaque[(byte)currentBlock];

                        //if (i < 6)
                        //{
                        //    blockOpacity = LightUtils.GetOpacity(currentBlock);
                        //}
                        //else
                        //{
                        //    blockOpacity = (byte)(LightUtils.GetOpacity(currentBlock) * 1.4);
                        //}

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
        public static async void PropagateAmbientLightAsync(List<LightNode> ambientLightList)
        {
            Debug.Log($"PropagateAmbientLight: {ambientLightList.Count}");
            ConcurrentQueue<LightNode> lightSpreadQueue = new ConcurrentQueue<LightNode>();

            await Task.Run(() =>
            {
                for(int i = 0; i < ambientLightList.Count; i++)
                {
                    int attempts = 0;
                    //chunk.SetAmbientLight(ambientLightList[i].GlobalPosition, ambientLightList[i].Intensity);
                    Main.Instance.SetAmbientLight(ambientLightList[i].GlobalPosition, ambientLightList[i].Intensity);
                    LightNode currentNode = ambientLightList[i];
                    //Debug.Log("AAAAA");
                    while (true)
                    {
                        Vector3Int dLPos = new Vector3Int(currentNode.GlobalPosition.x, currentNode.GlobalPosition.y - 1, currentNode.GlobalPosition.z);

                        //if (chunk.IsValidRelativePosition(dLPos) == false)
                        //{
                        //    break;
                        //}

                        if (Main.Instance.GetBlock(dLPos) == BlockType.Air || Main.Instance.GetBlock(dLPos) == BlockType.Water || Main.Instance.GetBlock(dLPos) == BlockType.Grass || Main.Instance.GetBlock(dLPos) == BlockType.TallGrass)
                        {
                            if (Main.Instance.GetAmbientLight(dLPos) + 1 < currentNode.Intensity && currentNode.Intensity > 0)
                            {
                                //Debug.Log("BBBBB");
                                //chunk.SetAmbientLight(dLPos, currentNode.Intensity);
                                Main.Instance.SetAmbientLight(dLPos, currentNode.Intensity);
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
                }

                Parallel.For(0, ambientLightList.Count, (i) =>
                {
                   
                });

            });
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
