using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

namespace CustomUI
{

    [UxmlElement]
    public partial class VectorChart : VisualElement
    {
        private List<Vector2> dataPoints = new List<Vector2>();
        private const float PADDING = 40f;
        private const float TICK_SIZE = 5f;
        private const float POINT_RADIUS = 3f;

        // Customizable properties
        public Color AxisColor { get; set; } = new Color(0.4f, 0.4f, 0.4f,0.4f);
        public Color LineColor { get; set; } = new Color(0.2f, 0.6f, 1f);
        public Color GridColor { get; set; } = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        public bool ShowGrid { get; set; } = true;
        public bool ShowPoints { get; set; } = true;



        [UxmlAttribute]
        public float LineWidth { get; set; } = 4f;

        public VectorChart()
        {

            generateVisualContent += OnGenerateVisualContent;
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        public void SetData(List<Vector2> points)
        {
            dataPoints = new List<Vector2>(points);
            MarkDirtyRepaint();
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            MarkDirtyRepaint();
        }

        private void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            if (dataPoints.Count < 2) return;

            var painter = mgc.painter2D;
            var rect = contentRect;

            // Calculate chart area
            var chartArea = new Rect(
                PADDING,
                PADDING,
                rect.width - (PADDING * 2),
                rect.height - (PADDING * 2)
            );
            //Debug.Log(chartArea);

            // Get data ranges
            float minX = dataPoints.Min(p => p.x);
            float maxX = dataPoints.Max(p => p.x);
            float minY = dataPoints.Min(p => p.y);
            float maxY = dataPoints.Max(p => p.y);

            // Add some padding to Y range
            float yPadding = (maxY - minY) * 0.1f;
            minY -= yPadding;
            maxY += yPadding;


            //DrawGrid(painter, chartArea, minX, maxX, minY, maxY);

            // Draw axes
            DrawAxes(painter, chartArea);

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
                //DrawText(painter, value.ToString("F1"),
                //    new Vector2(x, chartArea.y + chartArea.height + TICK_SIZE + 5),
                //    TextAnchor.UpperCenter);
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

        private void DrawDataLine(Painter2D painter, Rect chartArea, float minX, float maxX, float minY, float maxY)
        {
            painter.strokeColor = LineColor;
            painter.lineWidth = LineWidth;
            painter.BeginPath();

            for (int i = 0; i < dataPoints.Count; i++)
            {
                float x = Mathf.Lerp(chartArea.x, chartArea.x + chartArea.width,
                    (dataPoints[i].x - minX) / (maxX - minX));
                float y = Mathf.Lerp(chartArea.y + chartArea.height, chartArea.y,
                    (dataPoints[i].y - minY) / (maxY - minY));

                if (i == 0)
                    painter.MoveTo(new Vector2(x, y));
                else
                    painter.LineTo(new Vector2(x, y));
            }

            painter.Stroke();
        }

        private void DrawDataPoints(Painter2D painter, Rect chartArea, float minX, float maxX, float minY, float maxY)
        {
            foreach (var point in dataPoints)
            {
                float x = Mathf.Lerp(chartArea.x, chartArea.x + chartArea.width,
                    (point.x - minX) / (maxX - minX));
                float y = Mathf.Lerp(chartArea.y + chartArea.height, chartArea.y,
                    (point.y - minY) / (maxY - minY));

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
        public static VectorChart CreateVectorChart(this VisualElement parent)
        {
            var chart = new VectorChart();
            parent.Add(chart);
            return chart;
        }
    }

}

// Extension method for easy instantiation
