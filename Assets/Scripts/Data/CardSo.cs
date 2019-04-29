using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Cards/Card")]
    public class CardSo : ScriptableObject
    {
        // Front
        public Sprite m_frontSprite;
        public string m_frontTitle;

        // Back
        public Sprite m_backSprite;
        public string m_backTitle;
    }
}