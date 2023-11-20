using UnityEngine;
using UnityEngine.UI;

namespace PixelMiner.UI
{
    public class UIMapPreview : MonoBehaviour
    {
        private Image _image;

        
        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void SetImage(Texture2D texture)
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
            _image.sprite = sprite;
        }
    }
}
