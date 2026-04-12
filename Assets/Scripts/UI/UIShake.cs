using System;
using DG.Tweening;
using UnityEngine;

namespace PlayMeowInterview.UI
{
    [Serializable]
    public struct ShakeSettings
    {
        public bool enabled;
        public float duration;
        public Vector3 strength;
        [Range(1, 50)] 
        public int vibrato;
        [Range(0f, 180f)] 
        public float randomness;
        public bool fadeOut;
    }

    public class UIShake : MonoBehaviour
    {
        [SerializeField] private ShakeSettings positionShake = DefaultShakeSettings;

        [SerializeField] private ShakeSettings rotationShake = DefaultShakeSettings;

        private Sequence _shakeSequence;
        private static ShakeSettings DefaultShakeSettings => new ShakeSettings
        {
            enabled = true,
            duration = 0.5f,
            strength = new Vector3(10f, 10f, 10f),
            vibrato = 10,
            randomness = 90f,
            fadeOut = true
        };

        public void Shake()
        {
            _shakeSequence?.Kill(true);
            _shakeSequence = DOTween.Sequence();

            if (positionShake.enabled)
            {
                _shakeSequence.Join(transform.DOShakePosition(
                    positionShake.duration,
                    positionShake.strength,
                    positionShake.vibrato,
                    positionShake.randomness,
                    false,
                    positionShake.fadeOut
                ));
            }

            if (rotationShake.enabled)
            {
                _shakeSequence.Join(transform.DOShakeRotation(
                    rotationShake.duration,
                    rotationShake.strength,
                    rotationShake.vibrato,
                    rotationShake.randomness,
                    rotationShake.fadeOut
                ));
            }

            _shakeSequence.Play();
        }

        private void OnDestroy()
        {
            _shakeSequence?.Kill();
        }
    }
}
