using PixelMiner.Enums;
using UnityEngine;
using System.Collections.Generic;

namespace PixelMiner.Lighting
{
    public class LightUtils : MonoBehaviour
    {
        public static LightUtils Instance { get; private set; }
        public const int MaxLightIntensity = 150;

        private Dictionary<BlockType, byte> _opacityMap = new Dictionary<BlockType, byte>
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
            { BlockType.Leaves, 50 },     
        };

        public static byte[] BlocksOpaque = new byte[256];


        private void Awake()
        {
            Instance = this;


            for (int i = 0; i < BlocksOpaque.GetLength(0); i++)
            {
                BlocksOpaque[i] = 10;
            }

            foreach (var opaqueValue in _opacityMap)
            {
                BlocksOpaque[(byte)opaqueValue.Key] = opaqueValue.Value;
            }
        }



        public static Color32 GetLightColor(byte light)
        {
            //float channelValue = light / maxLight;
            //return new Color(channelValue, channelValue, channelValue, 1.0f);

            // Apply square function for a darker appearance
            float channelValue = Mathf.Pow(light / (float)MaxLightIntensity, 2);
            byte lightValue = (byte)(channelValue * 255);
            return new Color32(lightValue, lightValue, lightValue, 255);
        }
    }
}
