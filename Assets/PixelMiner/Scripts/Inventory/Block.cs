using UnityEngine;
using PixelMiner.DataStructure;
namespace PixelMiner
{
    public class Block : Item, IUseable
    {
        private int _remainingUses;
        public int RemainingUses { get => _remainingUses; }



        public override void Initialize(ItemData data)
        {
            base.Initialize(data);
            _remainingUses = Data.MaxUses;
        }



        public bool Use(Player player)
        {
            if (_remainingUses > 0)
            {
                // Implemeent use logic here.
                


                // Decrease the remaining uses
                _remainingUses--;


                // Handle item is broken.
                if (_remainingUses == 0)
                {
                    Debug.Log($"{Data.ItemName} has been broken.");
                    Destroy(this.gameObject);
                }
            }
            else
            {
                Debug.Log($"Out of uses for: {Data.ItemName}");
            }

            return true;
        }

        public override void EnablePhysics()
        {
            AABB bound = new AABB(transform.position.x, 
                                  transform.position.y, 
                                  transform.position.z,
                                  BoxSize.x, BoxSize.y, BoxSize.z);
            dEntity = new DynamicEntity(this.transform, bound, BoxOffset, PhysicLayer);
        }
    }
}
