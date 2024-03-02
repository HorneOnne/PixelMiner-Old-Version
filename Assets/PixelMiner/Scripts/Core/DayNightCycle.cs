using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelMiner.Time;

namespace PixelMiner.Core
{
    public class DayNightCycle : MonoBehaviour
    {
        public static DayNightCycle Instance { get; private set; }  
        public static event System.Action OnTimesOfTheDayChanged;

        private WorldTime _worldTime;
        public AnimationCurve SunLightIntensityCurve;


        //public Color MorningColor;
        //public Color AfternoonColor;
        //public Color EveningColor;
        //public Color NightColor;

        //public List<Material> MaterialsEffectedByLight;
        //private Color _currentSunLightColor;
        //private Color _targetColor;

        //private Coroutine _smoothlyLightColorTransitionCoroutine;


        #region Properties
        //public Color AmbientColor { get => _currentSunLightColor; }

        #endregion

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            //WorldTime.OnHourChange += UpdateSunLightIntensityMat;
        }
        private void OnDisable()
        {
            //WorldTime.OnHourChange -= UpdateSunLightIntensityMat;
        }

        private void Start()
        {
            _worldTime = WorldTime.Instance;

            //return;
            //if (_worldTime.Hours > 5)
            //{
            //    if (_worldTime.Hours < 16)
            //        _currentSunLightColor = MorningColor;
            //    else if (_worldTime.Hours < 18)
            //        _currentSunLightColor = AfternoonColor;
            //    else if (_worldTime.Hours < 20)
            //        _currentSunLightColor = EveningColor;
            //    else
            //        _currentSunLightColor = NightColor;
            //}
            //else
            //{
            //    _currentSunLightColor = NightColor;
            //}

            //_smoothlyLightColorTransitionCoroutine = StartCoroutine(SmoothlyLightColorTransition(_currentSunLightColor, _targetColor, _worldTime.GetRealtimeDuration(0.01f)));
        }

        private void Update()
        {
            float f = _worldTime.Hours + (_worldTime.Minutes / 60f);
            //Debug.Log(CalculateSunlightIntensity(_worldTime.Hours + _worldTime.Minutes / 60f, SunLightIntensityCurve));
            Shader.SetGlobalFloat("_AmbientLightIntensity", CalculateSunlightIntensity(_worldTime.Hours + _worldTime.Minutes / 60f, SunLightIntensityCurve));

            //return;
            //UpdateSunLightIntensityMat(_worldTime.Hours + (_worldTime.Minutes / 60));              
            //if (_worldTime.Hours > 5)
            //{
            //    if (_worldTime.Hours < 16)
            //    {
            //        if (!_targetColor.Equals(MorningColor))
            //        {
            //            OnTimesOfTheDayChanged?.Invoke();
            //            _targetColor = MorningColor;
            //            StopCoroutine(_smoothlyLightColorTransitionCoroutine);
            //            _smoothlyLightColorTransitionCoroutine = StartCoroutine(SmoothlyLightColorTransition(_currentSunLightColor, _targetColor, _worldTime.GetRealtimeDuration(0.5f)));
            //        }

            //    }
            //    else if (_worldTime.Hours < 18)
            //    {
            //        if (!_targetColor.Equals(AfternoonColor))
            //        {
            //            OnTimesOfTheDayChanged?.Invoke();
            //            _targetColor = AfternoonColor;
            //            StopCoroutine(_smoothlyLightColorTransitionCoroutine);
            //            _smoothlyLightColorTransitionCoroutine = StartCoroutine(SmoothlyLightColorTransition(_currentSunLightColor, _targetColor, _worldTime.GetRealtimeDuration(0.5f)));
            //        }

            //    }
            //    else if (_worldTime.Hours < 20)
            //    {
            //        if (!_targetColor.Equals(EveningColor))
            //        {
            //            OnTimesOfTheDayChanged?.Invoke();
            //            _targetColor = EveningColor;
            //            StopCoroutine(_smoothlyLightColorTransitionCoroutine);
            //            _smoothlyLightColorTransitionCoroutine = StartCoroutine(SmoothlyLightColorTransition(_currentSunLightColor, _targetColor, _worldTime.GetRealtimeDuration(0.5f)));
            //        }

            //    }
            //    else
            //    {
            //        if (!_targetColor.Equals(NightColor))
            //        {
            //            OnTimesOfTheDayChanged?.Invoke();
            //            _targetColor = NightColor;
            //            StopCoroutine(_smoothlyLightColorTransitionCoroutine);
            //            _smoothlyLightColorTransitionCoroutine = StartCoroutine(SmoothlyLightColorTransition(_currentSunLightColor, _targetColor, _worldTime.GetRealtimeDuration(0.5f)));
            //        }
            //    }
            //}
            //else
            //{
            //    if (!_targetColor.Equals(NightColor))
            //    {
            //        OnTimesOfTheDayChanged?.Invoke();
            //        _targetColor = NightColor;
            //        StopCoroutine(_smoothlyLightColorTransitionCoroutine);
            //        _smoothlyLightColorTransitionCoroutine = StartCoroutine(SmoothlyLightColorTransition(_currentSunLightColor, _targetColor, _worldTime.GetRealtimeDuration(0.5f)));
            //    }
            //}
        }


        //private void SetSunLightColorMat(Color lightColor)
        //{
        //    _currentSunLightColor = lightColor;
        //    for (int i = 0; i < MaterialsEffectedByLight.Count; i++)
        //    {
        //        MaterialsEffectedByLight[i].SetColor("_SunLight", lightColor);
        //    }
        //}

        //private IEnumerator SmoothlyLightColorTransition(Color startColor, Color endColor, float duration)
        //{
        //    float elapsed = 0f;

        //    while (elapsed < duration)
        //    {
        //        float t = elapsed / duration;
        //        Color lerpedColor = Color.Lerp(startColor, endColor, t);
        //        SetSunLightColorMat(lerpedColor);
        //        yield return null;
        //        elapsed += UnityEngine.Time.deltaTime;
        //    }

        //    // Ensure the final color is set
        //    SetSunLightColorMat(endColor);
        //}


        //private void UpdateSunLightIntensityMat(int hour)
        //{
        //    for (int i = 0; i < MaterialsEffectedByLight.Count; i++)
        //    {
        //        MaterialsEffectedByLight[i].SetFloat("_LightIntensity", CalculateSunlightIntensity(hour + _worldTime.Minutes / 60f, SunLightIntensityCurve));
        //    }
        //}

        public float CalculateSunlightIntensity(float hour, AnimationCurve sunLightIntensityCurve)
        {
            return sunLightIntensityCurve.Evaluate(hour / 24.0f);
        }

        public float AmbientlightIntensity { get => CalculateSunlightIntensity(_worldTime.Hours + _worldTime.Minutes / 60f, SunLightIntensityCurve); }
    }
}
