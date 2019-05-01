using System;
using Data;
using DG.Tweening;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Review
{
    public class AnswerButtonReference : MonoBehaviour
    {
        public static Action GoodAnswser;

        public Image ImageButton => m_image;
        public TMP_Text TextButton => m_answerText;

        [Header("References")] [SerializeField]
        private Button m_button;

        [SerializeField] private TMP_Text m_answerText;
        [SerializeField] private Image    m_image;

        [Header("Button Wrong Behaviour")] [SerializeField]
        private Color m_wrongAnswerColor = Color.red;
        [SerializeField] private float m_durationWrongAnswer = 0.25f;

        [SerializeField] private float m_rotationLimitWrongAnswer = 10f;
        [SerializeField] private float m_punchScaleWrongAnswer    = 0.5f;

        [Header("Button Good Behaviour")] [SerializeField]
        private Color m_goodAnswerColor = Color.green;
        [SerializeField] private float m_durationGoodAnswer = 0.25f;

        [SerializeField] private float m_rotationLimitGoodAnswer = 10f;
        [SerializeField] private float m_punchScaleGoodAnswer    = 0.5f;

        private Color m_imageColor;


        private void Start()
        {
            m_button.onClick.AddListener(CheckAnswer);
            m_button.interactable = false;
            m_imageColor          = new Color(m_image.color.r, m_image.color.g, m_image.color.b, 1);
        }

        private void CheckAnswer()
        {
            if (CardManager.Instance.IsRightAnswer(m_answerText.text))
            {
                GoodAnswer();
            }
            else
                WrongAnswerAnim();
        }

        private void GoodAnswer()
        {
            Sequence sequence = DOTween.Sequence();

            sequence.AppendCallback(() => m_button.interactable = false);
            sequence.Append(transform.DOPunchScale(new Vector3(m_punchScaleGoodAnswer, m_punchScaleGoodAnswer), m_durationGoodAnswer, 6));
            sequence.Join(transform.DOShakeRotation(m_durationGoodAnswer, new Vector3(0.0f, 0.0f, m_rotationLimitGoodAnswer)));
            sequence.Join(m_image.DOColor(m_goodAnswerColor, m_durationGoodAnswer));
            sequence.Join(m_answerText.DOColor(m_goodAnswerColor, m_durationGoodAnswer));
            sequence.Append(m_image.DOColor(m_imageColor, m_durationGoodAnswer));
            sequence.Join(m_answerText.DOColor(m_imageColor, m_durationGoodAnswer));
            sequence.AppendCallback(() => m_button.interactable = true);
            sequence.AppendCallback(() => GoodAnswser?.Invoke());
        }

        private void WrongAnswerAnim()
        {
            Sequence sequence = DOTween.Sequence();

            sequence.AppendCallback(() => m_button.interactable = false);
            sequence.Append(transform.DOPunchScale(new Vector3(m_punchScaleWrongAnswer, m_punchScaleWrongAnswer), m_durationWrongAnswer, 6));
            sequence.Join(transform.DOShakeRotation(m_durationWrongAnswer, new Vector3(0.0f, 0.0f, m_rotationLimitWrongAnswer)));
            sequence.Join(m_image.DOColor(m_wrongAnswerColor, m_durationWrongAnswer));
            sequence.Join(m_answerText.DOColor(m_wrongAnswerColor, m_durationWrongAnswer));
            sequence.Append(m_image.DOColor(m_imageColor, m_durationWrongAnswer));
            sequence.Join(m_answerText.DOColor(m_imageColor, m_durationWrongAnswer));
            sequence.AppendCallback(() => m_button.interactable = true);
        }

        public void Populate(CardSo p_card)
        {
            m_button.interactable = true;
            m_answerText.text     = p_card.m_backTitle;
        }
    }
}