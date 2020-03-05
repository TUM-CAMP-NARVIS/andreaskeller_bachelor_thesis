using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(StudyManager))]
public class StudyManagerHUD : MonoBehaviour
{
    StudyManager manager;
    WindowMaterialManager windowMaterialManager;
    TumorManager tumorManager;

    private string tumorPosition;
    private string comPort = "COM5";
    private string subjectID = "testSubject";
    private string trialAmountOverride = "0";

    public int offsetX, offsetY;

#if UNITY_WSA
    public bool showGUI = false;
#else
    public bool showGUI = true;
#endif

    private void Awake()
    {
        manager = GetComponent<StudyManager>();
        windowMaterialManager = FindObjectOfType<WindowMaterialManager>();
        tumorManager = FindObjectOfType<TumorManager>();



        tumorPosition = "0";

    }

    private void OnGUI()
    {
        if (!showGUI)
            return;

        GUILayout.BeginArea(new Rect(10 + offsetX, 40 + offsetY, 300, 9999));

        GUILayout.BeginHorizontal();
        GUILayout.Label("COM Port for Input Device:");
        comPort = GUILayout.TextField(comPort);
        if (GUILayout.Button("Apply"))
        {
            manager.SetComPort(comPort);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Subject ID:");
        subjectID = GUILayout.TextField(subjectID);
        manager.subjectID = subjectID;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Real Tumor Position:");
        tumorPosition = GUILayout.TextField(tumorPosition);
        float.TryParse(tumorPosition, out tumorManager.realObjectPosition);
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Label("Trial amount Override:");
        trialAmountOverride = GUILayout.TextField(trialAmountOverride);
        
        if (GUILayout.Button("Apply"))
        {
            int.TryParse(trialAmountOverride, out manager.totalTrials);
        }
        GUILayout.EndHorizontal();
        

        if (GUILayout.Button("Set ready"))
        {
            manager.state = StudyManager.State.Ready;
        }

        if (GUILayout.Button("Start trial"))
        {
            manager.StartTrial();
        }

        GUILayout.Label("Trial #"+manager.currentTrial+" of "+manager.totalTrials);
        GUILayout.Label("Status: " + manager.state);

        GUILayout.EndArea();
    }
}
