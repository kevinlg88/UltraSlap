using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    Camera camera;
    Crosshair crosshair;
    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponentInChildren<Camera>();
        crosshair = FindObjectOfType(typeof(Crosshair)) as Crosshair;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitData;
        Interactable hitInteractable = null;
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        bool rayHit = Physics.Raycast(ray, out hitData, 50.0f);
        if(rayHit)
        {
            hitInteractable = hitData.transform.gameObject.GetComponent<Interactable>();
        }

        if(rayHit && hitInteractable)
        {
            crosshair.SetInteractable();
        }
        else
        {
            crosshair.SetNonInteractable();
        }

        if(Input.GetMouseButtonDown(0))
        {
            if(rayHit && hitInteractable)
            {
                Animator animator = hitInteractable.GetAnimator();
                string triggerName = hitInteractable.GetAnimationTriggerName();

                animator.SetTrigger(triggerName);
            }
        }
    }
}
