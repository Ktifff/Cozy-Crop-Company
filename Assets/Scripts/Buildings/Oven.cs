using Game.Data;
using UnityEngine;

namespace Game.Entities.Buildings
{
    public class Oven : Building, IResourceProcessor
    {
        private static readonly int _workAnimatorHash = Animator.StringToHash("Work");

        public ResourceProcessor Processor { get; private set; } = new();

        [SerializeField] private Animator _animator;
        [SerializeField] private ParticleSystem _fire;

        public void PlayFire()
        {
            _fire.Play();
        }

        public void StopFire()
        {
            _fire.Stop();
        }

        private void OnEnable()
        {
            Processor.ProductionStarted += OnProductionStarted;
            Processor.ProductionFinished += OnProductionFinished;
        }

        private void OnDisable()
        {
            Processor.ProductionStarted -= OnProductionStarted;
            Processor.ProductionFinished -= OnProductionFinished;
        }

        private void OnProductionStarted(ProducibleItem item)
        {
            _animator.SetBool(_workAnimatorHash, true);
        }

        private void OnProductionFinished(ProducibleItem item)
        {
            _animator.SetBool(_workAnimatorHash, false);
        }
    }
}