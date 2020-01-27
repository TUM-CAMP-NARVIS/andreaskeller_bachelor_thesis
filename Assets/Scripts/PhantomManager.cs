using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomManager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject insides;
    public GameObject insides_Chroma;
    private int materialUsed = 0;
    public bool hatchingInverted = false;
    private bool hatchingInv = false;

    public bool useTriPlanar = false;
    private bool useTriPl = false;
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (hatchingInv != hatchingInverted)
            ToggleHatching();

        if (useTriPl != useTriPlanar)
            ToggleTriPlanar();

        Vector3 focusPosition = FindObjectOfType<FocusManager>().focusPosition;
        Vector3 focusNormal = FindObjectOfType<FocusManager>().focusNormal;
        foreach (Transform child in insides_Chroma.transform)
        {
            child.GetComponent<Renderer>().material.SetVector("_FocusPosition", focusPosition);
            child.GetComponent<Renderer>().material.SetVector("_FocusNormal", focusNormal);
        }
    }

    //public void ToggleManipulation()
    //{
    //    bool enable = true;
    //    if (GetComponent<BoundingBox>().enabled == true)
    //        enable = false;
    //
    //    SetManipulation(enable);
    //}
    //
    //public void SetManipulation(bool active)
    //{
    //    GetComponent<BoundingBox>().enabled = active;
    //    GetComponent<BoxCollider>().enabled = active;
    //    GetComponent<ManipulationHandler>().enabled = active;
    //    GetComponent<PointerHandler>().enabled = !active;
    //
    //    if (active)
    //    {
    //        transform.Find("skin").GetComponent<Renderer>().sharedMaterial = (Material)Resources.Load("Materials/brightskin", typeof(Material));
    //
    //    }
    //    else
    //    {
    //        transform.Find("skin").GetComponent<Renderer>().sharedMaterial = (Material)Resources.Load("Materials/Skin", typeof(Material));
    //    }
    //}

    public void ToggleHatching()
    {
        hatchingInv = !hatchingInv;
        
        if (hatchingInv)
        {
            foreach (Transform child in insides.transform)
            {
                child.GetComponent<Renderer>().material.EnableKeyword("_INVERTHATCHING");
            }
        }
        else
        {
            foreach (Transform child in insides.transform)
            {
                child.GetComponent<Renderer>().material.DisableKeyword("_INVERTHATCHING");
            }
        }
        
    }

    public void ToggleTriPlanar()
    {
        useTriPl = !useTriPl;

        if (useTriPl)
        {
            foreach (Transform child in insides.transform)
            {
                child.GetComponent<Renderer>().material.EnableKeyword("_TRIPLANAR");
            }
        }
        else
        {
            foreach (Transform child in insides.transform)
            {
                child.GetComponent<Renderer>().material.DisableKeyword("_TRIPLANAR");
            }
        }
    }

    public void ToggleSkin()
    {
        transform.Find("skin").GetComponent<MeshRenderer>().enabled = !transform.Find("skin").GetComponent<MeshRenderer>().enabled;
    }
}
