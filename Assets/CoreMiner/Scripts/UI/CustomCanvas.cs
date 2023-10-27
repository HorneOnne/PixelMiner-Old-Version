using UnityEngine;

namespace CoreMiner.UI
{
    public class CustomCanvas : MonoBehaviour
    {
        private Canvas _canvas;

        #region Properties
        public Canvas Canvas { get { return _canvas; } }
        #endregion

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
        }

        public void DisplayCanvas(bool isDisplay)
        {
            _canvas.enabled = isDisplay;
        }
    }

}
