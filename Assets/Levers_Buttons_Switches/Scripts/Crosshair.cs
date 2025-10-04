using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    Color nonInteractable = new Color(1f, 1f, 1f);
    Color interactable = new Color(1f, 0.7f, 0.7f);

    void Start()
    {
        SetNonInteractable();
    }

    public void SetInteractable()
    {
        foreach(Image image in GetComponentsInChildren<Image>())
        {
            image.color = interactable;
        }
    }

    public void SetNonInteractable()
    {
        foreach(Image image in GetComponentsInChildren<Image>())
        {
            image.color = nonInteractable;
        }
    }
}
