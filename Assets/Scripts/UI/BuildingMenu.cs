using Game.Controllers;
using Game.Data;
using Game.Managers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace Game.UI
{
    public class BuildingMenu : MonoBehaviour
    {
        private static readonly int _isShownAnimatorHash = Animator.StringToHash("IsShown");
        private static BuildingMenu _instance;

        private readonly List<ResourceRequirement> _recipes = new List<ResourceRequirement>();

        [SerializeField] private UnityEvent _onBuildingPlacementStarted;
        [Space]
        [SerializeField] private BuildingData[] _buildingsDatabase;
        [SerializeField] private BuildingData[] _availableBuildingsDatabase;
        [Space]
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _content;
        [SerializeField] private ResourceRequirement _resourceRequirementPrefab;
        [SerializeField] private BuildingPlacer _buildingPlacer;

        public static BuildingData GetBuildingByName(string name)
        {
            return _instance._buildingsDatabase.FirstOrDefault(x => x.name == name);
        }

        public void SetVisibility(bool value)
        {
            _animator.SetBool(_isShownAnimatorHash, value);
        }

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            foreach (var building in _availableBuildingsDatabase)
            {
                _recipes.Add(AddBuildingRecipe(building));
            }
        }

        private ResourceRequirement AddBuildingRecipe(BuildingData buildingData)
        {
            ResourceRequirement recipe = Instantiate(_resourceRequirementPrefab, _content);
            recipe.Init(buildingData.Icon, buildingData.Name, () => { StartBuilding(buildingData); });
            foreach (var item in buildingData.BuildMaterials)
            {
                recipe.AddRequestedResource(item.Item, item.Count);
            }
            return recipe;
        }

        private void StartBuilding(BuildingData buildingData)
        {
            if (GameManager.PlayerInventory.UseRecipe(buildingData.BuildMaterials))
            {
                _buildingPlacer.StartBuildingPlacement(buildingData);
                _onBuildingPlacementStarted?.Invoke();
            }
        }
    }
}