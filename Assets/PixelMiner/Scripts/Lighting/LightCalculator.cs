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
        //public GameObject TextPrefab;
        //private ConcurrentDictionary<Vector3Int, int> Dict = new ConcurrentDictionary<Vector3Int, int>();
        //private WaitForSeconds _wait = new WaitForSeconds(0.001f);

        public static async Task PropagateBlockLightAsync(Queue<LightNode> lightBfsQueue)
        {
            Debug.Log("Propagate light");
            int attempts = 0;
            if (Main.Instance.TryGetChunk(lightBfsQueue.Peek().GlobalPosition, out Chunk chunk) == false)
            {
                Debug.LogWarning("Chunk not found.");
                return;
            }
            chunk.SetBlockLight(lightBfsQueue.Peek().GlobalPosition, lightBfsQueue.Peek().Intensity);


            await Task.Run(() =>
            {
                while (lightBfsQueue.Count > 0)
                {
                    LightNode currentNode = lightBfsQueue.Dequeue();
                    var neighbors = chunk.GetVoxelNeighborPosition(currentNode.GlobalPosition);


                    for (int i = 0; i < neighbors.Length; i++)
                    {
                        if (IsValidPosition(neighbors[i]) == false ||
                        chunk.GetBlock(neighbors[i]) != BlockType.Air) continue;

                        if (chunk.GetBlockLight(neighbors[i]) + 2 <= currentNode.Intensity && currentNode.Intensity > 0)
                        {
                            LightNode neighborNode = new LightNode(neighbors[i], (byte)(currentNode.Intensity - 1));
                            lightBfsQueue.Enqueue(neighborNode);
                            chunk.SetBlockLight(neighborNode.GlobalPosition, neighborNode.Intensity);
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

        public static async Task RemoveBlockLightAsync(Queue<LightNode> removeLightBfsQueue)
        {
            Debug.Log("Remove Light");

            if (Main.Instance.TryGetChunk(removeLightBfsQueue.Peek().GlobalPosition, out Chunk chunk) == false)
            {
                Debug.LogWarning("Chunk not found.");
                return;
            }
            chunk.SetBlockLight(removeLightBfsQueue.Peek().GlobalPosition, 0);
            Queue<LightNode> spreadLightBfsQueue = new Queue<LightNode>();

            await Task.Run(() =>
            {
                int attempts = 0;
                while (removeLightBfsQueue.Count > 0)
                {
                    LightNode currentNode = removeLightBfsQueue.Dequeue();
                    var neighbors = chunk.GetVoxelNeighborPosition(currentNode.GlobalPosition);


                    for (int i = 0; i < neighbors.Length; i++)
                    {
                        if (IsValidPosition(neighbors[i]) == false) continue;

                        if (chunk.GetBlockLight(neighbors[i]) != 0)
                        {
                            if (chunk.GetBlockLight(neighbors[i]) < currentNode.Intensity)
                            {
                                LightNode neighborNode = new LightNode(neighbors[i], (byte)(currentNode.Intensity - 1));
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
                await PropagateBlockLightAsync(spreadLightBfsQueue);
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
    }
}
