using UnityEngine;
using PixelMiner.Core;
using PixelMiner.Enums;
using PixelMiner.DataStructure;
using PixelMiner.Physics;
namespace PixelMiner
{
    public class Pickaxe : Item, IUseable
    {
        public override void Initialize(ItemData data)
        {
            base.Initialize(data);
        }

        public bool Use(Player player)
        {      
            if (Main.Instance.RemoveBlock(player.PlayerBehaviour.SampleBlockTrans.position, out Enums.BlockType removedBlock))
            {
                //player.PlayerInventory.Inventory.AddItem(ItemFactory.GetItemData(Enums.ItemID.Dirt));

                ItemID itemID = (ItemID)removedBlock;
                ItemData removedData = ItemFactory.GetItemData(itemID);
                if (removedData == null)
                {
                    Debug.Log($"null: {itemID}");
                }
                else
                {
                    var item = ItemFactory.CreateItem(removedData, player.PlayerBehaviour.SampleBlockTrans.position, Vector3.zero);
                    item.EnablePhysics();
                    GamePhysics.Instance.AddDynamicEntity(item.DynamicEntity);
                }
                return true;
            }

            return false;
        }


        public override void EnablePhysics()
        {
            AABB bound = new AABB(transform.position.x, transform.position.y, transform.position.z, BoxSize.x, BoxSize.y, BoxSize.z);
            dEntity = new DynamicEntity(this.transform, bound, Vector3.zero, PhysicLayer);
            this.dEntity.PhysicLayer = this.gameObject.layer;
        }


    }
}
