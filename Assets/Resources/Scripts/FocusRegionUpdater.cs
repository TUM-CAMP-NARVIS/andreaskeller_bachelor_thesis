using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusRegionUpdater : MonoBehaviour
{
    public GameObject Cursor;
    public float focusSize = 0.15f;

    void Update()
    {
        if (Cursor)
        {
            Vector3 focusPosition = Cursor.transform.position;
            GetComponent<Renderer>().material.SetVector("_FocusPosition", focusPosition);
            GetComponent<Renderer>().material.SetFloat("_FocusRadius", focusSize);
        }
    }
}

