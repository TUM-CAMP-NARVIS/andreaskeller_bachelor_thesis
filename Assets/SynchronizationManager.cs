using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynchronizationManager : MonoBehaviour
{

    public GameObject imageTarget;
    public GameObject syncSpace;
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
        if (markerSeen && synchronized)
            return;
        Debug.Log("received imagetracker position");
        rot = imageTarget.transform.rotation;
        pos = imageTarget.transform.position + (rot*new Vector3(0,0,0.1f));
        
        syncSpace.transform.position = pos;
        syncSpace.transform.rotation = rot;
        markerSeen = true;
        Debug.Log(pos);
    }

    public void updateViveTracker(Vector3 posNetworked, Quaternion rot)
    {
        if (!synchronized && markerSeen)
        {
            Debug.Log("Synchronizing");
            syncSpace.transform.position = -posNetworked + syncSpace.transform.position;
            synchronized = true;
        }
        if (synchronized)
        {
            Debug.Log("Updating ViveTracker position:");
            viveTracker.transform.localPosition = posNetworked;
            viveTracker.transform.rotation = rot;
            Debug.Log(syncSpace.transform.position);
            Debug.Log(viveTracker.transform.localPosition);
        }
        
    }
}
