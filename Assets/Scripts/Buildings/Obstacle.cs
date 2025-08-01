using Game.Data;
using Game.Managers;
using UnityEngine;

namespace Game.Entities.Buildings
{
    public class Obstacle : Building, IResourceProcessor
    {
        private static readonly int _breakAnimatorHash = Animator.StringToHash("Break");
        private static readonly int _destroyAnimatorHash = Animator.StringToHash("Destroy");

        public ResourceProcessor Processor { get; private set; } = new();

        [SerializeField] private Animator _animator;
        [SerializeField] private BoxCollider _collider;
        [SerializeField] private ParticleSystem _shards;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _shardSound;

        public void OnDestroyAnimationFinished()
        {
            Destroy(gameObject);
        }

        public void PlayShards()
        {
            _shards.Play();
            _audioSource.PlayOneShot(_shardSound);
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
            _animator.SetBool(_breakAnimatorHash, true);
        }

        private void OnProductionFinished(ProducibleItem item)
        {
            _animator.SetBool(_destroyAnimatorHash, true);
            _collider.enabled = false;
            Processor.CollectResource();
            GameManager.RemoveBuilding(this);
        }
    }
} 