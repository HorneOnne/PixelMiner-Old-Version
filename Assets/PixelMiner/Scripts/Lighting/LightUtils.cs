using UnityEngine;

namespace PixelMiner.Lighting
{
    public static class LightUtils
    {
        public const int MaxLightIntensity = 15;

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
            return sunLightIntensityCurve.Evaluate(hour/24.0f);
        }
    }
}
