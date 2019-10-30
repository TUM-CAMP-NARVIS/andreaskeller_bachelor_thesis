using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewProjectInverse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Matrix4x4 mat = (GetComponent<Camera>().projectionMatrix * GetComponent<Camera>().worldToCameraMatrix).inverse;
        GetComponent<Renderer>().material.SetMatrix("_ViewProjectInverse", mat);

        var p = GL.GetGPUProjectionMatrix(GetComponent<Camera>().projectionMatrix, false);// Unity flips its 'Y' vector depending on if its in VR, Editor view or game view etc... (facepalm)
        p[2, 3] = p[3, 2] = 0.0f;
        p[3, 3] = 1.0f;
        var clipToWorld = Matrix4x4.Inverse(p * GetComponent<Camera>().worldToCameraMatrix) * Matrix4x4.TRS(new Vector3(0, 0, -p[2, 2]), Quaternion.identity, Vector3.one);
        GetComponent<Renderer>().material.SetMatrix("clipToWorld", clipToWorld);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
