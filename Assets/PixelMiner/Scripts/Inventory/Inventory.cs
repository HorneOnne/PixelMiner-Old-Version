using UnityEngine;
using System.Collections.Generic;
namespace PixelMiner
{
    [System.Serializable]
    public class Inventory
    {
        public List<ItemSlot> Slots;

        public Inventory(int size)
        {
            Slots = new List<ItemSlot>(size);
            for (int i = 0; i < size; i++)
            {
                if(i == 0)
                {
                    var itemData = ItemFactory.GetItemData(Enums.ItemID.StonePickaxe);
                    Slots.Add(new ItemSlot(ItemFactory.GetItemData(Enums.ItemID.StonePickaxe), 1));
                }
                else
                {
                    Slots.Add(new ItemSlot(null, 0));
                }
               
            }
        }

        public bool AddItem(IItem item)
        {
            // Finds an empty slot if there is no exist one.
            List<int> sameSlotsFoundIndex = new List<int>();
            int firstEmptySlot = -1;
            for(int i = 0; i < Slots.Count; i++)
            {
                if (Slots[i].ItemData.ID == item.Data.ID)
                {
                    sameSlotsFoundIndex.Add(i);
                }

                if (Slots[i] == null)
                {
                    firstEmptySlot = i;
                }
            }

            if(sameSlotsFoundIndex.Count > 0)
            {
                for(int j = 0; j < sameSlotsFoundIndex.Count; j++)
                {
                    if (Slots[sameSlotsFoundIndex[j]].TryAdd())
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
                    Slots[firstEmptySlot] = new ItemSlot(item.Data,1);
                    return true;
                }      
            }
        }
    }
}
