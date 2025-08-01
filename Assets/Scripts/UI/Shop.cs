using Game.Audio;
using Game.Data;
using Game.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class Shop : MonoBehaviour
    {
        private static readonly int _isShownAnimatorHash = Animator.StringToHash("IsShown");

        private readonly List<ShopItem> _items = new List<ShopItem>();

        [SerializeField] private ShopItemData[] _itemDatabase;
        [Space]
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _content;
        [SerializeField] private ShopItem _shopItemPrefab;

        public void SetVisibility(bool value)
        {
            _animator.SetBool(_isShownAnimatorHash, value);
        }

        private void Start()
        {
            foreach (var item in _itemDatabase)
            {
                _items.Add(AddItem(item));
            }
        }

        private ShopItem AddItem(ShopItemData item)
        {
            ShopItem shopItem = Instantiate(_shopItemPrefab, _content);
            shopItem.Init(item.Reward.Item.Icon);
            shopItem.SetCount(item.Reward.Count);
            shopItem.PriceInfo.Init(item.Price.Item.Icon);
            shopItem.PriceInfo.SetCount(item.Price.Count);
            shopItem.SetButtonCall(() => { BuyItem(item); });
            return shopItem;
        }

        private void BuyItem(ShopItemData item)
        {
            if (GameManager.PlayerInventory.UseRecipe(new RecipeItemData[]{ item.Price }))
            {
                GameManager.PlayerInventory.AddItem(item.Reward.Item, item.Reward.Count);
                AudioMixerManager.PlayPurchaseSound();
            }
        }
    }
}