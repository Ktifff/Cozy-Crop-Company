using Game.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using System;
using Game.Managers;
using Game.UI;

namespace Game.Entities.Buildings
{
    public class ResourceProducer : ISaveable
    {
        public event Action<ProducibleItem> ProductionStarted;
        public event Action<ProducibleItem> ProductionFinished;
        public event Action<ProducibleItem> ResourceCollected;

        protected List<ProducibleItem> _producibleItems;
        private ProducibleItem _currentProductionItem;
        private float _remainingProductionTime;

        public bool IsProducing => _currentProductionItem is not null;
        public float ProductionProgress => Mathf.Clamp01(1f - _remainingProductionTime / _currentProductionItem.ProductionTime);

        public string Save()
        {
            ResourceProducerSave save = new ResourceProducerSave(_currentProductionItem, _remainingProductionTime);
            string json = JsonUtility.ToJson(save, true);
            return json;
        }

        public void Load(string save)
        {
            var json = JsonUtility.FromJson<ResourceProducerSave>(save);
            ItemData item = InventoryView.GetItemByName(json.CurrentItem);
            if(item is not null)
            {
                ProduceResource(item);
                _remainingProductionTime = json.RemainingProductionTime;
            }
        }

        public void SetProducibleItems(ProducibleItem[] producibleItems)
        {
            _producibleItems = producibleItems.ToList();
        }

        public virtual async void ProduceResource(ItemData item)
        {
            if (IsProducing) return;
            var producibleItem = _producibleItems.FirstOrDefault(p => p.ResultItems.Item == item);
            if (producibleItem is null) return;
            await ProduceResourceAsync(producibleItem);
        }

        public virtual void CollectResource()
        {
            if (IsProducing && ProductionProgress == 1)
            {
                GameManager.PlayerInventory.AddItem(_currentProductionItem.ResultItems.Item, _currentProductionItem.ResultItems.Count);
                _currentProductionItem = null;
                _remainingProductionTime = 0;
                ResourceCollected?.Invoke(_currentProductionItem);
            }
        }

        protected virtual async Task ProduceResourceAsync(ProducibleItem item)
        {
            _currentProductionItem = item;
            _remainingProductionTime = _currentProductionItem.ProductionTime;
            ProductionStarted?.Invoke(_currentProductionItem);
            while (_remainingProductionTime > 0)
            {
                _remainingProductionTime -= Time.deltaTime;
                await Task.Yield();
            }
            ProductionFinished?.Invoke(_currentProductionItem);
        }

        [Serializable]
        protected class ResourceProducerSave
        {
            public string CurrentItem;
            public float RemainingProductionTime;

            public ResourceProducerSave(ProducibleItem currentItem, float remainingProductionTime)
            {
                if (currentItem is not null)
                {
                    CurrentItem = currentItem.ResultItems.Item.name;
                }
                RemainingProductionTime = remainingProductionTime;
            }
        }
    }
}