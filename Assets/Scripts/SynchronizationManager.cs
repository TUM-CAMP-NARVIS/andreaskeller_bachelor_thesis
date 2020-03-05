using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynchronizationManager : MonoBehaviour
{

    public GameObject imageTarget;
    public GameObject syncSpace;
    public GameObject viveTracker;
    public GameObject syncIndicator;

    public List<GameObject> trackedObjects = new List<GameObject>();

    enum State { Synchronized, Scale, Desynchronized}

    private Quaternion rotNetworked;
    private Vector3 posNetworked;

    private Vector3 pos;
    private Quaternion rot;

    private Vector3 pos1;

    private Quaternion offsetRot = Quaternion.identity;

    private State syncState = State.Desynchronized;
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
        if (markerSeen && syncState == State.Desynchronized)
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
        //Old Scene with the big Phantom
        /*
        if (syncState != State.Desynchronized)
            return;
        syncSpace.transform.localScale = new Vector3(1, 1, 1);
        //Debug.Log("received imagetracker position");
        rot = imageTarget.transform.rotation;
        pos = imageTarget.transform.position + (rot*new Vector3(0.1f,0,0.1f));
        
        viveTracker.transform.localPosition = new Vector3(0,0,0);
        viveTracker.transform.localRotation = Quaternion.identity;
        syncSpace.transform.position = pos;
        syncSpace.transform.rotation = rot;
        //Debug.Log(pos);
        */
        if (syncState != State.Desynchronized)
            return;
        syncSpace.transform.localScale = new Vector3(1, 1, 1);
        //Debug.Log("received imagetracker position");
        rot = imageTarget.transform.rotation * Quaternion.Euler(-90, 0, 0) ;
        pos = imageTarget.transform.position + (rot * new Vector3(0f, -0.1225f, 0.063969f));

        viveTracker.transform.localPosition = new Vector3(0, 0, 0);
        viveTracker.transform.localRotation = Quaternion.identity;

        syncSpace.transform.rotation = rot;
        syncSpace.transform.position = pos;
    }

    public void updateTrackedObject(short id, TrackedObjectMessage.Type type, Vector3 posNetworked, Quaternion rotNetworked)
    {
        //Debug.Log("Updating vive tracker position");
        this.rotNetworked = rotNetworked;
        this.posNetworked = posNetworked;
        if (false && syncState == State.Synchronized)
        {
            if (id != 0)
                return;
            viveTracker.transform.localPosition = posNetworked;
            viveTracker.transform.localRotation = rotNetworked;
        }
    }

    public void Synchronize()
    {
        switch (syncState)
        {
            case State.Desynchronized:
                
                syncSpace.transform.rotation = rot * Quaternion.Inverse(rotNetworked);
                syncSpace.transform.position = pos + (syncSpace.transform.TransformDirection(-posNetworked));//Quaternion.Inverse(syncSpace.transform.rotation)*(pos) - posNetworked;
                offsetRot = Quaternion.Inverse(syncSpace.transform.rotation);

                viveTracker.transform.localPosition = posNetworked;
                viveTracker.transform.localRotation = rotNetworked;

                syncIndicator.GetComponent<Renderer>().material.color = new Color(0, 1, 0);
                syncState = State.Synchronized;


                //pos1 = posNetworked;
                //syncSpace.transform.position = pos - (rot * new Vector3(0.2f, 0, 0));
                //pos = syncSpace.transform.position;
                //syncState = State.Scale;
                break;
            case State.Scale:
                var dist = Vector3.Distance(pos1, posNetworked);
                float scale = 0.2f / dist;
                syncSpace.transform.localScale = new Vector3(scale, scale, scale);
                syncSpace.transform.rotation = rot * Quaternion.Inverse(rotNetworked);
                syncSpace.transform.position = pos + (syncSpace.transform.TransformVector(-posNetworked));
                offsetRot = Quaternion.Inverse(syncSpace.transform.rotation);
                syncState = State.Synchronized;
                break;
            default:

                syncState = State.Desynchronized;
                syncIndicator.GetComponent<Renderer>().material.color = new Color(1f, 0.0f, 0.0f);
                break;
        }
        //syncSpace.transform.rotation = rot*Quaternion.Inverse(rotNetworked);
        //syncSpace.transform.position = pos +(syncSpace.transform.TransformDirection(-posNetworked));//Quaternion.Inverse(syncSpace.transform.rotation)*(pos) - posNetworked;
        //offsetRot = Quaternion.Inverse(syncSpace.transform.rotation);
        //synchronized = !synchronized;

    }

    internal void UnhideObjects()
    {
        syncSpace.SetActive(true);
        imageTarget.SetActive(true);
    }
}
