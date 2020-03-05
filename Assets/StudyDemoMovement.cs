using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SerialController))]
[RequireComponent(typeof(StudyInputController))]
public class StudyDemoMovement : MonoBehaviour
{
    public float ConfirmationTimeSeconds = 3.0f;
    public float CurrentConfirmationProgress = 0.0f;
    public bool ConfirmationCooldown = false;

    private StudyManager studyManager;

    StudyInputController studyInputCtrl;

    // Start is called before the first frame update
    void Start()
    {
        studyManager = FindObjectOfType<StudyManager>();

        studyInputCtrl = GetComponent<StudyInputController>();
        
        var serialCmp = GetComponent<SerialController>();
#if !UNITY_WSA
        serialCmp.portName = "COM5";
        serialCmp.baudRate = 9600;
        serialCmp.messageListener = gameObject;
        serialCmp.maxUnreadMessages = 3; // prevents "Queue is full" errors
        serialCmp.enabled = false;
        serialCmp.enabled = true;
#else
        
#endif

    }

    // Update is called once per frame
    void Update()
    {
        float posX = studyInputCtrl.Slider0 - 0.5f;
        float posY = studyInputCtrl.Slider1 - 0.5f;
        float posZ = studyInputCtrl.Slider2 - 0.5f;

#if !UNITY_WSA
        //gameObject.transform.position = new Vector3(posX, posY, posZ + 2.0f);
        studyManager.SliderMoved(studyInputCtrl.Slider0);


        if(studyInputCtrl.Button1) {
            studyManager.SendStudyManagerMessage();
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

        if (CurrentConfirmationProgress <= 0.05)
        {
            studyManager.buttonIndicator.GetComponent<Renderer>().material.color = new Color(1, 1, 1);
        }
        else
        {
            studyManager.buttonIndicator.GetComponent<Renderer>().material.color = new Color(1-CurrentConfirmationProgress, 1, 1-CurrentConfirmationProgress);
        }
#endif
    }

    void OnPositionConfirmed() {
        studyManager.ButtonInput();
    }
}
