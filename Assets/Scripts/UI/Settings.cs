using UnityEngine;

namespace Game.UI
{
    public class Settings : MonoBehaviour
    {
        private static readonly int _isShownAnimatorHash = Animator.StringToHash("IsShown");

        [SerializeField] private Animator _animator;

        public void SetVisibility(bool value)
        {
            _animator.SetBool(_isShownAnimatorHash, value);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}