using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Items", menuName = "ScriptableObjects/Item")]
public class ItemsSO : ScriptableObject
{
    public List<GameObject> headAccessories = new();
    public List<GameObject> faceAccessories = new();
    public List<GameObject> cloths = new();
    public List<Color> skinColors = new();
    public List<Color> teamColors = new();
}
