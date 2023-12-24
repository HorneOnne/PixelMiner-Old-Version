using PixelMiner.Enums;
using PixelMiner.WorldBuilding;
using System.Collections.Generic;
using UnityEngine;
using PixelMiner.Core;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using TMPro;

namespace PixelMiner.Lighting
{
    public struct LightNode
    {
        public Vector3Int position;
        public byte val;

        public LightNode(Vector3Int position, byte val)
        {
            this.position = position;
            this.val = val;
        }
    }

    public struct LightRemovalNode
    {
        public Vector3Int Position;
        public byte Light;
    }

    public struct Light
    {
        public ushort Torque;
        public byte Sun;
    }

    public enum LightType
    {
        Ambient, Block
    }



    public class LightCalculator
    {
        //public GameObject TextPrefab;
        //private ConcurrentDictionary<Vector3Int, int> Dict = new ConcurrentDictionary<Vector3Int, int>();
        //private WaitForSeconds _wait = new WaitForSeconds(0.001f);
        static int maxYLevelLightSpread = 9;

        public static async Task PropagateLightAsync(Queue<LightNode> lightBfsQueue)
        {
            Debug.Log("Propagate light");
            int attempts = 0;
            if (Main.Instance.TryGetChunk(lightBfsQueue.Peek().position, out Chunk chunk) == false)
            {
                Debug.LogWarning("Chunk not found.");
                return;
            }
            chunk.SetBlockLight(lightBfsQueue.Peek().position, lightBfsQueue.Peek().val);


            await Task.Run(() =>
            {
                while (lightBfsQueue.Count > 0)
                {
                    LightNode currentNode = lightBfsQueue.Dequeue();
                    var neighbors = chunk.GetVoxelNeighborPosition(currentNode.position);


                    for (int i = 0; i < neighbors.Length; i++)
                    {
                        if (IsValidPosition(neighbors[i]) == false ||
                        neighbors[i][1] > maxYLevelLightSpread ||
                        chunk.GetBlock(neighbors[i]) != BlockType.Air) continue;

                        if (chunk.GetBlockLight(neighbors[i]) + 2 <= currentNode.val && currentNode.val > 0)
                        {
                            LightNode neighborNode = new LightNode(neighbors[i], (byte)(currentNode.val - 1));
                            lightBfsQueue.Enqueue(neighborNode);
                            chunk.SetBlockLight(neighborNode.position, neighborNode.val);
                        }
                    }


                    attempts++;
                    if (attempts > 10000)
                    {
                        Debug.LogWarning("Infinite loop");
                        break;
                    }
                }

                //Debug.Log($"Attempts: {attempts}");
                bool IsValidPosition(Vector3Int position)
                {
                    return (position.x >= 0 && position.x < chunk._width &&
                        position.z >= 0 && position.z < chunk._depth);
                }
            });

        }

        public static async Task RemoveLightAsync(Queue<LightNode> removeLightBfsQueue)
        {
            Debug.Log("Remove Light");

            if (Main.Instance.TryGetChunk(removeLightBfsQueue.Peek().position, out Chunk chunk) == false)
            {
                Debug.LogWarning("Chunk not found.");
                return;
            }
            chunk.SetBlockLight(removeLightBfsQueue.Peek().position, 0);
            Queue<LightNode> spreadLightBfsQueue = new Queue<LightNode>();

            await Task.Run(() =>
            {
                int attempts = 0;
                while (removeLightBfsQueue.Count > 0)
                {
                    LightNode currentNode = removeLightBfsQueue.Dequeue();
                    var neighbors = chunk.GetVoxelNeighborPosition(currentNode.position);


                    for (int i = 0; i < neighbors.Length; i++)
                    {
                        if (IsValidPosition(neighbors[i]) == false ||
                       neighbors[i][1] > maxYLevelLightSpread) continue;

                        if (chunk.GetBlockLight(neighbors[i]) != 0)
                        {
                            if (chunk.GetBlockLight(neighbors[i]) < currentNode.val)
                            {
                                LightNode neighborNode = new LightNode(neighbors[i], (byte)(currentNode.val - 1));
                                removeLightBfsQueue.Enqueue(neighborNode);
                                chunk.SetBlockLight(neighbors[i], 0);
                            }
                            else
                            {

                                LightNode neighborNode = new LightNode(neighbors[i], chunk.GetBlockLight(neighbors[i]));
                                spreadLightBfsQueue.Enqueue(neighborNode);
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

                bool IsValidPosition(Vector3Int position)
                {
                    return (position.x >= 0 && position.x < chunk._width &&
                        position.z >= 0 && position.z < chunk._depth);
                }
            });


            if (spreadLightBfsQueue.Count > 0)
            {
                await PropagateLightAsync(spreadLightBfsQueue);
            }
        }


        #region Ambient Light
        public static async void PropagateAmbientLight(List<LightNode> ambientLightList)
        {
            Debug.Log("PropagateAmbientLight");
            ConcurrentQueue<LightNode> lightSpreadQueue = new ConcurrentQueue<LightNode>();


            await Task.Run(() =>
            {
                Parallel.For(0, ambientLightList.Count, (i) =>
                {
                    int attempts = 0;
                    if (Main.Instance.TryGetChunk(ambientLightList[i].position, out Chunk chunk) == false)
                    {
                        Debug.LogWarning("Chunk not found.");
                        return;
                    }
                    chunk.SetAmbientLight(ambientLightList[i].position, ambientLightList[i].val);
                    LightNode currentNode = ambientLightList[i];

                    //if(Dict.ContainsKey(currentNode.position) == false)
                    //{
                    //    Dict.TryAdd(currentNode.position, currentNode.val);
                    //}

                    while (true)
                    {
                        Vector3Int dLPos = new Vector3Int(currentNode.position.x, currentNode.position.y - 1, currentNode.position.z);

                        if (IsValidPosition(dLPos) == false)
                        {
                            break;
                        }

                        if (chunk.GetBlock(dLPos) == BlockType.Air)
                        {
                            if (chunk.GetAmbientLight(dLPos) + 1 < currentNode.val && currentNode.val > 0)
                            {
                                chunk.SetAmbientLight(dLPos, currentNode.val);
                                //if (Dict.ContainsKey(dLPos) == false)
                                //{
                                //    Dict.TryAdd(dLPos, currentNode.val);
                                //}

                                currentNode = new LightNode(dLPos, currentNode.val);


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

                    bool IsValidPosition(Vector3Int position)
                    {
                        return (position.x >= 0 && position.x < chunk._width &&
                            position.z >= 0 && position.z < chunk._depth);
                    }
                });
            });
        }

        #endregion
    }
}
