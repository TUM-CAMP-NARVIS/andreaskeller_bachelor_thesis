using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MenuHandler : MonoBehaviour
{
    public GameObject holoLens;
    public bool movement;
    private Vector3 originalPos;
    private Vector3 lastPos;
    private float originalDist;

    // Start is called before the first frame update
    void Start()
    {

        originalPos = this.transform.position;
        lastPos = holoLens.transform.position;
        originalDist = Vector3.Distance(originalPos, lastPos);
        
    }

    // Update is called once per frame
    void Update()
    {
        //Rotate towards camera
        if (holoLens)
        {
            Vector3 angle = Vector3.Normalize(this.transform.position - holoLens.transform.position);
            this.transform.rotation = Quaternion.LookRotation(angle);
            this.transform.rotation = new Quaternion(this.transform.rotation.x,this.transform.rotation.y,0.0f,this.transform.rotation.w);

            //Move towards camera

            if (movement)
            {
                Vector3 position = holoLens.transform.position;
                Vector3 angle1 = Vector3.Normalize(holoLens.transform.position - this.transform.position);
                Vector3 angle2 = Vector3.Normalize(holoLens.transform.forward);
                float dot = Mathf.Abs(Vector3.Dot(angle1, angle2));
                if (dot < 0.3)//Vector3.Distance(position, this.transform.position) > originalDist + 5.0)
                {
                    Quaternion rot = Quaternion.LookRotation(angle);
                    this.transform.position = position + holoLens.transform.rotation * originalPos;
                    lastPos = position;
                }
            }
            
            
        }
    }

   
}
