using PixelMiner.Enums;
using PixelMiner.WorldBuilding;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;

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




    public class LightCalculator : MonoBehaviour
    {
        public GameObject TextPrefab;
        private Dictionary<Vector3Int, GameObject> Dict = new Dictionary<Vector3Int, GameObject>();
        private WaitForSeconds _wait = new WaitForSeconds(0.001f);
        int maxYLevelLightSpread = 5;


        public void ProcessLight(Vector3Int startPosition, Chunk chunk)
        {
            //Dict.Clear();
            Debug.Log("Process Light");
            //startPosition = new Vector3Int(16, 3, 16);


            byte maxLight = 16;
            Queue<LightNode> lightBfsQueue = new Queue<LightNode>();
            LightNode startNode = new LightNode(startPosition, maxLight);
            lightBfsQueue.Enqueue(startNode);
            chunk.SetLight(startNode.position, startNode.val);
           
            int attempts = 0;
            while (lightBfsQueue.Count > 0)
            {    
                LightNode currentNode = lightBfsQueue.Dequeue();

                //if (Dict.ContainsKey(currentNode.position) == false)
                //{
                //    var textObject = Instantiate(TextPrefab, currentNode.position + new Vector3(0, 0.1f, 0), Quaternion.Euler(90, 0, 0));
                //    textObject.GetComponent<TextMeshPro>().text = $"{chunk.GetLight(currentNode.position)}";
                //    Dict.Add(currentNode.position, textObject);
                //}
                //else
                //{
                //    Dict[currentNode.position].GetComponent<TextMeshPro>().text = $"{chunk.GetLight(currentNode.position)}";
                //}

                var neighbors = chunk.GetVoxelNeighborPosition(currentNode.position);
  
                for (int i = 0; i < neighbors.Length; i++)
                {
                    if (IsValidPosition(neighbors[i]) == false ||
                        neighbors[i][1] > maxYLevelLightSpread ||
                        chunk.GetBlock(neighbors[i]) != BlockType.Air||
                        neighbors[i] == startPosition) continue;

                    if (chunk.GetLight(neighbors[i]) + 2 <= currentNode.val && currentNode.val > 0)
                    {
                        LightNode neighborNode = new LightNode(neighbors[i], (byte)(currentNode.val - 1));

                        if (lightBfsQueue.Contains(neighborNode) == false)
                        {
                            lightBfsQueue.Enqueue(neighborNode);
                            chunk.SetLight(neighborNode.position, neighborNode.val);
                        }
                    }

                }


                attempts++;
                if (attempts > 10000)
                {
                    Debug.LogWarning("Infinite loop");
                    break;
                }
                //yield return null;
            }

            Debug.Log($"attempt: {attempts}");


            bool IsValidPosition(Vector3Int position)
            {
                return (position.x >= 0 && position.x < chunk._width &&
                    position.z >= 0 && position.z < chunk._depth);
            }
        }

        public void RemoveLight(Vector3Int startPosition, Chunk chunk)
        {
            //Dict.Clear();
            Debug.Log("Remove Light");
            //startPosition = new Vector3Int(16, 3, 16);


            byte maxLight = 16;
            Queue<LightNode> removeLightBfsQueue = new Queue<LightNode>();
            Queue<LightNode> spreadLightBfsQueue = new Queue<LightNode>();
            LightNode startNode = new LightNode(startPosition, maxLight);
            removeLightBfsQueue.Enqueue(startNode);
            chunk.SetLight(startNode.position, 0);

            int attempts = 0;
            while (removeLightBfsQueue.Count > 0)
            {
                LightNode currentNode = removeLightBfsQueue.Dequeue();

              

                //if (chunk.GetLight(currentNode.position) != 0)
                //{
                //    if (Dict.ContainsKey(currentNode.position))
                //    {
                //        Dict[currentNode.position].GetComponent<TextMeshPro>().text = $"{chunk.GetLight(currentNode.position)}";
                //    }
                //}
                //else
                //{
                //    Destroy(Dict[currentNode.position].gameObject);
                //    Dict.Remove(currentNode.position);
                //}


                var neighbors = chunk.GetVoxelNeighborPosition(currentNode.position);

                for (int i = 0; i < neighbors.Length; i++)
                {
               
                    if (IsValidPosition(neighbors[i]) == false ||
                        neighbors[i][1] > maxYLevelLightSpread ||
                        neighbors[i] == startPosition) continue;

                    if (chunk.GetLight(neighbors[i]) != 0) 
                    {         
                        if (chunk.GetLight(neighbors[i]) < currentNode.val)
                        {
                
                            LightNode neighborNode = new LightNode(neighbors[i], (byte)(currentNode.val - 1));
                            removeLightBfsQueue.Enqueue(neighborNode);
                            chunk.SetLight(neighbors[i], 0);
                        }
                        //else if (chunk.GetLight(neighbors[i]) == currentNode.val)
                        //{
                        //    Dict[neighbors[i]].GetComponent<TextMeshPro>().color = Color.green;
                        //    Debug.Log($"Equal: {currentNode.val}");
                        //}
                        else
                        {
   
                            LightNode neighborNode = new LightNode(neighbors[i], chunk.GetLight(neighbors[i]));
                            spreadLightBfsQueue.Enqueue(neighborNode);

                            //if (Dict.ContainsKey(neighborNode.position))
                            //{
                            //    Dict[neighborNode.position].GetComponent<TextMeshPro>().color = Color.red;
                            //}

                        }

                    }
                   

                }


                attempts++;
                if (attempts > 10000)
                {
                    Debug.LogWarning("Infinite loop");
                    break;
                }
                //yield return _wait;
            }


            Debug.Log($"attempt: {attempts}");
            Debug.Log($"Need spred : {spreadLightBfsQueue.Count}");

            attempts = 0;
            while (spreadLightBfsQueue.Count > 0)
            {
                LightNode currentNode = spreadLightBfsQueue.Dequeue();


                //if (Dict.ContainsKey(currentNode.position) == false)
                //{
                //    var textObject = Instantiate(TextPrefab, currentNode.position + new Vector3(0, 0.1f, 0), Quaternion.Euler(90, 0, 0));
                //    textObject.GetComponent<TextMeshPro>().text = $"{chunk.GetLight(currentNode.position)}";
                //    Dict.Add(currentNode.position, textObject);
                //}
                //else
                //{
                //    Dict[currentNode.position].GetComponent<TextMeshPro>().text = $"{chunk.GetLight(currentNode.position)}";
                //}

                //if(chunk.GetLight(currentNode.position) == 0)
                //{
                //    Destroy(Dict[currentNode.position].gameObject);
                //    Dict.Remove(currentNode.position);
                //}

                var neighbors = chunk.GetVoxelNeighborPosition(currentNode.position);

                for (int i = 0; i < neighbors.Length; i++)
                {
                    if (IsValidPosition(neighbors[i]) == false ||
                        neighbors[i][1] > maxYLevelLightSpread ||
                        chunk.GetBlock(neighbors[i]) != BlockType.Air) continue;


                    if (chunk.GetLight(neighbors[i]) < currentNode.val && currentNode.val > 0)
                    {
   

                        LightNode neighborNode = new LightNode(neighbors[i], (byte)(currentNode.val - 1));

                        if (spreadLightBfsQueue.Contains(neighborNode) == false)
                        {
                            spreadLightBfsQueue.Enqueue(neighborNode);
                            chunk.SetLight(neighborNode.position, neighborNode.val);
                        }
                    }

                }


                attempts++;
                if (attempts > 10000)
                {
                    Debug.LogWarning("Infinite loop");
                    break;
                }

                //yield return _wait;
            }


            bool IsValidPosition(Vector3Int position)
            {
                return (position.x >= 0 && position.x < chunk._width &&
                    position.z >= 0 && position.z < chunk._depth);
            }
        }
    }
}
