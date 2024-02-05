using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PixelMiner.Items;
namespace PixelMiner.UI
{
    public class UIItemSlot : MonoBehaviour
    {
        [Header("References")]
        public Image Background;
        public Image Border;
        public Image ItemIcon;
        public TextMeshProUGUI QuantityText;


        public void UpdateQuantity(int quantity)
        {
            QuantityText.text = quantity.ToString();
        }

        public void UpdateIcon(ItemInstance item)
        {
            ItemIcon.sprite = item.ItemData.Icon;
            ItemIcon.enabled = true;
        }

        public void Clear()
        {
            ItemIcon.enabled = false;
            QuantityText.text = "";
        }
    }
}
