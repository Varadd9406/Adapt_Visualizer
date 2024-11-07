using UnityEngine;
using UnityEngine.UIElements;

public partial class GaugeMeter : VisualElement
{


    public GaugeMeter()
    {
        generateVisualContent += OnGenerateVisualContent;
    }

    void OnGenerateVisualContent(MeshGenerationContext mgc)
    {

        float width = contentRect.width;
        float height = contentRect.height;

        var painter = mgc.painter2D;


        painter.BeginPath();
        painter.lineWidth = 10f;
        painter.Arc(new Vector2(width*0.5f, height),width*0.5f,180f,0f);
        painter.ClosePath();
        painter.fillColor = Color.white;

        painter.Fill(FillRule.NonZero);
        painter.Stroke();
    }
}