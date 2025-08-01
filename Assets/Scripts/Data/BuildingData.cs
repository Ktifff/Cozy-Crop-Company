using Game.Entities.Buildings;
using System;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "BuildingData", menuName = "Scriptable Objects/BuildingData")]
    public class BuildingData : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private Sprite _icon;
        [SerializeField] private Building _prefab;
        [SerializeField] private Vector2Int _size;
        [SerializeField] private RecipeItemData[] _buildMaterials;
        [Header("ItemProducer")]
        [SerializeField] private bool _isItemProducer;
        [SerializeField] private ProducibleItem[] _producibleItems;
        [Header("ItemProcessor")]
        [SerializeField] private bool _isItemProcessor;
        [SerializeField] private ItemRecipe[] _recipes;

        public string Name => _name;
        public Sprite Icon => _icon;
        public Building Prefab => _prefab;
        public Vector2Int Size => _size;
        public RecipeItemData[] BuildMaterials => _buildMaterials;
        public bool IsItemProducer => _isItemProducer;
        public ProducibleItem[] ProducibleItems => _producibleItems;
        public bool IsItemPrecessor => _isItemProcessor;
        public ItemRecipe[] Recipes => _recipes;
    }

    [Serializable]
    public class ProducibleItem
    {
        [SerializeField] protected RecipeItemData _resultItems;
        [SerializeField] protected float _productionTime;

        public RecipeItemData ResultItems => _resultItems;
        public float ProductionTime => _productionTime;
    }

    [Serializable]
    public class ItemRecipe : ProducibleItem
    {
        [SerializeField] private RecipeItemData[] _recipeItems;

        public RecipeItemData[] RecipeItems => _recipeItems;
    }

    [Serializable]
    public class RecipeItemData
    {
        [SerializeField] private ItemData _item;
        [SerializeField] private int _count;

        public ItemData Item => _item;
        public int Count => _count;
    }
}