using UnityEngine;

namespace PixelMiner.UI
{
    public class UIMapPreviewManager : MonoBehaviour
    {
        public static UIMapPreviewManager Instance { get; private set; }
        public UIMapPreview HeightMapPreview { get; private set; }
        public UIMapPreview HeatMapPreview { get; private set; }
        public UIMapPreview MoistureMapPreview { get; private set; }
 


        private void Awake()
        {
            Instance = this;

            HeightMapPreview = transform.Find("HeightMapPreview")?.GetComponent<UIMapPreview>();
            HeatMapPreview = transform.Find("HeatMapPreview")?.GetComponent<UIMapPreview>();
            MoistureMapPreview = transform.Find("MoistureMapPreview")?.GetComponent<UIMapPreview>();
        }


        public bool HasHeightMap() => HeightMapPreview != null;
        public bool HasHeatMap() => HeatMapPreview != null;
        public bool HasMoistureMap() => MoistureMapPreview != null;


        public void SetActiveHeightMap()
        {
            if (HasHeightMap()) HeightMapPreview.gameObject.SetActive(true);
            if (HasHeatMap()) HeatMapPreview.gameObject.SetActive(false);
            if (HasMoistureMap()) MoistureMapPreview.gameObject.SetActive(false);
        }

        public void SetActiveHeatMap()
        {
            if (HasHeightMap()) HeightMapPreview.gameObject.SetActive(false);
            if (HasHeatMap()) HeatMapPreview.gameObject.SetActive(true);
            if (HasMoistureMap()) MoistureMapPreview.gameObject.SetActive(false);
        }

        public void SetActiveMoistureMap()
        {
            if (HasHeightMap()) HeightMapPreview.gameObject.SetActive(false);
            if (HasHeatMap()) HeatMapPreview.gameObject.SetActive(false);
            if (HasMoistureMap()) MoistureMapPreview.gameObject.SetActive(true);
        }


        public void CloseAllMap()
        {
            if (HasHeightMap()) HeightMapPreview.gameObject.SetActive(false);
            if (HasHeatMap()) HeatMapPreview.gameObject.SetActive(false);
            if (HasMoistureMap()) MoistureMapPreview.gameObject.SetActive(false);
        }
    }
}
