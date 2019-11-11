using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class SurfaceAlign : MonoBehaviour
{

    public GazeProvider gazeProvider;
    public GameObject skin;
    private Vector3[] vertexPos;
    private Vector3[] vertexWorldPos;
    private Vector3[] vertexNrm;
    private Vector3[] vertexWorldNrm;
    private float[] vertexDistance;
    //private GameObject[] spheres;

    private ComputeShader shader;

    // Start is called before the first frame update
    void Start()
    {
        
        Mesh mesh = this.transform.GetChild(0).GetComponent<MeshFilter>().mesh;
        vertexPos = new Vector3[mesh.vertexCount];
        vertexNrm = new Vector3[mesh.vertexCount];
        System.Array.Copy(mesh.vertices, vertexPos, mesh.vertexCount);
        System.Array.Copy(mesh.normals, vertexNrm, mesh.vertexCount);
        vertexWorldPos = new Vector3[mesh.vertexCount];
        vertexWorldNrm = new Vector3[mesh.vertexCount];
        vertexDistance = new float[mesh.vertexCount];

        /*spheres = new GameObject[mesh.vertexCount];
        for (int i = 0; i<mesh.vertexCount; i++)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            sphere.GetComponent<Collider>().enabled = false;
            spheres[i] = sphere;
        }
        */

    }

    // Update is called once per frame
    void Update()
    {
        

        if (gazeProvider.GazeTarget != skin)
        {
            //Debug.LogWarning(gazeProvider.HitPosition);
            this.transform.GetChild(0).gameObject.SetActive(false);
            
        }
        else
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
            this.transform.position = gazeProvider.HitPosition+gazeProvider.HitNormal*0.1f;
            this.transform.up = gazeProvider.HitNormal;
        }

        raycastPlane();
        applyPosition();

    }

    void raycastPlane()
    {
        //Get Current worldPos and worldNrm for each vertex
        for (int i = 0; i < vertexPos.Length; i++)
        {
            Vector3 v = vertexPos[i];
            Vector3 n = vertexNrm[i];

            
            Vector3 vW = transform.TransformPoint(v);
            Vector3 nW = transform.TransformDirection(n);
            vertexWorldNrm[i] = nW;
            vertexWorldPos[i] = vW;
        }

        for (int i = 0; i < vertexWorldPos.Length; i++)
        {
            RaycastHit hit;

            var ray = new Ray(vertexWorldPos[i], vertexWorldNrm[i] * (-1));
            if (Physics.Raycast(ray, out hit))
            {
                //spheres[i].transform.position = hit.point;
                vertexWorldPos[i] = hit.point;
                vertexWorldNrm[i] = hit.normal;
                vertexDistance[i] = hit.distance * (1.0f/transform.localScale.x);

            }
        }
        
    }

    void applyPosition()
    {
        Mesh mesh = this.transform.GetChild(0).GetComponent<MeshFilter>().mesh;
        Vector3[] newVertexPos = new Vector3[mesh.vertexCount];
        Vector3[] newVertexNrm = new Vector3[mesh.vertexCount];
        for (int i = 0; i < vertexWorldNrm.Length; i++)
        {
            newVertexPos[i] = transform.InverseTransformPoint(vertexWorldPos[i]);//vertexPos[i] + vertexNrm[i] * (-1) * vertexDistance[i];
            newVertexNrm[i] = transform.InverseTransformDirection(vertexWorldNrm[i]);
        }
        Vector2[] newUV = mesh.uv;
        int[] newTriangles = mesh.triangles;

        mesh.Clear();

        mesh.vertices = newVertexPos;
        mesh.normals = newVertexNrm;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;
    }

    
}
