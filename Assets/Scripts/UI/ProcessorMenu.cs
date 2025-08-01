using Game.Controllers;
using Game.Entities.Buildings;
using UnityEngine;
using Game.Data;
using UnityEngine.Events;

namespace Game.UI
{
    public class ProcessorMenu : SelectionMenu<ResourceRequirement, ItemRecipe>
    {
        [SerializeField] private Building _targetBuilding;
        [SerializeField] private LayerMask _buildingLayerMask;

        private Camera _camera;
        private bool _isBuildingSelected;
        private IResourceProcessor _resourceProcessor;

        protected override ResourceRequirement AddButton(ItemRecipe data, UnityAction call = null)
        {
            var result = base.AddButton(data, call);
            result.Init(data.ResultItems.Item.Icon, null, call);
            foreach (var item in data.RecipeItems)
            {
                result.AddRequestedResource(item.Item, item.Count);
            }
            return result;
        }

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void OnEnable()
        {
            CameraController.OnClick += OnClick;
            if (_resourceProcessor is null && _targetBuilding.TryGetComponent(out IResourceProcessor resourceProcessor))
            {
                _resourceProcessor = resourceProcessor;
            }
            if (_resourceProcessor is not null)
            {
                _resourceProcessor.Processor.ProductionStarted += OnProductionStarted;
                _resourceProcessor.Processor.ResourceCollected += OnResourceCollected;
            }
        }

        private void OnDisable()
        {
            CameraController.OnClick -= OnClick;
            if (_resourceProcessor is null && _targetBuilding.TryGetComponent(out IResourceProcessor resourceProcessor))
            {
                _resourceProcessor = resourceProcessor;
            }
            if (_resourceProcessor is not null)
            {
                _resourceProcessor.Processor.ProductionStarted -= OnProductionStarted;
                _resourceProcessor.Processor.ResourceCollected -= OnResourceCollected;
            }
        }

        private void OnProductionStarted(ProducibleItem item)
        {
            DeselectBuilding();
        }

        private void OnResourceCollected(ProducibleItem item)
        {
            DeselectBuilding();
        }

        private void SelectBuilding()
        {
            if (_isBuildingSelected) return;
            if (_resourceProcessor is not null)
            {
                _isBuildingSelected = true;
                ClearButtons();
                foreach (var item in _targetBuilding.Data.Recipes)
                {
                    AddButton(item, () =>
                    {
                        _resourceProcessor.Processor.ProduceResource(item.ResultItems.Item);
                        if (_resourceProcessor.Processor.IsProducing)
                        {
                            SetVisibility(false);
                        }
                    });
                }
                SetVisibility(true);
            }
        }

        private void DeselectBuilding()
        {
            _isBuildingSelected = false;
            SetVisibility(false);
        }

        private void OnClick()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (((1 << hit.collider.gameObject.layer) & _buildingLayerMask) != 0)
                {
                    if (hit.collider.gameObject == _targetBuilding.gameObject && _targetBuilding.enabled && !_resourceProcessor.Processor.IsProducing)
                    {
                        SelectBuilding();
                        return;
                    }
                }
            }
            DeselectBuilding();
            return;
        }
    }
}