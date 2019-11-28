using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class SurfaceAlign : MonoBehaviour
{

    private GameObject skin;
    private List<GameObject> windowMeshes = new List<GameObject>();
    private GameObject windowMesh;

    private int ctrPointsInt = -4;
    public int ctrPoints = 3;
    public int size = 10;

    public bool windowVisible = true;

    //For influence calculation
    public int amountControlPointsInfluencing = 1;
    public float maxDistanceInfluence = 0.2f;

    //Private Properties
    private FocusManager focusManager;

    private int lvlscaler;

    private List<ControlPoint> _controlPoints = new List<ControlPoint>();

    private List<GameObject> primitives = new List<GameObject>();

    private (int, float)[,] influenceID;

    //Window mesh data
    private Vector3[] windowVertices;
    private Vector3[] windowNormals;

    private bool calculated = false;


    //For influence calculation
    SortedList<float, int> influenceIDs = new SortedList<float, int>();
    
    private int amountControlPointInfluencing = -1;
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
        focusManager = FindObjectOfType<FocusManager>();
        skin = focusManager.GetSkin();
        SetupWindow();
    }

    // Update is called once per frame
    void Update()
    {
        if (!windowMesh)
        {
            SetupWindow();
            return;
        }
            
        if (!skin)
        {
            skin = focusManager.GetSkin();
            return;
        }
        //Check if any values were edited
        if (windowMesh.transform.localScale != windowMeshScale || amountControlPointInfluencing != amountControlPointsInfluencing || maxDistanceInfluence != maxInfluenceDistance || ctrPoints != ctrPointsInt)
        {
            calculated = false;
            windowMeshScale = windowMesh.transform.localScale;
            amountControlPointInfluencing = amountControlPointsInfluencing;
            maxInfluenceDistance = maxDistanceInfluence;
            ctrPointsInt = ctrPoints;
            lvlscaler = 1 + 2 * (ctrPointsInt - 1);
        }

        //Align the window if user is focusing on the phantom
        if (focusManager.isFocused && windowVisible)
        {
            windowMesh.SetActive(true);

            if (focusManager.isStatic)
                return;

            this.transform.position = focusManager.focusPosition + focusManager.focusNormal * 0.05f;
            this.transform.up = focusManager.focusNormal;

            windowMesh.transform.position = this.transform.position;
            windowMesh.transform.up = this.transform.up;

            _controlPoints.Clear();

            rayCastPoint(ctrPointsInt, new Vector3(0, 0, 0), new Vector3(0, 1, 0), 0, false, false, false, 0, direction.None, true);

            //Make sure the amount of control points that influence each vertex are in bounds
            if (amountControlPointInfluencing < 1)
                amountControlPointInfluencing = 1;
            if (_controlPoints.Count < amountControlPointInfluencing)
                amountControlPointInfluencing = _controlPoints.Count;


            if (Application.isEditor)
            {
                drawPrimitives();
            }

            calculateInfluence();
            moveWindow();
        }
        else
        {
            windowMesh.SetActive(false);
        }
    }

    void SetupWindow()
    {
        if (windowMeshes.Count < 1)
            return;
        windowMesh = windowMeshes[0];

        windowMeshScale = windowMesh.transform.localScale;


        Mesh mesh = windowMesh.GetComponent<MeshFilter>().mesh;
        windowVertices = mesh.vertices;
        windowNormals = mesh.normals;
    }

    public bool RegisterWindow(GameObject w)
    {
        if (w.GetComponent<MeshFilter>() != null)
        {
            windowMeshes.Add(w);
            return true;
        }
        return false;
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
            ctrlP.matrix_nrm = Matrix4x4.identity;
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

    void calculateInfluence()
    {
        if (calculated)
            return;
        Mesh mesh = windowMesh.GetComponent<MeshFilter>().mesh;
        Vector3[] verts = windowVertices;

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
                //Debug.Log("Influence vertex " + i + " ControlPoint " + id + " = " + influence + "j="+j);
                influenceID[i, j] = (id, influence);
                
            }
            int idFirst = influenceIDs.Values[0];
            influenceID[i, 0] = (idFirst, totalInfl);
        }

        calculated = true;
        
    }

    //Move the window mesh according to the control points
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


}
