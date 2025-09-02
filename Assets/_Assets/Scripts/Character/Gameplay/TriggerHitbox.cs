using FIMSpace.FProceduralAnimation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using System.Security.Cryptography;
using System.Diagnostics;

public class TriggerHitbox : MonoBehaviour
{
    [SerializeField] RagdollAnimator2 myragdoll;

    [SerializeField] GameObject effectPrefab;
    [SerializeField] float maxEffectPrefabScale = 3.5f; //Variável para definição de tamanho máximo que o vfx do tapa é pode chegar, de acordo com a intensidade do slapPower

    [SerializeField] float slapPower;
    [SerializeField] float slapPowerFallingThreshold;
    [SerializeField] private MMFeedbacks slapEnemy, slapProp, slapEnvironment;
    [SerializeField] bool isSlapping = false;

    [SerializeField] PlayerSlap playerSlap;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Colidiu: " + other.gameObject.name);
        if (!other.attachedRigidbody)
        {
            slapEnvironment.PlayFeedbacks();

            if (other.gameObject.layer == LayerMask.NameToLayer("Glass"))
            {
                if (isSlapping) return;
                isSlapping = true;
                
                if (other.TryGetComponent(out BreakableWindow breakableWindow))
                {
                    if (breakableWindow.isBroken || breakableWindow==null) return; // Se o vidro já estiver quebrado, saia


                    breakableWindow.health -= slapPower; // Diminui a vida do vidro

                    if (breakableWindow.health <= 0)
                    {
                        breakableWindow.breakWindow();
                    }
                    
                }

            }
        }


