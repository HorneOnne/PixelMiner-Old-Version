using UnityEngine;
using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;
using UnityEditor;

namespace CoreMiner.Utilities.NoiseGeneration
{
    public static class TextureGenerator
    {
        private static Color DeepColor = new Color(0, 0, 0.5f, 1);
        private static Color ShallowColor = new Color(25 / 255f, 25 / 255f, 150 / 255f, 1);
        private static Color SandColor = new Color(240 / 255f, 240 / 255f, 64 / 255f, 1);
        private static Color GrassColor = new Color(50 / 255f, 220 / 255f, 20 / 255f, 1);
        private static Color ForestColor = new Color(16 / 255f, 160 / 255f, 0, 1);
        private static Color RockColor = new Color(0.5f, 0.5f, 0.5f, 1);
        private static Color SnowColor = new Color(1, 1, 1, 1);

        private static Color Coldest = new Color(0, 1, 1, 1);
        private static Color Colder = new Color(170 / 255f, 1, 1, 1);
        private static Color Cold = new Color(0, 229 / 255f, 133 / 255f, 1);
        private static Color Warm = new Color(1, 1, 100 / 255f, 1);
        private static Color Warmer = new Color(1, 100 / 255f, 0, 1);
        private static Color Warmest = new Color(241 / 255f, 12 / 255f, 0, 1);

        private static Color Dryest = new Color(255 / 255f, 139 / 255f, 17 / 255f, 1);
        private static Color Dryer = new Color(245 / 255f, 245 / 255f, 23 / 255f, 1);
        private static Color Dry = new Color(80 / 255f, 255 / 255f, 0 / 255f, 1);
        private static Color Wet = new Color(85 / 255f, 255 / 255f, 255 / 255f, 1);
        private static Color Wetter = new Color(20 / 255f, 70 / 255f, 255 / 255f, 1);
        private static Color Wettest = new Color(0 / 255f, 0 / 255f, 100 / 255f, 1);

        public static Texture2D GetTexture(int width, int height, Tile[,] tiles)
        {
            var texture = new Texture2D(width, height);
            var pixels = new Color[width * height];

            float DeepWater = 0.2f;
            float ShallowWater = 0.4f;
            float Sand = 0.5f;
            float Grass = 0.7f;
            float Forest = 0.8f;
            float Rock = 0.9f;
            float Snow = 1;

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    float value = tiles[x, y].HeightValue;

                    //Set color range, 0 = black, 1 = white
                    if (value < DeepWater)
                        pixels[x + y * width] = DeepColor;
                    else if (value < ShallowWater)
                        pixels[x + y * width] = ShallowColor;
                    else if (value < Sand)
                        pixels[x + y * width] = SandColor;
                    else if (value < Grass)
                        pixels[x + y * width] = GrassColor;
                    else if (value < Forest)
                        pixels[x + y * width] = ForestColor;
                    else if (value < Rock)
                        pixels[x + y * width] = RockColor;
                    else if (value < Snow)
                        pixels[x + y * width] = SnowColor;


                    //pixels[x + y * width] = Color.Lerp(Color.black, Color.white, value);
                }
            }

