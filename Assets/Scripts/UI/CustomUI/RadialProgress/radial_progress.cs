using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace CustomUI
{
    // An element that displays progress inside a partially filled circle
    [UxmlElement]
    public partial class RadialProgress : VisualElement
    {

        // These are USS class names for the control overall and the label.
        public static readonly string ussClassName = "radial-progress";
        public static readonly string ussLabelClassName = "radial-progress__label";

        // These objects allow C# code to access custom USS properties.
        static CustomStyleProperty<Color> s_TrackColor = new CustomStyleProperty<Color>("--track-color");
        static CustomStyleProperty<Color> s_ProgressColor = new CustomStyleProperty<Color>("--progress-color");

        Color m_TrackColor = Color.gray;
        Color m_ProgressColor = Color.green;

        // This is the label that displays the percentage.
        Label m_ValueLabel;

        // This is the number that the Label displays as a percentage.
        float m_Progress;


        // A value between 0 and 100
        [UxmlAttribute]
        public float progress
        {
            // The progress property is exposed in C#.
            get => m_Progress;
            set
            {
                // Whenever the progress property changes, MarkDirtyRepaint() is named. This causes a call to the
                // generateVisualContents callback.
                m_Progress = value;
                m_ValueLabel.text = Mathf.Clamp(Mathf.Round(value), 0, 100) + "%";
                MarkDirtyRepaint();
            }
        }

        // This default constructor is RadialProgress's only constructor.
        public RadialProgress()
        {
            // Create a Label, add a USS class name, and add it to this visual tree.
            style.display = DisplayStyle.Flex;
            style.flexDirection = FlexDirection.Column; // Stack children vertically
            style.alignItems = Align.Center; // Center children horizontally
            //style.backgroundColor = Color.gray;
            //style.height = new StyleLength(Length.Percent(100)); // Take full height

            // Create and add value label
            m_ValueLabel = new Label();
            m_ValueLabel.AddToClassList(ussLabelClassName);
            m_ValueLabel.style.flexGrow = 1; // Takes up available space
            m_ValueLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            // If you want the label to take full width
            m_ValueLabel.style.width = new StyleLength(Length.Percent(100));
            Add(m_ValueLabel);

            // Add the USS class name for the overall control.
            AddToClassList(ussClassName);

            // Register a callback after custom style resolution.
            RegisterCallback<CustomStyleResolvedEvent>(evt => CustomStylesResolved(evt));

            // Register a callback to generate the visual content of the control.
            generateVisualContent += GenerateVisualContent;

            progress = 0.0f;
        }

        static void CustomStylesResolved(CustomStyleResolvedEvent evt)
        {
            RadialProgress element = (RadialProgress)evt.currentTarget;
            element.UpdateCustomStyles();
        }

        // After the custom colors are resolved, this method uses them to color the meshes and (if necessary) repaint
        // the control.
        void UpdateCustomStyles()
        {
            bool repaint = false;
            if (customStyle.TryGetValue(s_ProgressColor, out m_ProgressColor))
                repaint = true;

            if (customStyle.TryGetValue(s_TrackColor, out m_TrackColor))
                repaint = true;

            if (repaint)
                MarkDirtyRepaint();
        }

        void GenerateVisualContent(MeshGenerationContext context)
        {
            float width = contentRect.width;
            float height = contentRect.height;
            float radius = Math.Min(width, height)*0.5f;

            Vector2 center = new Vector2(width * 0.5f, height * 0.5f);

            var painter = context.painter2D;
            painter.lineWidth = radius*0.3f;
            radius = radius - painter.lineWidth * 0.5f;
            painter.lineCap = LineCap.Butt;

            // Draw the track
            painter.strokeColor = m_TrackColor;
            painter.BeginPath();
            painter.Arc(center,radius, 0.0f, 360.0f);
            painter.Stroke();

            // Draw the progress
            painter.strokeColor = m_ProgressColor;
            painter.BeginPath();
            painter.Arc(center,radius, -90.0f, 360.0f * (progress / 100.0f) - 90.0f);
            painter.Stroke();
        }
    }
}