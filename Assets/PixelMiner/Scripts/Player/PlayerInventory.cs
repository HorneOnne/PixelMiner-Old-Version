using UnityEngine;
namespace PixelMiner
{
    public class PlayerInventory : MonoBehaviour
    {
        public Inventory PInventory;
        private const int MAX_PLAYER_INVENTORY_SLOTS = 36;
        private void Start()
        {
            PInventory = new Inventory(MAX_PLAYER_INVENTORY_SLOTS);
        }
    }
}
