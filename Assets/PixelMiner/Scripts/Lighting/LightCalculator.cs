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

    public struct Light
    {
        public ushort Torque;
        public byte Sun;
    }

    public struct LightRemovalNode
    {
        public int Index;
        public byte Light;
    }

    public class LightCalculator : MonoBehaviour
    {
        public GameObject TextPrefab;
        private Dictionary<Vector3Int, GameObject> Dict = new Dictionary<Vector3Int, GameObject>();
        // Direction in 3D space around block
        //private byte[] _wL;
        //private byte[] _eL;
        //private byte[] _uL;
        //private byte[] _dL;
        //private byte[] _nL;
        //private byte[] _sL;

        //private byte[] _uwL;
        //private byte[] _ueL;
        //private byte[] _unL;
        //private byte[] _usL;
        //private byte[] _dwL;
        //private byte[] _deL;
        //private byte[] _dnL;
        //private byte[] _dsL;
        //private byte[] _swL;
        //private byte[] _seL;
        //private byte[] _nwL;
        //private byte[] _neL;

        //// Corner directions
        //private byte[] _uneL;
        //private byte[] _unwL;
        //private byte[] _useL;
        //private byte[] _uswL;
        //private byte[] _dneL;
        //private byte[] _dnwL;
        //private byte[] _dseL;
        //private byte[] _dswL;

        //private Queue<LightNode> _lightOps;
        //private Queue<int> _lightBFS;

        private WaitForSeconds _wait = new WaitForSeconds(0.2f);
        int maxYLevelLightSpread = 4;

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
                //    textObject.GetComponent<TextMeshPro>().text = $"{currentNode.val}";
                //    Dict.Add(currentNode.position, textObject);
                //}
                //else
                //{
                //    Dict[currentNode.position].GetComponent<TextMeshPro>().text = $"{currentNode.val}";
                //}

                var neighbors = chunk.GetVoxelNeighborPosition(currentNode.position);
  
                for (int i = 0; i < neighbors.Length; i++)
                {
                    if (IsValidPosition(neighbors[i]) == false ||
                        neighbors[i][1] > maxYLevelLightSpread ||
                        chunk.GetBlock(neighbors[i]) != BlockType.Air||
                        neighbors[i] == startPosition) continue;

                    if (chunk.GetLight(neighbors[i]) + 1 < currentNode.val && currentNode.val > 0)
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
    }
}
