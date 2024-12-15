using Adapt;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    public partial class AttitudeInstrument : VisualElement, IObserver
    {
        private float m_Bank;
        private float m_Yaw;
        private readonly VisualElement m_Outer;
        private readonly VisualElement m_Inner;
        private readonly VisualElement m_Marking;


        private float currentYaw = 0f;
        private float currentBank = 0f;

        private float rotationSpeed = 5f;


        [UxmlAttribute]
        public float bank
        {
            get => m_Bank;
            set
            {
                m_Bank = value;
                UpdateVisualState();
            }
        }

        [UxmlAttribute]
        public float yaw
        {
            get => m_Yaw;
            set
            {
                m_Yaw = value;
                UpdateVisualState();
            }
        }
        public AttitudeInstrument()
        {

            // Make clickable
            //RegisterCallback<ClickEvent>(OnClick);


            //generateVisualContent += GenerateVisualContent;

            generateVisualContent += OnGenerateVisualContent;


            //UpdateVisualState();

            //style.rotate = new Rotate(180);
        }



        void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            float yawRemaining = m_Yaw - currentYaw;

            float yawThisFrame = Mathf.Min(Mathf.Abs(yawRemaining), rotationSpeed * Time.deltaTime);

            if (yawRemaining < 0)
            {
                yawThisFrame *= -1;
            }
            currentYaw += yawThisFrame;


            float bankRemaining = m_Bank - currentBank;

            float bankThisFrame = Mathf.Min(Mathf.Abs(bankRemaining), rotationSpeed * Time.deltaTime);

            if (bankRemaining < 0)
            {
                bankThisFrame *= -1;
            }
            currentBank += bankThisFrame;



            var painter = mgc.painter2D;
            var rect = mgc.visualElement.contentRect;

            // Calculate dimensions based on container size
            float width = rect.width;
            float height = rect.height;
            //Debug.Log(width);

            // Colors
            UnityEngine.Color triangleColor = new UnityEngine.Color(1f, 0.4f, 0.2f); // Red-orange
            UnityEngine.Color barColor = new UnityEngine.Color(1f, 0.6f, 0.4f);      // Coral
            UnityEngine.Color stemColor = new UnityEngine.Color(0.6f, 0.4f, 0.3f);   // Brown
            UnityEngine.Color skyColor = new UnityEngine.Color(0.1f, 0.4f, 0.95f);
            UnityEngine.Color earthColor = new UnityEngine.Color(0.4f, 0.3f, 0.3f);

            float innerCircleRadius = 0.4f;
            float outerCircleRadius = 0.5f;

            float lineWidth = (outerCircleRadius - innerCircleRadius) * height;

            painter.BeginPath();
            painter.strokeColor = earthColor;
            painter.lineWidth = lineWidth;
            painter.Arc(changeSpace(new Vector2(0f, 0f), width, height), ((innerCircleRadius + outerCircleRadius) / 2) * height, currentYaw + 0, currentYaw + 180);
            painter.Stroke();


            painter.BeginPath();
            painter.strokeColor = skyColor;
            painter.lineWidth = lineWidth;
            painter.Arc(changeSpace(new Vector2(0f, 0f), width, height), ((innerCircleRadius + outerCircleRadius) / 2) * height, currentYaw + 180, currentYaw + 180 + 180);
            painter.Stroke();


            for (int i = 0; i <= 180; i += 30)
            {
                painter.BeginPath();
                painter.strokeColor = UnityEngine.Color.white;
                painter.lineWidth = lineWidth;
                painter.Arc(changeSpace(new Vector2(0f, 0f), width, height), ((innerCircleRadius + outerCircleRadius) / 2) * height, currentYaw + 180 + i - 1, currentYaw + 180 + i + 1);
                painter.Stroke();
            }

            painter.BeginPath();
            painter.strokeColor = UnityEngine.Color.white;
            painter.lineWidth = lineWidth/2;
            painter.Arc(changeSpace(new Vector2(0f, 0f), width, height), ((innerCircleRadius + outerCircleRadius) / 2) * height, currentYaw + 180 + 45 - 1, currentYaw + 180 + 45 + 1);
            painter.Stroke();

            painter.BeginPath();
            painter.strokeColor = UnityEngine.Color.white;
            painter.lineWidth = lineWidth/2;
            painter.Arc(changeSpace(new Vector2(0f, 0f), width, height), ((innerCircleRadius + outerCircleRadius) / 2) * height, currentYaw + 180 + 90+ 45 - 1, currentYaw + 180 +90+ 45 + 1);
            painter.Stroke();


            for(int i = -20;i<=20;i+=10)
            {
                painter.BeginPath();
                painter.strokeColor = UnityEngine.Color.white;
                painter.lineWidth = lineWidth / 4;
                painter.Arc(changeSpace(new Vector2(0f, 0f), width, height), ((innerCircleRadius + outerCircleRadius) / 2) * height, currentYaw + 180 + 90 + i - 1, currentYaw + 180 + 90 + i + 1);
                painter.Stroke();
            }

            //Inner Circle



            painter.BeginPath();
            painter.Arc(changeSpace(new Vector2(0f, 0f), width, height), innerCircleRadius*height, 0, 360);
            painter.ClosePath();

            painter.fillColor = (skyColor);
            painter.Fill();

            painter.BeginPath();
            painter.Arc(changeSpace(new Vector2(0f, 0f), width, height), innerCircleRadius * height, currentBank, 180 - currentBank);
            painter.ClosePath();

            painter.fillColor = (earthColor);
            painter.Fill();

            for (int i = 10; i <= 20; i += 10)
            {
                painter.BeginPath();
                painter.MoveTo(changeSpace(new Vector2(-0.1f*i/10, 2 * innerCircleRadius * Mathf.Sin((i - currentBank) * 0.017f)), width, height));
                mgc.DrawText(i.ToSafeString(), changeSpace(new Vector2(-0.1f * i / 10, 2 * innerCircleRadius * Mathf.Sin((i - currentBank) * 0.017f)), width, height), 8, UnityEngine.Color.white);
                painter.LineTo(changeSpace(new Vector2(0.1f * i / 10, 2 * innerCircleRadius * Mathf.Sin((i - currentBank) * 0.017f)), width, height));
                painter.strokeColor = UnityEngine.Color.white;
                painter.lineWidth = lineWidth / 8;
                painter.Stroke();
            }


            for (int i = -10; i >= -20; i -= 10)
            {
                painter.BeginPath();
                painter.MoveTo(changeSpace(new Vector2(-0.1f * i / 10,  2 * innerCircleRadius * Mathf.Sin((i - currentBank) * 0.017f)), width, height));
                mgc.DrawText(i.ToSafeString(), changeSpace(new Vector2(0.1f * i / 10, 2 * innerCircleRadius * Mathf.Sin((i - currentBank) * 0.017f)), width, height), 8, UnityEngine.Color.white);

                painter.LineTo(changeSpace(new Vector2(0.1f * i / 10, 2 * innerCircleRadius * Mathf.Sin((i - currentBank) * 0.017f)), width, height));
                painter.strokeColor = UnityEngine.Color.white;
                painter.lineWidth = lineWidth / 8;
                painter.Stroke();
            }







            // Draw T-shaped structure
            // Stem
            //painter.BeginPath();
            //painter.MoveTo(new Vector2(width * 0.48f, height * 0.55f));
            //painter.LineTo(new Vector2(width * 0.52f, height * 0.7f));
            //painter.LineTo(new Vector2(width * 0.52f, height * 0.7f));
            //painter.LineTo(new Vector2(width * 0.48f, height * 0.7f));
            //painter.ClosePath();
            //painter.fillColor = (stemColor);
            //painter.Fill();

            // Horizontal bars
            //// Left bar
            painter.BeginPath();

            painter.MoveTo(new Vector2(width * 0.3f, height * 0.49f));
            painter.LineTo(new Vector2(width * 0.45f, height * 0.49f));
            painter.LineTo(new Vector2(width * 0.45f, height * 0.51f));
            painter.LineTo(new Vector2(width * 0.3f, height * 0.51f));
            //// Right bar
            painter.MoveTo(new Vector2(width * 0.55f, height * 0.49f));
            painter.LineTo(new Vector2(width * 0.7f, height * 0.49f));
            painter.LineTo(new Vector2(width * 0.7f, height * 0.51f));
            painter.LineTo(new Vector2(width * 0.55f, height * 0.51f));
            painter.ClosePath();
            painter.fillColor = (barColor);
            painter.Fill();

            //// Center dot
            float dotRadius = width * 0.01f;
            painter.BeginPath();
            painter.Arc(new Vector2(width * 0.5f, height * 0.5f), dotRadius, 0, 360);
            painter.fillColor = (barColor);
            painter.Fill();

            painter.BeginPath();
            painter.MoveTo(changeSpace(new Vector2(0f, innerCircleRadius * 2), width, height));
            painter.LineTo(changeSpace(new Vector2(-0.05f, innerCircleRadius * 2 - 0.1f), width, height));
            painter.LineTo(changeSpace(new Vector2(0.05f, innerCircleRadius * 2 - 0.1f), width, height));
            painter.ClosePath();
            painter.fillColor = (UnityEngine.Color.magenta);
            painter.Fill();






        }


        private void UpdateVisualState()
        {
            MarkDirtyRepaint();

        }

        public void update(Dictionary<string, object> data)
        {
            // 'name' is assumed to be "attitude_instrument" 
            // so data[name] should refer to the JSON element containing { "bank": 15, "yaw": -11 }.
            if (data[name] is JsonElement element && element.ValueKind == JsonValueKind.Object)
            {
                // Attempt to get "yaw"
                if (element.TryGetProperty("yaw", out JsonElement yawElement) && yawElement.ValueKind == JsonValueKind.Number)
                {
                    if (yawElement.TryGetDouble(out double yawValue))
                    {
                        m_Yaw = (float)yawValue;
                    }
                }

                // Attempt to get "bank"
                if (element.TryGetProperty("bank", out JsonElement bankElement) && bankElement.ValueKind == JsonValueKind.Number)
                {
                    if (bankElement.TryGetDouble(out double bankValue))
                    {
                        m_Bank = (float)bankValue;
                    }
                }
            }

            UpdateVisualState();
            //Debug.Log(m_Yaw);
        }


        private Vector2 rotate(Vector2 point,float rotation)
        {
            Vector2 result = new Vector2() ;

            result.x = Mathf.Cos(rotation)*point.x + Mathf.Sin(rotation)*point.y;
            result.y = -1*Mathf.Sin(rotation) * point.x + Mathf.Cos(rotation) * point.y;

            return result;
        }

        private Vector2 changeSpace(Vector2 point,float width, float height)    
        {

            Vector2 result = new Vector2();
            result.x = (0.5f + point.x/2) * width;
            result.y = (0.5f - point.y/2) * height;
            //Debug.Log(result);
            return result;
        }

    }
}