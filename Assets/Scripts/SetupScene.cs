using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Experimental.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.XR.WSA;
using TMPro;

public class SetupScene : MonoBehaviour
{
    private SceneStatus sceneStatus = SceneStatus.FirstLoad;
    private WorldAnchorManager worldAnchorManager;
    private GameObject phantomAnchor;
    private GameObject phantom;
    private GameObject finishButton;
    enum SceneStatus {FirstLoad, Finished, WaitForAnchor, ManualAdjustment};
    // Start is called before the first frame update
    void Start()
    {
        phantomAnchor = GameObject.Find("PhantomAnchor");
        finishButton = GameObject.Find("FinishButton");
        finishButton.SetActive(false);
        phantom = GameObject.FindWithTag("Phantom");
        phantom.SetActive(false);
        //Check if there is a World Anchor already stored for the Phantom
        worldAnchorManager = this.GetComponent<WorldAnchorManager>();
        if (!worldAnchorManager)
            Debug.LogError("NO ANCHOR MANAGER");
        
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
            Debug.Log("AnchorExists!!!!");
            worldAnchorManager.AttachAnchor(phantom);
            sceneStatus = SceneStatus.Finished;
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
        GameObject instrText = GameObject.FindGameObjectWithTag("StepDescription");
        if (instrText)
            instrText.GetComponent<TextMeshProUGUI>().text = "Adjust Position and Rotation";
        sceneStatus = SceneStatus.ManualAdjustment;
        phantom.transform.Find("skin").GetComponent<Renderer>().sharedMaterial = (Material)Resources.Load("Materials/brightskin", typeof(Material));;
        finishButton.SetActive(true);
    }

    public void FinishManual()
    {
        phantom.GetComponent<BoundingBox>().enabled = false;
        phantom.GetComponent<BoxCollider>().enabled = false;
        phantom.GetComponent<ManipulationHandler>().enabled = false;
        finishButton.SetActive(false);
        phantom.transform.Find("skin").GetComponent<Renderer>().sharedMaterial = (Material)Resources.Load("Materials/Skin", typeof(Material)); ;
        sceneStatus = SceneStatus.Finished;
        worldAnchorManager.AttachAnchor(phantom);
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
}
