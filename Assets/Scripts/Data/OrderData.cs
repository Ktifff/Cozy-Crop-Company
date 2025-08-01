using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "OrderData", menuName = "Scriptable Objects/OrderData")]
    public class OrderData : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private RecipeItemData[] _orderRequest;
        [SerializeField] private RecipeItemData _reward;

        public string Name => _name;
        public RecipeItemData[] OrderRequest => _orderRequest;
        public RecipeItemData Reward => _reward;
    }
}