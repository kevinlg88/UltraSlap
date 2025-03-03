using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPositions : MonoBehaviour
{
    [SerializeField] private KeyCode keyResetPositions = KeyCode.R;
    [SerializeField] private List<GameObject> objectsToReset = new List<GameObject>(); // Ensure it's initialized

    private Dictionary<GameObject, Vector3> initialPositions = new Dictionary<GameObject, Vector3>();

    void Start()
    {
        if (objectsToReset == null || objectsToReset.Count == 0)
        {
            Debug.LogError("No objects assigned to 'objectsToReset' in the Inspector!");
            return;
        }

        // Store the initial positions
        foreach (GameObject obj in objectsToReset)
        {
            if (obj != null)
                initialPositions[obj] = obj.transform.position;
            else
                Debug.LogWarning("An object in 'objectsToReset' is null!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(keyResetPositions))
        {
            ResetObjects();
        }
    }

    private void ResetObjects()
    {
        foreach (GameObject obj in objectsToReset)
        {
            if (obj != null && initialPositions.ContainsKey(obj))
            {
                obj.transform.position = initialPositions[obj];
            }
            else
            {
                Debug.LogWarning($"Trying to reset a null object or an object not found in initialPositions!");
            }
        }
    }
}