using UnityEngine;
using PixelMiner.Core;
using PixelMiner.Enums;
using PixelMiner.DataStructure;
using PixelMiner.Physics;
namespace PixelMiner
{
    public class Pickaxe : Item, IUseable
    {
        public static System.Action OnItemBroken;

        private int _remainingUses;
        public int RemainingUses { get => _remainingUses; }


        public override void Initialize(ItemData data)
        {
            base.Initialize(data);
            _remainingUses = Data.MaxUses;
        }



        public bool Use(Player player)
        {
            Debug.Log($"Use: {Data.ID}");
            if (_remainingUses > 0)
            {
                // Implemeent use logic here.

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
                    

                    // Decrease the remaining uses
                    _remainingUses--;

                    // Handle item is broken.
                    if (_remainingUses == 0)
                    {
                        Debug.Log($"{Data.ItemName} has been broken.");
                        Destroy(this.gameObject);
                        OnItemBroken?.Invoke();
                    }

                    return true;
                }
            }
            else
            {
                Debug.Log($"Out of uses for: {Data.ItemName}");
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
