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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
