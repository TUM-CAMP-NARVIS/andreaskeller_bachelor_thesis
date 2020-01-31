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

    void Update()
    {
        if (markerSeen && !synchronized)
        {
            Synchronize();
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

    void Synchronize()
    {
        if (synchronized)
            return;
        Debug.Log("received imagetracker position");
        rot = imageTarget.transform.rotation;
        pos = imageTarget.transform.position + (rot*new Vector3(0,0,0.1f));
        
        syncSpace.transform.position = pos;
        syncSpace.transform.rotation = rot;
        Debug.Log(pos);
    }

    public void updateViveTracker(Vector3 posNetworked, Quaternion rotNetworked)
    {
        if (!synchronized && markerSeen)
        {
            Debug.Log("Synchronizing");
            syncSpace.transform.position = -posNetworked + syncSpace.transform.position;
            syncSpace.transform.rotation = Quaternion.Inverse(rotNetworked) * rot;//rotNetworked * x = rot -- x = rot * Quaternion.inv(rotNetworked)
            synchronized = true;
        }
        if (synchronized)
        {
            //Debug.Log("Updating ViveTracker position:");
            viveTracker.transform.localPosition = posNetworked;
            viveTracker.transform.localRotation = rotNetworked;
            //Debug.Log(syncSpace.transform.position);
            //Debug.Log(viveTracker.transform.localPosition);
        }
        
    }
}
