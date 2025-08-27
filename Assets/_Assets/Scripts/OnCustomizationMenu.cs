using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class OnCustomizationMenu : MonoBehaviour
{
    [SerializeField] private MMFeedbacks customizationSong;

    // Start is called before the first frame update
    void Start()
    {
        SoundsAndEffects();
    }

    public void SoundsAndEffects()
    {
        customizationSong.PlayFeedbacks(); // Toca música do menu de customização
    }

}
