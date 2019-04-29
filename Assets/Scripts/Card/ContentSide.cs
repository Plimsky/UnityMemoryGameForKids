using Data;
using UnityEngine;
using UnityEngine.UI;

namespace Card
{
    public class ContentSide : MonoBehaviour
    {
        public SideReferences m_references;

        public void UpdateReferences(CardSo p_card, bool p_isFront)
        {
            if (p_isFront)
            {
                m_references.UpdateImage(p_card.m_frontSprite);
                m_references.UpdateTitle(p_card.m_frontTitle);
            }
            else
            {
                m_references.UpdateImage(p_card.m_backSprite);
                m_references.UpdateTitle(p_card.m_backTitle);
            }
        }

        public void Init()
        {
            m_references = transform.GetChild(0).GetComponent<SideReferences>();
            m_references.Init();
        }
    }
}