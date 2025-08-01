using Game.Data;
using Game.Managers;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Game.UI
{
    public class InventoryView : MonoBehaviour
    {
        private static readonly int _isShownAnimatorHash = Animator.StringToHash("IsShown");
        private static InventoryView _instance;

        private readonly List<ItemInfo> _counters = new List<ItemInfo>();

        [SerializeField] private ItemData[] _itemDatabase;
        [Space]
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _content;
        [SerializeField] private ItemInfo _itemCounterPrefab;

        public static ItemData GetItemByName(string name)
        {
            return _instance._itemDatabase.FirstOrDefault(x => x.name == name);
        }

        public void SetVisibility(bool value)
        {
            _animator.SetBool(_isShownAnimatorHash, value);
        }

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            foreach(var item in _itemDatabase)
            {
                _counters.Add(AddItemCounter(item));
            }
        }

        private ItemInfo AddItemCounter(ItemData item)
        {
            ItemInfo counter = Instantiate(_itemCounterPrefab, _content);
            counter.Init(item.Icon);
            counter.SetTrackedItem(GameManager.PlayerInventory, item);
            return counter;
        }

        public void Cheat()
        {
            foreach(var item in _itemDatabase)
            {
                GameManager.PlayerInventory.AddItem(item, 100);
            }
        }
    }
}