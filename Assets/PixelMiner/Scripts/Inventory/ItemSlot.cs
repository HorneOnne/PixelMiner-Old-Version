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

        public ItemSlot(ItemData itemData, int quantity)
        {
            ItemData = itemData;
            Quantity = quantity;
        }

        public bool TryAdd()
        {
            if(Quantity < ItemData.MaxStack)
            {
                Quantity++;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
