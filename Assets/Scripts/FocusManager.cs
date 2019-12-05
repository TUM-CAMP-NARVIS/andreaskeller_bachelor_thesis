using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class FocusManager : MonoBehaviour
{
    private GameObject seeThroughObject;
    public float lazyMouseDistance = 0.005f;

    

    public bool isFocused { get; private set; } = false;

    public bool isStatic { get { return !active; } }

    public Vector3 focusPosition { get; private set; } = new Vector3();

    public Vector3 focusNormal { get; private set; } = new Vector3(0, 1, 0);

    private bool active = true;
    private GazeProvider gazeProvider;

    // Start is called before the first frame update
    void Start()
    {
        gazeProvider = FindObjectOfType<GazeProvider>();

        if (gazeProvider == null)
        {
            Debug.LogError("No GazeProvider in scene!");
        }            
    }

    // Update is called once per frame
    void Update()
    {
        if (!seeThroughObject)
            return;
        if (!active)
            return;

        //LazyMouse Behaviour
        float distanceToGaze = Vector3.Distance(focusPosition, gazeProvider.HitPosition);

        if (!isFocused)
        {
            if (gazeProvider.GazeTarget == seeThroughObject)
            {
                focusPosition = gazeProvider.HitPosition;
                focusNormal = gazeProvider.HitNormal;
                isFocused = true;
            }
        }
        else if (distanceToGaze > lazyMouseDistance)
        {
            Vector3 moveDirection = Vector3.Normalize(gazeProvider.HitPosition - focusPosition);
            float moveDistance = distanceToGaze - lazyMouseDistance;

            Vector3 rayOrigin = gazeProvider.GazeOrigin;
            Vector3 rayTarget = focusPosition + (moveDistance * moveDirection);
            Vector3 rayDirection = Vector3.Normalize(rayTarget - rayOrigin);

            RaycastHit hit;
            var ray = new Ray(rayOrigin, rayDirection);

            if (Physics.Raycast(ray, out hit))
            {
                focusPosition = hit.point;
                focusNormal = hit.normal;

                //Not focusing on the object anymore if we dont hit it
                if (hit.collider.gameObject != seeThroughObject)
                    isFocused = false;
                else
                    isFocused = true;
            }
            else
            {
                isFocused = false;
            }
        }
    }

    public void ToggleActive()
    {
        active = !active;
    }

    public void RegisterSkin(GameObject obj)
    {
        seeThroughObject = obj;
    }

    public GameObject GetSkin()
    {
        return seeThroughObject;
    }
}
