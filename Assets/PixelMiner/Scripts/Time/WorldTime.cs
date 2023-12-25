using UnityEngine;

namespace PixelMiner.Time
{
    public class WorldTime : MonoBehaviour
    {
        public static WorldTime Instance { get; private set; }
        public static event System.Action<int> OnMinuteChange;
        public static event System.Action<int> OnHourChange;
        public static event System.Action<int> OnDayEnd;


        public float _timeMultiplier = 1.0f; // Controls the speed of time
        [SerializeField] private float currentTime = 0.0f;
        public bool FreezeTime = false;


        [Range(0,60)] public int Minutes = 0;
        [Range(0, 24)] public int Hours = 0;
        public int Days = 0;


        private float _minutePerSecond = 0.1f;
        [SerializeField] private int _startHour = 12;


        private void Awake()
        {
            Instance = this;
            Hours = _startHour;
        }


        private void Start()
        {
            Debug.Log($"1 day = {GetRealtimeDuration(24)} s");
        }

        private void Update()
        {
            if (!FreezeTime)
            {

                currentTime += UnityEngine.Time.deltaTime * _timeMultiplier;

                // Trigger events at the end of each in-game minute, hour, and day
                if (currentTime >= _minutePerSecond) // In-game seconds in a minute
                {
                    currentTime -= _minutePerSecond;

                    Minutes++;                     
                    OnMinuteChange?.Invoke(Minutes);

                    if(Minutes == 60)
                    {
                        Minutes = 0;
                        Hours++;
                        OnHourChange?.Invoke(Hours);
                    }

                    if(Hours == 24)
                    {
                        Hours = 0;
                        Days++;
                        OnDayEnd?.Invoke(Days);
                    }
                }
            }
        }


        public float GetRealtimeDuration(float hour)
        {
            // Convert in-game hours to real-time seconds.
            float realTimeSeconds = (hour * 60 * _minutePerSecond) / _timeMultiplier;
            return realTimeSeconds;
        }
    }
}
