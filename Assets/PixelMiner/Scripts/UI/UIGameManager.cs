using UnityEngine;

namespace PixelMiner.UI
{
    public class UIGameManager : MonoBehaviour
    {
        public static UIGameManager Instance { get; private set;}
        public CanvasWorldGen CanvasWorldGen;

        private void Awake()
        {
            Instance = this;

            CanvasWorldGen = GameObject.FindAnyObjectByType<CanvasWorldGen>();
        }


        private void Start()
        {
            CloseAll();
        }

        public void CloseAll()
        {
            DisplayWorldGenSlider(false);
        }

        public void DisplayWorldGenSlider(bool isActive)
        {
            CanvasWorldGen.WorldGenSlider.gameObject.SetActive(isActive);
        }
    }
}
