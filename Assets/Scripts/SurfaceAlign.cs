using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Microsoft.MixedReality.Toolkit.Input;

public class SurfaceAlign : MonoBehaviour
{

    

    private GameObject skin;

    public GameObject headlight;
    private bool headlightB = true;

    private int ctrPointsInt = -4;
    public int ctrPoints = 3;
    public int size = 10;

    private bool visualizeControlPoints = false;

    public bool done { get; private set; } = false;

    //For influence calculation
    public int amountControlPointsInfluencing = 1;
    public float maxDistanceInfluence = 0.2f;

    //Private Properties
    private FocusManager focusManager;

    private int lvlscaler;

    private List<ControlPoint> _controlPoints = new List<ControlPoint>();

    public List<ControlPoint> controlPoints
    {
        get
        {
            return _controlPoints;
        }
        
    }

    private List<GameObject> primitives = new List<GameObject>();

    public struct ControlPoint
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
    }

    // Update is called once per frame
    void Update()
    {
        if (!skin)
        {
            skin = focusManager.GetSkin();
            return;
        }

        //Check if any values were edited
        if (ctrPoints != ctrPointsInt)
        {
            ctrPointsInt = ctrPoints;
            lvlscaler = 1 + 2 *  (ctrPointsInt - 1);

            
        }

        //Align the window if user is focusing on the phantom
        if (focusManager.isFocused)
        {
            if (focusManager.isStatic)
                return;

            this.transform.position = focusManager.focusPosition + focusManager.focusNormal * 0.05f;
            this.transform.rotation = Quaternion.LookRotation(new Vector3(0, 1, 0),focusManager.focusNormal);
            

            _controlPoints.Clear();

            rayCastOrigin(ctrPointsInt);


            if (visualizeControlPoints)
            {
                drawPrimitives();
            }

            done = true;
        }
    }

    enum Direction { Origin, Up, Right, Down,Left, None, UpRight, UpLeft, DownRight, DownLeft};

    void rayCastOrigin(int iters)
    {
        rayCast(iters, new Vector3(0, 0, 0), new Vector3(0, 1, 0), 0, Direction.Origin, new Vector3(0,0,0));
    }
    void rayCastPoint(int iters, Vector3 pos, Vector3 nrm, float dist, Direction d, Vector3 prevPos)
    {
        rayCast(iters, pos, nrm, dist, d, prevPos );
    }
    void rayCast(int iters, Vector3 pos, Vector3 nrm, float dist, Direction d, Vector3 prevPos)
    {
        //Only create as many controlpoints as wanted
        if (iters == 0)
            return;
        iters--;

        //calculation so the total size of the field stays the same regardless of control point amount
        float interCubeDist = (float)size / (float)lvlscaler;

        //Create new controlpoint
        ControlPoint ctrlP = new ControlPoint();


        Vector3 hitpos = prevPos;


        float distance = dist;


        if (d == Direction.Origin)
        {
            //worldposition and worldnormal of trace origin
            Vector3 wP = transform.TransformPoint(pos);
            Vector3 wN = transform.TransformDirection(nrm);

            ctrlP.position = wP;
            ctrlP.normal = wN;


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
                if (d != Direction.Origin)
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

                hitpos = hit.point;


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
        }
        else
        {
            Vector3 direction;
            switch(d)
            {
                case Direction.Left:
                    direction = new Vector3(-1,0,0);
                    break;
                case Direction.UpLeft:
                    direction = new Vector3(-1,0,1);
                    break;
                case Direction.Up:
                    direction = new Vector3(0,0,1);
                    break;
                case Direction.UpRight:
                    direction = new Vector3(1,0,1);
                    break;
                case Direction.Right:
                    direction = new Vector3(1,0,0);
                    break;
                case Direction.DownRight:
                    direction = new Vector3(1,0,-1);
                    break;
                case Direction.Down:
                    direction = new Vector3(0,0,-1);
                    break;
                case Direction.DownLeft:
                    direction = new Vector3(-1,0,-1);
                    break;
                default:
                    direction = new Vector3();
                    break;
            }
            direction = Vector3.Normalize(direction);
            direction = transform.TransformDirection(direction);

            float worldDistance = interCubeDist * transform.localScale.x;

            if (d==Direction.UpRight||d==Direction.UpLeft||d==Direction.DownRight||d==Direction.DownLeft)
                worldDistance = Mathf.Sqrt(Mathf.Pow(worldDistance,2)*2);

            
            Vector3 checkPoint = prevPos + direction*worldDistance;

            Vector3 close = skin.GetComponent<MeshCollider>().ClosestPoint(checkPoint);


            //Now check if that point is inside our Parameters
            //TODO

            Vector3 wP = transform.TransformPoint(pos);
            Vector3 wN = transform.TransformDirection(nrm);

            ctrlP.position = wP;
            ctrlP.normal = wN;

            Vector3 translation = close - wP;


            Matrix4x4 m = Matrix4x4.Translate(translation);
            ctrlP.matrix = m;
            ctrlP.matrix_nrm = Matrix4x4.identity;
            hitpos = close;

            _controlPoints.Add(ctrlP);
        }

        
        

        

        switch (d)
        {
            case Direction.Origin:
                //Spawn 8 points
                rayCastPoint(iters, new Vector3(pos.x - interCubeDist, 0, pos.z + interCubeDist), nrm, distance, Direction.UpLeft, hitpos);
                rayCastPoint(iters, new Vector3(pos.x, 0, pos.z + interCubeDist), nrm, distance, Direction.Up, hitpos);
                rayCastPoint(iters, new Vector3(pos.x + interCubeDist, 0, pos.z + interCubeDist), nrm, distance, Direction.UpRight, hitpos);
                rayCastPoint(iters, new Vector3(pos.x - interCubeDist, 0, pos.z), nrm, distance, Direction.Left, hitpos);
                rayCastPoint(iters, new Vector3(pos.x + interCubeDist, 0, pos.z), nrm, distance, Direction.Right, hitpos);
                rayCastPoint(iters, new Vector3(pos.x - interCubeDist, 0, pos.z - interCubeDist), nrm, distance, Direction.DownLeft, hitpos);
                rayCastPoint(iters, new Vector3(pos.x, 0, pos.z - interCubeDist), nrm, distance, Direction.Down, hitpos);
                rayCastPoint(iters, new Vector3(pos.x + interCubeDist, 0, pos.z - interCubeDist), nrm, distance, Direction.DownRight, hitpos);
                break;
            case Direction.Left:
                rayCastPoint(iters, new Vector3(pos.x - interCubeDist, 0, pos.z), nrm, distance, Direction.Left, hitpos);
                break;
            case Direction.Right:
                rayCastPoint(iters, new Vector3(pos.x + interCubeDist, 0, pos.z), nrm, distance, Direction.Right, hitpos);
                break;
            case Direction.Up:
                rayCastPoint(iters, new Vector3(pos.x, 0, pos.z + interCubeDist), nrm, distance, Direction.Up, hitpos);
                break;
            case Direction.Down:
                rayCastPoint(iters, new Vector3(pos.x, 0, pos.z - interCubeDist), nrm, distance, Direction.Down, hitpos);
                break;
            case Direction.UpLeft:
                rayCastPoint(iters, new Vector3(pos.x - interCubeDist, 0, pos.z), nrm, distance, Direction.Left, hitpos);
                rayCastPoint(iters, new Vector3(pos.x - interCubeDist, 0, pos.z + interCubeDist), nrm, distance, Direction.UpLeft, hitpos);
                rayCastPoint(iters, new Vector3(pos.x, 0, pos.z + interCubeDist), nrm, distance, Direction.Up, hitpos);
                break;
            case Direction.UpRight:
                rayCastPoint(iters, new Vector3(pos.x, 0, pos.z + interCubeDist), nrm, distance, Direction.Up, hitpos);
                rayCastPoint(iters, new Vector3(pos.x + interCubeDist, 0, pos.z + interCubeDist), nrm, distance, Direction.UpRight, hitpos);
                rayCastPoint(iters, new Vector3(pos.x + interCubeDist, 0, pos.z), nrm, distance, Direction.Right, hitpos);
                break;
            case Direction.DownLeft:
                rayCastPoint(iters, new Vector3(pos.x, 0, pos.z - interCubeDist), nrm, distance, Direction.Down, hitpos);
                rayCastPoint(iters, new Vector3(pos.x - interCubeDist, 0, pos.z - interCubeDist), nrm, distance, Direction.DownLeft, hitpos);
                rayCastPoint(iters, new Vector3(pos.x - interCubeDist, 0, pos.z), nrm, distance, Direction.Left, hitpos);
                break;
            default:
                rayCastPoint(iters, new Vector3(pos.x + interCubeDist, 0, pos.z), nrm, distance, Direction.Right, hitpos);
                rayCastPoint(iters, new Vector3(pos.x + interCubeDist, 0, pos.z - interCubeDist), nrm, distance, Direction.DownRight, hitpos);
                rayCastPoint(iters, new Vector3(pos.x, 0, pos.z - interCubeDist), nrm, distance, Direction.Down, hitpos);
                break;
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

    public void ToggleControlPoints()
    {
        visualizeControlPoints = !visualizeControlPoints;
        foreach (GameObject g in primitives)
            g.SetActive(visualizeControlPoints);
    }

    public void ToggleHeadLight()
    {
        headlightB = !headlightB;
        headlight.SetActive(headlightB);

    }

}
