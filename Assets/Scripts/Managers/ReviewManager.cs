using System.Collections;
using System.Collections.Generic;
using Data;
using DG.Tweening;
using Review;
using UnityEngine;

namespace Managers
{
    public class ReviewManager : MonoBehaviour
    {
        public static ReviewManager Instance;

        [SerializeField] private List<AnswerButtonReference> m_answerButtonReferences;
        [SerializeField] private float                       m_speedFadeListAnswers = 0.25f;

        private CanvasGroup m_canvasGroup;
        private TrainingSo  m_currentTraining;

        public void Start()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            m_canvasGroup       = GetComponent<CanvasGroup>();
            // m_canvasGroup.alpha = 0.0f;
            gameObject.SetActive(false);
            AnswerButtonReference.GoodAnswser += ShuffleAnswers;
        }

        private void ShuffleAnswers()
        {
            StartCoroutine(ShuffleAnswersCoroutine());
        }

        private IEnumerator ShuffleAnswersCoroutine()
        {
            m_answerButtonReferences.Shuffle();

            if (m_currentTraining.m_listOfCards.Count == m_answerButtonReferences.Count)
            {
                for (int i = 0; i < m_answerButtonReferences.Count; ++i)
                {
                    var buttonReference = m_answerButtonReferences[i];
                    var card = m_currentTraining.m_listOfCards[i];
                    var image = buttonReference.ImageButton;
                    var text = buttonReference.TextButton;

                    Sequence sequence = DOTween.Sequence();

                    sequence.Append(image.DOFade(0.0f, 0.25f));
                    sequence.Join(text.DOFade(0.0f, 0.25f));
                    sequence.AppendCallback(() => buttonReference.Populate(card));
                    sequence.Append(text.DOFade(1.0f, 0.25f));
                    sequence.Join(image.DOFade(1.0f, 0.25f));

                    yield return new WaitForSeconds (0.20f);
                }
            }

            yield return null;
        }

        public void PopulateAnswers(TrainingSo p_currentTraining)
        {
            m_currentTraining = p_currentTraining;
            ShuffleAnswers();
        }

        public void ShowListAnswers()
        {
            gameObject.SetActive(true);
            // DOTween.To(p_value => m_canvasGroup.alpha = p_value, 0.0f, 1.0f, m_speedFadeListAnswers);
        }

        public void HideListAnswers()
        {
            // DOTween.To(p_value => m_canvasGroup.alpha = p_value, 1.0f, 0.0f, m_speedFadeListAnswers);
            gameObject.SetActive(false);
        }
    }
}