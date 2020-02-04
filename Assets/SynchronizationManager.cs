using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynchronizationManager : MonoBehaviour
{

    public GameObject imageTarget;
    public GameObject syncSpace;
    public GameObject viveTracker;

    public List<GameObject> trackedObjects = new List<GameObject>();

    enum State { Synchronized, Desynchronized}

    private Quaternion rotNetworked;
    private Vector3 posNetworked;

    private Vector3 pos;
    private Quaternion rot;

    private Quaternion offsetRot = Quaternion.identity;

    private bool synchronized = false;
    private bool markerSeen = false;

    // Start is called before the first frame update
    void Start()
    {
        //This is only for the devices not natively supporting vive tracking
        if (Utils.IsVR)
            this.enabled = false;

    }

    void Update()
    {
        if (markerSeen && !synchronized)
        {
            MoveToMarker();
        }
    }

    public void targetIsVisible()
    {
        markerSeen = true;
    }

    public void targetLost()
    {
        markerSeen = false;
    }

    void MoveToMarker()
    {
        if (synchronized)
            return;
        //Debug.Log("received imagetracker position");
        rot = imageTarget.transform.rotation;
        pos = imageTarget.transform.position + (rot*new Vector3(0,0,0.1f));
        
        viveTracker.transform.localPosition = new Vector3(0,0,0);
        viveTracker.transform.localRotation = Quaternion.identity;
        syncSpace.transform.position = pos;
        syncSpace.transform.rotation = rot;
        //Debug.Log(pos);
    }

    public void updateTrackedObject(short id, TrackedObjectMessage.Type type, Vector3 posNetworked, Quaternion rotNetworked)
    {
        //Debug.Log("Updating vive tracker position");
        this.rotNetworked = rotNetworked;
        this.posNetworked = posNetworked;
        if (synchronized)
        {
            if (id != 0)
                return;
            viveTracker.transform.localPosition = posNetworked;
            viveTracker.transform.localRotation = rotNetworked;
        }
    }

    public void Synchronize()
    {
        syncSpace.transform.rotation = rot*Quaternion.Inverse(rotNetworked);
        syncSpace.transform.position = pos +(syncSpace.transform.TransformDirection(-posNetworked));//Quaternion.Inverse(syncSpace.transform.rotation)*(pos) - posNetworked;
        offsetRot = Quaternion.Inverse(syncSpace.transform.rotation);
        synchronized = !synchronized;

    }

    internal void UnhideObjects()
    {
        syncSpace.SetActive(true);
        imageTarget.SetActive(true);
    }
}
