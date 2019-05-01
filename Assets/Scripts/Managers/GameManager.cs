using System.Collections.Generic;
using Data;
using DG.Tweening;
using Review;
using TMPro;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public enum State
        {
            REVIEW,
            TEST
        }

        public static GameManager Instance;
        public        State       CurrentTrainingState { get; private set; }

        [SerializeField] private List<TrainingSo> m_listOfTrainings;
        [SerializeField] private TMP_Text         m_scoreText;
        [SerializeField] private float            m_jumpScoreAnim = 20.0f;

        private TrainingSo       m_selectedTraining;
        private int m_selectedTrainingIndex;
        private int m_actualScore;

        private void Start()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
            
            m_selectedTraining   = m_listOfTrainings[0];

            CurrentTrainingState              =  State.REVIEW;
            AnswerButtonReference.GoodAnswser += GoodAnswser;

            InitCardManager();
        }

        private void GoodAnswser()
        {
            if (CardManager.Instance.IsEndOfTrainingDeck())
                SelectNextTraining();
            else
            {
                CardManager.Instance.NextTestCard();
                ReviewManager.Instance.ShuffleAnswers();
            }

            IncreaseScore();
            UpdateScoreView();
        }

        private void SelectNextTraining()
        {
            ++m_selectedTrainingIndex;

            if (m_selectedTrainingIndex == m_listOfTrainings.Count)
                m_selectedTrainingIndex = 0;
         
            m_selectedTraining = m_listOfTrainings[m_selectedTrainingIndex];
            ReviewManager.Instance.HideListAnswers();
            ReviewManager.Instance.DisableListAnswers();
            CardManager.Instance.NextTraining(m_selectedTraining);

            CurrentTrainingState = State.REVIEW;
        }

        private void IncreaseScore()
        {
            m_actualScore++;
        }

        private void UpdateScoreView()
        {
            Sequence sequenceUpdate = DOTween.Sequence();

            sequenceUpdate.AppendCallback(() => m_scoreText.text = "Score : " + m_actualScore);
            sequenceUpdate.Join(m_scoreText.transform.DOPunchPosition(Vector3.up * m_jumpScoreAnim, 0.25f));
        }

        private void InitCardManager()
        {
            CardManager.Instance.SetNewTraining(m_selectedTraining);
            CardManager.Instance.Init();
            CardManager.Instance.FinishedReviewDeck += ToTestState;
        }

        private void ToTestState()
        {
            CurrentTrainingState = State.TEST;
            CardManager.Instance.DisableTouchCard();
            ReviewManager.Instance.EnableListAnswers();
            ReviewManager.Instance.PopulateAnswers(m_selectedTraining);
        }
    }
}