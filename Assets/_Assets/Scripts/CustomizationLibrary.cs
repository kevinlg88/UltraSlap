using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MaterialOption
{
    public string name; // Usado só para você identificar no editor
    public Material material;
}

[System.Serializable]
public class PropOption
{
    public string name; // Usado só para você identificar no editor
    public GameObject prefab;
}

[System.Serializable]
public class OutfitOption
{
    public string name;
    public GameObject prefab;
    public Material material;
}

[CreateAssetMenu(menuName = "Customization/Simple Library")]
public class CustomizationLibrary : ScriptableObject
{
    public List<PropOption> headProps;
    public List<MaterialOption> skinMaterials;
    public List<PropOption> eyesProps;
    public List<PropOption> earsProps;
    public List<PropOption> lowerFaceProps;
    public List<OutfitOption> outfits;
}