using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace PlayMeow.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class TypeWriterEffect : MonoBehaviour
    {
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private float charsPerSecond = 30f;

        private Coroutine _typingCoroutine;

        private void Start()
        {
            Play(dialogueText.text);
        }

        public void Play(string message)
        {
            if (_typingCoroutine != null)
            {
                StopCoroutine(_typingCoroutine);
            }

            _typingCoroutine = StartCoroutine(TypeRoutine(message));
        }

        private IEnumerator TypeRoutine(string message)
        {
            dialogueText.text = message;
            dialogueText.maxVisibleCharacters = 0;

            dialogueText.ForceMeshUpdate(); // ensures textInfo is valid immediately

            int totalChars = dialogueText.textInfo.characterCount;

            float timer = 0f;
            int visibleCount = 0;

            while (visibleCount < totalChars)
            {
                timer += Time.deltaTime * charsPerSecond;
                visibleCount = Mathf.FloorToInt(timer);

                dialogueText.maxVisibleCharacters = visibleCount;

                yield return null;
            }

            dialogueText.maxVisibleCharacters = totalChars;
        }
    }
}