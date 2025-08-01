using Game.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Entities.Buildings
{
    public class CropPlot : Building, IResourceProducer
    {
        public ResourceProducer Producer { get; private set; } = new();

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
        }

        private void OnEnable()
        {
            Producer.ProductionStarted += OnProductionStarted;
            Producer.ResourceCollected += OnResourceCollected;
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

    [Serializable]
    public class CropLifecycleData
    {
        private const float AnimationSpeed = 0.4f;

        [SerializeField] private ItemData _item;
        [SerializeField] private GameObject _crop;
        [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
        [SerializeField] private Gradient _lifecycleColor;
        [SerializeField] private int _cropMaterialIndex;

        private int _winterBlendShapeIndex;
        private int _seedBlendShapeIndex;
        private float _winterAnimationCounter;
        private int _winterAnimationDirection;
        private Material _cropMaterial;

        public ItemData Item => _item;

        public void SetVisibility(bool value)
        {
            if (value)
            {
                _winterBlendShapeIndex = _skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex("Winter");
                _seedBlendShapeIndex = _skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex("Seed");
            }
            _winterAnimationDirection = 1;
            _cropMaterial = _skinnedMeshRenderer.materials[_cropMaterialIndex];
            if (_crop != null)
            {
                _crop.gameObject.SetActive(value);
            }
        }

        public void SetLifecycleProgress(float value)
        {
            value = Mathf.Clamp01(value);
            _winterAnimationCounter += Time.deltaTime * _winterAnimationDirection * AnimationSpeed;
            if (_winterAnimationCounter > 1 || _winterAnimationCounter < 0)
            {
                _winterAnimationCounter = Mathf.Clamp01(_winterAnimationCounter);
                _winterAnimationDirection = -_winterAnimationDirection;
            }
            _skinnedMeshRenderer.SetBlendShapeWeight(_winterBlendShapeIndex, _winterAnimationCounter * 100f);
            _skinnedMeshRenderer.SetBlendShapeWeight(_seedBlendShapeIndex, (1f - value) * 100f);
            _cropMaterial.color = _lifecycleColor.Evaluate(value);
        }
    }
}