using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class CharacterFeelManager : MonoBehaviour
{

    [System.Serializable]
    public class NamedFeedback
    {
        public string name;
        public MMFeedbacks feedback;
    }

    [Header("Feedbacks")]
    public List<NamedFeedback> feedbacks;

    private Dictionary<string, MMFeedbacks> feedbackDict;

    void Awake()
    {
        feedbackDict = new Dictionary<string, MMFeedbacks>();
        foreach (var f in feedbacks)
        {
            if (!feedbackDict.ContainsKey(f.name))
            {
                feedbackDict.Add(f.name, f.feedback);
            }
        }
    }

    public void Play(string name)
    {
        if (feedbackDict.TryGetValue(name, out var feedback))
        {
            feedback.PlayFeedbacks();
        }
        else
        {
            Debug.LogWarning($"[FeelManager] Feedback '{name}' not found!");
        }
    }
}
