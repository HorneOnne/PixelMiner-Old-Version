using UnityEngine;
using PixelMiner.Enums;

namespace PixelMiner.Items
{
    [CreateAssetMenu]
    public class ItemData : ScriptableObject
    {
        public ItemID ID;
        public string ItemName;
        public Sprite Icon;
        public GameObject Model;
        public int MaxStack;
    }
}
