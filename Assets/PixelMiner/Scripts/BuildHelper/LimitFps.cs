using UnityEngine;

namespace PixelMiner.BuildHelper
{
    public class LimitFps : MonoBehaviour
    {
        [SerializeField] private int _maxFPS = 60;
        private void Awake()
        {
            Application.targetFrameRate = _maxFPS;
        }
  
    }
}
