using UnityEngine;
using System.Collections.Generic;
namespace PixelMiner.Items
{
    public class Inventory : MonoBehaviour
    {
        public List<ItemInstance> Items;
        [SerializeField] private int _size;

        private void Awake ()
        {
            Items = new List<ItemInstance>(_size);
            for(int i = 0; i < _size; i++)
            {
                Items.Add(new ItemInstance(null, 0));
            }

        }

        public bool AddItem(ItemInstance itemNeedAdd)
        {
            // Finds an empty slot if there is no exist one.
            List<int> sameSlotsFoundIndex = new List<int>();
            int firstEmptySlot = -1;
            for(int i = 0; i < _size; i++)
            {
                if (Items[i].ItemData.ID == itemNeedAdd.ItemData.ID)
                {
                    sameSlotsFoundIndex.Add(i);
                }

                if (Items[i] == null)
                {
                    firstEmptySlot = i;
                }
            }

            if(sameSlotsFoundIndex.Count > 0)
            {
                for(int j = 0; j < sameSlotsFoundIndex.Count; j++)
                {
                    if (Items[sameSlotsFoundIndex[j]].TryAdd())
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                if (firstEmptySlot == -1)
                    return false;
                else
                {
                    Items[firstEmptySlot] = itemNeedAdd;
                    return true;
                }
                   
            }
        }
    }
}
