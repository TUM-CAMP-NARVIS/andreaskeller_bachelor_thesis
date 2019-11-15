using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.XR.WSA;

public class ObjectPlacer : MonoBehaviour
{
    public GazeProvider gazeProvider;
    public SpatialMappingCollider spatialMappingCollider;
    private List<Vector3> corners = new List<Vector3>();
    private bool placed = false;
    // Start is called before the first frame update
    void Start()
    {
        if (!Application.isEditor)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void moveToPointer()
    {
        /*
        if (gazeProvider && !placed)
        {
            placed = true;
            transform.position = gazeProvider.HitPosition;
            //transform.rotation = new Quaternion(1, 0, 0, 0);
            spatialMappingCollider.enableCollisions = false;
        }
        */
        if (placed)
            return;

        if (corners.Count<3)
        {
            corners.Add(gazeProvider.HitPosition);
        }
        else
        {
            corners.Add(gazeProvider.HitPosition);
            //transform.up = gazeProvider.HitNormal;
            Vector3 middle = new Vector3();
            foreach (Vector3 v in corners)
            {
                middle += v;
            }
            middle.x = middle.x / 4.0f;
            middle.y = middle.y / 4.0f;
            middle.z = middle.z / 4.0f;
            transform.position = middle;
            transform.forward = 
            transform.forward = Vector3.Normalize(corners[1] - corners[2]);
            transform.right = Vector3.Normalize(corners[0] - corners[1]);
            transform.up = Vector3.Normalize(Vector3.Cross(corners[1]-corners[0],corners[2]-corners[0]))*-1;

            transform.GetChild(0).gameObject.SetActive(true);
            placed = true;

        }
        

        
    }
}
