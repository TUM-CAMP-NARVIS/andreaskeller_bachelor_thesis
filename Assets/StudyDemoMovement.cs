using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudyDemoMovement : MonoBehaviour
{
    public float ConfirmationTimeSeconds = 3.0f;
    public float CurrentConfirmationProgress = 0.0f;
    public bool ConfirmationCooldown = false;

    StudyInputController studyInputCtrl;

    // Start is called before the first frame update
    void Start()
    {
        var serialCmp = gameObject.AddComponent<SerialController>();
        serialCmp.portName = "COM3";
        serialCmp.baudRate = 9600;
        serialCmp.messageListener = gameObject;
        serialCmp.maxUnreadMessages = 3; // prevents "Queue is full" errors

        studyInputCtrl = gameObject.AddComponent<StudyInputController>();
    }

    // Update is called once per frame
    void Update()
    {
        float posX = studyInputCtrl.Slider0 - 0.5f;
        float posY = studyInputCtrl.Slider1 - 0.5f;
        float posZ = studyInputCtrl.Slider2 - 0.5f;

        gameObject.transform.position = new Vector3(posX, posY, posZ + 2.0f);

        if(studyInputCtrl.Button1) {
            if(!ConfirmationCooldown) {
            CurrentConfirmationProgress += Time.deltaTime / ConfirmationTimeSeconds;
            if(CurrentConfirmationProgress >= 1.0f) {
                OnPositionConfirmed();
                ConfirmationCooldown = true;
            }
            }
        }
        else {
            ConfirmationCooldown = false;
            CurrentConfirmationProgress = 0;
        }

        gameObject.GetComponent<Renderer>().material.SetFloat("_Confirmation", CurrentConfirmationProgress);
    }

    void OnPositionConfirmed() {
        Debug.Log("Position was confirmed!");
    }
}
