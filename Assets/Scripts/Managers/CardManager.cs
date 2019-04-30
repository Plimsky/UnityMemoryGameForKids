using System;
using Card;
using Data;
using DG.Tweening;
using Review;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

namespace Managers
{
    public class CardManager : MonoBehaviour
    {
        public        Action      FinishedReviewDeck;
        public static CardManager Instance;

        #region Properties Showed in Inspector

        [Header("References")] [SerializeField]
        private Button m_continueButton;

        [SerializeField] private GameObject  m_card;
        [SerializeField] private ContentSide m_frontContentSide;
        [SerializeField] private ContentSide m_backContentSide;

        [Header("Card configuration")] [SerializeField]
        private float m_speedCardRotation = 0.25f;

        [SerializeField] private float m_speedFadeCardContent    = 0.25f;
        [SerializeField] private float m_speedFadeContinueButton = 0.25f;

        #endregion

        #region Private Properties

        private TrainingSo m_selectedTraining;
        private GameObject m_mainContent;
        private Button     m_cardClickable;

        private bool m_isFrontSide;
        private int  m_totalCardCurrentTraining;
        private int  m_currentCardCurrentTraining;
        private bool m_flippedCardAtLeastOnce;

        private CanvasGroup m_frontCanvasGroup;
        private CanvasGroup m_backCanvasGroup;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        #endregion

        #region Public Methods

        public void Init()
        {
            m_mainContent   = m_card.transform.GetChild(0).gameObject;
            m_cardClickable = m_card.GetComponent<Button>();

            m_cardClickable.onClick.AddListener(TouchCard);
            m_continueButton.onClick.AddListener(ToNextCard);

            m_isFrontSide = true;

            m_frontContentSide.Init();
            m_backContentSide.Init();
            UpdateCardContent();
            m_continueButton.gameObject.SetActive(false);

            m_frontCanvasGroup = m_frontContentSide.GetComponent<CanvasGroup>();
            m_backCanvasGroup  = m_backContentSide.GetComponent<CanvasGroup>();
        }

        public void SetNewTraining(TrainingSo p_training)
        {
            m_selectedTraining           = p_training;
            m_currentCardCurrentTraining = 0;
            m_totalCardCurrentTraining   = m_selectedTraining.m_listOfCards.Count;
        }

        public bool IsRightAnswer(string p_answerText)
        {
            return m_selectedTraining.m_listOfCards[m_currentCardCurrentTraining].m_backTitle == p_answerText;
        }

        public void EnableTouchCard()
        {
            m_cardClickable.interactable = true;
        }

        public void DisableTouchCard()
        {
            m_cardClickable.interactable = false;
        }

        public bool IsEndOfTrainingDeck()
        {
            return m_currentCardCurrentTraining == m_totalCardCurrentTraining - 1;
        }

        public void NextTestCard()
        {
            ToNextCard();
        }

        public void NextTraining(TrainingSo p_selectedTraining)
        {
            SetNewTraining(p_selectedTraining);
            EnableTouchCard();
            UpdateCardContent();
        }

        #endregion

        #region Private Methods

        private void UpdateCardContent()
        {
            var currentCard = m_selectedTraining.m_listOfCards[m_currentCardCurrentTraining];

            m_frontContentSide.UpdateReferences(currentCard, true);
            m_backContentSide.UpdateReferences(currentCard, false);
        }

        private void TouchCard()
        {
            FlipCard();

            if (!m_flippedCardAtLeastOnce)
            {
                ShowContinueButton();
                m_flippedCardAtLeastOnce = true;
            }
        }

        private void ShowContinueButton()
        {
            Sequence continueButtonsequence = DOTween.Sequence();
            continueButtonsequence.Append(m_continueButton.GetComponent<Image>().DOFade(0.0f, 0f));
            continueButtonsequence.AppendCallback(() => m_continueButton.gameObject.SetActive(true));
            continueButtonsequence.AppendCallback(() => m_continueButton.interactable = true);
            continueButtonsequence.Append(m_continueButton.GetComponent<Image>().DOFade(1.0f, m_speedFadeContinueButton));
        }

        private void HideContinueButton()
        {
            Sequence continueButtonsequence = DOTween.Sequence();
            continueButtonsequence.AppendCallback(() => m_continueButton.interactable = false);
            continueButtonsequence.Append(m_continueButton.GetComponent<Image>().DOFade(1.0f, 0f));
            continueButtonsequence.Append(m_continueButton.GetComponent<Image>().DOFade(0.0f, m_speedFadeContinueButton));
            continueButtonsequence.AppendCallback(() => m_continueButton.gameObject.SetActive(false));
        }

        private void ToNextCard()
        {
            if (m_currentCardCurrentTraining < m_totalCardCurrentTraining - 1)
            {
                m_currentCardCurrentTraining++;
                UpdateNextCard();
            }
            else if (
                m_currentCardCurrentTraining == m_totalCardCurrentTraining - 1)
            {
                m_currentCardCurrentTraining = 0;
                UpdateNextCard();

                if (GameManager.Instance.CurrentTrainingState == GameManager.State.REVIEW)
                    FinishedReviewDeck?.Invoke();
            }
        }

        private void UpdateNextCard()
        {
            HideContinueButton();

            if (!m_isFrontSide)
                FlipCard(true);
            else
            {
                Sequence sequence = DOTween.Sequence();
                sequence.Append(DOTween.To(p_value => m_frontCanvasGroup.alpha = p_value, 1.0f, 0.0f, m_speedFadeCardContent));
                sequence.AppendCallback(UpdateCardContent);
                sequence.Append(DOTween.To(p_value => m_frontCanvasGroup.alpha = p_value, 0.0f, 1.0f, m_speedFadeCardContent));
            }

            m_flippedCardAtLeastOnce = false;
        }

        private void FlipCard(bool p_forceUpdate = false)
        {
            Sequence sequence             = DOTween.Sequence();
            float    yRotationDestination = Math.Abs(m_mainContent.transform.localEulerAngles.y) < Mathf.Epsilon ? 180.0f : 0.0f;

            sequence.AppendCallback(() => m_cardClickable.enabled = false);
            sequence.Append(m_mainContent.transform.DOLocalRotate(new Vector3(0, 90.0f, 0), m_speedCardRotation));

            if (p_forceUpdate)
                sequence.AppendCallback(UpdateCardContent);

            sequence.AppendCallback(() => m_frontContentSide.gameObject.SetActive(!m_frontContentSide.gameObject.activeSelf));
            sequence.AppendCallback(() => m_backContentSide.gameObject.SetActive(!m_backContentSide.gameObject.activeSelf));
            sequence.AppendCallback(() => m_isFrontSide = !m_isFrontSide);
            sequence.Append(m_mainContent.transform.DOLocalRotate(new Vector3(0, yRotationDestination, 0), m_speedCardRotation));
            sequence.AppendCallback(() => m_cardClickable.enabled = true);
        }

        #endregion
    }
}