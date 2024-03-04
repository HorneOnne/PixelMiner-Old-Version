using UnityEngine;
using PixelMiner.DataStructure;
using PixelMiner.Core;
using PixelMiner.Enums;
namespace PixelMiner
{
    public class Block : Item, IUseable
    {
        public override void Initialize(ItemData data)
        {
            base.Initialize(data);

        }

        public bool Use(Player player)
        {
            if (Main.Instance.PlaceBlock(player.PlayerBehaviour.SampleBlockTrans.position, (BlockType)Data.ID))
            {    
                return true;
            }
            return false;
        }

        public override void EnablePhysics()
        {
            AABB bound = new AABB(transform.position.x, 
                                  transform.position.y, 
                                  transform.position.z,
                                  BoxSize.x, BoxSize.y, BoxSize.z);
            dEntity = new DynamicEntity(this.transform, bound, BoxOffset, PhysicLayer);
            dEntity.SetConstraint(Constraint.X, true);
            dEntity.SetConstraint(Constraint.Z, true);
            dEntity.SetConstraint(Constraint.Y, false);
        }
    }
}
