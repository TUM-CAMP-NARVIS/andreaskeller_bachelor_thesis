using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.XR.WSA;

public class ObjectPlacer : MonoBehaviour
{
    public GazeProvider gazeProvider;
    public SpatialMappingCollider spatialMappingCollider;
    private bool placed = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void moveToPointer()
    {
        if (gazeProvider && !placed)
        {
            placed = true;
            transform.position = gazeProvider.HitPosition;
            //transform.rotation = new Quaternion(1, 0, 0, 0);
            spatialMappingCollider.enableCollisions = false;
        }
        
    }
}
