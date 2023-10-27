using UnityEngine;

namespace CoreMiner.UI
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
            DisplayWorldGenCanvas(false);
        }

        public void DisplayWorldGenCanvas(bool isActive)
        {
            CanvasWorldGen.DisplayCanvas(isActive);
        }
    }
}
