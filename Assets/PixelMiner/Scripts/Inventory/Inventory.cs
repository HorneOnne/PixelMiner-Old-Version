using UnityEngine;
using System.Collections.Generic;
namespace PixelMiner
{
    [System.Serializable]
    public class Inventory
    {
        public List<ItemSlot> Slots;

        public Inventory(int width, int height)
        {
            int size = width * height;
            // Init empty inventory
            Slots = new List<ItemSlot>(size);
            for (int i = 0; i < size; i++)
            {
                Slots.Add(new ItemSlot(null, 0));
            }
        }

    
        public bool AddItem(ItemData itemData)
        {
            bool canAddItem = false;

            for (int i = 0; i < Slots.Count; i++)
            {
                if (Slots[i].ItemData == null)
                {
                    Slots[i].TryAddItem(itemData);
                    canAddItem = true;
                    break;
                }
                else
                {
                    if (Slots[i].ItemData == itemData)
                    {
                        bool canAdd = Slots[i].TryAddItem(itemData);

                        if (canAdd == true)
                        {
                            canAddItem = true;
                            break;
                        }
                    }
                }
            }

            return canAddItem;
        }
    }
}
