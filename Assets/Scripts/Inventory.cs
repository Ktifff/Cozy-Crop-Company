using Game.Data;
using Game.Managers;
using Game.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entities
{
    public class Inventory : ISaveable
    {
        public event Action<ItemData, int> ItemCountChanged;

        private readonly Dictionary<ItemData, int> _items = new();

        public string Save()
        {
            InventorySave save = new InventorySave(_items);
            return JsonUtility.ToJson(save, true);
        }

        public void Load(string save)
        {
            var json = JsonUtility.FromJson<InventorySave>(save);
            for (int i = 0; i < json.Items.Length; i++)
            {
                ItemData item = InventoryView.GetItemByName(json.Items[i].Name);
                if (item != null)
                {
                    AddItem(item, json.Items[i].Count);
                }
            }
        }

        public int GetItemCount(ItemData item)
        {
            CheckItem(item);
            return _items[item];
        }

        public bool UseRecipe(RecipeItemData[] items)
        {
            foreach(var item in items)
            {
                CheckItem(item.Item);
                if (_items[item.Item] < item.Count)
                {
                    return false;
                }
            }
            foreach (var item in items)
            {
                AddItem(item.Item , - item.Count);
            }
            return true;
        }

        public void AddItem(ItemData item, int count)
        {
            CheckItem(item);
            int temp = _items[item] + count;
            if (temp >= 0)
            {
                _items[item] = temp;
                ItemCountChanged?.Invoke(item, temp);
            }
        }

        private void CheckItem(ItemData item)
        {
            if (!_items.ContainsKey(item))
            {
                _items.Add(item, 0);
            }
        }

        [Serializable]
        public class InventorySave
        {
            public ItemSave[] Items;

            public InventorySave(Dictionary<ItemData, int> items)
            {
                List<ItemSave> save = new();
                foreach(var item in items)
                {
                    save.Add(new ItemSave(item.Key.name, item.Value));
                }
                Items = save.ToArray();
            }

            [Serializable]
            public class ItemSave
            {
                public string Name;
                public int Count;

                public ItemSave(string name, int count)
                {
                    Name = name;
                    Count = count;
                }
            }
        }
    }
}