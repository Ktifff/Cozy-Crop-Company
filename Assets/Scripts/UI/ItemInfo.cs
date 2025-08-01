using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Entities;
using Game.Data;

namespace Game.UI
{
    public class ItemInfo : MonoBehaviour
    {
        [SerializeField] protected Image _image;
        [SerializeField] protected Image _shadow;
        [SerializeField] protected TextMeshProUGUI _countText;

        private Inventory _trackedInventory;
        private ItemData _trackedItem;
        private int _requestedCount;

        public void Init(Sprite image)
        {
            _image.sprite = _shadow.sprite = image;
        }

        public void SetTrackedItem(Inventory inventory, ItemData item, int requestedCount = 0)
        {
            if (inventory is null || item is null) return;
            if(_trackedItem is not null)
            {
                _trackedInventory.ItemCountChanged -= UpdateItemCount;
            }
            _trackedInventory = inventory;
            _trackedItem = item;
            _requestedCount = requestedCount;
            _trackedInventory.ItemCountChanged += UpdateItemCount;
            UpdateItemCount(item, inventory.GetItemCount(item));
        }

        public void SetCount(int count)
        {
            _countText.text = count.ToString();
        }

        private void UpdateItemCount(ItemData item, int count)
        {
            if (_trackedItem == item)
            {
                if (_requestedCount == 0)
                {
                    _countText.text = count.ToString();
                }
                else
                {
                    _countText.text = $"{count}/{_requestedCount}";
                }
            }
        }
    }
}