            texture.SetPixels(pixels);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.Apply();
            return texture;
        }




        public static Texture2D GenerateGradientTexture(int width, int height)
        {
            var texture = new Texture2D(width, height);
            var pixels = new Color[width * height];

            int coldestStart = (int)(0.05 * height); // 5% from the bottom
            int colderStart = (int)(0.15 * height);  // 15% from the bottom
            int warmStart = (int)(0.30 * height);    // 30% from the bottom
            int warmerStart = (int)(0.45 * height);  // 45% from the bottom

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    //pixels[x + y * width] = GetGradientColor(x, y, width, height);
                }
            }

            texture.SetPixels(pixels);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.Apply();
            return texture;
        }

        private static Color GetGradientColor(float heatValue)
        {
            float ColdestValue = 0.05f;
            float ColderValue = 0.18f;
            float ColdValue = 0.4f;
            float WarmValue = 0.6f;
            float WarmerValue = 0.8f;


            if (heatValue < ColdestValue)
            {
                return Coldest;
            }
            else if (heatValue < ColderValue)
            {
                return Colder;
            }
            else if (heatValue < ColdValue)
            {
                return Cold;
            }
            else if (heatValue < WarmValue)
            {
                return Warm;
            }
            else if (heatValue < WarmerValue)
            {
                return Warmer;
            }
            else
            {
                return Warmest;
            }
        }

        public static Texture2D GenerateNoiseGradient(int width, int height, float offsetX, float offsetY)
        {
            Texture2D gradientTexture = new Texture2D(width, height);
            float[,] heatData = GenerateGradientNoiseOffsetInfinite(width, height,offsetX, offsetY);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color color = GetGradientColor(heatData[x, y]);
                    gradientTexture.SetPixel(x, y, color);
                }
            }


            // Apply the texture
            gradientTexture.Apply();
            return gradientTexture;
        }

        public static Texture2D GenerateMoistureTexture(int width, int height)
        {
            var texture = new Texture2D(width, height);
            var pixels = new Color[width * height];
            float[,] moistureData = GetMoistureValues(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float moistureValue = moistureData[x, y];
                    if (moistureValue < DryerValue)
                    {
                        pixels[x + y * width] = Dryest;
                    }
                    else if (moistureValue < DryValue)
                    {
                        pixels[x + y * width] = Dryer;
                    }
                    else if (moistureValue < WetValue)
                    {
                        pixels[x + y * width] = Dry;
                    }
                    else if (moistureValue < WetterValue)
                    {
                        pixels[x + y * width] = Wet;
                    }
                    else if (moistureValue < WettestValue)
                    {
                        pixels[x + y * width] = Wetter;
                    }
                    else 
                    {
                        pixels[x + y * width] = Wettest;
                    }
                }
            }


            texture.SetPixels(pixels);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.Apply();
            return texture;
        }




        public static int HeatWidth = 256;
        public static int HeatHeight = 256;
        public static int HeatOctaves = 4;
        public static double HeatFrequency = 0.025;
        public static double HeatLacunarity = 2.0f;
        public static double HeatPersistence = 0.5f;
        public static int HeatSeed = 7;
        private static ModuleBase _heatModule;
        public static float[,] GenerateGradientNoiseOffset(int width, int height, float offsetX = 0, float offsetY = 0, int chunkFrameX = 0, int chunkFrameY = 0)
        {
            float[,] heatData = new float[width, height];
            

            // Calculate the center of the texture with the offset
            Vector2 center =  new Vector2(chunkFrameX * HeatWidth, chunkFrameY * HeatHeight) +  new Vector2(HeatWidth / 2f, HeatHeight / 2f);
            Vector2 centerOffset = center + new Vector2(offsetX, offsetY);
            Debug.Log($"offse: {offsetX} {offsetY} \t center: {center} \t {centerOffset}");

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float distance = Mathf.Abs(y - centerOffset.y);
                    float normalizedDistance = Mathf.Clamp01(distance / (HeatWidth / 2f));
                    heatData[x, y] = normalizedDistance;
                }
            }

            return heatData;
        }

        public static float[,] GenerateGradientNoiseOffsetInfinite(int width, int height, float frameX = 0, float frameY = 0)
        {
            float[,] heatData = new float[width, height];
            _heatModule = new Perlin(HeatFrequency, HeatLacunarity, HeatPersistence, HeatOctaves, HeatSeed, QualityMode.Medium);
            ScaleBias scaleBiasModule = new ScaleBias(0.5f, 0.5f, _heatModule);

            int chunkFrameX = (int)(frameX * width / HeatWidth);
            int chunkFrameY = -Mathf.CeilToInt(frameY * height / HeatHeight);

            // Calculate the center of the texture with the offset
            Vector2 chunkOffset = new Vector2(chunkFrameX * HeatWidth, chunkFrameY * HeatHeight);
            Vector2 centerOffset = chunkOffset + new Vector2(frameX * width, frameY * height);

            float min = float.MaxValue;
            float max = float.MinValue;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2 center = new Vector2(Mathf.FloorToInt(x / HeatWidth), Mathf.FloorToInt(y / HeatHeight)) * new Vector2(HeatWidth, HeatHeight) + new Vector2(HeatWidth / 2f, HeatHeight / 2f);
                    Vector2 centerWithOffset = center + centerOffset;

                    float distance = Mathf.Abs(y - centerWithOffset.y);
                    float normalizedDistance = 1.0f - Mathf.Clamp01(distance / (HeatWidth / 2f));
                    heatData[x, y] = normalizedDistance;

                    //heatData[x, y] = ((float)scaleBiasModule.GetValue(x, 0, y));
                    heatData[x, y] = ((float)scaleBiasModule.GetValue(x, 0, y) + 1.0f) * 0.5f * normalizedDistance;

                    if (min > heatData[x, y])
                    {
                        min = heatData[x, y];
                    }
                    if (max < heatData[x, y])
                    {
                        max = heatData[x, y];
                    }
                }
            }

            Debug.Log($"Min: {min}");
            Debug.Log($"Max: {max}");
            return heatData;
        }


        public static int MoistureOctaves = 4;
        public static double MoistureFrequency = 0.025;
        public static double MoistureLacunarity = 2.0f;
        public static double MoisturePersistence = 0.5f;
        public static int MoistureSeed = 7;
        private static ModuleBase _moistureModule;
        [Header("Moisture Map")]
        private static float DryerValue = 0.27f;
        private static float DryValue = 0.4f;
        private static float WetValue = 0.6f;
        private static float WetterValue = 0.8f;
        private static float WettestValue = 0.9f;
        public static float[,] GetMoistureValues(int width, int height)
        {
            float[,] moistureData = new float[width, height];
            _moistureModule = new Perlin(MoistureFrequency, MoistureLacunarity, MoisturePersistence, MoistureOctaves, MoistureSeed, QualityMode.High);
            ScaleBias scaleBiasModule = new ScaleBias(0.5f, 0.5f, _moistureModule);

            float min = float.MaxValue;
            float max = float.MinValue;

            float min2 = float.MaxValue;
            float max2 = float.MinValue;

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    moistureData[x,y] = (float)_moistureModule.GetValue(x, 0, y);

                    if (min > moistureData[x, y])
                    {
                        min = moistureData[x, y];
                    }
                    if (max < moistureData[x, y])
                    {
                        max = moistureData[x, y];
                    }
                }
            }

            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    float normalizevalue = (moistureData[x,y] - min) / (max - min);
                    moistureData[x, y] = normalizevalue;

                    if (min2 > moistureData[x, y])
                    {
                        min2 = moistureData[x, y];
                    }
                    if (max2 < moistureData[x, y])
                    {
                        max2 = moistureData[x, y];
                    }
                }
            }


            Debug.Log($"Min: {min}");
            Debug.Log($"Max: {max}");
            Debug.Log($"Min2: {min2}");
            Debug.Log($"Max2: {max2}");

            return moistureData;
        }
    }
}