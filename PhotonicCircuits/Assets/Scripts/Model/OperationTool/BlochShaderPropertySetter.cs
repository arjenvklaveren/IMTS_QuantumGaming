using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlochShaderPropertySetter : MonoBehaviour
{
    [Header("Shader settings")]
    [SerializeField] private Color sphereColor = new Color(0, 0, 0, 1);
    [SerializeField] private Color outlineColor = new Color(0, 0, 0, 1);
    [SerializeField, Range(0.5f, 1.5f)] private float outlineWidth = 1;
    [SerializeField, Range(10.0f, 15.0f)] private float outlineGap = 15;
    [SerializeField] private bool depthLines;

    [Header("Object references")]
    [SerializeField] Renderer sphereFrontMat;
    [SerializeField] Renderer sphereBackMat;
    [SerializeField] Renderer gridMat;

    private void OnValidate()
    {
        UpdateShaderProperties();
    }

    void UpdateShaderProperties()
    {
        if (sphereFrontMat)
        {
            sphereFrontMat.sharedMaterial.SetColor("_SphereColor", sphereColor);
            sphereFrontMat.sharedMaterial.SetColor("_LineColor", outlineColor);
            sphereFrontMat.sharedMaterial.SetFloat("_LineWidth", outlineWidth);
            int depthBoolInt = depthLines ? 1 : 0;
            sphereFrontMat.sharedMaterial.SetInt("_DepthLines", depthBoolInt);
        }
        if (sphereBackMat)
        {
            sphereBackMat.sharedMaterial.SetColor("_SphereColor", sphereColor);
            sphereBackMat.sharedMaterial.SetColor("_LineColor", outlineColor);
            sphereBackMat.sharedMaterial.SetFloat("_LineWidth", outlineWidth);
            sphereBackMat.sharedMaterial.SetFloat("_LineGap", outlineGap);
            int depthBoolInt = depthLines ? 1 : 0;
            sphereBackMat.sharedMaterial.SetInt("_DepthLines", depthBoolInt);
        }
        if (gridMat)
        {
            int drawLinesInt = depthLines ? 1 : 0;
            gridMat.sharedMaterial.SetColor("_LineColor", sphereColor);
            gridMat.sharedMaterial.SetInt("_DrawLines", drawLinesInt);
        }
    }
}
