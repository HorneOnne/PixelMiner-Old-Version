using UnityEngine;

namespace PixelMiner.WorldBuilding
{
    internal static class WorldGenUtilities
    {
        public static int GenerateNewSeed(int originalSeed)
        {
            const int LargePrime = 2147483647; // A large prime number to ensure randomness
            const int Multiplier = 31231;      // A multiplier for mixing bits

            // Perform a simple pseudo-random transformation on the original seed
            int transformedSeed = originalSeed * Multiplier % LargePrime;

            return transformedSeed;
        }


        public static float[] BlendMapData(float[] data01, float[] data02, float blendFactor)
        {
            int size = data01.Length;
      

            float[] blendedData = new float[size];

            for(int i = 0; i < size; i++)
            {
                blendedData[i] = Mathf.Lerp(data01[i], data02[i], blendFactor);
            }
            return blendedData;
        }

       

        public static int StringToSeed(string input)
        {
            // Check if the input consists only of digits and has a length of 10
            if (input.Length == 10 && int.TryParse(input, out int intValue))
            {
                return intValue; // Return the parsed integer value
            }
            else
            {
                // If the input is not a 10-digit number, use GetHashCode() as before
                int hash = input.GetHashCode();

                // Ensure the hash value is non-negative (GetHashCode() may return a negative value)
                int seedValue = hash & int.MaxValue;

                return seedValue;
            }
        }

        public static int IndexOf(int x, int y, int z, int width, int height)
        {
            return x + width * (y + height * z);
        }

        public static int IndexOf(int x, int y, int width)
        {
            return x + y * width;
        }

        public static void CoordinatesOf(int index, int width, int height, out int x, out int y, out int z)
        {
            x = index % width;
            y = (index / width) % height;
            z = index / (width * height);
        }

        public static void CoordinatesOf(int index, byte width, out int x, out int y)
        {
            x = index % width;
            y = index / width;
        }
    }
}

