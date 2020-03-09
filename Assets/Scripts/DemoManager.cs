//This controls the scene and enables changes to the visuals at runtime
//
//
//
//
//
using UnityEngine;
using UnityEngine.SpatialTracking;
using XRTK.SDK.Input;
using System.Collections.Generic;


public class DemoManager : MonoBehaviour
{

    public GameObject phantomAnchor;
    public GameObject vuforiaMarker;

    //Manager objects in scene
    private MenuManager menuMan;
    private PhantomManager phantomManager;
    private GazeProvider gazeProvider;

    private List<VisualizationMethod> visualizationMethods;


    // Start is called before the first frame update
    void Start()
    {
        phantomAnchor.SetActive(true);

        menuMan = FindObjectOfType<MenuManager>();
        phantomManager = FindObjectOfType<PhantomManager>();
        gazeProvider = FindObjectOfType<GazeProvider>();

        if (Utils.IsHoloLens)
        {
            setupHololens();
        }

        visualizationMethods = new List<VisualizationMethod>();
        //visualizationMethods.Add(new VisualizationMethod())
        phantomManager.ApplyDemoVis();
    }

    // Update is called once per frame
    void Update()
    {
        if (gazeProvider.GazeTarget == phantomManager.skin)
        {
            gazeProvider.GazeCursor.SetVisibility(false);
        }
        else
        {
            gazeProvider.GazeCursor.SetVisibility(true);
        }
    }


    void setupHololens()
    {
        var cam = Camera.main.gameObject;

        if (cam.GetComponent<Vuforia.VuforiaBehaviour>() == null)
        {
            var vuf = cam.AddComponent<Vuforia.VuforiaBehaviour>();
        }

        menuMan.HideAll();
    }

    public void MoveToMarker()
    {
        phantomAnchor.transform.position = vuforiaMarker.transform.position;
        phantomAnchor.transform.rotation = vuforiaMarker.transform.rotation;
    }

    public void CycleVisualizations()
    {
        phantomManager.CycleDemoVis();
    }

}
