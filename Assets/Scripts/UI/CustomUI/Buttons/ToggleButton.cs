using UnityEngine;
using UnityEngine.UIElements;

namespace CustomUI
{
    [UxmlElement]
    public partial class ToggleButton : VisualElement
    {
        private bool m_State;
        private readonly Image buttonImage;
        private Texture2D onTexture;
        private Texture2D offTexture;

        public System.Action<bool> OnStateChanged;

        [UxmlAttribute]
        public bool state
        {
            get => m_State;
            set
            {
                m_State = value;
                UpdateVisualState();
            }
        }

        public ToggleButton()
        {

            onTexture = Resources.Load<Texture2D>("UI_icons/switch-on");
            offTexture = Resources.Load<Texture2D>("UI_icons/switch-off");
            // Create image element
            buttonImage = new Image();
            buttonImage.style.width = Length.Percent(100);
            buttonImage.style.height = Length.Percent(100);
            Add(buttonImage);

            // Make clickable
            RegisterCallback<ClickEvent>(OnClick);

            // Set default size
            //style.width = 60;
            //style.height = 30;
        }

        public void SetTextures(Texture2D onTex, Texture2D offTex)
        {
            onTexture = onTex;
            offTexture = offTex;
            UpdateVisualState();
        }

        private void OnClick(ClickEvent evt)
        {
            //Debug.Log("Clicked");
            m_State = !m_State;
            UpdateVisualState();
        }

        private void UpdateVisualState()
        {
            if (buttonImage != null)
            {
                buttonImage.image = m_State ? onTexture : offTexture;
            }
        }
    }
}