using FIMSpace.FProceduralAnimation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using System.Diagnostics;
using System.Security.Cryptography;

public class TriggerHitbox : MonoBehaviour
{
    [SerializeField] RagdollAnimator2 myragdoll;

    [SerializeField] GameObject effectPrefab;
    [SerializeField] float maxEffectPrefabScale = 3.5f; //Variável para definição de tamanho máximo que o vfx do tapa é pode chegar, de acordo com a intensidade do slapPower

    [SerializeField] float slapPower;
    [SerializeField] private MMFeedbacks slapEnemy, slapProp, slapEnvironment;
    bool isSlapping = false;

    [SerializeField] PlayerSlap playerSlap;


    private void Awake()
    {
        tag = gameObject.tag;
    }
    private void OnTriggerStay(Collider other)
    {

        //Debug.Log("trigger:" + other.name);
        if (!other.attachedRigidbody)
        {
            if (isSlapping) return;
            isSlapping = true;



            slapEnvironment.PlayFeedbacks();

            if (other.gameObject.layer == LayerMask.NameToLayer("Glass"))
            {
                
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
        //#### Detect Components ####

        //Component[] components = other.GetComponents<Component>();
        //foreach (Component component in components)
        //{
        //    Debug.Log($"Componente: {component.GetType().Name}");
        //}

        if (other.TryGetComponent(out Rigidbody rigidbody))
        {
            UnityEngine.Debug.Log("bateu");

            if (isSlapping || other.gameObject.layer == 10) return;
            isSlapping = true;
            RagdollAnimator2 ragdoll = GetRagdoll(rigidbody.gameObject);
            if (ragdoll != null)
            {
                UnityEngine.Debug.Log("bateu");
                if (ragdoll.gameObject.name == myragdoll.gameObject.name) return;
                ragdoll.User_SwitchFallState();
                slapEnemy.PlayFeedbacks();
            }
            //ragdoll.RA2Event_AddHeadImpact(this.gameObject.transform.forward * slapPower);
            //ragdoll.
            //ragdoll.GetComponent<Rigidbody>().AddForce(this.gameObject.transform.forward * slapPower, ForceMode.Impulse);
            //ragdoll.User_AddAllBonesImpact(this.gameObject.transform.forward * slapPower, 0.0f, ForceMode.Impulse);
            //ragdoll.User_AddRigidbodyImpact(rigidbody,this.gameObject.transform.forward * slapPower, 0.0f, ForceMode.Impulse);
            //ragdoll.RA2Event_AddHeadImpact(this.gameObject.transform.forward * slapPower,0.0f, ForceMode.Impulse);
            rigidbody.AddForce(this.gameObject.transform.forward * slapPower, ForceMode.Impulse);



            // Normaliza o slapPower entre 0 e 1
            float t = Mathf.InverseLerp(playerSlap.GetMinPower(), playerSlap.GetMaxPower(), slapPower);
            // Interpola entre 1 (escala base) e o valor máximo
            float finalScale = Mathf.Lerp(1f, maxEffectPrefabScale, t);

            GameObject go = Instantiate(effectPrefab,this.transform.position, Quaternion.identity);
            go.transform.localScale = Vector3.one * finalScale; // a escala do VFX é definida de acordo com a intensidade do powerSlap
            
            // Mover o efeito levemente nos eixos globais Y (altura) e Z (frente da câmera)
            go.transform.position += new Vector3(0f, 0.9f, 1.0f); // Ajuste esses valores conforme necessário

            Destroy(go,1f);
            //Debug.Log("aplicou força");


        }
    }


    private RagdollAnimator2 GetRagdoll(GameObject go)
    {
        
        GameObject parent = go.transform.parent.gameObject;
        if(!parent) return null;

        if(parent.TryGetComponent(out RagdollAnimatorDummyReference ragdollAnimatorDummyRef))
        {

            return ragdollAnimatorDummyRef.ParentComponent as RagdollAnimator2;
        }

        slapProp.PlayFeedbacks();
        return null;
    }

    private void OnEnable()
    {

        //Time.timeScale = 0.1f; // Deixa o jogo rodando a 10% da velocidade normal (camera lenta)

        UnityEngine.Debug.Log("Scale: " + transform.localScale);


    }

    private void OnDisable()
    {
        transform.localScale = new Vector3(0.74759531f, 0.899999976f, 0.899999976f);

        //Time.timeScale = 1.0f; // Deixa o jogo rodando na velocidade normal

        isSlapping = false;
    }

    public void SetSlapPower(float value)
    {
        slapPower = value;
    }
}
