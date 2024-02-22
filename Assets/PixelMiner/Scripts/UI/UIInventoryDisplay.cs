using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PixelMiner;

namespace PixelMiner.UI
{
    public class UIInventoryDisplay : MonoBehaviour
    {
        private const string PLAYER_TAG = "Player";


        [SerializeField] private Button _closeBtn;
        [SerializeField] private GameObject _previewHotbar;

        [Header("References")]
        public Player Player;
        private PlayerInventory _pInventory;
        [HideInInspector] public List<UIItemSlot> InBagSlots;
        [HideInInspector] public List<UIItemSlot> HotbarSlots;
        [SerializeField] private UIItemSlot _uiSlotPrefab;
        [SerializeField] private Transform _hotbbarSlotsParent;
        [SerializeField] private Transform _bagHotbbarSlotsParent;
        [SerializeField] private Transform _bagSlotsParent;



        private void Awake()
        {
            _previewHotbar.SetActive(false);
        }

        private void Start()
        {
            Player = GameObject.FindGameObjectWithTag(PLAYER_TAG).GetComponent<Player>();
            _pInventory = Player.PlayerInventory;
            if (Player != null)
            {
                //InitializeHotbar();
                InitializeInventory();
                UpdateInventory();
            }
            else
            {
                Debug.LogWarning("Missing player inventory!");
            }

            _closeBtn.onClick.AddListener(() =>
            {

            });
        }

        private void Update()
        {
            UpdateInventory();
        }

        private void OnDestroy()
        {
            _closeBtn.onClick.RemoveAllListeners();
        }

 
        private void InitializeHotbar()
        {
            for (int i = 0; i < PlayerInventory.WIDTH; i++)
            {
                HotbarSlots.Add(Instantiate(_uiSlotPrefab, _hotbbarSlotsParent));
            }
        }

        private void InitializeInventory()
        {
            // Hotbar
            InitializeHotbar();

            // Bag
            for (int i = 0; i < _pInventory.Inventory.Slots.Count; i++)
            {
                // Hotbar in bag
                if(i < PlayerInventory.WIDTH)
                {
                    InBagSlots.Add(Instantiate(_uiSlotPrefab, _bagHotbbarSlotsParent));
                }
                else
                {
                    // bag
                    InBagSlots.Add(Instantiate(_uiSlotPrefab, _bagSlotsParent));
                }
               
            }
        }

        public void UpdateInventory()
        {
            for (int i = 0; i < HotbarSlots.Count; i++)
            {
                if(_pInventory.CurrentHotbarSlotIndex == i)
                {
                    HotbarSlots[i].Select(true);
                }
                else
                {
                    HotbarSlots[i].Select(false);
                }
    
                HotbarSlots[i].UpdateSlot(_pInventory.Inventory.Slots[i]);
            }

            for (int i = 0; i < InBagSlots.Count; i++)
            {
                InBagSlots[i].UpdateSlot(_pInventory.Inventory.Slots[i]);
            }
        }

    }
}
