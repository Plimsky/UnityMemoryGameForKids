using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Card
{
    public class SideReferences : MonoBehaviour
    {
        public GameObject m_image;
        public GameObject m_title;

        private Image    m_imageComponent;
        private TMP_Text m_titleTMPComponent;

        public void UpdateImage(Sprite frontSprite)
        {
            if (m_imageComponent != null)
                m_imageComponent.sprite = frontSprite;
        }

        public void UpdateTitle(string frontTitle)
        {
            if (m_titleTMPComponent)
                m_titleTMPComponent.text = frontTitle;
        }

        public void Init()
        {
            if (m_image != null)
                m_imageComponent = m_image.GetComponent<Image>();

            if (m_title != null)
                m_titleTMPComponent = m_title.GetComponent<TMP_Text>();
        }
    }
}