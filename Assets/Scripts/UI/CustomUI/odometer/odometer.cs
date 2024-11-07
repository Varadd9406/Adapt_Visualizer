using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace CustomUI
{
    // An element that displays progress inside a partially filled circle
    [UxmlElement]
    public partial class Odometer : VisualElement
    {

        // These are USS class names for the control overall and the label.
        public static readonly string ussClassName = "odometer";
        public static readonly string ussLabelClassName = "odometer__label";

        // These objects allow C# code to access custom USS properties.
        static CustomStyleProperty<Color> s_TrackColor = new CustomStyleProperty<Color>("--track-color");
        static CustomStyleProperty<Color> s_StartColor = new CustomStyleProperty<Color>("--start-color");
        static CustomStyleProperty<Color> s_EndColor = new CustomStyleProperty<Color>("--end-color");
        static CustomStyleProperty<float> s_StartValue = new CustomStyleProperty<float>("--start-value");
        static CustomStyleProperty<float> s_EndValue = new CustomStyleProperty<float>("--end-value");

        Color m_TrackColor = Color.gray;
        Color m_StartColor = Color.blue;

        Color m_EndColor = Color.red;

        float m_StartValue = 0;
        float m_EndValue = 100;

        // This is the label that displays the percentage.
        Label m_Label;

        // This is the number that the Label displays as a percentage.
        float m_value;


        // A value between 0 and 100
        [UxmlAttribute]
        public float value
        {
            // The progress property is exposed in C#.
            get => m_value;
            set
            {
                // Whenever the progress property changes, MarkDirtyRepaint() is named. This causes a call to the
                // generateVisualContents callback.
                m_value = value;
                m_Label.text = Mathf.Clamp(Mathf.Round(value), 0, 100) + "%";
                MarkDirtyRepaint();
            }
        }

        // This default constructor is RadialProgress's only constructor.
        public Odometer()
        {
            // Create a Label, add a USS class name, and add it to this visual tree.
            m_Label = new Label();
            m_Label.AddToClassList(ussLabelClassName);
            Add(m_Label);

            // Add the USS class name for the overall control.
            AddToClassList(ussClassName);

            // Register a callback after custom style resolution.
            RegisterCallback<CustomStyleResolvedEvent>(evt => CustomStylesResolved(evt));

            // Register a callback to generate the visual content of the control.
            generateVisualContent += GenerateVisualContent;

            value = 0.0f;
        }

        static void CustomStylesResolved(CustomStyleResolvedEvent evt)
        {
            Odometer element = (Odometer)evt.currentTarget;
            element.UpdateCustomStyles();
        }

        // After the custom colors are resolved, this method uses them to color the meshes and (if necessary) repaint
        // the control.
        void UpdateCustomStyles()
        {
            bool repaint = false;
            if (customStyle.TryGetValue(s_StartColor, out m_StartColor))
                repaint = true;

            if (customStyle.TryGetValue(s_EndColor, out m_EndColor))
                repaint = true;

            if (customStyle.TryGetValue(s_TrackColor, out m_TrackColor))
                repaint = true;

            if (customStyle.TryGetValue(s_StartValue, out m_StartValue))
                repaint = true;

            if (customStyle.TryGetValue(s_EndValue, out m_EndValue))
                repaint = true;

            if (repaint)
                MarkDirtyRepaint();
        }

        void GenerateVisualContent(MeshGenerationContext context)
        {
            float width = contentRect.width;
            float height = contentRect.height;
            float radius = Math.Min(width, height) * 0.5f;

            Vector2 center = new Vector2(width * 0.5f, height * 0.5f);

            var painter = context.painter2D;
            painter.lineWidth = radius * 0.2f;
            radius = radius - painter.lineWidth * 0.5f;
            painter.lineCap = LineCap.Butt;

            // Draw the track
            painter.strokeColor = m_TrackColor;
            painter.BeginPath();
            painter.Arc(center, radius,150.0f, 390.0f);
            painter.Stroke();

            // Draw the progress
            painter.strokeColor = Color.Lerp(m_StartColor, m_EndColor, value / 100.0f); ;
            painter.BeginPath();
            painter.Arc(center, radius, 150.0f, (390.0f - 150.0f) * (value / 100.0f) + 150.0f);
            painter.Stroke();
        }
    }
}