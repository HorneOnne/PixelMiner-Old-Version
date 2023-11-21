using UnityEngine;

namespace PixelMiner.Utilities
{
    public static class Utilities
    {
        public static void ShowTimeMethodExecution(System.Action method)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            method.Invoke();
            stopwatch.Stop();
            Debug.Log($"{method.ToString()} {stopwatch.ElapsedMilliseconds / 1000f}");
        }
    }
}