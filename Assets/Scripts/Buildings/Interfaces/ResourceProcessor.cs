using Game.Data;
using System.Linq;
using System.Threading.Tasks;
using Game.Managers;

namespace Game.Entities.Buildings
{
    public class ResourceProcessor : ResourceProducer
    {
        public override async void ProduceResource(ItemData item)
        {
            if (IsProducing) return;
            var producibleItem = _producibleItems.OfType<ItemRecipe>().FirstOrDefault(p => p.ResultItems.Item == item);
            if (producibleItem is null) return;
            await ProduceResourceAsync(producibleItem);
        }

        protected override async Task ProduceResourceAsync(ProducibleItem item)
        {
            if (item is ItemRecipe itemRecipe && GameManager.PlayerInventory.UseRecipe(itemRecipe.RecipeItems))
            {
                await base.ProduceResourceAsync(item);
            }
        }
    }
}