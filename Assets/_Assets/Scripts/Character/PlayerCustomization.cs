using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerCustomization : MonoBehaviour
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
            colorIndex++;
            if (colorIndex >= item.colors.Count) colorIndex = 0;
        }
        else
        {
            colorIndex--;
            if (colorIndex < 0) colorIndex = item.colors.Count - 1;
        }
        body.materials[0].color = item.colors[colorIndex];
        head.materials[0].color = item.colors[colorIndex];
    }
    public void ChangePlayerColor()
    {
        body.materials[0].color = item.colors[colorIndex];
        head.materials[0].color = item.colors[colorIndex];
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

    public void ChangeHeadAccessory()
    {
        RemoveAccessory(headSlot);
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
    public void ChangeFaceAccessory()
    {
        RemoveAccessory(faceSlot);
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
    public void ChangeSkin()
    {
        body.materials[1].mainTexture = item.skins[skinIndex];
    }

    public void SaveCharacterVisual(PlayerData playerData)
    {
        if (playerData != null)
        {
            playerData.PlayerVisual = new CharacterVisualData(headIndex, faceIndex, skinIndex, colorIndex);
        }
    }

    public void LoadCharacterVisual(PlayerData playerData)
    {
        if (playerData != null)
        {
            headIndex = playerData.PlayerVisual.GetHeadIndex();
            faceIndex = playerData.PlayerVisual.GetFaceIndex();
            skinIndex = playerData.PlayerVisual.GetSkinIndex();
            colorIndex = playerData.PlayerVisual.GetColorIndex();

            ChangeHeadAccessory();
            ChangeFaceAccessory();
            ChangeSkin();
            ChangePlayerColor();
        }
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
