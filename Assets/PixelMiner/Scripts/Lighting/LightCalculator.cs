using PixelMiner.Enums;
using PixelMiner.World;
using System.Collections.Generic;
using UnityEngine;
using PixelMiner.Core;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections;
using TMPro;

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
    public class LightCalculator : MonoBehaviour
    {
        private static Vector3Int[] _neighborsPosition = new Vector3Int[10];
        private static Vector3Int[] _diagonalNeighborsPosition = new Vector3Int[4];
        private static Vector3Int CHUNK_VOLUME = new Vector3Int(32, 10, 32);

        //public GameObject TextPrefab;
        //private Dictionary<Vector3Int, GameObject> Dict = new Dictionary<Vector3Int, GameObject>();
        //private WaitForSeconds _wait = new WaitForSeconds(0.2f);
        //public bool ShowDiagonal = false;
        //public float RemoveTime;
        //public float PropagateTime;

        public static async Task PropagateBlockLightAsync(Queue<LightNode> lightBfsQueue, HashSet<Chunk> chunkNeedUpdate)
        {
            //Debug.Log("Propagate light");
            int attempts = 0;
            Main main = Main.Instance;
            LightNode startNode = lightBfsQueue.Peek();
            main.SetBlockLight(startNode.GlobalPosition, startNode.Intensity);


            await Task.Run(() =>
            {
                while (lightBfsQueue.Count > 0)
                {
                    LightNode currentNode = lightBfsQueue.Dequeue();


                    if (main.TryGetChunk(currentNode.GlobalPosition, out Chunk chunk))
                    {
                        if (!chunkNeedUpdate.Contains(chunk))
                        {
                            chunkNeedUpdate.Add(chunk);
                        }
                    }


                    var neighbors = GetVoxelNeighborPosition(currentNode.GlobalPosition);
                    for (int i = 0; i < neighbors.Length; i++)
                    {
                        if (neighbors[i].y > CHUNK_VOLUME[1] - 1) continue;


                        BlockType currentBlock = main.GetBlock(neighbors[i]);
                        byte blockOpacity;
                        if (i < 6)
                        {
                            blockOpacity = LightUtils.GetOpacity(currentBlock);
                        }
                        else
                        {
                            blockOpacity = (byte)(LightUtils.GetOpacity(currentBlock) * 1.4);
                        }



                        if (main.GetBlockLight(neighbors[i]) + blockOpacity < currentNode.Intensity && currentNode.Intensity > 0)
                        {
                            LightNode neighborNode = new LightNode(neighbors[i], (byte)(currentNode.Intensity - blockOpacity));
                            lightBfsQueue.Enqueue(neighborNode);
                            main.SetBlockLight(neighborNode.GlobalPosition, neighborNode.Intensity);
                        }
                    }

                    attempts++;
                    if (attempts > 10000)
                    {
                        Debug.LogWarning("Infinite loop");
                        break;
                    }
                }

                //Debug.Log($"Propagate Attempts: {attempts}");
            });

            
        }

        public static async Task RemoveBlockLightAsync(Queue<LightNode> removeLightBfsQueue, HashSet<Chunk> chunkNeedUpdate)
        {
            //Debug.Log("Remove Light");
            Main main = Main.Instance;
            main.SetBlockLight(removeLightBfsQueue.Peek().GlobalPosition, 0);
            Queue<LightNode> spreadLightBfsQueue = new Queue<LightNode>();
            Dictionary<Vector3Int, byte> spreadLightDict = new Dictionary<Vector3Int, byte>();
            int attempts = 0;



            await Task.Run(() =>
            {
                while (removeLightBfsQueue.Count > 0)
                {
                    LightNode currentNode = removeLightBfsQueue.Dequeue();
                    if (main.TryGetChunk(currentNode.GlobalPosition, out Chunk chunk))
                    {
                        if (!chunkNeedUpdate.Contains(chunk))
                        {
                            chunkNeedUpdate.Add(chunk);
                        }
                    }

                    var neighbors = GetVoxelNeighborPosition(currentNode.GlobalPosition);
                    for (int i = 0; i < neighbors.Length; i++)
                    {

                        if (main.GetBlockLight(neighbors[i]) != 0)
                        {
                        
                            BlockType currentBlock = main.GetBlock(neighbors[i]);
                            byte blockOpacity;
                            if (i < 6)
                            {
                                blockOpacity = LightUtils.GetOpacity(currentBlock);
                            }
                            else
                            {
                                blockOpacity = (byte)(LightUtils.GetOpacity(currentBlock) * 1.4);
                            }

                            if (main.GetBlockLight(neighbors[i]) + blockOpacity <= currentNode.Intensity)
                            {
                                LightNode neighborNode = new LightNode(neighbors[i], (byte)(currentNode.Intensity - blockOpacity));
                                removeLightBfsQueue.Enqueue(neighborNode);
                                main.SetBlockLight(neighbors[i], 0);

                                if (spreadLightDict.ContainsKey(neighbors[i]))
                                {
                                    spreadLightDict.Remove(neighbors[i]);
                                }
                            }
                            else
                            {
                                LightNode neighborNode = new LightNode(neighbors[i], main.GetBlockLight(neighbors[i]));


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
                    }



                    attempts++;
                    if (attempts > 10000)
                    {
                        Debug.LogWarning("Infinite loop");
                        break;
                    }
                }
            });
           

            //Debug.Log($"Remove block Attempts: {attempts}");

            foreach(var node in spreadLightDict)
            {
                spreadLightBfsQueue.Enqueue(new LightNode(node.Key, node.Value));
            }
            if (spreadLightBfsQueue.Count > 0)
            {
                await PropagateBlockLightAsync(spreadLightBfsQueue, chunkNeedUpdate);
            }
        }


        #region Ambient Light
        public static async void PropagateAmbientLight(Chunk chunk, List<LightNode> ambientLightList)
        {
            //Debug.Log("PropagateAmbientLight");
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


            _neighborsPosition[6] = position + new Vector3Int(-1, 0, 1);
            _neighborsPosition[7] = position + new Vector3Int(1, 0, 1);
            _neighborsPosition[8] = position + new Vector3Int(-1, 0, -1);
            _neighborsPosition[9] = position + new Vector3Int(1, 0, -1);


            return _neighborsPosition;
        }
        #endregion
    }
}
