using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.UI
{
    public class ItemButton : MonoBehaviour
    {
        [SerializeField] protected Image _image;
        [SerializeField] protected Image _backImage;
        [SerializeField] protected Image _shadow;
        [SerializeField] protected Button _button;

        public void Init(Sprite image, UnityAction call = null)
        {
            _image.sprite = _backImage.sprite = _shadow.sprite = image;
            if (call is not null)
            {
                _button.onClick.AddListener(call);
            }
        }

        public void SetFillAmount(float value)
        {
            value = Mathf.Clamp01(value);
            _image.fillAmount = value;
            _shadow.fillAmount = value;
        }
    }
}