using FIMSpace.FProceduralAnimation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField] private int maxHealth, health;
    private RagdollAnimator2 ragdoll;


    // Start is called before the first frame update
    void Start()
    {
        ragdoll = GetComponent<RagdollAnimator2>();
    }

    void OnEnable()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public int GetHealth()
    {
        return health;
    }

    public void SetHealth(int newHealth)
    {
        health = newHealth;

        //UnityEngine.Debug.Log("Vai desabilitar o ragdoll" + ragdoll);
        if (health <= 0) {
            ragdoll.User_SwitchFallState();
        }
    }
}
