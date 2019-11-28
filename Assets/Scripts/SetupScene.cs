using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Experimental.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit;
using UnityEngine.XR.WSA;
using TMPro;

public class SetupScene : MonoBehaviour
{
    private SceneStatus sceneStatus = SceneStatus.FirstLoad;
    private WorldAnchorManager worldAnchorManager;
    private GameObject phantomAnchor;
    private GameObject phantom;
    private GameObject instrText;
    private bool perf = true;
    enum SceneStatus {FirstLoad, Finished, WaitForAnchor, ManualAdjustment};
    // Start is called before the first frame update
    void Start()
    {
        phantomAnchor = GameObject.Find("PhantomAnchor");
        phantom = GameObject.FindWithTag("Phantom");
        phantom.SetActive(false);
        //Check if there is a World Anchor already stored for the Phantom
        worldAnchorManager = this.GetComponent<WorldAnchorManager>();
        if (!worldAnchorManager)
            Debug.LogError("NO ANCHOR MANAGER");

        instrText = GameObject.FindGameObjectWithTag("StepDescription");

    }

    // Update is called once per frame
    void Update()
    {
        switch (sceneStatus)
        {
            case SceneStatus.FirstLoad:
                CheckAnchors();
                break;
            default:
                return;

        }
    }

    void CheckAnchors()
    {
        if (worldAnchorManager.AnchorStore.anchorCount != 0)
        {
            //Load anchor
            worldAnchorManager.AttachAnchor(phantom);
            sceneStatus = SceneStatus.Finished;
            if (instrText)
                instrText.SetActive(false);
            phantom.SetActive(true);
            phantom.GetComponent<PhantomManager>().SetManipulation(true);

        }
        else
        {
            GetComponent<SpatialMappingRenderer>().renderState = SpatialMappingRenderer.RenderState.Visualization;
            sceneStatus = SceneStatus.WaitForAnchor;
        }
    }

    void SetAnchor(Vector3 position, Vector3 nrm)
    {
        phantomAnchor.transform.position = position;
        phantomAnchor.transform.up = nrm;
        phantom.SetActive(true);
        GetComponent<SpatialMappingRenderer>().renderState = SpatialMappingRenderer.RenderState.None;
        
        if (instrText)
            instrText.GetComponent<TextMeshProUGUI>().text = "Adjust Position and Rotation";
        sceneStatus = SceneStatus.ManualAdjustment;
    }

    public void RegisterInput(MixedRealityPointerEventData d)
    {
        switch (sceneStatus)
        {
            case SceneStatus.WaitForAnchor:
                SetAnchor(d.Pointer.Result.Details.Point, d.Pointer.Result.Details.Normal);
                break;
            default:
                break;
        }
    }

    public void ToggleFineAdjustment()
    {
        if (sceneStatus == SceneStatus.Finished)
        {
            worldAnchorManager.RemoveAnchor(phantom);
            phantom.GetComponent<PhantomManager>().ToggleManipulation();
            sceneStatus = SceneStatus.ManualAdjustment;
        }
        else if (sceneStatus == SceneStatus.ManualAdjustment)
        {
            phantom.GetComponent<PhantomManager>().ToggleManipulation();
            worldAnchorManager.AttachAnchor(phantom);
            sceneStatus = SceneStatus.Finished;
            instrText.SetActive(false);
        }
    }

    public void RedoInitialPlacement()
    {
        worldAnchorManager.RemoveAllAnchors();
        phantom.SetActive(false);
        GetComponent<SpatialMappingRenderer>().renderState = SpatialMappingRenderer.RenderState.Visualization;
        sceneStatus = SceneStatus.WaitForAnchor;
        instrText.SetActive(true);
        instrText.GetComponent<TextMeshProUGUI>().text = "Select Phantom Location";

    }

    public void ToggleVisualProfiler()
    {
        return;
        if (perf)
        {
            var mrtkScene = FindObjectOfType<MixedRealityToolkit>();
            mrtkScene.ActiveProfile = (MixedRealityToolkitConfigurationProfile)Resources.Load("CustomProfiles/MRTKConfigProfile_NoDiag", typeof(MixedRealityToolkitConfigurationProfile));
        }
        else
        {
            var mrtkScene = FindObjectOfType<MixedRealityToolkit>();
            mrtkScene.ActiveProfile = (MixedRealityToolkitConfigurationProfile)Resources.Load("CustomProfiles/MRTKConfigProfile", typeof(MixedRealityToolkitConfigurationProfile));
        }
        
    }
}
