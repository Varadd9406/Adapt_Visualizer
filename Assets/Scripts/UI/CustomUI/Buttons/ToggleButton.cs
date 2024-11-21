using Adapt;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomUI
{
    [UxmlElement]
    public partial class ToggleButton : VisualElement, IObserver
    {
        private bool m_State;
        private readonly Image buttonImage;
        private string m_OnTexturePath;
        private string m_OffTexturePath;
        private string m_DefaultTexturePath;
        private Texture2D m_OnTexture;
        private Texture2D m_OffTexture;
        private Texture2D m_DefaultTexture;
        private Action m_onClickBehavior;

        [UxmlAttribute]
        public string default_img
        {
            get => m_DefaultTexturePath;
            set
            {
                m_DefaultTexturePath = value;
                m_DefaultTexture = Resources.Load<Texture2D>(m_DefaultTexturePath);
                UpdateVisualState();
            }
        }

        [UxmlAttribute]
        public string on_img
        {
            get => m_OnTexturePath;
            set
            {
                m_OnTexturePath = value;
                m_OnTexture = Resources.Load<Texture2D>(m_OffTexturePath);
                UpdateVisualState();
            }
        }

        [UxmlAttribute]
        public string off_img
        {
            get => m_OffTexturePath;
            set
            {
                m_OffTexturePath = value;
                m_OffTexture = Resources.Load<Texture2D>(m_OnTexturePath);
                UpdateVisualState();
            }
        }
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

            // Create image element
            buttonImage = new Image();
            buttonImage.style.width = Length.Percent(100);
            buttonImage.style.height = Length.Percent(100);


            Add(buttonImage);


            // Make clickable
            RegisterCallback<ClickEvent>(OnClick);


            generateVisualContent += GenerateVisualContent;
            UpdateVisualState();


        }

        public void SetTextures(Texture2D onTex, Texture2D offTex)
        {
            m_OnTexture = onTex;
            m_OffTexture = offTex;
            UpdateVisualState();
        }

        public void SetOnClickBehavior(Action callback)
        {
            m_onClickBehavior = callback;
        }

        private void OnClick(ClickEvent evt)
        {
            m_State = !m_State;


            if (m_onClickBehavior != null)
            {
                m_onClickBehavior();
                Debug.Log("Called");
            }

            UpdateVisualState();
        }
        void GenerateVisualContent(MeshGenerationContext context)
        {
            m_OnTexture = Resources.Load<Texture2D>(m_OnTexturePath);
            m_OffTexture = Resources.Load<Texture2D>(m_OffTexturePath);
            m_DefaultTexture = Resources.Load<Texture2D>(m_DefaultTexturePath);
        }


        private void UpdateVisualState()
        {
            if (m_DefaultTexturePath==null)
            {
                buttonImage.image = m_State ? m_OnTexture : m_OffTexture;
            }
            else
            {
                buttonImage.image = m_DefaultTexture;
            }
        }

        public void update(Dictionary<string, object> data)
        {
            return;
        }

    }
}