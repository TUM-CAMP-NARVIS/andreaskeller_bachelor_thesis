using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusManager : MonoBehaviour
{
    public GameObject seeThroughObject;
    public float lazyMouseDistance = 0.005f;
    public GameObject cam;

    

    public bool isFocused { get; private set; } = false;

    public bool isStatic { get { return !active; } }

    public Vector3 focusPosition { get; private set; } = new Vector3();

    public Vector3 focusNormal { get; private set; } = new Vector3();

    private bool active = true;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!cam)
        {
            if (Camera.main != null)
                cam = Camera.main.gameObject;
            else
            {
                Debug.LogError("No Camera specified and no MainCamera found");
                return;
            }
                
        }
            
        if (!seeThroughObject)
            return;
        if (!active)
            return;

        var cameraPos = cam.transform.position;
        var cameraForward = cam.transform.forward;

        //Do one Raycast per frame and save position and normal
        //Debug.Log("Updating Focus Position");
        Vector3 hitPosition;
        Vector3 hitNormal;
        GameObject hitObject;


        RaycastHit hit;
        Ray ray = new Ray(cameraPos, cameraForward);

        

        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;
            hitObject = objectHit.gameObject;
            hitPosition = hit.point;
            hitNormal = hit.normal;
            //Debug.Log("Hit Object: "+hitObject.name);

            //LazyMouse Behaviour
            float distanceToGaze = Vector3.Distance(focusPosition, hitPosition);

            //return if we are looking at an unrelated collider
            if (hitObject != seeThroughObject)
            {
                isFocused = false;
                return;
            }

            //if status is not focused but we are actually looking at the object
            if (!isFocused)
            {
                focusPosition = hitPosition;
                focusNormal = hitNormal;
                isFocused = true;
            }
            else if (distanceToGaze > lazyMouseDistance)
            {
                Vector3 moveDirection = Vector3.Normalize(hitPosition - focusPosition);
                float moveDistance = distanceToGaze - lazyMouseDistance;

                Vector3 rayOrigin = cameraPos + (moveDistance * moveDirection);
                Vector3 rayTarget = focusPosition + (moveDistance * moveDirection);
                Vector3 rayDirection = Vector3.Normalize(rayTarget - rayOrigin);

                ray = new Ray(rayOrigin, cameraForward);

                if (Physics.Raycast(ray, out hit))
                {

                    //Not focusing on the object anymore if we dont hit it
                    if (hit.collider.gameObject != seeThroughObject)
                    {
                        isFocused = false;
                        return;
                    }

                    focusPosition = hit.point;
                    focusNormal = hit.normal;

                }
                else
                {
                    isFocused = false;
                }
            }

        }
        else
        {
            isFocused = false;
            //Debug.Log("Didnt hit anything");
            return;
        }
    }

    public void ToggleActive()
    {
        active = !active;
    }

    public void RegisterSkin(GameObject obj)
    {
        seeThroughObject = obj;
    }

    public GameObject GetSkin()
    {
        return seeThroughObject;
    }
}
