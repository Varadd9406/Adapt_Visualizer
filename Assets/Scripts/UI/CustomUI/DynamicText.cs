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
    public partial class DynamicText : VisualElement, IObserver
    {

        private string m_Value;

        private Label m_Label;

        [UxmlAttribute]
        public string value
        {
            get => m_Value;
            set
            {
                m_Value = value;
                UpdateVisualState();
            }
        }

        public DynamicText()
        {


            m_Label = new Label();

            m_Label.text = "N/A";

            Add(m_Label);
        }


        private void UpdateVisualState()
        {
            m_Label.text = m_Value;
        }



        public void update(Dictionary<string, object> data)
        {
            if (data.ContainsKey(name))
            {
                if (data[name] is JsonElement element && element.ValueKind == JsonValueKind.String)
                {
                    string strValue = element.GetString();
                    m_Value = strValue;
                }
            }
            else
            {
                Debug.Log(String.Format("Key - {0} not found",name));
            }
            UpdateVisualState();
        }


    }
}
