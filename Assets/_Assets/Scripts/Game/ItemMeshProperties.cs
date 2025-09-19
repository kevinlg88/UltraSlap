using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemMeshProperties : MonoBehaviour
{
    [SerializeField] Renderer currentRenderer;
    [SerializeField] Material skinMaterial;
    [SerializeField] Material primaryMaterial;
    [SerializeField] Material neutralMaterial;

    public void SetSkinColor(SkinColor skinColor)
    {
        currentRenderer.materials = currentRenderer.materials
            .Select(m =>
            {
                if (m == skinMaterial || m.name.StartsWith(skinMaterial.name))
                {
                    var newMat = new Material(m)
                    {
                        color = skinColor.color
                    };
                    newMat.SetTexture("_ColorRampTex", skinColor.texture);
                    newMat.SetFloat("_ColorRampLuminosity", skinColor.ColorRampLuminosity);
                    newMat.SetFloat("_ColorRampBlend", skinColor.ColorRampBlend);
                    newMat.SetFloat("_SpecularAtten", skinColor.SpecularAttenuation);
                    return newMat;
                }
                return m;
            })
            .ToArray();
    }

    public void SetPrimaryColor(Color color)
    {
        currentRenderer.materials = currentRenderer.materials
            .Select(m => (m == primaryMaterial || m.name.StartsWith(primaryMaterial.name)) ? new Material(m) { color = color } : m)
            .ToArray();
    }
}
