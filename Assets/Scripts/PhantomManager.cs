using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using XRTK.SDK.UX;
//using XRTK.SDK.Input;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;

public class PhantomManager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject insides;
    public GameObject headlight;
    private int materialUsed = 0;
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (headlight && headlight.activeSelf)
        {
            Vector4 p = new Vector4(-1*headlight.transform.forward.x, -1 * headlight.transform.forward.y, -1 * headlight.transform.forward.z, 1);
            foreach (Transform child in insides.transform)
            {
                child.GetComponent<Renderer>().material.SetVector("_AdditionalLightDir", p);
                child.GetComponent<Renderer>().material.SetVector("_AdditionalLightPos", headlight.transform.position);
            }
        }
        else
        {
            Vector4 p = new Vector4(1, 1, 1, 0);
            foreach (Transform child in insides.transform)
            {
                child.GetComponent<Renderer>().material.SetVector("_AdditionalLightDir", p);
                child.GetComponent<Renderer>().material.SetVector("_AdditionalLightPos", headlight.transform.position);
            }
        }
    }

    public void ToggleManipulation()
    {
        bool enable = true;
        if (GetComponent<BoundingBox>().enabled == true)
            enable = false;

        SetManipulation(enable);
    }

    public void SetManipulation(bool active)
    {
        GetComponent<BoundingBox>().enabled = active;
        GetComponent<BoxCollider>().enabled = active;
        GetComponent<ManipulationHandler>().enabled = active;
        GetComponent<PointerHandler>().enabled = !active;

        if (active)
        {
            transform.Find("skin").GetComponent<Renderer>().sharedMaterial = (Material)Resources.Load("Materials/brightskin", typeof(Material));

        }
        else
        {
            transform.Find("skin").GetComponent<Renderer>().sharedMaterial = (Material)Resources.Load("Materials/Skin", typeof(Material));
        }
    }

    public void ToggleHatching()
    {
        float invert = 0;
        if (materialUsed == 0)
        {
            invert = 1;
            materialUsed = 1;
        }
        else
            materialUsed = 0;
        

        foreach (Transform child in insides.transform)
        {
            child.GetComponent<Renderer>().material.SetFloat("_InvertHatching", invert);
        }
    }

    public void ToggleHeadlight()
    {
        if (headlight)
            headlight.SetActive(!headlight.activeSelf);
    }

    public void ToggleTriPlanar()
    {

        foreach (Transform child in insides.transform)
        {
            child.GetComponent<Renderer>().material.SetFloat("_UseTriPlanar",1-child.GetComponent<Renderer>().material.GetFloat("_UseTriPlanar"));
        }
    }
}
