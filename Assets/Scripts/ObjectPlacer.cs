using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Microsoft.MixedReality.Toolkit.Input;
//using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using UnityEngine.XR.WSA;

public class ObjectPlacer : MonoBehaviour
{
    //private GazeProvider gazeProvider;
    private SpatialMappingCollider spatialMappingCollider;
    private SpatialMappingRenderer spatialMappingRenderer;
    private List<Vector3> corners = new List<Vector3>();
    private bool placed = false;
    private GameObject[] cubes;
    // Start is called before the first frame update
    void Start()
    {
        cubes = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
            primitive.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            primitive.GetComponent<Collider>().enabled = false;
            primitive.SetActive(false);
            cubes[i]=primitive;
            //primitive.transform.up = _ctrlPointNrms[i];

        }
        //if (!Application.isEditor)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        spatialMappingCollider = FindObjectOfType<SpatialMappingCollider>();
        spatialMappingRenderer = FindObjectOfType<SpatialMappingRenderer>();
        //gazeProvider = FindObjectOfType<GazeProvider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void undoPlacing()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        spatialMappingCollider.layer = 0;
        spatialMappingRenderer.renderState = SpatialMappingRenderer.RenderState.Visualization;
        placed = false;
        foreach(GameObject c in cubes)
        {
            c.SetActive(false);
        }
        corners.Clear();
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

        cubes[corners.Count].transform.position = new Vector3();// gazeProvider.HitPosition;
        cubes[corners.Count].SetActive(true);

        if (corners.Count<3)
        {
            corners.Add(new Vector3());// gazeProvider.HitPosition);
        }
        else
        {
            corners.Add(new Vector3());// gazeProvider.HitPosition);
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
            
            transform.forward = Vector3.Normalize(corners[1] - corners[2]);
            //transform.right = Vector3.Normalize(corners[0] - corners[1]);
            transform.up = Vector3.Normalize(Vector3.Cross(corners[1]-corners[0],corners[2]-corners[0]))*-1;

            transform.GetChild(0).gameObject.SetActive(true);
            placed = true;
            spatialMappingRenderer.renderState = SpatialMappingRenderer.RenderState.None;
            spatialMappingCollider.layer = 2;
        }
        

        
    }
}
