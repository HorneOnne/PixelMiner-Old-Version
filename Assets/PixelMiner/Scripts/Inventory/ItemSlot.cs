namespace PixelMiner
{
    [System.Serializable]
    public class ItemSlot
    {
        //private ItemData _itemData;
      
        //public ItemData ItemData { get => _itemData; }

        public IItem Item { get; set; }
        public int Quantity { get; private set; }

        public ItemData ItemData => Item.Data;

        public ItemSlot(ItemData itemData, int quantity)
        {
            Item.Data = itemData;
            Quantity = quantity;
        }

        public bool TryAdd()
        {
            if(Quantity < Item.Data.MaxStack)
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
