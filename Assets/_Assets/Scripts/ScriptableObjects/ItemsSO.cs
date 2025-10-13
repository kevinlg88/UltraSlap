using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Items", menuName = "ScriptableObjects/Item")]
public class ItemsSO : ScriptableObject
{
    public List<GameObject> headAccessories = new();
    public List<GameObject> eyeAccessories = new();
    public List<GameObject> faceAccessories = new();
    public List<GameObject> cloths = new();
    public List<SkinColor> skinColors = new();
    public List<Color> teamColors = new();
}
[Serializable]
public class SkinColor
{
    public Color color;
    public Texture2D texture;
    public float ColorRampLuminosity;
    public float ColorRampBlend;
    public float SpecularAttenuation;
}