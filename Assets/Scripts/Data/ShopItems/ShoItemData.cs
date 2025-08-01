using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "ShopItemData", menuName = "Scriptable Objects/ShopItemData")]
    public class ShopItemData : ScriptableObject
    {
        [SerializeField] private RecipeItemData _price;
        [SerializeField] private RecipeItemData _reward;

        public RecipeItemData Price => _price;
        public RecipeItemData Reward => _reward;
    }
}