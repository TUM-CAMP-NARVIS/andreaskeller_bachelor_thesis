using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudyInputController : MonoBehaviour
{
    public float Slider0;
    public float Slider1;
    public float Slider2;

    public bool Button0;
    public bool Button1;
    public bool Switch0;
    public bool Switch1;

    // Start is called before the first frame update
    void Start()
    {
        // got nothing to do here..
    }

    void Update() {
        GetComponent<SerialController>().SendSerialMessage("u");
    }

    // Invoked when a line of data is received from the serial device.  
    void OnMessageArrived(string msg)
    {
        string[] cmps = msg.Split(',');
        Slider0 = float.Parse(cmps[0]) / 1023.0f;
        Slider1 = float.Parse(cmps[1]) / 1023.0f;
        Slider2 = float.Parse(cmps[2]) / 1023.0f;
        Button0 = cmps[3] == "1";
        Button1 = cmps[4] == "1";
        Switch0 = cmps[5] == "1";
        Switch1 = cmps[6] == "1";
    }

    // Invoked when a connect/disconnect event occurs. The parameter 'success'
    // will be 'true' upon connection, and 'false' upon disconnection or
    // failure to connect.
    void OnConnectionEvent(bool success)
    {
        if(success) {
            Debug.Log("Connected to Input Controller for study.");
        }
        else {
            Debug.LogWarning("Disconnected from Input Controller.");
        }
    }
}
