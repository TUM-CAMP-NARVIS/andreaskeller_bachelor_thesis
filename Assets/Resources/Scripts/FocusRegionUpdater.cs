using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class FocusRegionUpdater : MonoBehaviour
{
    public GazeProvider gazeProvider;
    public float focusSize = 0.15f;

    void Update()
    {
        if (gazeProvider)
        {
            Vector3 focusPosition = gazeProvider.HitPosition;
            GetComponent<Renderer>().material.SetVector("_FocusPosition", focusPosition);
            GetComponent<Renderer>().material.SetFloat("_FocusRadius", focusSize);
        }
    }
}

