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
        private readonly Image m_Compass;
        private readonly Image m_Plane;

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
            m_Compass = new Image();
            m_Plane = new Image();

            m_Compass.style.position = Position.Absolute;

            m_Plane.style.position = Position.Absolute;

            m_Compass.image = Resources.Load<Texture2D>("UI_Icons/heading_compass");
            m_Plane.image = Resources.Load<Texture2D>("UI_Icons/plane_icon");


            //m_Plane.style.backgroundColor = Color.green;

            m_Plane.style.left = new Length(25, LengthUnit.Percent);
            m_Plane.style.top = new Length(25, LengthUnit.Percent);


            m_Compass.style.transformOrigin = new TransformOrigin(Length.Percent(50), Length.Percent(50));
            m_Plane.style.transformOrigin = new TransformOrigin(Length.Percent(50), Length.Percent(50));
            //m_PlaneTexture = Resources.Load<Texture2D>(m_CompassTexturePath);


            m_Compass.style.width = Length.Percent(100);
            m_Compass.style.height = Length.Percent(100);

            m_Plane.style.width = Length.Percent(50);
            m_Plane.style.height = Length.Percent(50);

            Add(m_Compass);
            Add(m_Plane);



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
            m_Compass.style.rotate = new Rotate(new Angle(currentRotation, AngleUnit.Degree));

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
