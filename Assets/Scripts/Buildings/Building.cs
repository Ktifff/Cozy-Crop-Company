using Game.Controllers;
using Game.Data;
using Game.Managers;
using System;
using UnityEngine;

namespace Game.Entities.Buildings
{
    public abstract class Building : MonoBehaviour, ISaveable
    {
        [SerializeField] protected BuildingData _data;

        public BuildingData Data => _data;

        public virtual void Load(string save)
        {
            enabled = true;
            var json = JsonUtility.FromJson<BuildingSave>(save);
            BuildingPlacer.PlaceBuilding(this, json.Position);
            if (_data.IsItemPrecessor && this is IResourceProcessor resourceProcessor)
            {
                resourceProcessor.Processor.Load(json.Producer);
            }
            else if (_data.IsItemProducer && this is IResourceProducer resourceProducer)
            {
                resourceProducer.Producer.Load(json.Producer);
            }
        }

        public virtual string Save()
        {
            string producerSave = null;
            if (_data.IsItemPrecessor && this is IResourceProcessor resourceProcessor)
            {
                producerSave = resourceProcessor.Processor.Save();
            }
            else if (_data.IsItemProducer && this is IResourceProducer resourceProducer)
            {
                producerSave = resourceProducer.Producer.Save();
            }
            BuildingSave save = new BuildingSave(transform.position, producerSave);
            string json = JsonUtility.ToJson(save, true);
            return json;
        }

        protected virtual void Awake()
        {
            if (_data.IsItemProducer && this is IResourceProducer resourceProducer)
            {
                resourceProducer.Producer.SetProducibleItems(_data.ProducibleItems);
            }
            if (_data.IsItemPrecessor && this is IResourceProcessor resourceProcessor)
            {
                resourceProcessor.Processor.SetProducibleItems(_data.Recipes);
            }
            if (this is not Obstacle)
            {
                enabled = false;
            }
        }

        [Serializable]
        protected class BuildingSave
        {
            public Vector3 Position;
            public string Producer;

            public BuildingSave(Vector3 position, string producer)
            {
                Position = position;
                Producer = producer;
            }
        }
    }
}