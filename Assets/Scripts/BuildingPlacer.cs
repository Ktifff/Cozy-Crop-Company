using Game.Audio;
using Game.Data;
using Game.Entities.Buildings;
using Game.Managers;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Controllers
{
    public class BuildingPlacer : MonoBehaviour
    {
        public event Action<Building> BuildingPlaced;

        private static BuildingPlacer _instance;

        [SerializeField] private UnityEvent _onBuildingPlacementFinished;
        [Header("Grid Settings")]
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private LayerMask _placedStructureMask;
        [SerializeField] private GameObject[] _greenBlocks;
        [SerializeField] private GameObject[] _redBlocks;
        [SerializeField] private Obstacle[] _obstacles;

        private Building _previewObject;
        private ChessplaneBuilder _greenPlane;
        private ChessplaneBuilder _redPlane;

        public void SpawnObstacles(bool newGame)
        {
            if (newGame)
            {
                foreach(var obstacle in _obstacles)
                {
                    BuildingPlaced?.Invoke(obstacle);
                }
            }
            else
            {
                foreach (var obstacle in _obstacles)
                {
                    Destroy(obstacle.gameObject);
                }
            }
        }

        public static void PlaceBuilding(Building building, Vector3 position)
        {
            if (_instance is null) return;
            building.transform.position = position;
            if (_instance.IsPlacementValid(position, building.Data.Size))
            {
                building.enabled = true;
                if (_instance._greenPlane is not null)
                {
                    _instance._greenPlane.Destroy();
                }
                if (_instance._redPlane is not null)
                {
                    _instance._redPlane.Destroy();
                }
                _instance.BuildingPlaced?.Invoke(building);
                _instance._onBuildingPlacementFinished?.Invoke();
            }
        }

        public void StartBuildingPlacement(BuildingData buildingData)
        {
            if (_previewObject == null)
            {
                _previewObject = Instantiate(buildingData.Prefab);
                _greenPlane = new ChessplaneBuilder(_greenBlocks, buildingData.Size, 0.01f);
                _redPlane = new ChessplaneBuilder(_redBlocks, buildingData.Size, 0.01f);
                AudioMixerManager.PlayBuildingSound();
            }
        }

        private void Awake()
        {
            _instance = this;
        }

        private void Update()
        {
            if (_previewObject == null) return;
            Vector3? gridPosition = GetMouseGridPosition();
            if (gridPosition.HasValue)
            {
                Vector3 snappedPos = SnapToGrid(gridPosition.Value, _previewObject.Data.Size);
                UpdatePreview(snappedPos);
                if (Input.GetMouseButtonDown(1))
                {
                    PlaceBuilding(_previewObject, snappedPos);
                    _previewObject = null;
                }
            }
        }

        private Vector3? GetMouseGridPosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, _groundMask))
            {
                return hit.point;
            }
            return null;
        }

        private Vector3 SnapToGrid(Vector3 position, Vector2Int size)
        {
            Vector2 groundOffset = new Vector2((size.x % 2 == 0) ? 0.5f : 0f, (size.y % 2 == 0) ? 0.5f : 0f);
            Vector2 offset = new Vector2((size.x % 2 == 0) ? 0 : 0.5f, (size.y % 2 == 0) ? 0 : 0.5f);
            float x = Mathf.Floor(position.x + groundOffset.x) + offset.x;
            float z = Mathf.Floor(position.z + groundOffset.y) + offset.y;
            return new Vector3(x, 0, z);
        }

        private void UpdatePreview(Vector3 position)
        {
            _previewObject.transform.position = _greenPlane.Position = _redPlane.Position = position;
            bool isValidPlace = IsPlacementValid(position, _previewObject.Data.Size);
            _greenPlane.SetActive(isValidPlace);
            _redPlane.SetActive(!isValidPlace);
        }

        private bool IsPlacementValid(Vector3 position, Vector2Int size)
        {
            Collider[] colliders = Physics.OverlapBox(position, new Vector3(0.5f * size.x - 0.01f, 1, 0.5f * size.y - 0.01f), Quaternion.identity, _placedStructureMask);
            return colliders.Length < 2; 
        }
    }
}