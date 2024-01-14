using PixelMiner.Enums;
using UnityEngine;
using System.Collections.Generic;

namespace PixelMiner.Lighting
{
    public static class LightUtils
    {
        public const int MaxLightIntensity = 150;

        private static Dictionary<BlockType, byte> _opacityMap = new Dictionary<BlockType, byte>
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
        };
        public static byte GetOpacity(BlockType blockType)
        {
            if (_opacityMap.TryGetValue(blockType, out byte opacity))
            {
                return opacity;
            }
            else
            {
                // Default opacity if the block type is not in the map
                return 15;
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
