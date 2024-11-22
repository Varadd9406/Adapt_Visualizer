using Adapt;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Rendering.FilterWindow;
using static UnityEngine.Rendering.DebugUI;

namespace CustomUI
{
    [UxmlElement]
    public partial class HeadingInstrument : VisualElement, IObserver
    {
        private float m_Angle;
        private readonly Image m_Outer;
        private readonly Image m_Inner;
        private readonly Image m_Marking;


        private float currentRotation = 0f;
        private float rotationSpeed = 25f;

        [UxmlAttribute]
        public float angle
        {
            get => m_Angle;
            set
            {
                m_Angle = value;
                UpdateVisualState();
            }
        }
        public HeadingInstrument()
        {

            // Create image element
            m_Inner = new Image();
            m_Outer = new Image();
            m_Marking = new Image();

            m_Outer.style.position = Position.Absolute;
            m_Inner.style.position = Position.Absolute;
            m_Marking.style.position = Position.Absolute;


            m_Outer.image = Resources.Load<Texture2D>("UI_Icons/altitude_indicator_outer");
            m_Inner.image = Resources.Load<Texture2D>("UI_Icons/altitude_indicator_inner");
            m_Marking.image = Resources.Load<Texture2D>("UI_icons/artificial_horizon_indicator");


            m_Outer.style.width = Length.Percent(100);
            m_Outer.style.height = Length.Percent(100);

            m_Inner.style.width = Length.Percent(150);
            m_Inner.style.height = Length.Percent(150);

            m_Marking.style.width = Length.Percent(100);
            m_Marking.style.height = Length.Percent(100);

            m_Inner.style.left = new Length(-25, LengthUnit.Percent);
            m_Inner.style.top = new Length(-25, LengthUnit.Percent);
            //m_Outer.style.backgroundColor = Color.green;

            //m_Inner.style.left = new Length(25, LengthUnit.Percent);
            //m_Inner.style.backgroundColor = Color.green;


            m_Outer.style.transformOrigin = new TransformOrigin(Length.Percent(50), Length.Percent(50));
            m_Inner.style.transformOrigin = new TransformOrigin(Length.Percent(50), Length.Percent(50));
            m_Marking.style.transformOrigin = new TransformOrigin(Length.Percent(50), Length.Percent(50));

            //m_PlaneTexture = Resources.Load<Texture2D>(m_CompassTexturePath);




            Add(m_Inner);
            Add(m_Outer);
            Add(m_Marking);



            // Make clickable
            //RegisterCallback<ClickEvent>(OnClick);


            //generateVisualContent += GenerateVisualContent;

            EditorApplication.update += RotateImage;

            UpdateVisualState();

            //style.rotate = new Rotate(180);
        }



        void GenerateVisualContent(MeshGenerationContext context)
        {
        }


        private void UpdateVisualState()
        {

        }
        private void RotateImage()
        {
            //Debug.Log(m_Angle);
            // Calculate new rotation based on time
            float rotationRemaining = m_Angle - currentRotation;

            float rotationThisFrame = Mathf.Min(Mathf.Abs(rotationRemaining), rotationSpeed * Time.deltaTime);

            if (Mathf.Abs(rotationRemaining) < 0.1f)
            {
                return;
            }


            if (rotationRemaining < 0)
            {
                rotationThisFrame *= -1;
            }
            currentRotation += rotationThisFrame;

            //Debug.Log($"{currentRotation} {rotationThisFrame}");

            // Apply the rotation
            m_Outer.style.rotate = new Rotate(new Angle(currentRotation, AngleUnit.Degree));

            // Optional: Reset rotation after 360 degrees to prevent floating-point issues over time
            if (currentRotation >= 360f)
            {
                currentRotation -= 360f;
            }
        }

        public void update(Dictionary<string, object> data)
        {
            if (data[name] is JsonElement element)
            {
                if (element.ValueKind == JsonValueKind.Number)
                {
                    if (element.TryGetDouble(out double doubleValue))
                    {
                        m_Angle = (float)doubleValue;
                    }
                }
            }
        }

    }
}