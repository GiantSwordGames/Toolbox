    using System;
    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
    
    

[RequireComponent(typeof(MeshRenderer))]
public class MaterialColorPropertyOverride : MonoBehaviour
{
    public string property = "_Color";
    public Color color = Color.white;
    [Min(0)]
    public int materialIndex = 0;
    
    // Start is called before the first frame update
    void OnValidate()
    {
        UpdateColor();
    }

    private void Awake()
    {
        UpdateColor();
    }

    private void UpdateColor()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Material material = meshRenderer.sharedMaterials[materialIndex];
        if (material == null)
            return;

        int propertyIndex = material.shader.FindPropertyIndex(property);
        if (propertyIndex < 0)
        {
            Debug.Log($"Material {material} does not have a property: {property}");
            return;
        }

        // material.shader.GetPropertyType(
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(propertyBlock, materialIndex);
        propertyBlock.SetColor(property, color);
        meshRenderer.SetPropertyBlock(propertyBlock, materialIndex);
        
    }

    [ContextMenu("Reset")]
    private void OnDrawGizmos()
    { 
        
        return;
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(propertyBlock, materialIndex);
        propertyBlock.Clear();
        meshRenderer.SetPropertyBlock(propertyBlock, materialIndex);
    
    }
}
