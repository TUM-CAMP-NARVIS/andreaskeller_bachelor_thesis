using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowMesh : MonoBehaviour
{
    public int amountControlPointsInfluencing = 3;
    public float maxDistanceInfluence = 0.2f;

    private SurfaceAlign surfaceAlign;
    private bool setup = false;

    private Mesh mesh;
    private Vector3[] verts;
    private Vector3[] normals;
    private Vector3 oldPos;
    private Vector3 scale;
    private (int, float)[,] influenceIDs;
    // Start is called before the first frame update

    //Save rotation, position
    private Vector3 position;
    private Quaternion rotation;
    void Start()
    {
        position = transform.position;
        rotation = transform.rotation;
        surfaceAlign = FindObjectOfType<SurfaceAlign>();
        setupWindow();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = surfaceAlign.transform.position;// + position;
        transform.rotation = surfaceAlign.transform.rotation * rotation;

        

        if (surfaceAlign.done)
        {
            if (!setup)
            {
                calculateInfluence();
            }
            if (oldPos != transform.position)
            {
                move();
                oldPos = transform.position;
            }
                
        }
    }

    private void setupWindow()
    {
        Mesh m = GetComponent<MeshFilter>().mesh;
        mesh = m;
        verts = m.vertices;
        normals = m.normals;
        scale = transform.localScale;
    }

    private void calculateInfluence()
    {
        Vector3[] verts = this.verts;
        float maxDistance = maxDistanceInfluence;


        var infIDs = new (int, float)[verts.Length, amountControlPointsInfluencing];

        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 vert = verts[i];
            Vector3 worldPos = transform.TransformPoint(vert);

            //Calculate distance to each controlpoint

            SortedList<float, int> distanceToCtrlP = new SortedList<float, int>();

            for (int j = 0; j < surfaceAlign.controlPoints.Count; j++)
            {
                SurfaceAlign.ControlPoint c = surfaceAlign.controlPoints[j];
                float dist = Vector3.Distance(worldPos, c.position);
                while (distanceToCtrlP.ContainsKey(dist))
                {
                    dist += 0.0001f;
                }
                distanceToCtrlP.Add(dist, j);

            }

            float totalInfl = 1.0f;

            for (int j = (amountControlPointsInfluencing - 1); j > 0; j--)
            {
                float dist = distanceToCtrlP.Keys[j];
                int id = distanceToCtrlP.Values[j];
                float influence = 0;
                if (dist < maxDistance)
                {
                    influence = (totalInfl / ((float)(j + 1))) * (1 - dist / maxDistance);
                }
                totalInfl -= influence;
                infIDs[i, j] = (id, influence);

            }
            int idFirst = distanceToCtrlP.Values[0];
            infIDs[i, 0] = (idFirst, totalInfl);
        }
        influenceIDs = infIDs;

        setup = true;
    }

    void move()
    {

        Mesh mesh = this.mesh;
        Vector3[] newVertexPos = new Vector3[this.verts.Length];

        for (int i = 0; i < newVertexPos.Length; i++)
        {
            //Transform vertices according to controlpoints
            Vector3 vert = transform.TransformPoint(this.verts[i]);

            Vector3 newVertPos = new Vector3();

            for (int j = 0; j < influenceIDs.GetLength(1); j++)
            {
                int ctrlPID = influenceIDs[i, j].Item1;
                float ctrlPInfl = influenceIDs[i, j].Item2;
                Vector3 newVert = surfaceAlign.controlPoints[ctrlPID].matrix.MultiplyPoint(vert);
                newVert *= ctrlPInfl;
                newVertPos += newVert;
            }

            newVertexPos[i] = transform.InverseTransformPoint(newVertPos);
        }

        Vector2[] newUV = mesh.uv;
        int[] newTriangles = mesh.triangles;

        mesh.Clear();

        mesh.vertices = newVertexPos;
        mesh.normals = normals;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;
    }

}
