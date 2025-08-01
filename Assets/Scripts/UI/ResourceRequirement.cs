using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using Game.Data;
using System.Collections.Generic;
using Game.Managers;

namespace Game.UI
{
    public class ResourceRequirement : MonoBehaviour
    {
        [SerializeField] protected ItemInfo _itemInfo;
        [SerializeField] protected TextMeshProUGUI _nameText;
        [SerializeField] protected Transform _resourcesList;
        [SerializeField] protected Button _button;
        [SerializeField] protected Button _cancelButton;
        [SerializeField] protected ItemInfo _requestedResourcePrefab;

        private readonly List<ItemInfo> _requestedResources = new();

        public void Init(Sprite image, string name, UnityAction call)
        {
            _itemInfo.Init(image);
            if(_nameText is not null)
            { 
                _nameText.text = name;
            }
            if (call is not null)
            {
                _button.onClick.AddListener(call);
            }
        }

        public void Init(Sprite image, string name, int rewardCount, UnityAction call, UnityAction cancelCall)
        {
            _itemInfo.Init(image);
            _itemInfo.SetCount(rewardCount);
            _nameText.text = name;
            if (call is not null)
            {
                _button.onClick.AddListener(call);
            }
            if (cancelCall is not null)
            {
                _cancelButton.onClick.AddListener(cancelCall);
            }
        }

        public void AddRequestedResource(ItemData item, int count)
        {
            ItemInfo resource = Instantiate(_requestedResourcePrefab, _resourcesList);
            resource.Init(item.Icon);
            resource.SetTrackedItem(GameManager.PlayerInventory, item, count);
            _requestedResources.Add(resource);
        }
    }
}