using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TumorManager : MonoBehaviour
{
    public float yPosition;
    public float realObjectPosition = 0.0f;
    private float rng = 0;
    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, 0.21f, transform.localPosition.z);
        yPosition = transform.localPosition.y;
    }


    public void RepositionTumor(Vector3 position)
    {
        this.gameObject.transform.localPosition = position;
    }

    public float GetDeltaPosition()
    {
        return transform.localPosition.y - realObjectPosition;
    }

    public void SetTumorFront()
    {
        rng = Random.Range(0, 0.01f);
        transform.localPosition = new Vector3(transform.localPosition.x, 0.27f - (rng), transform.localPosition.z);
        yPosition = 0.27f;
    }
    public void SetTumorBack()
    {
        rng = Random.Range(0, 0.01f);
        transform.localPosition = new Vector3(transform.localPosition.x, 0.15f + (rng), transform.localPosition.z);
        yPosition = 0.15f;
    }

    public void MoveTumor(float position)
    {
        
        if (yPosition > 0.22f)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, 0.27f-(rng) -position*0.1f, transform.localPosition.z);
        }
        else if (yPosition < 0.20f)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, 0.15f+(rng) + (1-position) * 0.1f, transform.localPosition.z);
        }
    }
}
