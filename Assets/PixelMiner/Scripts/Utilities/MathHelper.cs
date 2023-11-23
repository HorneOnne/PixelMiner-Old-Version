using System;
using UnityEngine;

namespace PixelMiner.Utilities
{
    public static class MathHelper
    {
        public static readonly Vector2 LeftVector = Vector2.left;
        public static readonly Vector2 RightVector = Vector2.right;
        public static readonly Vector2 UpVector = Vector2.up;
        public static readonly Vector2 DownVector = Vector2.down;
        public static readonly Vector2 UpLeftVector = new Vector2(-1,1);
        public static readonly Vector2 UpRightVector = new Vector2(1,1);
        public static readonly Vector2 DownLeftVector = new Vector2(-1,-1);
        public static readonly Vector2 DownRightVector = new Vector2(1,-1);

        public static double Clamp(double v, double l, double h)
        {
            if (v < l) v = l;
            if (v > h) v = h;
            return v;
        }

        public static double Lerp(double t, double a, double b)
        {
            return a + t * (b - a);
        }

        public static double QuinticBlend(double t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        public static double Bias(double b, double t)
        {
            return Math.Pow(t, Math.Log(b) / Math.Log(0.5));
        }

        public static double Gain(double g, double t)
        {
            if (t < 0.5)
            {
                return Bias(1.0 - g, 2.0 * t) / 2.0;
            }
            else
            {
                return 1.0 - Bias(1.0 - g, 2.0 - 2.0 * t) / 2.0;
            }
        }

        public static int Mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }


        /// <summary>
        /// Maps a value from one range to another.
        /// </summary>
        /// <param name="value">The value to be mapped.</param>
        /// <param name="fromMin">The minimum value of the source range.</param>
        /// <param name="fromMax">The maximum value of the source range.</param>
        /// <param name="toMin">The minimum value of the target range.</param>
        /// <param name="toMax">The maximum value of the target range.</param>
        /// <returns>The mapped value within the target range.</returns>
        public static float Map(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            value = Mathf.Clamp(value, fromMin, fromMax);
            return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
        }


        /// <summary>
        /// Rounds the input to the nearest 0.5.
        /// </summary>
        /// <param name="input">The input float to be rounded.</param>
        /// <returns>The rounded float.</returns>
        public static float RoundToNearest(float input)
        {
            float roundedValue = Mathf.Round(input * 2) / 2f;
            return roundedValue;
        }
        /// <summary>
        /// Rounds each component of a Vector2 to the nearest 0.5.
        /// </summary>
        /// <param name="v">The input Vector2 to be rounded.</param>
        /// <returns>The Vector2 with each component rounded to the nearest 0.5.</returns>
        public static Vector2 RoundToNearest(Vector2 v)
        {
            return new Vector2(RoundToNearest(v.x), RoundToNearest(v.y));
        }

        public static float SqrtDistance(Vector2 point1, Vector2 point2)
        {
            float deltaX = point1.x - point2.x;
            float deltaY = point1.y - point2.y;
            return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }

    }
}