using UnityEngine;
namespace PixelMiner
{
    [System.Serializable]
    public class ItemSlot 
    {
        //private ItemData _itemData;

        //public ItemData ItemData { get => _itemData; }

        //public IItem Item { get; set; }
        [field: SerializeField] public int Quantity { get; private set; }

        [field: SerializeField] public ItemData ItemData { get; private set; }

        public ItemSlot(ItemData itemData = null, int quantity = 0)
        {
            ItemData = itemData;
            Quantity = quantity;
        }



        public bool TryAddItem(ItemData itemData)
        {
            if (ItemData == null)
            {
                ItemData = itemData;
                Quantity = 1;
                return true;
            }
            else
            {
                if(ItemData.ID == itemData.ID)
                {
                    Quantity++;
                    if (Quantity > this.ItemData.MaxStack)
                    {
                        Quantity = this.ItemData.MaxStack;
                        return false;
                    }
                    return true;
                }
                return false;
            }
        }

    }
}
