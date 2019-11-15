using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class FocusManager : MonoBehaviour
{
    public GameObject seeThroughObject;
    public float lazyMouseDistance = 0.005f;

    private GazeProvider gazeProvider;
    private Vector3 _focusPosition;
    private Vector3 _focusNormal;

    public bool isFocused { get; private set; } = false;

    public Vector3 focusPosition
    {
        get
        {
            return _focusPosition;
        }
    }

    public Vector3 focusNormal
    {
        get
        {
            return _focusNormal;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gazeProvider = FindObjectOfType<GazeProvider>();
        _focusPosition = new Vector3();
        _focusNormal = new Vector3(0, 1, 0);
        if (!seeThroughObject)   
        {
            Debug.LogError("No phatom specified");
            this.gameObject.SetActive(false);
        }
        if (seeThroughObject && seeThroughObject.GetComponents<Collider>().Length == 0)
        {
            Debug.LogError("Phantom has no collision");
            this.gameObject.SetActive(false);
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        //LazyMouse Behaviour
        float distanceToGaze = Vector3.Distance(_focusPosition, gazeProvider.HitPosition);

        if (!isFocused)
        {
            if (gazeProvider.GazeTarget == seeThroughObject)
            {
                _focusPosition = gazeProvider.HitPosition;
                _focusNormal = gazeProvider.HitNormal;
                isFocused = true;
            }
        }
        else if (distanceToGaze > lazyMouseDistance)
        {
            Vector3 moveDirection = Vector3.Normalize(gazeProvider.HitPosition - _focusPosition);
            float moveDistance = distanceToGaze - lazyMouseDistance;

            Vector3 rayOrigin = gazeProvider.GazeOrigin;
            Vector3 rayTarget = _focusPosition + (moveDistance * moveDirection);
            Vector3 rayDirection = Vector3.Normalize(rayTarget - rayOrigin);

            RaycastHit hit;
            var ray = new Ray(rayOrigin, rayDirection);

            if (Physics.Raycast(ray, out hit))
            {
                _focusPosition = hit.point;
                _focusNormal = hit.normal;

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
}
