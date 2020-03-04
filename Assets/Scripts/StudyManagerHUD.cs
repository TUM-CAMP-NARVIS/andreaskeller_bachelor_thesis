using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(StudyManager))]
public class StudyManagerHUD : MonoBehaviour
{
    StudyManager manager;
    MultiplatformSceneManager multiplatformSceneManager;
    WindowMaterialManager windowMaterialManager;
    TumorManager tumorManager;

    private string tumorPosition;

    public int offsetX, offsetY;

#if UNITY_WSA
    public bool showGUI = false;
#else
    public bool showGUI = true;
#endif

    private void Awake()
    {
        manager = GetComponent<StudyManager>();
        multiplatformSceneManager = FindObjectOfType<MultiplatformSceneManager>();
        windowMaterialManager = FindObjectOfType<WindowMaterialManager>();
        tumorManager = FindObjectOfType<TumorManager>();


        tumorPosition = "";

    }

    private void OnGUI()
    {
        if (!showGUI)
            return;

        GUILayout.BeginArea(new Rect(10 + offsetX, 40 + offsetY, 300, 9999));



        GUILayout.BeginHorizontal();
        GUILayout.Label("Real Tumor Position:");
        tumorPosition = GUILayout.TextField(tumorPosition);
        float.TryParse(tumorPosition, out tumorManager.realObjectPosition);
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
