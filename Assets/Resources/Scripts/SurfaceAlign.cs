﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class SurfaceAlign : MonoBehaviour
{

    public GazeProvider gazeProvider;
    public GameObject skin;
    public GameObject windowMesh;

    /*
    private Vector3[] vertexPos;
    private Vector3[] vertexWorldPos;
    private Vector3[] vertexNrm;
    private Vector3[] vertexWorldNrm;
    private float[] vertexDistance;
    */

    public int ctrPoints = 3;
    public int size = 10;

    private int lvlscaler;

    private List<ControlPoint> _controlPoints = new List<ControlPoint>();

    private List<GameObject> primitives = new List<GameObject>();


    private Vector3 position = new Vector3(10,10,10);
    private Vector3 lookDirection = new Vector3(0, 1, 0);

    private (int, float)[,] influenceID;

    //Window mesh data
    private Vector3[] windowVertices;
    private Vector3[] windowNormals;

    private bool calculated = false;


    //For influence calculation
    SortedList<float, int> influenceIDs = new SortedList<float, int>();
    public int amountControlPointsInfluencing = 1;
    private int amountControlPointInfluencing = -1;
    public float maxDistanceInfluence = 0.2f;
    private float maxInfluenceDistance = -0.2f;

    private Vector3 windowMeshScale;


    struct ControlPoint
    {
        public Vector3 position;
        public Vector3 normal;
        public Matrix4x4 matrix;
        public Matrix4x4 matrix_nrm;
    }

    // Start is called before the first frame update
    void Start()
    {
        /*
        Mesh mesh = this.transform.GetChild(0).GetComponent<MeshFilter>().mesh;
        vertexPos = new Vector3[mesh.vertexCount];
        vertexNrm = new Vector3[mesh.vertexCount];
        System.Array.Copy(mesh.vertices, vertexPos, mesh.vertexCount);
        System.Array.Copy(mesh.normals, vertexNrm, mesh.vertexCount);
        vertexWorldPos = new Vector3[mesh.vertexCount];
        vertexWorldNrm = new Vector3[mesh.vertexCount];
        vertexDistance = new float[mesh.vertexCount];
        */
        lvlscaler = 1 + 2 * (ctrPoints - 1);

        windowMeshScale = windowMesh.transform.localScale;
        


        Mesh mesh = windowMesh.GetComponent<MeshFilter>().mesh;
        windowVertices = mesh.vertices;
        windowNormals = mesh.normals;

    }

    // Update is called once per frame
    void Update()
    {
        if (windowMesh.transform.localScale != windowMeshScale)
        {
            calculated = false;
            windowMeshScale = windowMesh.transform.localScale;
        }
            
        //Recalculate influence only if the user defined variables change
        if ((amountControlPointInfluencing != amountControlPointsInfluencing)||maxDistanceInfluence != maxInfluenceDistance)
        {
            amountControlPointInfluencing = amountControlPointsInfluencing;
            maxInfluenceDistance = maxDistanceInfluence;
            calculated = false;
        }
        //Make sure min 1 controlpoint always influences the verts
        if (amountControlPointInfluencing < 1)
            amountControlPointInfluencing = 1;

        if (false)//gazeProvider.GazeTarget != skin)
        {
            windowMesh.gameObject.SetActive(false);
            
        }
        else
        {
           windowMesh.gameObject.SetActive(true);
            this.transform.position = gazeProvider.HitPosition+gazeProvider.HitNormal*0.05f;
            this.transform.up = gazeProvider.HitNormal;
        }

        //if (gazeProvider.HitPosition.x - position.x > 0.5 || gazeProvider.GazeDirection.x - lookDirection.x > 0.5)
        {
            position = gazeProvider.HitPosition;
            lookDirection = gazeProvider.GazeDirection;

            _controlPoints.Clear();

            rayCastPoint(ctrPoints, new Vector3(0, 0, 0), new Vector3(0,1,0), 0, false, false, false, 0, direction.None, true);

            //Can't have more controlpoints influence a vertex than there are
            if (_controlPoints.Count < amountControlPointInfluencing)
                amountControlPointInfluencing = _controlPoints.Count;

            if (Application.isEditor)
            {
                drawPrimitives();
            }
            
            calculateInfluence();
            moveWindow();
        }

        

        //raycastPlane();
        //applyPosition();

    }
    enum direction {Up, Right, Down,Left, None};

    void rayCastPoint(int iters, Vector3 pos, Vector3 nrm, float dist = 0, bool corner = false, bool up=true, bool right=true, int id = 0, direction d = direction.None, bool origin = false)
    {
        //Only create as many controlpoints as wanted
        if (iters == 0)
            return;
        iters--;

        if (origin)
            id = 0;

        //calculation so the total size of the field stays the same regardless of control point amount
        float interCubeDist = (float)size / (float)lvlscaler;

        //Create new controlpoint
        ControlPoint ctrlP = new ControlPoint();

        
        //worldposition and worldnormal of trace origin
        Vector3 wP = transform.TransformPoint(pos);
        Vector3 wN = transform.TransformDirection(nrm);

        ctrlP.position = wP;
        ctrlP.normal = wN;
        float distance = dist;


        //trace a ray from trace origin towards the model
        RaycastHit hit;
        var ray = new Ray(wP, wN * (-1));
        
        
        if (Physics.Raycast(ray, out hit))
        {
            

            //use hit normal to calculate the next points
            nrm = transform.InverseTransformDirection(hit.normal);

            //translation from original point to point on model
            Vector3 translation = hit.point - wP;

            distance = Vector3.Magnitude(translation);
            if (!origin)
            {
                float differenceDist = Mathf.Abs(distance - dist);
                if (differenceDist > 2 * interCubeDist)
                {
                    if (distance - dist > 0)
                        translation = Vector3.Normalize(translation) * (dist + 2 * interCubeDist);
                    else
                        translation = Vector3.Normalize(translation) * (dist - 2 * interCubeDist);

                    distance = Vector3.Magnitude(translation);
                }
            }
            
            

            //rotation - this breaks everything, TODO
            Quaternion q = new Quaternion();
            q.SetFromToRotation(ctrlP.normal, nrm);
            Matrix4x4 rm = Matrix4x4.Rotate(q);
            ctrlP.matrix_nrm = rm;
            //scale
            Vector3 scale = new Vector3(0, 0, 0);

            //dont use trs for now, only translation
            //Matrix4x4 m = Matrix4x4.TRS(translation, q, scale);
            Matrix4x4 m = Matrix4x4.Translate(translation);
            ctrlP.matrix = m;
        }
        else
        {
            //if there is no target, dont move the point
            ctrlP.matrix = Matrix4x4.identity;
        }
        _controlPoints.Add(ctrlP);

        if (origin) //Origin
        {
            //Spawn 8 new points
            for (int i = 0; i<3; i++)
            {
                for (int j = 0; j<3; j++)
                {
                    Vector3 newPoint = new Vector3(pos.x - interCubeDist + i*interCubeDist, 0, pos.z - interCubeDist + j*interCubeDist);
                    if (newPoint == pos)
                    {
                        continue;
                    }
                    bool newCorner = false;
                    if (Mathf.Abs(Mathf.Abs(newPoint.x) - Mathf.Abs(newPoint.z)) < (interCubeDist*0.1))
                    {
                        newCorner = true;
                    }
                    bool newUp = newPoint.z - pos.z > (interCubeDist*0.5) ? true : false;
                    bool newRight = newPoint.x - pos.x > (interCubeDist * 0.5) ? true : false;
                    direction newD = direction.None;
                    if (!newCorner)
                    {
                        if (newPoint.x > (interCubeDist * 0.1))
                        {
                            newD = direction.Right;
                        }
                        else if (newPoint.x < -(interCubeDist * 0.1))
                        {
                            newD = direction.Left;
                        }
                        else if (newPoint.z > (interCubeDist * 0.1))
                        {
                            newD = direction.Up;
                        }
                        else
                        {
                            newD = direction.Down;
                        }
                    }

                    rayCastPoint(iters, newPoint, nrm, distance, newCorner, newUp, newRight, 0, newD);
                }
            }

        }
        else if (corner) //corner
        {
            //Spawn 3 new points
            float compRight = right ? interCubeDist : -interCubeDist;
            float compUp = up ? interCubeDist : -interCubeDist;

            Vector3 newPoint = new Vector3(pos.x + compRight,0, pos.z + compUp);
            rayCastPoint(iters, newPoint, nrm, distance, true, up, right);

            direction newD = right ? direction.Right : direction.Left;
            
            newPoint = new Vector3(pos.x + compRight, 0, pos.z);
            rayCastPoint(iters, newPoint, nrm, distance, false, up, right, 0, newD);

            newD = up ? direction.Up : direction.Down;
            newPoint = new Vector3(pos.x, 0, pos.z+compUp);
            rayCastPoint(iters, newPoint, nrm, distance, false, up, right, 0, newD);
        }
        else
        {
            Vector3 newPoint = pos;
            switch (d)
            {
                case direction.Right:
                    newPoint.x += interCubeDist;
                    break;
                case direction.Left:
                    newPoint.x -= interCubeDist;
                    break;
                case direction.Up:
                    newPoint.z += interCubeDist;
                    break;
                default:
                    newPoint.z -= interCubeDist;
                    break;
            }
            rayCastPoint(iters, newPoint, nrm, distance, false, up, right, 0, d);
        }
    }

    //Visualize ControlPoints with a Primitive shape
    void drawPrimitives()
    {
        if (primitives.Count !=_controlPoints.Count)
        {
            for (int i = 0; i < _controlPoints.Count; i++)
            {
                GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
                primitive.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                primitive.GetComponent<Collider>().enabled = false;
                primitives.Add(primitive);
                //primitive.transform.up = _ctrlPointNrms[i];

            }
        }
        else
        {
            for (int i = 0; i < _controlPoints.Count; i++)
            {
                primitives[i].transform.position = _controlPoints[i].matrix.MultiplyPoint(_controlPoints[i].position);
                primitives[i].transform.up = _controlPoints[i].normal;
            }
        }

        
    }

    /*
    void raycastPlane()
    {
        //fixed amount of controlpoints for now, 9
        int depth = 2;
        Queue<(Vector3, Vector3, int)> points = new Queue<(Vector3, Vector3, int)>();
        Vector3 point = new Vector3(0, 0, 0);
        Vector3 dir = new Vector3(0, 1, 0);
        points.Enqueue((point,dir, 0));
        List<Vector3> done = new List<Vector3>();

        for (int i = 0; i<4; i++)
        {
            

        }

        for (int i = 0; i<1; i++)
        {
            RaycastHit hit;

            var ray = new Ray(point, dir * (-1));
            if (Physics.Raycast(ray, out hit))
            {
                //spheres[i].transform.position = hit.point;
                vertexWorldPos[i] = hit.point;
                vertexWorldNrm[i] = hit.normal;
                vertexDistance[i] = hit.distance * (1.0f / transform.localScale.x);

            }
        }



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
    */

    void calculateInfluence()
    {
        if (calculated)
            return;
        Mesh mesh = windowMesh.GetComponent<MeshFilter>().mesh;
        Vector3[] verts = windowVertices;

        /* Old Method
        //Method for selecting influence tbd, right now just take the next controlpoint
        influenceID = new int[mesh.vertexCount];
        //string debug = "";
        for (int i = 0; i<verts.Length; i++) 
        {
            Vector3 vert = verts[i];
            Vector3 wP = transform.TransformPoint(vert);
            int closest = 0;
            float distance = 100;
            for (int j = 0; j<_controlPoints.Count; j++)
            {
                ControlPoint c = _controlPoints[j];
                float dist = Vector3.Distance(wP, c.position);
                if ( dist < distance)
                {
                    distance = dist;
                    closest = j;
                }
            }
            influenceID[i] = closest;
            //debug += "Vertex " + i + ": " + closest;
        }
        */

        //New method, influence of userdefined amount of closest controlpoints

        //if controlpoint is further away from vert than this distance, set its influence to 0
        float maxDist = maxInfluenceDistance;
        
        influenceID = new (int, float)[verts.Length, amountControlPointInfluencing];
        for (int i = 0; i<verts.Length; i++)
        {
            Vector3 vert = verts[i];
            Vector3 worldPos = windowMesh.transform.TransformPoint(vert);
            influenceIDs.Clear();
            for (int j = 0; j<_controlPoints.Count; j++)
            {
                ControlPoint c = _controlPoints[j];
                float dist = Vector3.Distance(worldPos, c.position);
                while (influenceIDs.ContainsKey(dist))
                {
                    dist += 0.0001f;
                }
                influenceIDs.Add(dist, j);

            }
            float totalInfl = 1.0f;

            for (int j = (amountControlPointInfluencing-1); j>0; j--)
            {
                float dist = influenceIDs.Keys[j];
                int id = influenceIDs.Values[j];
                float influence = 0;
                if (dist < maxDist)
                {
                    influence = (totalInfl/((float) (j+1))) * (1 - dist / maxDist);
                }
                totalInfl -= influence;
                Debug.Log("Influence vertex " + i + " ControlPoint " + id + " = " + influence + "j="+j);
                influenceID[i, j] = (id, influence);
                
            }
            //Closest controlpoint always has at least 1/amountControlPointInfluencing influence, and a maximum of 1
            int idFirst = influenceIDs.Values[0];
            //Debug.Log("Influence vertex " + i + " ControlPoint " + idFirst + " = " + totalInfl);
            influenceID[i, 0] = (idFirst, totalInfl);
        }

        calculated = true;
        //Debug.Log(debug);
        
    }

    void moveWindow()
    {

        Mesh mesh = windowMesh.GetComponent<MeshFilter>().mesh;
        Vector3[] newVertexPos = new Vector3[mesh.vertexCount];
        Vector3[] newVertexNrm = new Vector3[mesh.vertexCount];

        for (int i = 0; i<newVertexPos.Length; i++)
        {
            Vector3 vert = windowMesh.transform.TransformPoint(windowVertices[i]);
            Vector3 nrm = windowNormals[i];
            Vector3[] vertsNew = new Vector3[amountControlPointInfluencing];
            Vector3[] nrmsNew = new Vector3[amountControlPointInfluencing];
            for (int j = 0; j<amountControlPointInfluencing; j++)
            {
                
                vertsNew[j] = _controlPoints[(influenceID[i,j].Item1)].matrix.MultiplyPoint(vert);
                vertsNew[j] *= influenceID[i, j].Item2;

                nrmsNew[j] = _controlPoints[(influenceID[i, j].Item1)].matrix_nrm.MultiplyVector(nrm);
                nrmsNew[j] *= influenceID[i, j].Item2;

            }
            Vector3 vertNew = new Vector3();
            Vector3 nrmNew = new Vector3();
            for (int j = 0; j<amountControlPointInfluencing; j++)
            {
                Vector3 v = vertsNew[j];
                Vector3 n = nrmsNew[j];
                vertNew += v;
                nrmNew += n;
            }
            newVertexPos[i] = windowMesh.transform.InverseTransformPoint(vertNew);
            newVertexNrm[i] = Vector3.Normalize(nrmNew);
        }

        Vector2[] newUV = mesh.uv;
        int[] newTriangles = mesh.triangles;

        mesh.Clear();

        mesh.vertices = newVertexPos;
        mesh.normals = newVertexNrm;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;
    }

    /*
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
    */
    
}
