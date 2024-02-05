using System.Collections.Generic;
using UnityEngine;
using PixelMiner.Items;

namespace PixelMiner.UI
{
    public class UIInventoryDisplay: MonoBehaviour
    {
        public Inventory PInventory;
        [HideInInspector] public List<UIItemSlot> ItemSlots;
        [SerializeField] private UIItemSlot _uiSlotPrefab;
        [SerializeField] private Transform _slotParent;

        private const string PLAYER_INVENTORY_TAG = "PInventory";

        private void Start()
        {
            PInventory = GameObject.FindGameObjectWithTag(PLAYER_INVENTORY_TAG).GetComponent<Inventory>();
            if(PInventory != null)
            {
                InitializeInventory();
                UpdateInventory();
            }
            else
            {
                Debug.LogWarning("Missing player inventory!");
            }
        }

        private void InitializeInventory()
        {
            for (int i = 0; i < PInventory.Items.Count; i++)
            {
                ItemSlots.Add(Instantiate(_uiSlotPrefab, _slotParent));
            }
        }

        public void UpdateInventory()
        {
            for(int i = 0; i < ItemSlots.Count; i++)
            {

            }
        }
    }
}
