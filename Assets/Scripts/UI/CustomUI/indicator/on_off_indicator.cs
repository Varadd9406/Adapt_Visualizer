using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace CustomUI
{
    // An element that displays progress inside a partially filled circle
    [UxmlElement]
    public partial class Indicator : VisualElement
    {

        // These are USS class names for the control overall and the label.
        public static readonly string ussClassName = "radial-progress";
        public static readonly string ussLabelClassName = "radial-progress__label";

        // These objects allow C# code to access custom USS properties.
        static CustomStyleProperty<Color> s_OffColor = new CustomStyleProperty<Color>("--off-color");
        static CustomStyleProperty<Color> s_OnColor = new CustomStyleProperty<Color>("--on-color");

        Color m_OffColor = Color.red;
        Color m_OnColor = Color.green;

        // This is the number that the Label displays as a percentage.
        bool m_state;

        // A value between 0 and 100
        [UxmlAttribute]
        public bool state
        {
            // The progress property is exposed in C#.
            get => m_state;
            set
            {
                // Whenever the progress property changes, MarkDirtyRepaint() is named. This causes a call to the
                // generateVisualContents callback.
                m_state = value;
                MarkDirtyRepaint();
            }
        }

        // This default constructor is RadialProgress's only constructor.
        public Indicator()
        {
            // Create a Label, add a USS class name, and add it to this visual tree

            // Add the USS class name for the overall control.
            AddToClassList(ussClassName);

            // Register a callback after custom style resolution.
            RegisterCallback<CustomStyleResolvedEvent>(evt => CustomStylesResolved(evt));

            // Register a callback to generate the visual content of the control.
            generateVisualContent += GenerateVisualContent;
        }

        static void CustomStylesResolved(CustomStyleResolvedEvent evt)
        {
            Indicator element = (Indicator)evt.currentTarget;
            element.UpdateCustomStyles();
        }

        // After the custom colors are resolved, this method uses them to color the meshes and (if necessary) repaint
        // the control.
        void UpdateCustomStyles()
        {
            bool repaint = false;
            if (customStyle.TryGetValue(s_OffColor, out m_OffColor))
                repaint = true;

            if (customStyle.TryGetValue(s_OnColor, out m_OnColor))
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
            painter.lineWidth = radius;
            radius = radius - painter.lineWidth * 0.5f;
            painter.lineCap = LineCap.Butt;

            // Draw the track
            if(m_state)
            {
                painter.strokeColor = m_OnColor;

            }
            else
            {
                painter.strokeColor = m_OffColor;
            }

            painter.BeginPath();
            painter.Arc(center, radius, 0.0f, 360.0f);
            painter.Stroke();
        }
    }
}