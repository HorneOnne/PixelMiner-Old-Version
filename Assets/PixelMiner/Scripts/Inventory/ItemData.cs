using UnityEngine;
using PixelMiner.Enums;

namespace PixelMiner
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "PixelMiner/ItemData", order = 51)]
    public class ItemData : ScriptableObject
    {
        public ItemID ID;
        public string ItemName;
        public Sprite Icon;
        public int MaxUses;
        public int MaxStack;
        public GameObject Model;
    }
}
