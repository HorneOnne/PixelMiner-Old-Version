using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PixelMiner;

namespace PixelMiner.UI
{
    public class UIInventoryDisplay : MonoBehaviour
    {
        private const string PLAYER_INVENTORY_TAG = "PInventory";
        private readonly Vector2 OFFSET_COLLAPSE_BTN = new Vector2(10, -10f);
        private int DEFAULT_INVENTORY_COLUMNS_SHOWN = 2;
        private const int MIN_INVENTORY_COLUMNS_SHOWN = 1;
        private const int MAX_INVENTORY_COLUMNS_SHOWN = 3;


        [Header("References")]
        public Inventory PInventory;
        [HideInInspector] public List<UIItemSlot> ItemSlots;
        [SerializeField] private UIItemSlot _uiSlotPrefab;
        [SerializeField] private Transform _slotParent;

        [Header("Utilities")]
        public Button CollapseExpandBtn;
        public RectTransform _collapseExpandBtnRect;
        public RectTransform _content;

        private Rect _defaultCollapseExpandRect;
        private int _currentIventoryColumnsShown;
        private GridLayoutGroup _contentGroup;
    

        private void Awake()
        {
            _defaultCollapseExpandRect = _content.rect;
            _collapseExpandBtnRect = CollapseExpandBtn.GetComponent<RectTransform>();
            _currentIventoryColumnsShown = DEFAULT_INVENTORY_COLUMNS_SHOWN;
            _contentGroup = _content.GetComponent<GridLayoutGroup>();
            _contentGroup.constraintCount = DEFAULT_INVENTORY_COLUMNS_SHOWN;
        }

        private void Start()
        {
            return;
            PInventory = GameObject.FindGameObjectWithTag(PLAYER_INVENTORY_TAG).GetComponent<PlayerInventory>().PInventory;
            if (PInventory != null)
            {
                InitializeInventory();
                UpdateInventory();
            }
            else
            {
                Debug.LogWarning("Missing player inventory!");
            }

            CollapseExpandBtn.onClick.AddListener(() =>
            {
                _currentIventoryColumnsShown++;
                if(_currentIventoryColumnsShown > MAX_INVENTORY_COLUMNS_SHOWN)
                {
                    _currentIventoryColumnsShown = MIN_INVENTORY_COLUMNS_SHOWN;
                }
                _contentGroup.constraintCount = _currentIventoryColumnsShown;
            });

            //InvokeRepeating(nameof(UpdateCollapseExpandRectAnchor), 0f, 0.02f);
        }
        private void OnDestroy()
        {
            CollapseExpandBtn.onClick.RemoveAllListeners();
        }

 
        private void InitializeInventory()
        {
            for (int i = 0; i < PInventory.Slots.Count; i++)
            {
                ItemSlots.Add(Instantiate(_uiSlotPrefab, _slotParent));
            }
        }

        public void UpdateInventory()
        {
            for (int i = 0; i < ItemSlots.Count; i++)
            {
                ItemSlots[i].UpdateSlot(PInventory.Slots[i]);
            }
        }

        private void UpdateCollapseExpandRectAnchor()
        {
            _collapseExpandBtnRect.anchoredPosition = _defaultCollapseExpandRect.position + new Vector2(_content.rect.width + OFFSET_COLLAPSE_BTN.x, OFFSET_COLLAPSE_BTN.y);
        }
    }
}
