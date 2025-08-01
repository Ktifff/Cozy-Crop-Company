using Game.Entities;
using UnityEngine;
using System.IO;
using System;
using Game.Controllers;
using Game.Entities.Buildings;
using System.Collections.Generic;
using Game.UI;
using Game.Data;

namespace Game.Managers
{
    public class GameManager : MonoBehaviour
    {
        private static string _savePath;
        private static GameManager _instance;

        private readonly Inventory _playerInventory = new();

        [SerializeField] private BuildingPlacer _buildingPlacer;
        [SerializeField] private OrdersMenu _ordersMenu;
        [SerializeField] private StartingItem[] _startingItems;

        private List<Building> _buildings = new();

        public static Inventory PlayerInventory => _instance._playerInventory;

        public static void RemoveBuilding(Building building)
        {
            if (_instance is null || !_instance._buildings.Contains(building)) return;
            _instance._buildings.Remove(building);
        }

        private void Awake()
        {
            _instance = this;
            _savePath = Path.Combine(Application.dataPath, "save.json");
        }

        private void Start()
        {
            Load();
        }

        void OnApplicationQuit()
        {
            Save();
        }

        private void OnEnable()
        {
            _buildingPlacer.BuildingPlaced += OnBuildingPlaced;
        }

        private void OnDisable()
        {
            _buildingPlacer.BuildingPlaced -= OnBuildingPlaced;
        }

        private void OnBuildingPlaced(Building building)
        {
            _buildings.Add(building);
        }

        public void Load()
        {
            if (File.Exists(_savePath))
            {
                _buildingPlacer.SpawnObstacles(false);
                string save = File.ReadAllText(_savePath);
                var json = JsonUtility.FromJson<GameSave>(save);
                for (int i = 0; i < json.Buildings.Length; i++)
                {
                    BuildingData buildingData = BuildingMenu.GetBuildingByName(json.Buildings[i].Name);
                    if (buildingData is not null)
                    {
                        Building building = Instantiate(buildingData.Prefab);
                        building.Load(json.Buildings[i].Save);
                    }
                }
                PlayerInventory.Load(json.Inventory);
                _ordersMenu.Load(json.Orders);
            }
            else
            {
                _buildingPlacer.SpawnObstacles(true);
                _ordersMenu.Load(null);
                foreach(var item in _startingItems)
                {
                    PlayerInventory.AddItem(item.Item, item.Count);
                }
            }
        }

        public void Save()
        {
            GameSave gameSave = new GameSave(_buildings, PlayerInventory.Save(), _ordersMenu.Save());
            string json = JsonUtility.ToJson(gameSave, true);
            File.WriteAllText(_savePath, json);
        }

        [Serializable]
        public class GameSave
        {
            public JsonSave[] Buildings;
            public string Inventory;
            public string Orders;

            public GameSave(List<Building> buildings, string inventory, string orders)
            {
                List<JsonSave> buildingsSave = new List<JsonSave>();
                for (int i = 0; i < buildings.Count; i++)
                {
                    if (buildings[i] != null)
                    {
                        buildingsSave.Add(new JsonSave(buildings[i].Data.name, buildings[i].Save()));
                    }
                }
                Buildings = buildingsSave.ToArray();
                Inventory = inventory;
                Orders = orders;
            }

            [Serializable]
            public class JsonSave
            {
                public string Name;
                public string Save;

                public JsonSave(string name, string save)
                {
                    Name = name;
                    Save = save;
                }
            }
        }


        [Serializable]
        public class StartingItem
        {
            [SerializeField] private ItemData _item;
            [SerializeField] private int _count;

            public ItemData Item => _item;
            public int Count => _count;
        }
    }

    public interface ISaveable
    {
        string Save();
        void Load(string save);
    }
}