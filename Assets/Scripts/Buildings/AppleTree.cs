using Game.Data;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Entities.Buildings
{
    public class AppleTree : Building, IResourceProducer
    {
        public ResourceProducer Producer { get; private set; } = new();

        [SerializeField] private ItemData _apple;
        [SerializeField] private CropLifecycleData[] _crops;

        private CancellationTokenSource _cancellationTokenSource;

        private async void OnProductionStarted(ProducibleItem item)
        {
            CropLifecycleData currentCrop = null;
            foreach (var crop in _crops)
            {
                bool isCurrentCrop = crop.Item == item.ResultItems.Item;
                crop.SetVisibility(isCurrentCrop);
                if (isCurrentCrop)
                {
                    currentCrop = crop;
                }
            }
            if (currentCrop is not null)
            {
                _cancellationTokenSource = new();
                while (Producer.IsProducing)
                {
                    if (_cancellationTokenSource.Token.IsCancellationRequested) return;
                    currentCrop.SetLifecycleProgress(Producer.ProductionProgress);
                    await Task.Yield();
                }
            }
        }

        private void OnResourceCollected(ProducibleItem item)
        {
            foreach (var crop in _crops)
            {
                crop.SetVisibility(false);
            }
            Producer.ProduceResource(_apple);
        }

        private void OnEnable()
        {
            Producer.ProductionStarted += OnProductionStarted;
            Producer.ResourceCollected += OnResourceCollected;
            Producer.ProduceResource(_apple);
            if (_cancellationTokenSource is not null)
            {
                _cancellationTokenSource = new();
            }
        }

        private void OnDisable()
        {
            Producer.ProductionStarted -= OnProductionStarted;
            Producer.ResourceCollected -= OnResourceCollected;
            if (_cancellationTokenSource is not null)
            {
                _cancellationTokenSource.Cancel();
            }
        }
    }
}