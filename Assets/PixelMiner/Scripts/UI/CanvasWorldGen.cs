using UnityEngine.UI;

namespace PixelMiner.UI
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
