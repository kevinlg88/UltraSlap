using FIMSpace.FProceduralAnimation;
using UnityEngine;
using MoreMountains.Feedbacks;
using System.Collections.Generic;

public class TriggerHitbox : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerController player;
    private PlayerSlap playerSlap;
    private RagdollAnimator2 myragdoll;
    [SerializeField] private MMFeedbacks slapEnemy, slapProp, slapEnvironment;

    [Header("Target Control")] // mantém controle sobre o que já foi atingido
    private HashSet<GameObject> hitObjects = new HashSet<GameObject>(); // objetos comuns
    private HashSet<RagdollAnimator2> hitRagdolls = new HashSet<RagdollAnimator2>(); // ragdolls já atingidos
    private bool propFeedbackPlayed = false; //novo controle para evitar múltiplos slapProp

    [Header("Slap Setup")]
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

    private void OnEnable()
    {
        hitObjects.Clear();  // Cada vez que a hitbox é instanciada, o histórico de acertos é limpo.
        hitRagdolls.Clear(); // Cada vez que a hitbox é instanciada, o histórico de acertos é limpo.

        propFeedbackPlayed = false; //reseta a flag quando a hitbox é recriada
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"Bateu no seguinte objeto: {other}");

        if (!other) return;
        if (hitObjects.Contains(other.gameObject)) return; // evita hits duplicados

        if (!other.attachedRigidbody)
        {
            slapEnvironment.PlayFeedbacks(); // Implementar o Manager de feedbacks para ativar por eventos

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

        RagdollAnimator2 ragdoll = GetRagdoll(other.attachedRigidbody.gameObject);

        // Caso tenha atingido um ragdoll, verifica se já acertou o mesmo dono
        if (ragdoll != null)
        {
            if (ragdoll == myragdoll) return; // ignora o próprio jogador

            if (hitRagdolls.Contains(ragdoll))
                return; // já atingiu esse ragdoll antes neste tapa

            hitRagdolls.Add(ragdoll); // registra que esse ragdoll foi atingido

            // Aplica dano e efeitos
            ApplyDamage(ragdoll.GetComponent<PlayerController>());
            if (playerSlap.GetPower() >= slapPowerFallingThreshold)
                ragdoll.GetComponent<PlayerController>().SetIsFalling();

            slapEnemy?.PlayFeedbacks();
            ragdoll.RA2Event_AddHeadImpact(transform.forward * playerSlap.GetPower());
        }

        else // Não é ragdoll (pode ser uma caixa, vaso, etc.)
        {
            var responsive = other.GetComponent<ResponsiveProp>();

            if (responsive != null) // Caso o objeto tenha o componente ResponsiveProp
            {
                // Se não estiver solto (detached = false) e ainda não tocou slapProp neste hitbox
                if (!responsive.detached && !propFeedbackPlayed)
                {
                    slapProp?.PlayFeedbacks();
                    propFeedbackPlayed = true; // marca como tocado
                }
            }
            else
            {
                // Se o objeto NÃO tiver ResponsiveProp, toca sempre (pois são objetos independentes)
                slapProp?.PlayFeedbacks();
            }

            // Aplica força física no objeto
            other.attachedRigidbody.AddForce(transform.forward * playerSlap.GetPower(), ForceMode.Impulse);

        }

        SpawnSlapEffect();
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
        
        //slapProp.PlayFeedbacks(); //TODO => EVENT DE FEEDBACKS
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
