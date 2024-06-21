using System;
using UnityEngine;

public class Wire : MonoBehaviour
{
    public void SetPhotons(double intensity)
	{
        Renderer renderer = GetComponent<Renderer>();
        Material mat = renderer.material;
        
        float emission = Math.Min(1.0f,(float)intensity / 30.0f);
        // Debug.Log(emission);
        Color baseColor = Color.green;
        Color finalColor = baseColor * emission;

        mat.SetColor("_EmissionColor", finalColor);
    }
}