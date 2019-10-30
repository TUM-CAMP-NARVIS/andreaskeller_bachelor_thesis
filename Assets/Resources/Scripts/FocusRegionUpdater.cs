﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class FocusRegionUpdater : MonoBehaviour
{
    public GazeProvider gazeProvider;
    public float focusSize = 0.15f;
    public bool active = true;
    public GameObject sphere;

    void Awake()
    {
        if (sphere)
        {
            Vector3[] normals = sphere.GetComponent<MeshFilter>().mesh.normals;
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = -normals[i];
            }
            sphere.GetComponent<MeshFilter>().mesh.normals = normals;

            int[] triangles = sphere.GetComponent<MeshFilter>().mesh.triangles;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                int t = triangles[i];
                triangles[i] = triangles[i + 2];
                triangles[i + 2] = t;

            }

            sphere.GetComponent<MeshFilter>().mesh.triangles = triangles;
        }
        
    }

    void Update()
    {
        GameObject model = null;
        if (gazeProvider && active)
        {
            Vector3 focusPosition = gazeProvider.HitPosition;
            foreach (Transform child in transform)
            {
                child.GetComponent<Renderer>().material.SetVector("_FocusPosition", focusPosition);
                if (child.name.Equals( "skin"))
                {
                    model = child.gameObject;
                }
                //child.GetComponent<Renderer>().material.SetFloat("_FocusRadius", focusSize);
            }
            if (sphere)
            {
                if (model!=null)
                {
                    var mat = model.GetComponent<Renderer>().material;
                    //var size = mat.GetFloat("_FocusSize")*2;
                    //sphere.transform.localScale = new Vector3(size, size, size);
                }

                sphere.transform.position = focusPosition;
                
            }
            
        }
    }

    public void ToggleMovement()
    {
        active = !active;
    }
}

