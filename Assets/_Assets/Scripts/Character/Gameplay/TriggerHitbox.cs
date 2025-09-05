using FIMSpace.FProceduralAnimation;
using UnityEngine;
using MoreMountains.Feedbacks;

public class TriggerHitbox : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerController player;
    private PlayerSlap playerSlap;
    private RagdollAnimator2 myragdoll;

    [Header("Slap Setup")]
    [SerializeField] float slapPowerFallingThreshold;
    [SerializeField] private MMFeedbacks slapEnemy, slapProp, slapEnvironment;
    [SerializeField] bool isSlapping = false;


    [Header("Slap Effect Setup")]
    [SerializeField] GameObject effectPrefab;
    [SerializeField] float maxEffectPrefabScale = 3.5f; //Variável para definição de tamanho máximo que o vfx do tapa é pode chegar, de acordo com a intensidade do slapPower


    void Awake()
    {
        playerSlap = player?.GetComponent<PlayerSlap>();
        myragdoll = player?.GetComponent<RagdollAnimator2>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.attachedRigidbody)
        {
            slapEnvironment.PlayFeedbacks(); // Implementar o Manager de feedbacks para ativar por eventos
            if (other.gameObject.layer == LayerMask.NameToLayer("Glass")) SlapWindow(other);
        }
        else Slap(other);
    }

    private void Slap(Collider other)
    {
        if (isSlapping || other.gameObject.layer == 10) return;
        isSlapping = true;
        RagdollAnimator2 ragdoll = GetRagdoll(other.attachedRigidbody.gameObject);
        if (ragdoll != null)
        {
            if (ragdoll.gameObject.name == myragdoll.gameObject.name) return;
            // Debug.Log($"Bateu: {ragdoll.gameObject.name}" + "\n" +
            // $"slapPower: {playerSlap.GetPower()} de slapPowerFallingThreshold {slapPowerFallingThreshold}");
            ApplyDamage(player);
            if (playerSlap.GetPower() >= slapPowerFallingThreshold) //esse if força a ativação do falling no adversário que recebe o ataque, se o slapPower >= slapPowerFallingThreshold
            {
                player.SetIsFalling();
                
                //ragdoll.User_SwitchFallState(); //TODO => Colocar estado de cair no player controller
                // Rigidbody gameRigidbody = ragdoll.Handler.GetAnchorBoneController.GameRigidbody;
                // Animator mecanim = ragdoll.Handler.Mecanim;

                // //TODO => Mover este setup de ragdoll fall para o player controller
                // mecanim.CrossFadeInFixedTime("Fall", 0.25f);
                // gameRigidbody.maxAngularVelocity = 20f;
                // Vector3 rotationPower = ragdoll.User_BoneWorldRight(ragdoll.Handler.GetAnchorBoneController) * 30f;
                // ragdoll.User_SetPhysicalTorqueOnRigidbody(gameRigidbody, rotationPower, 0.75f, false, ForceMode.VelocityChange);
                // ragdoll.User_ChangeAllRigidbodiesDrag(0.5f);
                // ragdoll.User_SwitchAllBonesMaxVelocity(30f);
                // ragdoll.RagdollBlend = 1;
            }
            slapEnemy.PlayFeedbacks(); //TODO => mover para evento de feedbacks
        }

        if (ragdoll) ragdoll.RA2Event_AddHeadImpact(this.gameObject.transform.forward * playerSlap.GetPower());
        else other.attachedRigidbody.AddForce(this.gameObject.transform.forward * playerSlap.GetPower(), ForceMode.Impulse);

        SpawnSlapEffect();
    }
    private void SlapWindow(Collider other)
    {
        if (isSlapping) return;
        isSlapping = true;

        if (other.TryGetComponent(out BreakableWindow breakableWindow))
        {
            if (breakableWindow.isBroken || breakableWindow == null) return; // Se o vidro já estiver quebrado, saia
            breakableWindow.health -= playerSlap.GetPower(); // Diminui a vida do vidro
            if (breakableWindow.health <= 0) breakableWindow.breakWindow();
        }
    }
    private void ApplyDamage(PlayerController targetStatus)
    {
        int newHealth = targetStatus.GetHealth() - Mathf.RoundToInt(playerSlap.GetPower()); //Subtraindo o dano (arredondado) causado do health atual
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
        // Normaliza o slapPower entre 0 e 1
        float t = Mathf.InverseLerp(playerSlap.GetMinPower(), playerSlap.GetMaxPower(), playerSlap.GetPower());
        // Interpola entre 1 (escala base) e o valor máximo
        float finalScale = Mathf.Lerp(1f, maxEffectPrefabScale, t);
        GameObject go = Instantiate(effectPrefab, this.transform.position, Quaternion.identity);
        go.transform.localScale = Vector3.one * finalScale; // a escala do VFX é definida de acordo com a intensidade do powerSlap
        // Mover o efeito levemente nos eixos globais Y (altura) e Z (frente da câmera)
        go.transform.position += new Vector3(0f, 0.9f, 1.0f); // Ajuste esses valores conforme necessário
        Destroy(go, 1f);
    }
    private void OnDisable() => isSlapping = false;
}
