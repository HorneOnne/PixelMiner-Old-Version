using UnityEngine;
using System.Collections.Generic;
namespace PixelMiner
{
    public class ItemFactory : MonoBehaviour
    {
        public List<ItemData> Datas = new List<ItemData>();

        public static IItem CreateItem(ItemData itemData, Vector3 gPosition)
        {
            IItem item = Instantiate(itemData.Model, gPosition, Quaternion.identity).GetComponent<IItem>();
            item.Data = itemData;
            item.Initialize();
            return item;
        }
    }
}
