using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.UI
{
    public class ShopItem : ItemInfo
    {
        [SerializeField] protected ItemInfo _priceInfo;
        [SerializeField] protected Button _button;

        public ItemInfo PriceInfo => _priceInfo;

        public void SetButtonCall(UnityAction call)
        {
            if(call is not null)
            {
                _button.onClick.AddListener(call);
            }
        }
    }
}