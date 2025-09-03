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

    int teamIndex = 0;
    int colorSkinIndex = 0;
    int headIndex = 0;
    int faceIndex = 0;
    int clothIndex = 0;

    #region  ==== CUSTOMIZATION ====

    public void ChangeTeam(bool isNext)
    {
        if (isNext)
        {
            teamIndex++;
            if (teamIndex >= item.teamColors.Count) teamIndex = 0;
        }
        else
        {
            teamIndex--;
            if (teamIndex < 0) teamIndex = item.skinColors.Count - 1;
        }
        Debug.Log($"team: {teamIndex}");
    }
    public void ChangePlayerSkinColor(bool isNext)
    {
        if (isNext)
        {
            colorSkinIndex++;
            if (colorSkinIndex >= item.skinColors.Count) colorSkinIndex = 0;
        }
        else
        {
            colorSkinIndex--;
            if (colorSkinIndex < 0) colorSkinIndex = item.skinColors.Count - 1;
        }
        body.materials[0].color = item.skinColors[colorSkinIndex];
        head.materials[0].color = item.skinColors[colorSkinIndex];
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

        GameObject accessory = Instantiate(item.headAccessories[headIndex], headSlot);
        accessory.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        accessory.transform.localScale = Vector3.one;
        // if (accessory.TryGetComponent(out SkinnedMeshRenderer smr))
        // {
        //     smr.bones = (body as SkinnedMeshRenderer).bones;
        //     smr.rootBone = (body as SkinnedMeshRenderer).rootBone;
        // }
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

        GameObject accessory = Instantiate(item.faceAccessories[faceIndex], faceSlot);
        accessory.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        accessory.transform.localScale = Vector3.one;
        // if (accessory.TryGetComponent(out SkinnedMeshRenderer smr))
        // {
        //     smr.bones = (body as SkinnedMeshRenderer).bones;
        //     smr.rootBone = (body as SkinnedMeshRenderer).rootBone;
        // }
    }
    public void ChangeCloth(bool isNext)
    {
        if (isNext)
        {
            clothIndex++;
            if (clothIndex >= item.cloths.Count) clothIndex = 0;
        }
        else
        {
            clothIndex--;
            if (clothIndex < 0) clothIndex = item.cloths.Count - 1;
        }
        // GameObject accessory = Instantiate(item.faceAccessories[faceIndex], faceSlot);
        // accessory.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        // accessory.transform.localScale = Vector3.one;
        // if (accessory.TryGetComponent(out SkinnedMeshRenderer smr))
        // {
        //     smr.bones = (body as SkinnedMeshRenderer).bones;
        //     smr.rootBone = (body as SkinnedMeshRenderer).rootBone;
        // }
    }
    #endregion

    #region ==== LOAD CUSTOMIZATION ====

    public void LoadPlayerSkinColor()
    {
        body.materials[0].color = item.skinColors[colorSkinIndex];
        head.materials[0].color = item.skinColors[colorSkinIndex];
    }
    public void LoadHeadAccessory()
    {
        RemoveAccessory(headSlot);
        GameObject accessory = Instantiate(item.headAccessories[headIndex], headSlot.position, Quaternion.identity);
        SkinnedMeshRenderer smr = accessory.GetComponent<SkinnedMeshRenderer>();
        smr.bones = (body as SkinnedMeshRenderer).bones;
        smr.rootBone = (body as SkinnedMeshRenderer).rootBone;
        accessory.transform.SetParent(headSlot);
    }
    public void LoadFaceAccessory()
    {
        RemoveAccessory(faceSlot);
        GameObject accessory = Instantiate(item.faceAccessories[faceIndex], faceSlot.position, Quaternion.identity);
        SkinnedMeshRenderer smr = accessory.GetComponent<SkinnedMeshRenderer>();
        smr.bones = (body as SkinnedMeshRenderer).bones;
        smr.rootBone = (body as SkinnedMeshRenderer).rootBone;
        accessory.transform.SetParent(faceSlot);
    }
    public void LoadCloth()
    {
        //body.materials[1].mainTexture = item.skins[skinIndex];
    }
    #endregion
    public void SaveCharacterVisual(PlayerData playerData)
    {
        if (playerData != null)
        {

            playerData.Team = new Team((TeamEnum) teamIndex, item.teamColors[teamIndex]);
            playerData.PlayerVisual = new CharacterVisualData(headIndex, faceIndex, clothIndex, colorSkinIndex);
        }
    }

    public void LoadCharacterVisual(PlayerData playerData)
    {
        if (playerData != null)
        {
            teamIndex = (int)playerData.Team.TeamEnum;
            colorSkinIndex = playerData.PlayerVisual.GetColorIndex();
            headIndex = playerData.PlayerVisual.GetHeadIndex();
            faceIndex = playerData.PlayerVisual.GetFaceIndex();
            clothIndex = playerData.PlayerVisual.GetSkinIndex();

            LoadPlayerSkinColor();
            LoadHeadAccessory();
            LoadFaceAccessory();
            LoadCloth();
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
