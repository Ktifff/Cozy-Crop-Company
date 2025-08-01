using Game.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.UI
{
    public abstract class SelectionMenu<T1, T2> : MonoBehaviour where T1 : MonoBehaviour
    {
        private static readonly int _isShownAnimatorHash = Animator.StringToHash("IsShown");
        private static readonly int _messageAnimatorHash = Animator.StringToHash("Message");

        private readonly List<T1> _buttons = new List<T1>();

        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _content;
        [SerializeField] private T1 _itemButtonPrefab;

        protected void ClearButtons()
        {
            foreach(var button in _buttons)
            {
                Destroy(button.gameObject);
            }
            _buttons.Clear();
        }

        protected virtual T1 AddButton(T2 data, UnityAction call = null)
        {
            T1 button = Instantiate(_itemButtonPrefab, _content);
            _buttons.Add(button);
            return button;
        }

        protected void ShowMessage()
        {
            _animator.SetTrigger(_messageAnimatorHash);
        }

        protected void SetVisibility(bool value)
        {
            _animator.SetBool(_isShownAnimatorHash, value);
        }
    }
}