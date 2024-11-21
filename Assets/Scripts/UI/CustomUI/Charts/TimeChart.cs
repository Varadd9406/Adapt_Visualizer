using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using Adapt;
using static UnityEngine.Rendering.DebugUI;
using System.Text.Json;
using Unity.VisualScripting;
using System;

namespace CustomUI
{

    [UxmlElement]
    public partial class TimeChart : VisualElement, IObserver
    {
        private const float PADDING = 40f;
        private const float TICK_SIZE = 5f;
        private const float POINT_RADIUS = 3f;
        private const int BUFFER_SIZE = 100;

        private SlidingBuffer<System.Numerics.Vector<double>> dataPoints = new SlidingBuffer<System.Numerics.Vector<double>>(BUFFER_SIZE);


        private List<TextElement> labelElements = new List<TextElement>();

        // Customizable properties
        public Color AxisColor { get; set; } = new Color(0.7f, 0.7f, 0.7f,1f);
        public Color LineColor { get; set; } = new Color(0.2f, 0.6f, 1f);
        public Color GridColor { get; set; } = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        public bool ShowGrid { get; set; } = true;
        public bool ShowPoints { get; set; } = true;



        [UxmlAttribute]
        public float LineWidth { get; set; } = 4f;

        public TimeChart()
        {

            generateVisualContent += OnGenerateVisualContent;
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        public void update(Dictionary<string, object> data)
        {
            if (data["timestamp"] is JsonElement x_value && data[name] is JsonElement y_value)
            {
                if (x_value.ValueKind == JsonValueKind.Number && y_value.ValueKind == JsonValueKind.Number)
                {
                    if (x_value.TryGetDouble(out double timeValue) && y_value.TryGetDouble(out double value))
                    {
                        dataPoints.Add(new System.Numerics.Vector<double>(new double[] { timeValue, value }));
                        MarkDirtyRepaint();
                        RecreateLabels();
                    }
                }
            }
        }
        public void ResetData()
        {
            dataPoints = new SlidingBuffer<System.Numerics.Vector<double>>(BUFFER_SIZE);
            MarkDirtyRepaint();
            RecreateLabels();
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            MarkDirtyRepaint();
            RecreateLabels();
        }

        private void ClearLabels()
        {
            foreach (var label in labelElements)
            {
                Remove(label);
            }
            labelElements.Clear();
        }
        private void RecreateLabels()
        {
            if (dataPoints.Count() < 2) return;

            ClearLabels();

            var rect = contentRect;
            var chartArea = new Rect(
                PADDING,
                PADDING,
                rect.width - (PADDING * 2),
                rect.height - (PADDING * 2)
            );

            double minX = dataPoints.Min(p => p[0]);
            double maxX = dataPoints.Max(p => p[0]);
            double minY = dataPoints.Min(p => p[1]);
            double maxY = dataPoints.Max(p => p[1]);

            // Add padding to Y range
            double yPadding = (maxY - minY) * 0.1f;
            minY -= yPadding;
            maxY += yPadding;

            // X axis labels
            for (int i = 0; i <= 5; i++)
            {
                float x = chartArea.x + (chartArea.width * i / 5);
                double value = Adapt.MathUtil.LerpDouble(minX, maxX, i / 5f);
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds((long)value);
                string dateString = dateTimeOffset.ToString("HH:mm:ss");

                var label = new TextElement
                {

                    text = dateString,
                    style =
                    {
                        position = Position.Absolute,
                        left = x - 20,
                        top = chartArea.y + chartArea.height + TICK_SIZE + 5,
                        color = new StyleColor(AxisColor),
                        unityTextAlign = TextAnchor.UpperCenter,
                        fontSize = 12
                    }
                };
                Add(label);
                labelElements.Add(label);
            }

            // Y axis labels
            for (int i = 0; i <= 5; i++)
            {
                float y = chartArea.y + (chartArea.height * (5 - i) / 5);
                double value = Adapt.MathUtil.LerpDouble(minY, maxY, i / 5f);

                var label = new TextElement
                {
                    text = value.ToString("F1"),
                    style =
                    {
                        position = Position.Absolute,
                        left = PADDING - 50,
                        top = y - 8,
                        color = new StyleColor(AxisColor),
                        unityTextAlign = TextAnchor.MiddleRight,
                        fontSize = 12
                    }
                };
                Add(label);
                labelElements.Add(label);
            }
        }



        private void OnGenerateVisualContent(MeshGenerationContext mgc)
        {

            var painter = mgc.painter2D;
            var rect = contentRect;

            // Calculate chart area
            var chartArea = new Rect(
                PADDING,
                PADDING,
                rect.width - (PADDING * 2),
                rect.height - (PADDING * 2)
            );

            DrawAxes( painter, chartArea );
            //Debug.Log(chartArea);

            if (dataPoints.Count() < 2) return;


            // Get data ranges
            double minX = dataPoints.Min(p => p[0]);
            double maxX = dataPoints.Max(p => p[0]);
            double minY = dataPoints.Min(p => p[1]);
            double maxY = dataPoints.Max(p => p[1]);

            // Add some padding to Y range
            double yPadding = (maxY - minY) * 0.1f;
            minY -= yPadding;
            maxY += yPadding;


            //DrawGrid(painter, chartArea, minX, maxX, minY, maxY);

            // Draw axes

            // Draw labels
            //DrawAxisLabels(painter, chartArea, minX, maxX, minY, maxY);

            // Draw data line
            DrawDataLine(painter, chartArea, minX, maxX, minY, maxY);

            //if (ShowPoints)
            //{
            //    DrawDataPoints(painter, chartArea, minX, maxX, minY, maxY);
            //}
        }

        private void DrawGrid(Painter2D painter, Rect chartArea, float minX, float maxX, float minY, float maxY)
        {
            painter.strokeColor = GridColor;
            painter.lineWidth = 1f;

            // Vertical grid lines
            int numVerticalLines = 10;
            for (int i = 0; i <= numVerticalLines; i++)
            {
                float x = chartArea.x + (chartArea.width * i / numVerticalLines);
                painter.BeginPath();
                painter.MoveTo(new Vector2(x, chartArea.y));
                painter.LineTo(new Vector2(x, chartArea.y + chartArea.height));
                painter.Stroke();
            }

            // Horizontal grid lines
            int numHorizontalLines = 5;
            for (int i = 0; i <= numHorizontalLines; i++)
            {
                float y = chartArea.y + (chartArea.height * i / numHorizontalLines);
                painter.BeginPath();
                painter.MoveTo(new Vector2(chartArea.x, y));
                painter.LineTo(new Vector2(chartArea.x + chartArea.width, y));
                painter.Stroke();
            }
        }

        private void DrawAxes(Painter2D painter, Rect chartArea)
        {
            painter.strokeColor = AxisColor;
            painter.lineWidth = 2f;

            // X axis
            painter.BeginPath();
            painter.MoveTo(new Vector2(chartArea.x, chartArea.y + chartArea.height));
            painter.LineTo(new Vector2(chartArea.x + chartArea.width, chartArea.y + chartArea.height));
            painter.Stroke();

            // Y axis
            painter.BeginPath();
            painter.MoveTo(new Vector2(chartArea.x, chartArea.y));
            painter.LineTo(new Vector2(chartArea.x, chartArea.y + chartArea.height));
            painter.Stroke();
        }

        private void DrawAxisLabels(Painter2D painter, Rect chartArea, float minX, float maxX, float minY, float maxY)
        {
            // X axis labels
            for (int i = 0; i <= 5; i++)
            {
                float x = chartArea.x + (chartArea.width * i / 5);
                float value = Mathf.Lerp(minX, maxX, i / 5f);

                // Draw tick
                painter.BeginPath();
                painter.MoveTo(new Vector2(x, chartArea.y + chartArea.height));
                painter.LineTo(new Vector2(x, chartArea.y + chartArea.height + TICK_SIZE));
                painter.Stroke();

                // Draw label
            }

            // Y axis labels
            for (int i = 0; i <= 5; i++)
            {
                float y = chartArea.y + (chartArea.height * (5 - i) / 5);
                float value = Mathf.Lerp(minY, maxY, i / 5f);

                // Draw tick
                painter.BeginPath();
                painter.MoveTo(new Vector2(chartArea.x - TICK_SIZE, y));
                painter.LineTo(new Vector2(chartArea.x, y));
                painter.Stroke();

                // Draw label
                //DrawText(painter, value.ToString("F1"),
                //    new Vector2(chartArea.x - TICK_SIZE - 5, y),
                //    TextAnchor.MiddleRight);
            }
        }

        private void DrawDataLine(Painter2D painter, Rect chartArea, double minX, double maxX, double minY, double maxY)
        {
            painter.strokeColor = LineColor;
            painter.lineWidth = LineWidth;
            painter.BeginPath();

            bool firstVal = true;

            foreach (var point in dataPoints)
            {

                float x = Mathf.Lerp(chartArea.x, chartArea.x + chartArea.width,
                    (float)((point[0] - minX) / (maxX - minX)));


                float y = Mathf.Lerp(chartArea.y + chartArea.height, chartArea.y,
                    (float)((point[1] - minY) / (maxY - minY)));

                if (firstVal)
                {
                    firstVal = false;
                    painter.MoveTo(new Vector2(x, y));
                }
                else
                {
                    painter.LineTo(new Vector2(x, y));
                }
            }

            painter.Stroke();
        }

        private void DrawDataPoints(Painter2D painter, Rect chartArea, float minX, float maxX, float minY, float maxY)
        {
            foreach (var point in dataPoints)
            {
                float x = Mathf.Lerp(chartArea.x, chartArea.x + chartArea.width,
                    (float)((point[0] - minX) / (maxX - minX)));


                float y = Mathf.Lerp(chartArea.y + chartArea.height, chartArea.y,
                    (float)((point[1] - minY) / (maxY - minY)));

                painter.BeginPath();
                painter.Arc(new Vector2(x, y), POINT_RADIUS, 0, 360);
                painter.Fill(FillRule.NonZero);
            }
        }

        private void AddTextElement(Painter2D painter, string text, Vector2 position, TextAnchor anchor)
        {
            var textElement = new TextElement { text = text };
            textElement.style.position = Position.Absolute;
            textElement.style.left = position.x;
            textElement.style.top = position.y;
            textElement.style.color = AxisColor;
            textElement.style.unityTextAlign = anchor;
            textElement.style.fontSize = 12;
            Add(textElement);
        }
    }

    public static class VectorChartExtensions
    {
        public static TimeChart CreateVectorChart(this VisualElement parent)
        {
            var chart = new TimeChart();
            parent.Add(chart);
            return chart;
        }
    }

}

// Extension method for easy instantiation
