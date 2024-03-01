using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PixelMiner.Enums;

namespace PixelMiner.UI
{
    public class UIItemHold : MonoBehaviour
    {
        public Image RaycastTarget { get; private set; }    
        public Image ItemIcon;
        public TextMeshProUGUI QuantityText;
        public int IndexOfInventory { get; private set; }
        public InventoryID InventoryID { get; private set; }


        public void UpdateSlot(ItemSlot item)
        {
            UpdateIcon(item);
            UpdateQuantity(item.Quantity);
        }

        private void UpdateQuantity(int quantity)
        {
            if (quantity > 1)
                QuantityText.text = quantity.ToString();
            else
                QuantityText.text = "";
        }

        private void UpdateIcon(ItemSlot item)
        {
            if (item.UseableItemData.ItemData == null)
            {
                ItemIcon.enabled = false;
                return;
            }
            ItemIcon.sprite = item.UseableItemData.ItemData.Icon;
            ItemIcon.enabled = true;
        }

        public void SetActive(bool active)
        {
            if(active)
            {
                RaycastTarget.enabled = true;
                ItemIcon.enabled = true;
                QuantityText.enabled = true;
            }
            else
            {
                RaycastTarget.enabled = false;
                ItemIcon.enabled = false;
                QuantityText.enabled = false;
            }
        }
    }
}
