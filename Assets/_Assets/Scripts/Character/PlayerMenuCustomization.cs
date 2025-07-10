using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenuCustomization : MonoBehaviour
{

    [Header("Itens References")]
    [SerializeField] private ItemsSO item;


    [Header("Renderers References")]
    [SerializeField] private Renderer head;
    [SerializeField] private Renderer body;


    [Header("Slots Point References")]
    [SerializeField] private Transform headSlot;
    [SerializeField] private Transform faceSlot;
    [SerializeField] private Transform GlovesSlot;

    int headIndex = 0;
    int faceIndex = 0;
    int skinIndex = 0;
    int colorIndex = 0;

    public void ChangePlayerColor(bool isNext)
    {
        if (isNext)
        {
            skinIndex++;
            if (skinIndex >= item.colors.Count) skinIndex = 0;
        }
        else
        {
            skinIndex--;
            if (skinIndex < 0) skinIndex = item.colors.Count - 1;
        }
        body.materials[0].color = item.colors[skinIndex];
        head.materials[0].color = item.colors[skinIndex];
    }

    public void ChangeHeadAccessory(bool isNext)
    {
        RemoveAccessory(headSlot);
        if (isNext)
        {
            headIndex++;
            if (headIndex >= item.headAccessories.Count) headIndex = 0;
        }
        else
        {
            headIndex--;
            if (headIndex < 0) headIndex = item.headAccessories.Count - 1;
        }

        GameObject accessory = Instantiate(item.headAccessories[headIndex], headSlot.position, Quaternion.identity);
        SkinnedMeshRenderer smr = accessory.GetComponent<SkinnedMeshRenderer>();
        smr.bones = (body as SkinnedMeshRenderer).bones;
        smr.rootBone = (body as SkinnedMeshRenderer).rootBone;
        accessory.transform.SetParent(headSlot);
    }

    public void ChangeFaceAccessory(bool isNext)
    {
        RemoveAccessory(faceSlot);
        if (isNext)
        {
            faceIndex++;
            if (faceIndex >= item.faceAccessories.Count) faceIndex = 0;
        }
        else
        {
            faceIndex--;
            if (faceIndex < 0) faceIndex = item.faceAccessories.Count - 1;
        }

        GameObject accessory = Instantiate(item.faceAccessories[faceIndex], faceSlot.position, Quaternion.identity);
        SkinnedMeshRenderer smr = accessory.GetComponent<SkinnedMeshRenderer>();
        smr.bones = (body as SkinnedMeshRenderer).bones;
        smr.rootBone = (body as SkinnedMeshRenderer).rootBone;
        accessory.transform.SetParent(faceSlot);
    }

    public void ChangeSkin(bool isNext)
    {
        if (isNext)
        {
            skinIndex++;
            if (skinIndex >= item.skins.Count) skinIndex = 0;
        }
        else
        {
            skinIndex--;
            if (skinIndex < 0) skinIndex = item.skins.Count - 1;
        }
        body.materials[1].mainTexture = item.skins[skinIndex];
    }

    private void RemoveAccessory(Transform slot)
    {
        if (slot.childCount > 0)
        {
            foreach (Transform child in slot)
            {
                Debug.Log("Removing accessory from slot: " + child.name);
                Destroy(child.gameObject);
            }
        }
    }

}
