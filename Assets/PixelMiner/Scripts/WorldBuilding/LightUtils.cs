using UnityEngine;

namespace PixelMiner.WorldBuilding
{
    public static class LightUtils
    {
        public static Color32 GetLightColor(byte light)
        {
            float maxLight = 16.0f;
            //float channelValue = light / maxLight;
            //return new Color(channelValue, channelValue, channelValue, 1.0f);

            // Apply square function for a darker appearance
            float channelValue = Mathf.Pow(light / maxLight, 2);
            byte lightValue = (byte)(channelValue * 255);
            return new Color32(lightValue, lightValue, lightValue, 255);
        }
    }
}
