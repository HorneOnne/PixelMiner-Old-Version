using PixelMiner.Enums;
using UnityEngine;
using System.Collections.Generic;

namespace PixelMiner.Core
{
    public class LightUtils : MonoBehaviour
    {
        public static LightUtils Instance { get; private set; }


        private Dictionary<BlockType, byte> _lightResistanceMap = new Dictionary<BlockType, byte>
        {
            { BlockType.Air, 10 },     
            { BlockType.DirtGrass, 150 },
            { BlockType.Dirt, 150 },
            { BlockType.Stone, 150 },

            { BlockType.Water, 30 },     
            { BlockType.Sand, 150 },
            { BlockType.Glass, 10 },   
            { BlockType.Snow, 150 },

            { BlockType.Light, 10 },  
            
            { BlockType.Wood, 150 },     
            { BlockType.Leaves, 20 },     
            { BlockType.PineLeaves, 20 },     
        };

        private Dictionary<BlockType, byte> _lightMap = new Dictionary<BlockType, byte>
        {
            { BlockType.Light, 150 }
        };


        public static byte[] BlocksLightResistance = new byte[(int)BlockType.Count];
        public static byte[] BlocksLight = new byte[(int)BlockType.Count];


        private void Awake()
        {
            Instance = this;

            // Light
            for (int i = 0; i < BlocksLight.GetLength(0); i++)
            {
                BlocksLight[i] = 0;
            }
            foreach (var b in _lightMap)
            {
                BlocksLight[(byte)b.Key] = b.Value;
            }



            // Light resistance
            for (int i = 0; i < BlocksLightResistance.GetLength(0); i++)
            {
                BlocksLightResistance[i] = 10;
            }
            foreach (var opaqueValue in _lightResistanceMap)
            {
                BlocksLightResistance[(byte)opaqueValue.Key] = opaqueValue.Value;
            }





        }



        //public static Color32 GetLightColor(byte light)
        //{
        //    //float channelValue = light / maxLight;
        //    //return new Color(channelValue, channelValue, channelValue, 1.0f);

        //    // Apply square function for a darker appearance
        //    float channelValue = Mathf.Pow(light / (float)MaxLightIntensity, 2);
        //    byte lightValue = (byte)(channelValue * 255);
        //    return new Color32(lightValue, lightValue, lightValue, 255);
        //}
    }
}
