using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Items", menuName = "ScriptableObjects/Item")]
public class ItemsSO : ScriptableObject
{
    public List<GameObject> headAccessories = new List<GameObject>();
    public List<GameObject> faceAccessories = new List<GameObject>();
    public List<Texture2D> skins = new List<Texture2D>();
    public List<Color> colors = new List<Color>();
}
