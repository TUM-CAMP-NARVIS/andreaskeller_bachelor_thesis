using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TumorManager : MonoBehaviour
{
    public float yPosition;
    // Start is called before the first frame update
    void Start()
    {
        yPosition = transform.localPosition.y;
    }

#if !UNITY_WSA
    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, yPosition, transform.localPosition.z);
    }
#endif

    public void RepositionTumor(Vector3 position)
    {
        this.gameObject.transform.localPosition = position;
    }
}
