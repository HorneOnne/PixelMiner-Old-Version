using PixelMiner.Enums;
using UnityEngine;
using System.Collections.Generic;

namespace PixelMiner.Lighting
{
    public static class LightUtils
    {
        public const int MaxLightIntensity = 15;

        private static Dictionary<BlockType, byte> _opacityMap = new Dictionary<BlockType, byte>
        {
            { BlockType.Air, 1 },     
            { BlockType.GrassTop, 15 },  
            { BlockType.GrassSide, 15 },
            { BlockType.Dirt, 15 },
            { BlockType.Stone, 15 },

            { BlockType.Water, 3 },     
            { BlockType.Sand, 15 },
            { BlockType.Glass, 1 },   
            { BlockType.Snow, 15 },

            { BlockType.Light, 0 },     
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
            float maxLight = 15.0f;
            //float channelValue = light / maxLight;
            //return new Color(channelValue, channelValue, channelValue, 1.0f);

            // Apply square function for a darker appearance
            float channelValue = Mathf.Pow(light / maxLight, 2);
            byte lightValue = (byte)(channelValue * 255);
            return new Color32(lightValue, lightValue, lightValue, 255);
        }

        public static float CalculateSunlightIntensity(float hour, AnimationCurve sunLightIntensityCurve)
        {
            return sunLightIntensityCurve.Evaluate(hour / 24.0f);
        }
    }
}
