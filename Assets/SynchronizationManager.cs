using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynchronizationManager : MonoBehaviour
{

    public GameObject imageTarget;
    public GameObject syncSpace;
    public GameObject viveTracker;

    private Quaternion rotNetworked;
    private Vector3 posNetworked;

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
        if (Input.GetKeyDown("b"))
        {
            Calculate();
        }
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
        //Debug.Log("received imagetracker position");
        rot = imageTarget.transform.rotation;
        pos = imageTarget.transform.position + (rot*new Vector3(0,0,0.1f));
        
        viveTracker.transform.localPosition = new Vector3(0,0,0);
        viveTracker.transform.localRotation = Quaternion.identity;
        syncSpace.transform.position = pos;
        syncSpace.transform.rotation = rot;
        //Debug.Log(pos);
    }

    public void updateViveTracker(Vector3 posNetworked, Quaternion rotNetworked)
    {
        //Debug.Log("Updating vive tracker position");
        this.rotNetworked = rotNetworked;
        this.posNetworked = posNetworked;
        if (synchronized)
        {
            

            Debug.Log("Updating ViveTracker position and rotation");
            viveTracker.transform.localPosition = offsetRot*posNetworked;
            //Debug.Log(viveTracker.transform.localPosition + " - " + posNetworked);
            
            viveTracker.transform.localRotation = rotNetworked;
            
        }
        //Debug.Log(posNetworked);
    }

    public void Calculate()
    {
        syncSpace.transform.rotation = rot*Quaternion.Inverse(rotNetworked);
        syncSpace.transform.position = (-posNetworked + pos);
        offsetRot = Quaternion.Inverse(syncSpace.transform.rotation);
        synchronized = !synchronized;

    }
}
