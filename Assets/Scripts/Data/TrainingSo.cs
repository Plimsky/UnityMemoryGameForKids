using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "New Training", menuName = "Cards/Training")]
    public class TrainingSo : ScriptableObject
    {
        public List<CardSo> m_listOfCards = new List<CardSo>();
    }
}