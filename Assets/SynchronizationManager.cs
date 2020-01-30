using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynchronizationManager : MonoBehaviour
{

    public GameObject imageTarget;
    public GameObject viveTracker;

    private Vector3 pos;
    private Quaternion rot;

    private Vector3 offsetPos = new Vector3(0,0,0);
    private Quaternion offsetRot = Quaternion.identity;

    private bool synchronized = false;
    private bool markerSeen = false;
    // Start is called before the first frame update
    void Start()
    {
        if (Utils.IsVR)
            this.enabled = false;
    }

    void Synchronize()
    {
        pos = imageTarget.transform.position;
        rot = imageTarget.transform.rotation;
        markerSeen = true;
    }

    public void updateViveTracker(Vector3 pos, Quaternion rot)
    {
        if (!synchronized && markerSeen)
        {
            offsetPos = pos - this.pos;
            offsetRot = Quaternion.Inverse(rot) * this.rot;
        }

        viveTracker.transform.position = pos + offsetPos;
        viveTracker.transform.rotation = rot * offsetRot;
    }
}
