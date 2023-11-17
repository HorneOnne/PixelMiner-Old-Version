using UnityEngine;
using UnityEngine.UI;

namespace CoreMiner.UI
{
    public class UIMapPreviewManager : MonoBehaviour
    {
        [SerializeField] private Image _heightMapImage;
        [SerializeField] private Image _heatMapImage;
        [SerializeField] private Image _moistureMapImage;


        private void Awake()
        {
            transform.Find("HeightMapPreview")?.TryGetComponent<Image>(out _heightMapImage);
            transform.Find("HeatMapPreview")?.TryGetComponent<Image>(out _heatMapImage);
            transform.Find("MoistureMapPreview")?.TryGetComponent<Image>(out _moistureMapImage);
        }


        public bool HasHeightMap() => _heightMapImage != null;
        public bool HasHeatMap() => _heatMapImage != null;
        public bool HasMoistureMap() => _moistureMapImage != null;


    }
}
