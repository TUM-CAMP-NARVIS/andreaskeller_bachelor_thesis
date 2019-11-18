﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class FocusRegionUpdater : MonoBehaviour
{
    private FocusManager focusManager;
    public float focusSize = 0.15f;
    public bool active = true;
    public bool sphereActive = true;
    public bool skinActive = true;
    public GameObject sphere;
    private GameObject skin;

    void Start()
    {
        focusManager = FindObjectOfType<FocusManager>();
    }

    void Awake()
    {
        skin = transform.GetChild(0).gameObject;
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
        if (skinActive)
        {
            GameObject model = null;
            if (focusManager && active)
            {
                Vector3 focusPosition = focusManager.focusPosition;
                foreach (Transform child in transform)
                {
                    child.GetComponent<Renderer>().material.SetVector("_FocusPosition", focusPosition);
                    if (child.name.Equals("skin"))
                    {
                        model = child.gameObject;
                    }
                    //child.GetComponent<Renderer>().material.SetFloat("_FocusRadius", focusSize);
                }
                if (sphere)
                {
                    if (focusPosition == new Vector3(0, 0, 0))
                    {
                        sphere.SetActive(false);
                    }
                    else
                    {
                        if (sphereActive == true)
                        {
                            sphere.SetActive(true);
                        }
                    }
                    if (model != null)
                    {
                        var mat = model.GetComponent<Renderer>().material;
                        var size = mat.GetFloat("_FocusRadius");
                        size *= 2;
                        sphere.transform.localScale = new Vector3(size, size, size);
                    }

                    sphere.transform.position = focusPosition;

                }

            }
            else
            {
                bool gaze = focusManager ? true : false;
                Debug.Log("Gaze:" + gaze + "\nActive:" + active);
            }
        }
    }

    public void ToggleMovement()
    {
        active = !active;
    }

    public void EnableSphere(bool enable)
    {
        sphereActive = enable;
        sphere.SetActive(enable);
    }

    public void EnableSkin(bool enable)
    {
        skinActive = enable;
        skin.GetComponent<MeshRenderer>().enabled = enable;
    }

    
}

