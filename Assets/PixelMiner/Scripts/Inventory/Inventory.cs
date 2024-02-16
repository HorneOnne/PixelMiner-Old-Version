using UnityEngine;
using System.Collections.Generic;
namespace PixelMiner
{
    public class Inventory : MonoBehaviour
    {
        public List<ItemSlot> Slots;
        [SerializeField] private int _size;

        private void Awake ()
        {
            Slots = new List<ItemSlot>(_size);
            for(int i = 0; i < _size; i++)
            {
                Slots.Add(new ItemSlot(null, 0));
            }

        }

        public bool AddItem(IItem item)
        {
            // Finds an empty slot if there is no exist one.
            List<int> sameSlotsFoundIndex = new List<int>();
            int firstEmptySlot = -1;
            for(int i = 0; i < _size; i++)
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

        public void UseItem(int slotIndex)
        {
          
        }
    }
}
