using UnityEngine;
using UnityEngine.Audio;

namespace Game.Audio
{
    public class AudioMixerManager : MonoBehaviour
    {
        private static AudioMixerManager _instance;

        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private AudioSource _audioSource;
        [Space]
        [SerializeField] private AudioClip _collectSound;
        [SerializeField] private AudioClip _buttonSound;
        [SerializeField] private AudioClip _buildingSound;
        [SerializeField] private AudioClip _orderSound;
        [SerializeField] private AudioClip _purchaseSound;

        public static void PlayCollectSound() => _instance._audioSource.PlayOneShot(_instance._collectSound);
        public static void PlayButtonSound() => _instance._audioSource.PlayOneShot(_instance._buttonSound);
        public static void PlayBuildingSound() => _instance._audioSource.PlayOneShot(_instance._buildingSound);
        public static void PlayOrderSound() => _instance._audioSource.PlayOneShot(_instance._orderSound);
        public static void PlayPurchaseSound() => _instance._audioSource.PlayOneShot(_instance._purchaseSound);

        public void SetMasterVolume(float value) => _audioMixer.SetFloat("MasterVolume", value);
        public void SetSoundVolume(float value) => _audioMixer.SetFloat("SoundsVolume", value);
        public void SetMusicVolume(float value) => _audioMixer.SetFloat("MusicVolume", value);

        private void Awake()
        {
            _instance = this;
        }
    }
}