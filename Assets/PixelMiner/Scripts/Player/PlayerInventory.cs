using UnityEngine;
using PixelMiner.Enums;
namespace PixelMiner
{
    public class PlayerInventory : MonoBehaviour
    {
        public Inventory Inventory;
        private InputHander _input;
        public int MAX_PLAYER_INVENTORY_SLOTS { get; private set; }
        public const int WIDTH = 9;
        public const int HEIGHT = 1;


        public int CurrentHotbarSlotIndex = -1;

        private float _directionalTimer = 0.0f;
        private float _directionalTime = 0.25f;
        private bool _canDirectionalHotbar = true;

        public bool OpenHotbarInventory { get; private set; } = false;

        private void Awake()
        {
            MAX_PLAYER_INVENTORY_SLOTS = WIDTH * HEIGHT;
            Inventory = new Inventory(WIDTH, HEIGHT);
        }


        private void Start()
        {
            _input = InputHander.Instance;
            CurrentHotbarSlotIndex = 0;
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("Alpha1");
                bool success = Inventory.AddItem(ItemFactory.GetItemData(ItemID.Dirt));
                Debug.Log($"Success: {success}");
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("Alpha2");
                Inventory.AddItem(ItemFactory.GetItemData(ItemID.DirtGrass));
            }



            if(_input.AccessHorbarInventory == -1)
            {
                OpenHotbarInventory = true;
            }
            else if(_input.AccessHorbarInventory == 1)
            {
                OpenHotbarInventory = false;
            }
            if (_canDirectionalHotbar && OpenHotbarInventory)
            {
                if (_input.DirectionalHorbarInventory == 1)
                {
                    Next();
                    _canDirectionalHotbar = false;
                    Invoke(nameof(ResetDirectionalHotbar), 0.2f);
                }
                else if (_input.DirectionalHorbarInventory == -1)
                {
                    Previous();
                    _canDirectionalHotbar = false;
                    Invoke(nameof(ResetDirectionalHotbar), 0.2f);
                }

            }
        }

        private void ResetDirectionalHotbar()
        {
            _canDirectionalHotbar = true;
        }

        public void Next()
        {
            //CurrentHotbarSlotIndex = (CurrentHotbarSlotIndex + 1) % WIDTH;
            CurrentHotbarSlotIndex++;
            if (CurrentHotbarSlotIndex == WIDTH)
            {
                CurrentHotbarSlotIndex = -1;
            }

        }


        public void Previous()
        {
            //CurrentHotbarSlotIndex = (CurrentHotbarSlotIndex - 1 + WIDTH) % WIDTH;

            CurrentHotbarSlotIndex--;
            if (CurrentHotbarSlotIndex == -2)
            {
                CurrentHotbarSlotIndex = WIDTH - 1;
            }

            
        }
    }
}
