using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using PixelMiner.Enums;

namespace PixelMiner.UI
{
    public class UIItemSlot : MonoBehaviour, IPointerClickHandler
    {
        public static event System.Action<int> OnSlotClicked;

        [Header("References")]
        public Image Background;
        public Image Border;
        public Image Selected;
        public Image CurrentUse;
        public Image ItemIcon;
        public TextMeshProUGUI QuantityText;

        public int IndexOfInventory { get; private set; }
        public InventoryID InventoryID { get; private set; }

        public void Initialized(int slotIndex, InventoryID inventoryID)
        {
            IndexOfInventory = slotIndex;
            InventoryID = inventoryID;
         
        }

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

        public void Use(bool use)
        {
            if(use)
            {
                CurrentUse.enabled = true;
            }
            else
            {
                CurrentUse.enabled = false;
            }
        }


        public void Clear()
        {
            ItemIcon.enabled = false;
            QuantityText.text = "";
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //Debug.Log("POinter enter");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //Debug.Log("Pointer exit");
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnSlotClicked?.Invoke(IndexOfInventory);
            Debug.Log($"Pointer click: {IndexOfInventory}  {InventoryID}");
        }
    }
}
