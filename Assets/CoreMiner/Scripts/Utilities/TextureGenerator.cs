using UnityEngine;
using LibNoise;
using LibNoise.Generator;

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
            //float[,] heatData = GenerateGradientNoise(textureWidth, textureHeight);
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



        public static float[,] GenerateGradientNoise(int textureWidth, int textureHeight)
        {
            float[,] heatData = new float[textureWidth, textureHeight];
            // Calculate the center of the texture
            Vector2 center = new Vector2(textureWidth / 2f, textureHeight / 2f);

            for (int x = 0; x < textureWidth; x++)
            {
                for (int y = 0; y < textureHeight; y++)
                {
                    float distance = Mathf.Abs(y - center.y);
                    float normalizedDistance = Mathf.Clamp01(distance / (textureWidth / 2f));
                    heatData[x, y] = normalizedDistance;
                }
            }

            return heatData;
        }


        public static int HeatWidth = 128;
        public static int HeatHeight = 128;
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

            int chunkFrameX = (int)(frameX * width / HeatWidth);
            int chunkFrameY = -Mathf.CeilToInt(frameY * height / HeatHeight);

            // Calculate the center of the texture with the offset
            Vector2 chunkOffset = new Vector2(chunkFrameX * HeatWidth, chunkFrameY * HeatHeight);
            Vector2 centerOffset = chunkOffset + new Vector2(frameX * width, frameY * height);
  


            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2 center = new Vector2(Mathf.FloorToInt(x / HeatWidth),Mathf.FloorToInt(y / HeatHeight)) * new Vector2(HeatWidth, HeatHeight) + new Vector2(HeatWidth / 2f, HeatHeight / 2f);
                    Vector2 centerWithOffset = center + centerOffset;

                    float distance = Mathf.Abs(y - centerWithOffset.y);
                    float normalizedDistance = Mathf.Clamp01(distance / (HeatWidth / 2f));
                    heatData[x, y] = normalizedDistance;
                }
            }

            return heatData;
        }
    }
}