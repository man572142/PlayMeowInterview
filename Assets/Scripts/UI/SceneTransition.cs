using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayMeow.UI
{
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private string targetSceneName;

        public void StartTransition()
        {
            StartCoroutine(LoadSceneWithFade());
        }

        private IEnumerator LoadSceneWithFade()
        {
            var asyncOp = SceneManager.LoadSceneAsync(targetSceneName);
            asyncOp.allowSceneActivation = false;

            canvasGroup.DOFade(0f, 0.5f);

            yield return new WaitUntil(() => canvasGroup.alpha < 0.1f && asyncOp.progress >= 0.9f);

            asyncOp.allowSceneActivation = true;
        }
    }
}