using UnityEngine;

namespace CoreMiner
{
    public static class WorldGenUtilities
    {
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
    }
}

