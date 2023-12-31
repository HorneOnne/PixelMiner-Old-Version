using System.Collections;
using UnityEngine;

namespace PixelMiner.Miscellaneous
{
    public class PerformanceManager : MonoBehaviour
    {
        public static PerformanceManager Instance { get; private set; }
        public static System.Diagnostics.Stopwatch Stopwatch = new System.Diagnostics.Stopwatch();

        public bool ManagePerformance { get; set; }
        public int MaxUsableMilliseconds { get; set; }

        [Range(0f, 30f)]
        public int performanceMaxMilliseconds = 16;



        private void Awake()
        {
            Instance = this;
            ManagePerformance = true;
            Stopwatch.Start();
            StartCoroutine(PerformFrameManagement());
        }



        /// <summary>
        /// Returns whether the processing limit has been reached for the current frame.
        /// </summary>
        /// <returns></returns>
        public bool HitFrameLimit()
        {
            if (!ManagePerformance)
                return false;

            return performanceMaxMilliseconds - (int)Stopwatch.ElapsedMilliseconds > 0 ? false : true;
        }


        /// <summary>
        /// Infinite timer that executes at the end of each frame and manages frame time tracking setup.
        /// </summary>
        /// <returns></returns>
        private IEnumerator PerformFrameManagement()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                Stopwatch.Reset();
                Stopwatch.Start();
            }
        }
    }
}

