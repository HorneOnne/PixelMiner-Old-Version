using UnityEngine;
using System.Collections.Generic;
using PixelMiner.Enums;
namespace PixelMiner
{
    public class ItemFactory : MonoBehaviour
    {
        public List<ItemData> Datas = new List<ItemData>();
        private static Dictionary<ItemID, ItemData> _itemDictionary = new Dictionary<ItemID, ItemData>();


        private void Awake()
        {
            for(int i = 0; i < Datas.Count; i++)
            {
                _itemDictionary.Add(Datas[i].ID, Datas[i]);
            }
        }

        public static IItem CreateItem(ItemData itemData, Vector3 gPosition)
        {
            IItem item = Instantiate(itemData.Model, gPosition, Quaternion.identity).GetComponent<IItem>();
            item.Data = itemData;
            item.Initialize();
            return item;
        }

        public static IItem CreateItem(ItemID itemId, Vector3 gPosition)
        {
            if(_itemDictionary.TryGetValue(itemId, out ItemData itemData))
            {
                return CreateItem(itemData, gPosition);
            }
            Debug.LogWarning("Not found this item id.");
            return null;
        }

        public static ItemData GetItemData(ItemID itemId)
        {
            ItemData itemData;
            _itemDictionary.TryGetValue(itemId, out itemData);
            return itemData;
        }


        private void OnApplicationQuit()
        {
            Datas.Clear();
            _itemDictionary.Clear();
        }
    }
}
