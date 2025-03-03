using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class LevelManager : MonoBehaviour
{

    [SerializeField] private MMFeedbacks levelSong;


    // Start is called before the first frame update
    void Start()
    {
        levelSong.PlayFeedbacks();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
