using Game.Entities.Buildings;
using UnityEngine;
using Game.Data;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Events;
using Game.Audio;

namespace Game.UI
{
    public class ProductionIndicator : SelectionMenu<ItemButton, ItemData>
    {
        [SerializeField] private Building _targetBuilding;

        private ResourceProducer _resourceProducer;
        private CancellationTokenSource _cancellationTokenSource;

        protected override ItemButton AddButton(ItemData data, UnityAction call = null)
        {
            var result = base.AddButton(data, call);
            result.Init(data.Icon, call);
            return result;
        }

        private void Awake()
        {
            if (_targetBuilding.TryGetComponent(out IResourceProducer resourceProducer))
            {
                _resourceProducer = resourceProducer.Producer;
            }
            if (_targetBuilding.TryGetComponent(out IResourceProcessor resourceProcessor))
            {
                _resourceProducer = resourceProcessor.Processor;
            }
        }

        private void OnEnable()
        {
            if (_resourceProducer is not null)
            {
                _resourceProducer.ProductionStarted += IndicateProductionProgressAsync;
                _resourceProducer.ResourceCollected += OnResourceCollected;
            }
        }

        private void OnDisable()
        {
            if (_resourceProducer is not null)
            {
                _resourceProducer.ProductionStarted -= IndicateProductionProgressAsync;
                _resourceProducer.ResourceCollected -= OnResourceCollected;
            }
            if(_cancellationTokenSource is not null)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        private async void IndicateProductionProgressAsync(ProducibleItem item)
        {
            if (_resourceProducer is null) return;
            ClearButtons();
            SetVisibility(true);
            ItemButton button = AddButton(item.ResultItems.Item, CollectResource);
            _cancellationTokenSource = new();
            while (_resourceProducer.IsProducing && _resourceProducer.ProductionProgress < 1)
            {
                if (_cancellationTokenSource.Token.IsCancellationRequested) return;
                button.SetFillAmount(_resourceProducer.ProductionProgress);
                await Task.Yield();
            }
            button.SetFillAmount(_resourceProducer.ProductionProgress);
            ShowMessage();
        }

        private void OnResourceCollected(ProducibleItem item)
        {
            if (_resourceProducer is null) return;
            SetVisibility(false);
            AudioMixerManager.PlayCollectSound();
        }

        private void CollectResource()
        {
            if (_resourceProducer is null) return;
            _resourceProducer.CollectResource();
        }
    }
}