using UnityEngine;
using System.Collections.Generic;
using PixelMiner.Enums;
using Sirenix.OdinInspector;

namespace PixelMiner
{
    public class ItemFactory : MonoBehaviour
    {
        [AssetList(Path = "PixelMiner/Scripts/Inventory")]
        public List<ItemData> Datas = new List<ItemData>();
        private static Dictionary<ItemID, ItemData> _itemDictionary = new Dictionary<ItemID, ItemData>();


        private void Awake()
        {
            for (int i = 0; i < Datas.Count; i++)
            {
                if (_itemDictionary.ContainsKey(Datas[i].ID) == false)
                {
                    _itemDictionary.Add(Datas[i].ID, Datas[i]);
                }
                else
                {
                    Debug.LogWarning($"{Datas[i].ID} exist.");
                }
            }
        }

        public static Item CreateItem(ItemData itemData, Vector3 position, Vector3 eulerAngles, Transform parent = null)
        {
            Item item = Instantiate(itemData.Model, parent).GetComponent<Item>();
            item.transform.localPosition = position + item.Offset;
            item.transform.localEulerAngles = eulerAngles + item.RotAngles;
            item.Data = itemData;
            item.Initialize(itemData);
            return item;
        }
  

        public static Item CreateItem(ItemID itemId, Vector3 position, Vector3 eulerAngles, Transform parent)
        {
            if (_itemDictionary.TryGetValue(itemId, out ItemData itemData))
            {
                return CreateItem(itemData, position, eulerAngles, parent);
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
        //public static ItemData GetItemBlockData(BlockType blockType)
        //{
        //    ItemData itemData;
        //    _itemDictionary.TryGetValue(itemId, out itemData);
        //    return itemData;
        //}


        private void OnApplicationQuit()
        {
            Datas.Clear();
            _itemDictionary.Clear();
        }
    }
}
