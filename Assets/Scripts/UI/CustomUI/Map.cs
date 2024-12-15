using Adapt;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomUI
{

    [UxmlElement]
    public partial class Map : VisualElement, IObserver
    {

        private string m_Latitude;
        private string m_Longitude;

        private Image m_MapImage;

        [UxmlAttribute]
        public string latitude
        {
            get => m_Latitude;
            set
            {
                m_Latitude = value;
                UpdateVisualState();
            }
        }
        
        [UxmlAttribute]
        public string longitude
        {
            get => m_Longitude;
            set
            {
                m_Longitude = value;
                UpdateVisualState();
            }
        }

        public Map()
        {


            m_MapImage = new Image();

            m_MapImage.image = Resources.Load<Texture2D>("antartica_map");
            //m_MapImage.style.position = Position.Absolute;
            m_MapImage.style.width = Length.Percent(100);
            m_MapImage.style.height = Length.Percent(100);
            Add(m_MapImage);


            VisualElement overlay = new VisualElement();

            overlay.generateVisualContent += OnGenerateVisualContent;

            // Style overlay so it matches the same size as Map
            overlay.style.flexGrow = 1;
            //overlay.style.backgroundColor = Color.white;
            overlay.style.position = Position.Absolute;
            overlay.style.width = Length.Percent(100);
            overlay.style.height = Length.Percent(100);


            // Add overlay *after* adding the image
            Add(overlay);

        }

        void OnGenerateVisualContent(MeshGenerationContext mgc)
        {


            var painter = mgc.painter2D;
            var rect = mgc.visualElement.contentRect;

            // Calculate dimensions based on container size
            float width = rect.width;
            float height = rect.height;


            Color dotColor = new UnityEngine.Color(102/255, 255/255, 255/255);
            //// Center dot
            float dotRadius = width * 0.01f;
            painter.BeginPath();
            painter.Arc(new Vector2(width * 0.4f, height * 0.5f), dotRadius, 0, 360);
            painter.fillColor = dotColor;
            painter.Fill();
            //Debug.Log(width);
        }

        private void UpdateVisualState()
        {
            MarkDirtyRepaint();
        }



        public void update(Dictionary<string, object> data)
        {
            // 'name' is assumed to be "location" 
            if (data[name] is JsonElement element && element.ValueKind == JsonValueKind.Object)
            {
                // Attempt to get "latitude"
                if (element.TryGetProperty("latitude", out JsonElement latElement) && latElement.ValueKind == JsonValueKind.String)
                {
                    m_Latitude = latElement.GetString();
                }

                // Attempt to get "longitude"
                if (element.TryGetProperty("longitude", out JsonElement longElement) && longElement.ValueKind == JsonValueKind.String)
                {
                    m_Longitude = longElement.GetString();
                }
            }
            //Debug.Log(m_Longitude);
            UpdateVisualState();
        }


        private Vector2 changeSpace(Vector2 point, float width, float height)
        {

            Vector2 result = new Vector2();
            result.x = (0.5f + point.x / 2) * width;
            result.y = (0.5f - point.y / 2) * height;
            //Debug.Log(result);
            return result;
        }


    }
}
