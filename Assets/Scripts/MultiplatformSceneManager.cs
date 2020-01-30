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

public struct ViveTrackerMessage : IMessageBase
{
    public Vector3 position;
    public Quaternion rotation;

    public ViveTrackerMessage(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }

    public void Deserialize(NetworkReader reader)
    {
        position = reader.ReadVector3();
        rotation = reader.ReadQuaternion();
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.WriteVector3(position);
        writer.WriteQuaternion(rotation);
    }
}

public class MultiplatformSceneManager : MonoBehaviour
{
    private SceneStatus sceneStatus = SceneStatus.FirstLoad;

    public GameObject vuforiaCam;
    public GameObject zedStereoRig;
    private GameObject mainCamera;
    public GameObject phantomAnchor;
    public GameObject viveTracker;

    public GameObject prefab;

    private SurfaceAlign surfaceAlign;
    private FocusManager focusManager;
    private SynchronizationManager syncMan;

    private NetworkManager networkManager;

    private GameObject spawnedObject;

    private bool perf = true;
    enum SceneStatus {FirstLoad, Finished, WaitForAnchor, ManualAdjustment};
    // Start is called before the first frame update
    void Start()
    {
        surfaceAlign = FindObjectOfType<SurfaceAlign>();
        focusManager = FindObjectOfType<FocusManager>();
        syncMan = FindObjectOfType<SynchronizationManager>();
        mainCamera = Camera.main.gameObject;

        networkManager = FindObjectOfType<NetworkManager>();
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
    void Update()
    {
        if (!Utils.IsVR)
            return;
        if (Input.GetKeyDown("b"))
        {
            var viveTracker = Instantiate(prefab);

            var poseDriver = viveTracker.AddComponent<TrackedPoseDriver>();
            poseDriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRController, TrackedPoseDriver.TrackedPose.LeftPose);
            NetworkServer.Spawn(viveTracker);

            spawnedObject = viveTracker;
            //spawnedObjectNetID = viveTracker.GetComponent<NetworkIdentity>().netId;
        }

        if (spawnedObject!=null)
        {
            var tracker = new ViveTrackerMessage();
            tracker.position = spawnedObject.transform.position;
            tracker.rotation = spawnedObject.transform.rotation;
            NetworkServer.SendToAll<ViveTrackerMessage>(tracker);
        }
        
    }
	
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
            vuforiaCam.tag = "MainCamera";
            mainCamera.tag = "None";
            mainCamera.transform.parent = vuforiaCam.transform;
            mainCamera.GetComponent<Camera>().enabled = false;
        }

        connectToServer("192.168.1.116");
        
    }
	
	void setupZedMini()
	{
        if (zedStereoRig)
            zedStereoRig.SetActive(true);

        if (phantomAnchor&&false)
        {
            var posedriver = phantomAnchor.AddComponent<TrackedPoseDriver>();
            posedriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRController, TrackedPoseDriver.TrackedPose.LeftPose);

        }

        networkManager.StartServer();
        
	}

    void connectToServer(string ip)
    {
        networkManager.networkAddress = ip;
        networkManager.StartClient();
        NetworkClient.RegisterHandler<ViveTrackerMessage>(OnTrackerMessage);
        //NetworkClient.RegisterHandler<ConnectMessage>(OnConnected);
        //NetworkClient.RegisterHandler<DisconnectMessage>(OnDisconnected);
        //NetworkClient.RegisterHandler<SpawnMessage>(OnSpawnMessage, false);
        //NetworkClient.Connect(ip);
    }

    private void OnTrackerMessage(NetworkConnection arg1, ViveTrackerMessage arg2)
    {
        //phantomAnchor.transform.position = arg2.position;
        //phantomAnchor.transform.rotation = arg2.rotation;
        syncMan.updateViveTracker(arg2.position, arg2.rotation);
        
    }

    private void OnConnected(NetworkConnection arg1, ConnectMessage arg2)
    {
        Debug.Log("connected to server!");
    }
    private void OnDisconnected(NetworkConnection arg1, DisconnectMessage arg2)
    {
        Debug.Log("disconnected from server!");
    }


   
}
