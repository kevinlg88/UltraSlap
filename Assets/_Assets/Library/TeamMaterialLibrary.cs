using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TeamMaterialOption
{
    public string name; // Usado só para você identificar no editor
    public Material material;
}

[CreateAssetMenu(menuName = "Customization/Team Material Library")]
public class TeamMaterialLibrary : ScriptableObject
{
    public List<TeamMaterialOption> teamMaterials;
}
