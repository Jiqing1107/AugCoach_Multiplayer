/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class ColorSync : RealtimeComponent<ColorSyncModel>
{
    [SerializeField] private MeshRenderer[] meshRenderers;
    [SerializeField] private Color color;
    private Color lastColor;

    private void Update()
    {
        if(color != lastColor)
        {
            model.color = color;
            lastColor = color;
        }
    }

    private void UpdateMeshRendererColor()
    {
        foreach (var renderer in meshRenderers)
        {
            foreach (var mat in renderer.materials)
            {
                mat.color = model.color;
            }
        }
    }

    protected override void OnRealtimeModelReplaced(ColorSyncModel previousModel, ColorSyncModel currentModel)
    {
        if (previousModel != null)
            previousModel.colorDidChange -= DidColorChange;

        if (currentModel.isFreshModel)
            currentModel.color = meshRenderers[0].material.color;

        UpdateMeshRendererColor();

        currentModel.colorDidChange += DidColorChange;
    }

    private void DidColorChange(ColorSyncModel mode, Color value)
    {
        UpdateMeshRendererColor();
    }
}
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Normal.Realtime;

public class ColorSync : RealtimeComponent<ColorSyncModel>
{
    private Color newColor;
    private Color currentColor;
    private MeshRenderer meshRenderer;
    private string text;

    public TMP_InputField selectedInputField;

    void Start()
    {
        newColor = Color.green;
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            currentColor = meshRenderer.material.GetColor("_Color");
        }
        else
        {
            // Provide a default color if there are no materials
            currentColor = Color.green;
        }
        Debug.Log("Set model color to be currentColor");
        model.color = currentColor;
        text = "";
        model.text = text;
    }

    private void Update()
    {
        string currentText = selectedInputField.text;
        if (currentText != text)
        {
            Debug.Log("Text box was updated");
            model.text = currentText;
            text = currentText;
        }

    }
    // Method to change the color of the object's material
    public void ChangeMaterialColor()
    {

        meshRenderer.material.SetColor("_Color", newColor);

        // Update the Realtime model's color
        model.color = newColor;
    }

    private void UpdateMeshRendererColor()
    {

        if (meshRenderer != null)
        {
            meshRenderer.material.SetColor("_Color", model.color);
        }
        else
        {
            Debug.LogWarning("MeshRenderer is null in UpdateMeshRendererColor()");
        }
    }

    protected override void OnRealtimeModelReplaced(ColorSyncModel previousModel, ColorSyncModel currentModel)
    {
        /*
        if (previousModel != null)
            previousModel.colorDidChange -= DidColorChange;

        // Set the initial color of the object's material to match the Realtime model's color
        UpdateMeshRendererColor();

        if (currentModel.isFreshModel)
        {
            // Set the initial color of the Realtime model to match the object's material color
            currentModel.color = currentColor;
        }

        currentModel.colorDidChange += DidColorChange;
        */
        if (previousModel != null)
        {
            previousModel.colorDidChange -= DidColorChange;
            previousModel.textDidChange -= DidTextChange;
        }

        // Set the initial color and text of the object's material to match the Realtime model's color and text
        UpdateMeshRendererColor();
        UpdateTextMeshText();

        if (currentModel.isFreshModel)
        {
            // Set the initial color and text of the Realtime model to match the object's material color and text
            currentModel.color = currentColor;
            currentModel.text = "testing";
        }

        currentModel.colorDidChange += DidColorChange;
        currentModel.textDidChange += DidTextChange;
    }

    private void DidColorChange(ColorSyncModel mode, Color value)
    {
        UpdateMeshRendererColor();
    }

    private void DidTextChange(ColorSyncModel nameSyncModel, string value)
    {
        UpdateTextMeshText();
    }

    private void UpdateTextMeshText()
    {
        if (selectedInputField != null)
        {
            Debug.Log("Text content is synchronized.");
            selectedInputField.text = model.text;
        }
    }

}
