using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class FocusRegionUpdateHelper : MonoBehaviour
{
    public GameObject focusHelper;
    public float focusSize = 0.15f;

    void Update()
    {
        if (focusHelper)
        {
            Vector3 focusPosition = focusHelper.transform.position;
            foreach (Transform child in transform)
            {
                child.GetComponent<Renderer>().material.SetVector("_FocusPosition", focusPosition);
                child.GetComponent<Renderer>().material.SetFloat("_FocusRadius", focusSize);
            }
            
        }
    }
}

