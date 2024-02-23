using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace PixelMiner.UI
{
    public class ExpandInventoryBtn : MonoBehaviour
    {
        private Button _button;
        [SerializeField] private Image _selectedImage;
        public Button Button => _button;
        private void Awake()
        {
            _button = GetComponent<Button>();   
        }
    }
}
