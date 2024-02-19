using UnityEngine;

namespace PixelMiner
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "PixelMiner/ItemData", order = 51)]
    public class ItemData : EntityData
    {     
        public int MaxUses;
        public int MaxStack;
        public GameObject Model;
    }


}
