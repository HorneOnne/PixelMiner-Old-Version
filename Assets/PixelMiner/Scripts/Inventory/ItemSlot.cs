using UnityEngine;
namespace PixelMiner
{
    [System.Serializable]
    public class ItemSlot 
    {    
        [field: SerializeField] public UseableItemData UseableItemData { get; private set; }
        [field: SerializeField] public int Quantity { get; private set; }

        public ItemSlot(UseableItemData itemData = null, int quantity = 0)
        {
            UseableItemData = itemData;
            Quantity = quantity;
        }



        public bool TryAddItem(ItemData itemData)
        {
            if (UseableItemData.ItemData == null)
            {
                UseableItemData.ItemData = itemData;
                UseableItemData.RemainingUse = itemData.MaxUses;
                Quantity = 1;
                return true;
            }
            else
            {
                if(UseableItemData.ItemData.ID == itemData.ID)
                {
                    Quantity++;
                    if (Quantity > this.UseableItemData.ItemData.MaxStack)
                    {
                        Quantity = this.UseableItemData.ItemData.MaxStack;
                        return false;
                    }
                    return true;
                }
                return false;
            }
        }


        public int RemoveItem()
        {
            if (Quantity > 1)
            {
                Quantity--;
            }
            else
            {
                ClearSlot();
            }

            return Quantity;
        }


        public void ClearSlot()
        {
            UseableItemData.ItemData = null;
            Quantity = 0;
        }
    }
}
