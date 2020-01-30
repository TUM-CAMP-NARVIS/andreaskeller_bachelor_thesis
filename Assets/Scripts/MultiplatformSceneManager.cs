//This controls the scene and enables changes to the visuals at runtime
//
//
//
//
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
//using Microsoft.MixedReality.Toolkit.Experimental.Utilities;
//using Microsoft.MixedReality.Toolkit.Input;
//using Microsoft.MixedReality.Toolkit.UI;
//using Microsoft.MixedReality.Toolkit;
using Mirror;
using UnityEngine.XR.WSA;
using TMPro;
using System;

public class MultiplatformSceneManager : MonoBehaviour
{
    private SceneStatus sceneStatus = SceneStatus.FirstLoad;

    public GameObject vuforiaCam;
    public GameObject zedStereoRig;
    public GameObject phantomAnchor;

    public GameObject prefab;

    private SurfaceAlign surfaceAlign;
    private FocusManager focusManager;
    private PhantomManager phantomManager;
    private bool perf = true;
    enum SceneStatus {FirstLoad, Finished, WaitForAnchor, ManualAdjustment};
    // Start is called before the first frame update
    void Start()
    {
        surfaceAlign = FindObjectOfType<SurfaceAlign>();
        focusManager = FindObjectOfType<FocusManager>();
        phantomManager = FindObjectOfType<PhantomManager>();

        if (Utils.IsHoloLens)
		{
			setupHololens();
		}
        else if (Utils.IsIOS)
		{
			setupIOS();
		}
		else
		{
			setupZedMini();
		}

		

    }

    // Update is called once per frame
    //void Update()
    //{
	//	
    //}
	//
	void setupHololens()
	{
        var networkManager = FindObjectOfType<NetworkManager>();

        connectToServer("192.168.1.116");

    }

    void setupIOS()
    {
        if (vuforiaCam)
        {
            vuforiaCam.SetActive(true);
            focusManager.cam = vuforiaCam;
        }

        connectToServer("192.168.1.116");
        
    }
	
	void setupZedMini()
	{
        if (zedStereoRig)
            zedStereoRig.SetActive(true);

        if (phantomAnchor)
        {
            var posedriver = phantomAnchor.AddComponent<TrackedPoseDriver>();
            posedriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRController, TrackedPoseDriver.TrackedPose.LeftPose);

        }

        //Networking
        var networkManager = FindObjectOfType<NetworkManager>();
        networkManager.StartServer();

        var viveTracker = Instantiate(prefab);
        NetworkServer.Spawn(viveTracker);
        connectToServer("localhost");
        
	}

    void connectToServer(string ip)
    {
        NetworkClient.RegisterHandler<ConnectMessage>(OnConnected,false);
        NetworkClient.RegisterHandler<DisconnectMessage>(OnDisconnected,false);
        NetworkClient.Connect(ip);
    }

    private void OnConnected(NetworkConnection arg1, ConnectMessage arg2)
    {
        Debug.Log("connected to server!");
    }
    private void OnDisconnected(NetworkConnection arg1, DisconnectMessage arg2)
    {
        Debug.Log("disconnected from server!");
    }


    //void CheckAnchors()
    //{
    //    if (false)//worldAnchorManager.AnchorStore.anchorCount != 0)
    //    {
    //        //Load anchor
    //        //worldAnchorManager.AttachAnchor(phantom);
    //        sceneStatus = SceneStatus.Finished;
    //        if (instrText)
    //            instrText.SetActive(false);
    //        phantom.SetActive(true);
    //        //phantom.GetComponent<PhantomManager>().SetManipulation(false);
    //        //GetComponent<SpatialMappingCollider>().layer = 2;
    //
    //
    //
    //    }
    //    else
    //    {
    //        //GetComponent<SpatialMappingRenderer>().renderState = SpatialMappingRenderer.RenderState.Visualization;
    //        sceneStatus = SceneStatus.WaitForAnchor;
    //    }
    //}
    //
    //void SetAnchor(Vector3 position, Vector3 nrm)
    //{
    //    phantomAnchor.transform.position = position;
    //    phantomAnchor.transform.rotation = Quaternion.LookRotation(new Vector3(0, 1, 0), nrm);
    //    phantom.SetActive(true);
    //    //GetComponent<SpatialMappingRenderer>().renderState = SpatialMappingRenderer.RenderState.None;
    //    //phantom.GetComponent<PhantomManager>().SetManipulation(true);
    //    //GetComponent<SpatialMappingCollider>().layer = 2;
    //    if (instrText)
    //        instrText.GetComponent<TextMeshProUGUI>().text = "Adjust Position and Rotation";
    //    sceneStatus = SceneStatus.ManualAdjustment;
    //}

    //public void RegisterInput(MixedRealityPointerEventData d)
    //{
    //    switch (sceneStatus)
    //    {
    //        case SceneStatus.WaitForAnchor:
    //            SetAnchor(d.Pointer.Result.Details.Point, d.Pointer.Result.Details.Normal);
    //            break;
    //        default:
    //            break;
    //    }
    //}

    //public void ToggleFineAdjustment()
    //{
    //    if (sceneStatus == SceneStatus.Finished)
    //    {
    //        worldAnchorManager.RemoveAnchor(phantom);
    //        phantom.GetComponent<PhantomManager>().SetManipulation(true);
    //        sceneStatus = SceneStatus.ManualAdjustment;
    //    }
    //    else if (sceneStatus == SceneStatus.ManualAdjustment)
    //    {
    //        phantom.GetComponent<PhantomManager>().SetManipulation(false);
    //        worldAnchorManager.AttachAnchor(phantom);
    //        sceneStatus = SceneStatus.Finished;
    //        instrText.SetActive(false);
    //    }
    //}

    //public void RedoInitialPlacement()
    //{
    //    worldAnchorManager.RemoveAnchor(phantom);
    //    phantom.SetActive(false);
    //    GetComponent<SpatialMappingRenderer>().renderState = SpatialMappingRenderer.RenderState.Visualization;
    //    GetComponent<SpatialMappingCollider>().layer = 31;
    //    sceneStatus = SceneStatus.WaitForAnchor;
    //    instrText.SetActive(true);
    //    instrText.GetComponent<TextMeshProUGUI>().text = "Select Phantom Location";
    //
    //}

    //public void ToggleVisualProfiler()
    //{
    //    return;
    //    if (perf)
    //    {
    //        var mrtkScene = FindObjectOfType<MixedRealityToolkit>();
    //        mrtkScene.ActiveProfile = (MixedRealityToolkitConfigurationProfile)Resources.Load("CustomProfiles/MRTKConfigProfile_NoDiag", typeof(MixedRealityToolkitConfigurationProfile));
    //    }
    //    else
    //    {
    //        var mrtkScene = FindObjectOfType<MixedRealityToolkit>();
    //        mrtkScene.ActiveProfile = (MixedRealityToolkitConfigurationProfile)Resources.Load("CustomProfiles/MRTKConfigProfile", typeof(MixedRealityToolkitConfigurationProfile));
    //    }
    //    
    //}
}
