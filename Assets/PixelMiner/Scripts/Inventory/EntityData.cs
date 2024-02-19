using UnityEngine;
using PixelMiner.Enums;

namespace PixelMiner
{
    public abstract class EntityData : ScriptableObject
    {
        public ItemID ID;
        public string ItemName;
        public Sprite Icon;
    }


}
