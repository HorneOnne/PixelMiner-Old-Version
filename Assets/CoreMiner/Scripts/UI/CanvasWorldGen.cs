using UnityEngine.UI;

namespace CoreMiner.UI
{
    public class CanvasWorldGen : CustomCanvas
    {
        public Slider WorldGenSlider;

        public void SetWorldGenSlider(float value)
        {
            WorldGenSlider.value = value;
        }
    }
}
