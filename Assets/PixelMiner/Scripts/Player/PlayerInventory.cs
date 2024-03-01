using UnityEngine;
using PixelMiner.Enums;
using PixelMiner.Physics;
using PixelMiner.DataStructure;
using System.Collections.Generic;
using PixelMiner.Miscellaneous;
namespace PixelMiner
{
    public class PlayerInventory : MonoBehaviour
    {
        public static event System.Action OnCurrentUseItemChanged;

        private Player _player;
        public Inventory Inventory;
        private InputHander _input;
        public int MAX_PLAYER_INVENTORY_SLOTS { get; private set; }
        public const int WIDTH = 9;
        public const int HEIGHT = 1;


        public int CurrentHotbarSlotIndex = -1;
        public int CurrentHotbarUseSlotIndex = -1;

        private float _directionalTimer = 0.0f;
        private float _directionalTime = 0.25f;
        private bool _canDirectionalHotbar = true;

        public bool OpenHotbarInventory { get; private set; } = false;

        [SerializeField] private Item _currentItem;
        [SerializeField] private Transform _rightHand;
        private int _useItemTimesPersecond = 5;
        private float _useItemResetTime;
        private bool _canUseItem = true;

        [Header("Looot items")]
        [SerializeField] private Vector3 _center;
        [SerializeField] private Vector3 _halfSize;
        [SerializeField] private Color _boundColor;
        [SerializeField] private bool _showBounds;
        [SerializeField] private DynamicEntity[] _itemEntites;
        private const int MAX_ITEM_DETECT_IN_FRAME = 8;
        [SerializeField] private LayerMask _itemLayer;
        public int EntitiesHit;

        private void Awake()
        {
            MAX_PLAYER_INVENTORY_SLOTS = WIDTH * HEIGHT;
            Inventory = new Inventory(WIDTH, HEIGHT);
            _itemEntites = new DynamicEntity[MAX_ITEM_DETECT_IN_FRAME];

        }


        private void Start()
        {
            _input = InputHander.Instance;
            _player = GetComponent<Player>();
            CurrentHotbarSlotIndex = 0;


            // Add init items for testing purposes
            Inventory.AddItem(ItemFactory.GetItemData(ItemID.StonePickaxe));
            Inventory.AddItem(ItemFactory.GetItemData(ItemID.StoneSword));

            _useItemResetTime = 1.0f / _useItemTimesPersecond;

        }

        private void OnDestroy()
        {

        }

        private void FixedUpdate()
        {
            int itemHit = GamePhysics.Instance.OverlapBoxNonAlloc(transform.position + _center, _halfSize, _itemEntites, _itemLayer);
            EntitiesHit = itemHit;
            if (itemHit > 0)
            {
                for (int i = 0; i < itemHit; i++)
                {
                    if (_itemEntites[i].Transform.TryGetComponent<Item>(out Item item))
                    {
                        Debug.Log("hit item");
                        GamePhysics.Instance.RemoveDynamicEntity(_itemEntites[i]);
                        //Destroy(item.gameObject);
                        Inventory.AddItem(item.Data);                   
                    }
                    else
                    {
                        Debug.Log("not player");
                    }
                    //System.Array.Clear(_itemEntites, 0, _itemEntites.Length);
                }
            }

        }

