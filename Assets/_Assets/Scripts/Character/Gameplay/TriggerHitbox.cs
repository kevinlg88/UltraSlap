using FIMSpace.FProceduralAnimation;
using UnityEngine;
using MoreMountains.Feedbacks;

public class TriggerHitbox : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerController player;
    private PlayerSlap playerSlap;
    private RagdollAnimator2 myragdoll;
    [SerializeField] private MMFeedbacks slapEnemy, slapProp, slapEnvironment;

    [Header("Slap Setup")]
    [SerializeField] private LayerMask glassLayer;
    [SerializeField] float slapPowerFallingThreshold;
    [SerializeField] bool isSlapping = false;


    [Header("Slap Effect Setup")]
    [SerializeField] GameObject effectPrefab;
    [SerializeField] float maxEffectPrefabScale = 3.5f;



    void Awake()
    {
        playerSlap = player?.GetComponent<PlayerSlap>();
        myragdoll = player?.GetComponent<RagdollAnimator2>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"Bateu no seguinte objeto: {other}");

        if (isSlapping || other.gameObject.layer == 10) return; //Se bateu em splinters (cacos de vidro), ignore
        if (!other.attachedRigidbody)
        {
            slapEnvironment.PlayFeedbacks(); // Implementar o Manager de feedbacks para ativar por eventos
            if (other.gameObject.layer == glassLayer) SlapWindow(other);

            if (other.gameObject.layer == 12) //Se bateu em um device activator
            {
                // Pega o MMF_Player no objeto que bateu
                MMF_Player feedbackPlayer = other.gameObject.GetComponent<MMF_Player>();
                if (feedbackPlayer != null)
                {
                    feedbackPlayer.PlayFeedbacks();
                }
                else
                {
                    Debug.LogWarning($"O objeto {other.gameObject.name} está na layer DeviceActivator mas não tem MMF_Player.");
                }
            }
        }
        else Slap(other);
    }

    private void Slap(Collider other)
    {
        isSlapping = true;
        RagdollAnimator2 ragdoll = GetRagdoll(other.attachedRigidbody.gameObject);
        if (ragdoll != null)
        {
            if (ragdoll.gameObject.name == myragdoll.gameObject.name) return;
            ApplyDamage(ragdoll.GetComponent<PlayerController>());
            if (playerSlap.GetPower() >= slapPowerFallingThreshold)
            {
                ragdoll.GetComponent<PlayerController>().SetIsFalling();
            }
            slapEnemy.PlayFeedbacks();
        }

        if (ragdoll) ragdoll.RA2Event_AddHeadImpact(this.gameObject.transform.forward * playerSlap.GetPower());
        else other.attachedRigidbody.AddForce(this.gameObject.transform.forward * playerSlap.GetPower(), ForceMode.Impulse);

        SpawnSlapEffect();
    }
    private void SlapWindow(Collider other)
    {
        isSlapping = true;
        if (other.TryGetComponent(out BreakableWindow breakableWindow))
        {
            if (breakableWindow.isBroken || breakableWindow == null) return;
            breakableWindow.health -= playerSlap.GetPower();
            if (breakableWindow.health <= 0) breakableWindow.breakWindow();
        }
    }
    private void ApplyDamage(PlayerController targetStatus)
    {
        int newHealth = targetStatus.GetHealth() - Mathf.RoundToInt(playerSlap.GetPower());
        targetStatus.TakeHit(newHealth);
    }

    private RagdollAnimator2 GetRagdoll(GameObject go)
    {
        GameObject parent = go.transform.parent.gameObject;
        if(!parent) return null;

        if(parent.TryGetComponent(out RagdollAnimatorDummyReference ragdollAnimatorDummyRef))
            return ragdollAnimatorDummyRef.ParentComponent as RagdollAnimator2;
        
        slapProp.PlayFeedbacks(); //TODO => EVENT DE FEEDBACKS
        return null;
    }

    private void SpawnSlapEffect()
    {
        float t = Mathf.InverseLerp(playerSlap.GetMinPower(), playerSlap.GetMaxPower(), playerSlap.GetPower());
        float finalScale = Mathf.Lerp(1f, maxEffectPrefabScale, t);
        GameObject go = Instantiate(effectPrefab, this.transform.position, Quaternion.identity);
        go.transform.localScale = Vector3.one * finalScale; 
        go.transform.position += new Vector3(0f, 0.9f, 1.0f);
        Destroy(go, 1f);
    }
    private void OnDisable() => isSlapping = false;
}
