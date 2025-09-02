using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterVisualData
{
    int headIndex = 0;
    int faceIndex = 0;
    int skinIndex = 0;
    int colorIndex = 0;

    public CharacterVisualData(int headIndex, int faceIndex, int skinIndex, int colorIndex)
    {
        this.headIndex = headIndex;
        this.faceIndex = faceIndex;
        this.skinIndex = skinIndex;
        this.colorIndex = colorIndex;
    }

    public int GetHeadIndex() { return headIndex; }
    public int GetFaceIndex() { return faceIndex; }
    public int GetSkinIndex() { return skinIndex; }
    public int GetColorIndex() { return colorIndex; }
}
