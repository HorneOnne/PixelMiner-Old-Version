using UnityEngine;

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
    }
}