        if (other.TryGetComponent(out Rigidbody rigidbody))
        {
            UnityEngine.Debug.Log("Rigidbody: " + rigidbody.gameObject.name);
            if (isSlapping || other.gameObject.layer == 10) return;
            isSlapping = true;
            RagdollAnimator2 ragdoll = GetRagdoll(rigidbody.gameObject);
            //Debug.Log("ragdoll: " + ragdoll.name);
            if (ragdoll != null)
            {
                //UnityEngine.Debug.Log("ragdoll: " + ragdoll.name);
                if (ragdoll.gameObject.name == myragdoll.gameObject.name) return;

                UnityEngine.Debug.Log("bateu: " + ragdoll.gameObject.name + "\n" +
                "slapPower: " + slapPower + " de " + "slapPowerFallingThreshold " + slapPowerFallingThreshold);


                ApplyDamage(ragdoll.gameObject.GetComponent<PlayerController>()); //Aqui trata do dano causado ao HEALTH do alvo

                var ragdollBlend = ragdoll.RagdollBlend;
                if (slapPower >= slapPowerFallingThreshold) //esse if força a ativação do falling no adversário que recebe o ataque, se o slapPower >= slapPowerFallingThreshold
                {
                    ragdoll.gameObject.GetComponent<PlayerController>().SetIsFalling();

                    //ragdoll.User_SwitchFallState();


                    //ragdoll.Handler.Mecanim.CrossFadeInFixedTime( "Fall", 0.25f );
                    //ragdoll.Handler.GetAnchorBoneController.GameRigidbody.maxAngularVelocity = 20f;
                    //ragdoll.User_SetPhysicalTorqueOnRigidbody( ragdoll.Handler.GetAnchorBoneController.GameRigidbody, ragdoll.User_BoneWorldRight( ragdoll.Handler.GetAnchorBoneController ) * 30f, 0.75f, false, ForceMode.VelocityChange );
                    //ragdoll.User_ChangeAllRigidbodiesDrag( 0.5f );
                    //ragdoll.User_SwitchAllBonesMaxVelocity( 30f );
                    //ragdoll.RagdollBlend = 1;
                }

                slapEnemy.PlayFeedbacks();
            }
            UnityEngine.Debug.Log("other:" + other.name);
            //ragdoll.RA2Event_AddHeadImpact(this.gameObject.transform.forward * slapPower);
            //ragdoll.
            //ragdoll.GetComponent<Rigidbody>().AddForce(this.gameObject.transform.forward * slapPower, ForceMode.Impulse);
            //ragdoll.User_AddAllBonesImpact(this.gameObject.transform.forward * slapPower, 0.0f, ForceMode.Impulse);
            //ragdoll.User_AddRigidbodyImpact(rigidbody,this.gameObject.transform.forward * slapPower, 0.0f, ForceMode.Impulse);
            //ragdoll.RA2Event_AddHeadImpact(this.gameObject.transform.forward * slapPower,0.0f, ForceMode.Impulse);

            if(ragdoll)ragdoll.RA2Event_AddHeadImpact(this.gameObject.transform.forward * slapPower);
            else rigidbody.AddForce(this.gameObject.transform.forward * slapPower, ForceMode.Impulse);


            // Normaliza o slapPower entre 0 e 1
            float t = Mathf.InverseLerp(playerSlap.GetMinPower(), playerSlap.GetMaxPower(), slapPower);
            // Interpola entre 1 (escala base) e o valor máximo
            float finalScale = Mathf.Lerp(1f, maxEffectPrefabScale, t);

            GameObject go = Instantiate(effectPrefab,this.transform.position, Quaternion.identity);
            go.transform.localScale = Vector3.one * finalScale; // a escala do VFX é definida de acordo com a intensidade do powerSlap
            
            // Mover o efeito levemente nos eixos globais Y (altura) e Z (frente da câmera)
            go.transform.position += new Vector3(0f, 0.9f, 1.0f); // Ajuste esses valores conforme necessário

            Destroy(go,1f);
            //UnityEngine.Debug.Log("aplicou for?a");
            

        }
    }


    private void ApplyDamage(PlayerController targetStatus)
    {
        int newHealth;

        newHealth = targetStatus.GetHealth() - Mathf.RoundToInt(slapPower); //Subtraindo o dano (arredondado) causado do health atual
        targetStatus.TakeHit(newHealth);
        

    }

    private RagdollAnimator2 GetRagdoll(GameObject go)
    {

        GameObject parent = go.transform.parent.gameObject;
        if(!parent) return null;

        if(parent.TryGetComponent(out RagdollAnimatorDummyReference ragdollAnimatorDummyRef))
        {
            RagdollAnimator2 originalRagdoll = ragdollAnimatorDummyRef.ParentComponent as RagdollAnimator2;
            //GameObject originalGameObject = originalRagdoll.gameObject;

            // if (originalGameObject.GetComponent<PlayerSlap>()?.GetChargingTime() >= playerSlap.GetQuickSlapThreshold())
            // {
            //     //UnityEngine.Debug.Log("ChargingTime do Adversário é: " + originalGameObject.GetComponent<PlayerSlap>()?.GetChargingTime());

            //     originalGameObject.GetComponent<PlayerSlap>()?.animator.SetBool("isInterrupted", true);
            //     //originalGameObject.GetComponent<PlayerSlap>()?.stopSlapFeedback();
            //     originalGameObject.GetComponent<PlayerSlap>()?.SlappingEnd();
                    
            // }

            return ragdollAnimatorDummyRef.ParentComponent as RagdollAnimator2;
        }

        slapProp.PlayFeedbacks();
        return null;
    }

    private void OnEnable()
    {

        //Time.timeScale = 0.1f; // Deixa o jogo rodando a 10% da velocidade normal (camera lenta)

        //UnityEngine.Debug.Log("Scale: " + transform.localScale);


    }

    private void OnDisable()
    {
        //transform.localScale = new Vector3(0.74759531f, 0.899999976f, 0.899999976f);

        //Time.timeScale = 1.0f; // Deixa o jogo rodando na velocidade normal

        isSlapping = false;
    }

    public void SetSlapPower(float value)
    {
        slapPower = value;
    }
}
