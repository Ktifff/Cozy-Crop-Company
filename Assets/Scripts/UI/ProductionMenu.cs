using Game.Controllers;
using Game.Entities.Buildings;
using UnityEngine;
using Game.Data;
using UnityEngine.Events;

namespace Game.UI
{
    public class ProductionMenu : SelectionMenu<ItemButton, ItemData>
    {
        [SerializeField] private Building _targetBuilding;
        [SerializeField] private LayerMask _buildingLayerMask;

        private Camera _camera;
        private bool _isBuildingSelected;
        private IResourceProducer _resourceProducer;

        protected override ItemButton AddButton(ItemData data, UnityAction call = null)
        {
            var result = base.AddButton(data, call);
            result.Init(data.Icon, call);
            return result;
        }

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void OnEnable()
        {
            CameraController.OnClick += OnClick;
            if (_resourceProducer is null && _targetBuilding.TryGetComponent(out IResourceProducer resourceProducer))
            {
                _resourceProducer = resourceProducer;
            }
            if (_resourceProducer is not null)
            {
                _resourceProducer.Producer.ProductionStarted += OnProductionStarted;
                _resourceProducer.Producer.ResourceCollected += OnResourceCollected;
            }
        }

        private void OnDisable()
        {
            CameraController.OnClick -= OnClick;
            if (_resourceProducer is null && _targetBuilding.TryGetComponent(out IResourceProducer resourceProducer))
            {
                _resourceProducer = resourceProducer;
            }
            if (_resourceProducer is not null)
            {
                _resourceProducer.Producer.ProductionStarted -= OnProductionStarted;
                _resourceProducer.Producer.ResourceCollected -= OnResourceCollected;
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
            if (_resourceProducer is not null)
            {
                _isBuildingSelected = true;
                ClearButtons();
                foreach (var item in _targetBuilding.Data.ProducibleItems)
                {
                    AddButton(item.ResultItems.Item, () => 
                    { 
                        _resourceProducer.Producer.ProduceResource(item.ResultItems.Item);
                        SetVisibility(false);
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
                    if (hit.collider.gameObject == _targetBuilding.gameObject && _targetBuilding.enabled && !_resourceProducer.Producer.IsProducing)
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