using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using Unity.VisualScripting;
using Adapt;
namespace CustomUI
{
    // An element that displays progress inside a partially filled circle
    [UxmlElement]
    public partial class Odometer : VisualElement,IObserver
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
        static CustomStyleProperty<String> s_StringAfterValue = new CustomStyleProperty<String>("--string-after-value");
        static CustomStyleProperty<String> s_Caption = new CustomStyleProperty<String>("--caption");


        Color m_TrackColor = Color.gray;
        Color m_StartColor = Color.blue;

        Color m_EndColor = Color.red;

        float m_StartValue = 0f;
        float m_EndValue = 100f;

        // This is the label that displays the percentage.
        Label m_ValueLabel;
        Label m_CaptionLabel;


        // This is the number that the Label displays as a percentage.
        float m_value;
        string m_CaptionText;

        float defaultThickness = 0.2f;
        float hoverThickness = 0.3f;



        bool m_hover = false;


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
                m_ValueLabel.text = (value.ToSafeString());
                MarkDirtyRepaint();
            }
        }


        [UxmlAttribute]
        public string caption
        {
            // The progress property is exposed in C#.
            get => m_CaptionText;
            set
            {
                // Whenever the progress property changes, MarkDirtyRepaint() is named. This causes a call to the
                // generateVisualContents callback.
                m_CaptionText = value;
                m_CaptionLabel.text = value;
                MarkDirtyRepaint();
            }
        }

        // This default constructor is RadialProgress's only constructor.
        public Odometer()
        {
            // Set up container (this) with flex display
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

            // Create and add caption
            m_CaptionLabel = new Label();
            m_CaptionLabel.AddToClassList(ussLabelClassName);
            m_CaptionLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            // If you want the caption to take full width
            m_CaptionLabel.style.width = new StyleLength(Length.Percent(100));
            Add(m_CaptionLabel);

            // Add the USS class name for the overall control.
            AddToClassList(ussClassName);

            // Register a callback after custom style resolution.
            RegisterCallback<CustomStyleResolvedEvent>(evt => CustomStylesResolved(evt));

            RegisterCallback<PointerEnterEvent>(evt =>
            {
                m_hover = true;
                MarkDirtyRepaint();
            });

            RegisterCallback<PointerLeaveEvent>(evt =>
            {
                m_hover = false;
                MarkDirtyRepaint();
            });


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

            //if (customStyle.TryGetValue(s_StartColor, out m_StartColor))
            //    repaint = true;

            //if (customStyle.TryGetValue(s_EndColor, out m_EndColor))
            //    repaint = true;

            //if (customStyle.TryGetValue(s_TrackColor, out m_TrackColor))
            //    repaint = true;

            //if (customStyle.TryGetValue(s_StartValue, out m_StartValue))
            //    repaint = true;

            //if (customStyle.TryGetValue(s_EndValue, out m_EndValue))
            //    repaint = true;

            if (repaint)
                MarkDirtyRepaint();
        }

        void GenerateVisualContent(MeshGenerationContext context)
        {
            float width = contentRect.width;
            float height = contentRect.height;
            float radius = Math.Min(width, height) * 0.5f;
            float startAngle = 165f;
            float endAngle = 375f;

            Vector2 center = new Vector2(width * 0.5f, height * 0.5f);

            var painter = context.painter2D;
            painter.lineWidth = radius * defaultThickness;

            if (m_hover)
            {
                //Debug.Log(radius);
                painter.lineWidth = radius * hoverThickness;
            }

            radius = radius - painter.lineWidth * 0.5f;
            painter.lineCap = LineCap.Butt;

            // Draw the track
            painter.strokeColor = m_TrackColor;
            painter.BeginPath();
            painter.Arc(center, radius, startAngle, endAngle);
            painter.Stroke();
            //Debug.Log(m_TrackColor);

            // Draw the progress
            painter.strokeColor = Color.Lerp(m_StartColor, m_EndColor, value / (m_EndValue - m_StartValue)); ;
            painter.BeginPath();
            painter.Arc(center, radius, startAngle, (endAngle - startAngle) * (value / (m_EndValue - m_StartValue)) + startAngle);
            painter.Stroke();
        }

        public void update(Data data)
        {
        }
    }
}