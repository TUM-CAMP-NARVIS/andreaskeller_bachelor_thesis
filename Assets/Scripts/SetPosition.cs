using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.XR.WSA;

public class SetPosition : MonoBehaviour
{
    private bool placed = false;

    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).gameObject.SetActive(placed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Place()
    {
        if (placed)
            return;
        //GazeProvider gazeProvider = FindObjectOfType<GazeProvider>();
        if (false)//gazeProvider)
        {
            //transform.position = gazeProvider.HitPosition;
            //transform.up = gazeProvider.HitNormal;
            placed = true;
            transform.GetChild(0).gameObject.SetActive(placed);
            this.gameObject.AddComponent<WorldAnchor>();
            SpatialMappingCollider sp = FindObjectOfType<SpatialMappingCollider>();
            sp.layer = 31;
            SpatialMappingRenderer sr = FindObjectOfType<SpatialMappingRenderer>();
            sr.renderState = SpatialMappingRenderer.RenderState.None;
        }
    }
}