        private void Update()
        {
            if (_showBounds)
            {
                Bounds b = new Bounds(transform.position + _center, _halfSize * 2);
                DrawBounds.Instance.AddBounds(b, _boundColor);
            }

            if ((_input.ControlScheme & Enums.ControlScheme.KeyboardAndMouse) != 0)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    CurrentHotbarSlotIndex = 0;
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    CurrentHotbarSlotIndex = 1;
                }
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    CurrentHotbarSlotIndex = 2;
                }
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    CurrentHotbarSlotIndex = 3;
                }
                if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    CurrentHotbarSlotIndex = 4;
                }
                if (Input.GetKeyDown(KeyCode.Alpha6))
                {
                    CurrentHotbarSlotIndex = 5;
                }
                if (Input.GetKeyDown(KeyCode.Alpha7))
                {
                    CurrentHotbarSlotIndex = 6;
                }
                if (Input.GetKeyDown(KeyCode.Alpha8))
                {
                    CurrentHotbarSlotIndex = 7;
                }
                if (Input.GetKeyDown(KeyCode.Alpha9))
                {
                    CurrentHotbarSlotIndex = 8;
                }

                if (CurrentHotbarSlotIndex != CurrentHotbarUseSlotIndex)
                {
                    CurrentHotbarUseSlotIndex = CurrentHotbarSlotIndex;
                    DestroyOldItem();
                    CreateNewItem();

                    OnCurrentUseItemChanged?.Invoke();
                }
            }
            //if (Input.GetKeyDown(KeyCode.Alpha1))
            //{
            //    Debug.Log("Alpha1");
            //    bool success = Inventory.AddItem(ItemFactory.GetItemData(ItemID.Dirt));
            //    Debug.Log($"Success: {success}");
            //}
            //if (Input.GetKeyDown(KeyCode.Alpha2))
            //{
            //    Debug.Log("Alpha2");
            //    Inventory.AddItem(ItemFactory.GetItemData(ItemID.DirtGrass));
            //}



            if (_input.InventoryDirectional.y == -1)
            {
                OpenHotbarInventory = true;
            }
            else if (_input.InventoryDirectional.y == 1)
            {
                OpenHotbarInventory = false;
            }
            if (_canDirectionalHotbar && OpenHotbarInventory)
            {
                if (_input.InventoryDirectional.x == 1)
                {
                    Next();
                    _canDirectionalHotbar = false;
                    Invoke(nameof(ResetDirectionalHotbar), 0.2f);
                }
                else if (_input.InventoryDirectional.x == -1)
                {
                    Previous();
                    _canDirectionalHotbar = false;
                    Invoke(nameof(ResetDirectionalHotbar), 0.2f);
                }

            }


            if (CurrentHotbarSlotIndex != CurrentHotbarUseSlotIndex &&
                CurrentHotbarSlotIndex != -1 &&
                _input.InventoryDirectional.y == 1)
            {
                CurrentHotbarUseSlotIndex = CurrentHotbarSlotIndex;

                DestroyOldItem();
                CreateNewItem();

                OnCurrentUseItemChanged?.Invoke();
            }


            // Use item
            if (_input.Fire1 && _canUseItem)
            {
                IUseable useableItem = _currentItem as IUseable;

                if (useableItem != null)
                {
                    // Use the item
                    if (useableItem.Use(_player))
                    {
                        _canUseItem = false;
                        Invoke(nameof(EnableCanUseItem), _useItemResetTime);

                        //if(useableItem.RemainingUses == 0)
                        //{
                        //    if (Inventory.RemoveItem(CurrentHotbarUseSlotIndex) > 0)
                        //    {
                        //        Debug.Log("A");
                        //    }
                        //    else
                        //    {
                        //        Debug.Log("B");
                        //        Destroy(_currentItem.gameObject);
                        //        _currentItem = null;
                        //    }
                        //}

                        int remainingUse =  Inventory.Slots[CurrentHotbarUseSlotIndex].UseableItemData.RemainingUse--;
                        if (remainingUse > 1)
                        {

                        }
                        else
                        {
                            Inventory.RemoveItem(CurrentHotbarUseSlotIndex);

                            if (Inventory.Slots[CurrentHotbarUseSlotIndex].Quantity > 0)
                            {
                      
                            }
                            else
                            {
                                Destroy(_currentItem.gameObject);
                                _currentItem = null;
                            }        
                        }
                    }
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

        private void DestroyOldItem()
        {
            if (_currentItem != null)
            {
                Destroy(_currentItem.gameObject);
                _currentItem = null;
            }
        }
        private void CreateNewItem()
        {
            ItemSlot currentSlot = Inventory.Slots[CurrentHotbarUseSlotIndex];
            if (currentSlot != null && 
                currentSlot.UseableItemData != null && 
                currentSlot.UseableItemData.ItemData != null &&
                currentSlot.UseableItemData.ItemData.Model != null)
            {
                _currentItem = CreateNewItemObject(currentSlot.UseableItemData.ItemData);
            }
        }


        private Item CreateNewItemObject(ItemData itemData)
        {
            Item item = ItemFactory.CreateItem(itemData, Vector3.zero, Vector3.zero, _rightHand);
            return item;
        }


        private void EnableCanUseItem()
        {
            _canUseItem = true;
        }



    }
}
