namespace PixelMiner.Items
{
    [System.Serializable]
    public class ItemInstance
    {
        private ItemData _itemData;
        public int Quantity{get; private set;}
        public ItemData ItemData { get => _itemData; }


        public ItemInstance(ItemData itemData, int quantity)
        {
            _itemData = itemData;
            Quantity = quantity;
        }

        public bool TryAdd()
        {
            if(Quantity < _itemData.MaxStack)
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
