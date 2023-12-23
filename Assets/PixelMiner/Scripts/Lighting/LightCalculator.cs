using PixelMiner.Enums;
using PixelMiner.WorldBuilding;
using System.Collections.Generic;
using UnityEngine;
using PixelMiner.Core;

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




    public class LightCalculator
    {
        //public GameObject TextPrefab;
        //private WaitForSeconds _wait = new WaitForSeconds(0.001f);
        static int maxYLevelLightSpread = 5;

        public static void PropagateLight(Queue<LightNode> lightBfsQueue)
        {
            int attempts = 0;
            if (Main.Instance.TryGetChunk(lightBfsQueue.Peek().position, out Chunk chunk) == false)
            {
                Debug.LogWarning("Chunk not found.");
                return;
            }
            chunk.SetBlockLight(lightBfsQueue.Peek().position, lightBfsQueue.Peek().val);

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

                        if (lightBfsQueue.Contains(neighborNode) == false)
                        {
                            lightBfsQueue.Enqueue(neighborNode);
                            chunk.SetBlockLight(neighborNode.position, neighborNode.val);
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
        }

        public static void RemoveLight(Queue<LightNode> removeLightBfsQueue)
        {
            Debug.Log("Remove Light");
            if (Main.Instance.TryGetChunk(removeLightBfsQueue.Peek().position, out Chunk chunk) == false)
            {
                Debug.LogWarning("Chunk not found.");
                return;
            }
            chunk.SetBlockLight(removeLightBfsQueue.Peek().position, 0);

            Queue<LightNode> spreadLightBfsQueue = new Queue<LightNode>();
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


            if(spreadLightBfsQueue.Count > 0)
            {
                PropagateLight(spreadLightBfsQueue);
            }
            
            bool IsValidPosition(Vector3Int position)
            {
                return (position.x >= 0 && position.x < chunk._width &&
                    position.z >= 0 && position.z < chunk._depth);
            }
        }


        #region Ambient Light
        public static void PropagateAmbientLight(Queue<LightNode> lightBfsQueue)
        {
            Debug.Log("PropagateAmbientLight");
            int attempts = 0;
            if (Main.Instance.TryGetChunk(lightBfsQueue.Peek().position, out Chunk chunk) == false)
            {
                Debug.LogWarning("Chunk not found.");
                return;
            }
            chunk.SetAmbientLight(lightBfsQueue.Peek().position, lightBfsQueue.Peek().val);

            while (lightBfsQueue.Count > 0)
            {
                LightNode currentNode = lightBfsQueue.Dequeue();
                Vector3Int dL = new Vector3Int(currentNode.position.x, currentNode.position.y -1, currentNode.position.z);

                if (IsValidPosition(dL) == false ||
                       dL[1] >= 10 ||
                       chunk.GetBlock(dL) != BlockType.Air) continue;

                if (chunk.GetAmbientLight(dL) + 1 < currentNode.val && currentNode.val > 0)
                {
                    LightNode neighborNode = new LightNode(dL, (byte)(currentNode.val));

                    if (lightBfsQueue.Contains(neighborNode) == false)
                    {
                        lightBfsQueue.Enqueue(neighborNode);
                        chunk.SetAmbientLight(neighborNode.position, neighborNode.val);
                    }
                }


                attempts++;
                if (attempts > 10000)
                {
                    Debug.LogWarning("Infinite loop");
                    break;
                }
            }

            Debug.Log($"Attempts: {attempts}");

            bool IsValidPosition(Vector3Int position)
            {
                return (position.x >= 0 && position.x < chunk._width &&
                    position.z >= 0 && position.z < chunk._depth);
            }
        }

        #endregion
    }
}
