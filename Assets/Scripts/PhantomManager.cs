using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        GetComponent<BoundingBox>().enabled = enable;
        GetComponent<BoxCollider>().enabled = enable;
        GetComponent<ManipulationHandler>().enabled = enable;
    }
}
