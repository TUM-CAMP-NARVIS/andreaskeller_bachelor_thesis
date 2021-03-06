﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusRegionUpdater : MonoBehaviour
{
    private FocusManager focusManager;

    void Start()
    {
        focusManager = FindObjectOfType<FocusManager>();
        if (focusManager.GetSkin()==null)
            focusManager.RegisterSkin(transform.Find("skin").gameObject);
    }


    void Update()
    {
        if (focusManager)
        {
            Vector3 focusPosition = focusManager.focusPosition;
            Vector3 focusNormal = focusManager.focusNormal;
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Renderer>())
                {
                    child.GetComponent<Renderer>().material.SetVector("_FocusPosition", focusPosition);
                }
                    

            }

        }
    }

    
}

