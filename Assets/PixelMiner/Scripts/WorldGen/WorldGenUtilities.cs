using UnityEngine;

namespace PixelMiner
{
    public static class WorldGenUtilities
    {
        [Header("Color")]
        // Height
        public static Color RiverColor = new Color(30 / 255f, 120 / 255f, 200 / 255f, 1);
        public static Color DeepColor = new Color(0, 0, 0.5f, 1);
        public static Color ShallowColor = new Color(25 / 255f, 25 / 255f, 150 / 255f, 1);
        public static Color SandColor = new Color(240 / 255f, 240 / 255f, 64 / 255f, 1);
        public static Color GrassColor = new Color(50 / 255f, 220 / 255f, 20 / 255f, 1);
        public static Color ForestColor = new Color(16 / 255f, 160 / 255f, 0, 1);
        public static Color RockColor = new Color(0.5f, 0.5f, 0.5f, 1);
        public static Color SnowColor = new Color(1, 1, 1, 1);

        // Heat
        public static Color ColdestColor = new Color(0, 1, 1, 1);
        public static Color ColderColor = new Color(170 / 255f, 1, 1, 1);
        public static Color ColdColor = new Color(0, 229 / 255f, 133 / 255f, 1);
        public static Color WarmColor = new Color(1, 1, 100 / 255f, 1);
        public static Color WarmerColor = new Color(1, 100 / 255f, 0, 1);
        public static Color WarmestColor = new Color(241 / 255f, 12 / 255f, 0, 1);

        // Moisture
        public static Color Dryest = new Color(255 / 255f, 139 / 255f, 17 / 255f, 1);
        public static Color Dryer = new Color(245 / 255f, 245 / 255f, 23 / 255f, 1);
        public static Color Dry = new Color(80 / 255f, 255 / 255f, 0 / 255f, 1);
        public static Color Wet = new Color(85 / 255f, 255 / 255f, 255 / 255f, 1);
        public static Color Wetter = new Color(20 / 255f, 70 / 255f, 255 / 255f, 1);
        public static Color Wettest = new Color(0 / 255f, 0 / 255f, 100 / 255f, 1);

        public static int GenerateNewSeed(int originalSeed)
        {
            const int LargePrime = 2147483647; // A large prime number to ensure randomness
            const int Multiplier = 31231;      // A multiplier for mixing bits

            // Perform a simple pseudo-random transformation on the original seed
            int transformedSeed = originalSeed * Multiplier % LargePrime;

            return transformedSeed;
        }


        public static float[,] BlendMapData(float[,] data01, float[,] data02, float blendFactor)
        {
            int width = data01.GetLength(0);
            int height = data02.GetLength(1);

            float[,] blendedData = new float[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    blendedData[x, y] = Mathf.Lerp(data01[x, y], data02[x, y], blendFactor);
                }
            }

            return blendedData;
        }

        public static Color GetGradientColor(float heatValue)
        {
            // predefine heat value threshold
            float ColdestValue = 0.05f;
            float ColderValue = 0.18f;
            float ColdValue = 0.4f;
            float WarmValue = 0.6f;
            float WarmerValue = 0.8f;


            if (heatValue < ColdestValue)
            {
                return ColdestColor;
            }
            else if (heatValue < ColderValue)
            {
                return ColderColor;
            }
            else if (heatValue < ColdValue)
            {
                return ColdColor;
            }
            else if (heatValue < WarmValue)
            {
                return WarmColor;
            }
            else if (heatValue < WarmerValue)
            {
                return WarmerColor;
            }
            else
            {
                return WarmestColor;
            }
        }
    }
}

