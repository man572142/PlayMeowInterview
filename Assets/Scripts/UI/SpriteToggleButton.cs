using System;
using UnityEngine;
using UnityEngine.UI;

namespace PlayMeow.UI
{
    public class SpriteToggleButton : MonoBehaviour
    {
        public event Action<bool> OnValueChanged;
        
        [Header("Button")]
        [SerializeField] private Button button;

        [Header("Target Image")]
        [SerializeField] private Image targetImage;

        [Header("Sprites")]
        [SerializeField] private Sprite offSprite;
        [SerializeField] private Sprite onSprite;

        [Header("State")]
        [SerializeField] private bool isOn;

        private void Awake()
        {
            button.onClick.AddListener(OnClicked);
            Apply();
        }

        private void Start()
        {
            OnValueChanged?.Invoke(isOn);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(OnClicked);
        }

        private void OnClicked()
        {
            isOn = !isOn;
            Apply();
            OnValueChanged?.Invoke(isOn);
        }

        private void Apply()
        {
            targetImage.sprite = isOn ? onSprite : offSprite;
        }
    }
}
