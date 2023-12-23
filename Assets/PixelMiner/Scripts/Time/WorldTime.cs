using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelMiner.Time
{
    public class WorldTime : MonoBehaviour
    {
        public float _secondsPerRealSecond = 1.0f; // Controls the speed of time
        private float currentTime = 0.0f;
        private bool isTimePaused = false;

        private int _minutes;
        private int _hours;
        private int _days;


        private void Update()
        {
            if (!isTimePaused)
            {
                // Update the current time based on real time
                currentTime += UnityEngine.Time.deltaTime * _secondsPerRealSecond;

                // Reset the time to start a new day
                if (currentTime > 24.0f * 60.0f * 60.0f) // Assuming 24 hours in a day
                {
                    _days++;
                    currentTime = 0.0f;
                }
            }
        }

        // Get the current time in seconds
        public float GetTimeInSeconds()
        {
            return currentTime;
        }

        // Set the current time in seconds
        public void SetTimeInSeconds(float newTime)
        {
            currentTime = newTime;
        }

        // Pause or resume the time
        public void TogglePause()
        {
            isTimePaused = !isTimePaused;
        }

        // Reset the time to the beginning of the day
        public void ResetTime()
        {
            currentTime = 0.0f;
        }
    }
}
