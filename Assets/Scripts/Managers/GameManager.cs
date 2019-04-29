using System;
using Card;
using Data;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        private enum State
        {
            REVIEW,
            TEST
        }


        [Header("References")] [SerializeField]
        private Button m_continueButton;

        [SerializeField] private GameObject  m_card;
        [SerializeField] private TrainingSo  m_selectedTraining;
        [SerializeField] private ContentSide m_frontContentSide;
        [SerializeField] private ContentSide m_backContentSide;

        [Header("Card configuration")] [SerializeField]
        private float m_speedCardRotation = 0.25f;

        private GameObject m_mainContent;
        private Button     m_cardClickable;

        private bool  m_isFrontSide;
        private State m_currentTrainingState;
        private int   m_totalCardCurrentTraining;
        private int   m_currentCardCurrentTraining;
        private bool  m_flippedCardAtLeastOnce;

        private void Start()
        {
            m_mainContent   = m_card.transform.GetChild(0).gameObject;
            m_cardClickable = m_card.GetComponent<Button>();

            m_cardClickable.onClick.AddListener(TouchCard);
            m_continueButton.onClick.AddListener(TouchContinue);
            m_isFrontSide              = true;
            m_currentTrainingState     = State.REVIEW;
            m_totalCardCurrentTraining = m_selectedTraining.m_listOfCards.Count;

            m_frontContentSide.Init();
            m_backContentSide.Init();
            UpdateCard();
            m_continueButton.gameObject.SetActive(false);
        }

        private void UpdateCard()
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
            continueButtonsequence.Append(m_continueButton.GetComponent<Image>().DOFade(1.0f, 0.5f));
        }

        private void HideContinueButton()
        {
            Sequence continueButtonsequence = DOTween.Sequence();
            continueButtonsequence.Append(m_continueButton.GetComponent<Image>().DOFade(1.0f, 0f));
            continueButtonsequence.Append(m_continueButton.GetComponent<Image>().DOFade(0.0f, 0.5f));
            continueButtonsequence.AppendCallback(() => m_continueButton.gameObject.SetActive(false));
        }

        private void TouchContinue()
        {
            switch (m_currentTrainingState)
            {
                case State.REVIEW when m_currentCardCurrentTraining < m_totalCardCurrentTraining - 1:
                {
                    m_currentCardCurrentTraining++;
                    NextCard();
                    break;
                }
                case State.REVIEW when m_currentCardCurrentTraining == m_totalCardCurrentTraining - 1:
                {
                    m_currentCardCurrentTraining = 0;
                    m_currentTrainingState = State.TEST;
                    NextCard();
                    break;
                }
                case State.TEST:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void NextCard()
        {
            HideContinueButton();

            if (!m_isFrontSide)
                FlipCard(true);
            else
                UpdateCard();

            m_flippedCardAtLeastOnce = false;
        }

        private void FlipCard(bool p_forceUpdate = false)
        {
            Sequence sequence             = DOTween.Sequence();
            float    yRotationDestination = Math.Abs(m_mainContent.transform.localEulerAngles.y) < Mathf.Epsilon ? 180.0f : 0.0f;

            sequence.AppendCallback(() => m_cardClickable.enabled = false);
            sequence.Append(m_mainContent.transform.DOLocalRotate(new Vector3(0, 90.0f, 0), m_speedCardRotation));

            if (p_forceUpdate)
                sequence.AppendCallback(UpdateCard);
            
            sequence.AppendCallback(() => m_frontContentSide.gameObject.SetActive(!m_frontContentSide.gameObject.activeSelf));
            sequence.AppendCallback(() => m_backContentSide.gameObject.SetActive(!m_backContentSide.gameObject.activeSelf));
            sequence.AppendCallback(() => m_isFrontSide = !m_isFrontSide);
            sequence.Append(m_mainContent.transform.DOLocalRotate(new Vector3(0, yRotationDestination, 0), m_speedCardRotation));
            sequence.AppendCallback(() => m_cardClickable.enabled = true);
        }
    }
}