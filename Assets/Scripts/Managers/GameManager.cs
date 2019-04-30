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

        private List<TrainingSo> m_tmpListOfTrainings;
        private TrainingSo       m_selectedTraining;


        private int m_actualScore;

        private void Start()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            int selectedTrainingIndex = Random.Range(0, m_listOfTrainings.Count - 1);

            m_tmpListOfTrainings = new List<TrainingSo>(m_listOfTrainings);
            m_selectedTraining   = m_tmpListOfTrainings[selectedTrainingIndex];
            m_tmpListOfTrainings.RemoveAt(selectedTrainingIndex);

            CurrentTrainingState              =  State.REVIEW;
            AnswerButtonReference.GoodAnswser += GoodAnswser;

            InitCardManager();
        }

        private void GoodAnswser()
        {
            if (CardManager.Instance.IsEndOfTrainingDeck())
                SelectNextTraining();
            else
                CardManager.Instance.NextTestCard();

            IncreaseScore();
            UpdateScoreView();
        }

        private void SelectNextTraining()
        {
            int selectedTrainingIndex = Random.Range(0, m_listOfTrainings.Count - 1);

            m_selectedTraining = m_tmpListOfTrainings[selectedTrainingIndex];
            ReviewManager.Instance.HideListAnswers();
            CardManager.Instance.NextTraining(m_selectedTraining);

            if (m_tmpListOfTrainings.Count == 0)
                m_tmpListOfTrainings = new List<TrainingSo>(m_listOfTrainings);

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
            ReviewManager.Instance.ShowListAnswers();
            ReviewManager.Instance.PopulateAnswers(m_selectedTraining);
        }
    }
}