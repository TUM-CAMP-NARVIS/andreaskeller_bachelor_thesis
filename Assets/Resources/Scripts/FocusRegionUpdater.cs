using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class FocusRegionUpdater : MonoBehaviour
{
    public GazeProvider gazeProvider;
    public float focusSize = 0.15f;
    public bool active = true;

    void Update()
    {
        if (gazeProvider && active)
        {
            Vector3 focusPosition = gazeProvider.HitPosition;
            foreach (Transform child in transform)
            {
                child.GetComponent<Renderer>().material.SetVector("_FocusPosition", focusPosition);
                //child.GetComponent<Renderer>().material.SetFloat("_FocusRadius", focusSize);
            }
            
        }
    }

    public void ToggleMovement()
    {
        active = !active;
    }
}

