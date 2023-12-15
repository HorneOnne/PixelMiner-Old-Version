using UnityEngine;
using UnityEngine.UI;

namespace PixelMiner.UI.WorldGen
{
    public class UIMapPreview : MonoBehaviour
    {
        [HideInInspector] public Image Image;

        private void Awake()
        {
            Image = GetComponent<Image>();
        }

        public void SetImage(Texture2D texture)
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
            Image.enabled = true;
            Image.sprite = sprite;
        }
    }
}
