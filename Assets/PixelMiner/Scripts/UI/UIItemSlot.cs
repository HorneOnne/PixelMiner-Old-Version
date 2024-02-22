using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PixelMiner;

namespace PixelMiner.UI
{
    public class UIItemSlot : MonoBehaviour
    {
        [Header("References")]
        public Image Background;
        public Image Border;
        public Image Selected;
        public Image ItemIcon;
        public TextMeshProUGUI QuantityText;


        public void UpdateSlot(ItemSlot item)
        {
            UpdateIcon(item);
            UpdateQuantity(item.Quantity);
        }

        public void UpdateQuantity(int quantity)
        {
            if (quantity > 0)
                QuantityText.text = quantity.ToString();
            else
                QuantityText.text = "";
        }

        public void UpdateIcon(ItemSlot item)
        {
            if (item.ItemData == null)
            {
                ItemIcon.enabled = false;
                return;
            }
            ItemIcon.sprite = item.ItemData.Icon;
            ItemIcon.enabled = true;
        }

        public void Select(bool select)
        {
            if(select)
            {
                Selected.enabled = true;
            }
            else
            {
                Selected.enabled = false;
            }
        }

        public void Clear()
        {
            ItemIcon.enabled = false;
            QuantityText.text = "";
        }
    }
}
