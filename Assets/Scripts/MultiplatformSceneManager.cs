﻿//This controls the scene and enables changes to the visuals at runtime
//
//
//
//
//
using UnityEngine;
using UnityEngine.SpatialTracking;
using Mirror;
using Mirror.Authenticators;
using Mirror.Discovery;
using XRTK.SDK.Input;


public class MultiplatformSceneManager : MonoBehaviour
{

    public GameObject zedStereoRig;
    public GameObject phantomAnchor;
    public GameObject syncIndicator;

    public GameObject prefab;

    //Manager objects in scene
    private SynchronizationManager syncMan;
    private MenuManager menuMan;
    private NetworkManager networkManager;
    private PhantomManager phantomManager;
    private NetworkDiscovery networkDiscovery;

    private TumorManager tumorManager;

    private GameObject spawnedObject;

    private bool m_bPhantomAttached = false;
    private int counter = 0;
    public int framesBetweenUpdates = 60;

    // Start is called before the first frame update
    void Start()
    {
        phantomAnchor.SetActive(true);

        syncMan = FindObjectOfType<SynchronizationManager>();
        menuMan = FindObjectOfType<MenuManager>();
        networkManager = FindObjectOfType<NetworkManager>();
        phantomManager = FindObjectOfType<PhantomManager>();
        tumorManager = FindObjectOfType<TumorManager>();

        networkDiscovery = FindObjectOfType<NetworkDiscovery>();
        networkDiscovery.secretHandshake = 1234567890;

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

        GazeProvider gazeProvider = FindObjectOfType<GazeProvider>();
        gazeProvider.GazeCursor.SetVisibility(false);

    }

    // Update is called once per frame
    void Update()
    {
        counter++;
        if (!Utils.IsVR)
        {
            return;
        }
        

        //Networked Vive Tracker Position
        if (spawnedObject!=null)
        {
            var trackedObjectMessage = new TrackedObjectMessage();
            trackedObjectMessage.id = 0;
            trackedObjectMessage.type = TrackedObjectMessage.Type.ViveTracker;
            trackedObjectMessage.position = spawnedObject.transform.position;
            trackedObjectMessage.rotation = spawnedObject.transform.rotation;
            NetworkServer.SendToAll<TrackedObjectMessage>(trackedObjectMessage);
        }

        //Tumor position
        if (tumorManager != null)
        {
            var tumorPositionMessage = new TumorPositionMessage(tumorManager.transform.localPosition);
            NetworkServer.SendToAll<TumorPositionMessage>(tumorPositionMessage);
        }

        if (syncIndicator != null)
        {
            syncIndicator.GetComponent<Renderer>().material.color = new Color(0.5f, 1, 0.5f);
        }

        //Scene State update - deactived for study
        /*
        if (counter >= framesBetweenUpdates)
        {
            var update = phantomManager.GetFullUpdate();
            NetworkServer.SendToAll<SceneStateMessage>(update);
            counter = 0;
        }
        */

        if (Input.GetKeyDown("c"))
        {
            DisableCursor();
        }
        if (Input.GetKeyDown("v"))
        {
            EnableCursor();
        }


    }

    #region PhantomViveTracker
    public void AttachPhantomToTracker()
    {
        phantomAnchor.transform.rotation = spawnedObject.transform.rotation * Quaternion.Euler(90,0,0);
        phantomAnchor.transform.position = spawnedObject.transform.position + (spawnedObject.transform.rotation * new Vector3(0f, 0.1225f, -0.063969f));
        phantomAnchor.transform.parent = spawnedObject.transform;
        
        m_bPhantomAttached = true;

    }

    public void DetachPhantomFromTracker()
    {
        phantomAnchor.transform.SetParent(null, true);
        m_bPhantomAttached = false;
    }

    public void ToggleCursor()
    {
        if (FindObjectOfType<GazeProvider>().GazeCursor.IsVisible)
        {
            DisableCursor();
        }
        else
        {
            EnableCursor();
        }
    }

    public void EnableCursor()
    {
        var inputMessage = new InputVisualizationMessage();
        inputMessage.enableHeadCursor = true;
        NetworkServer.SendToAll<InputVisualizationMessage>(inputMessage);

        GazeProvider gazeProvider = FindObjectOfType<GazeProvider>();
        gazeProvider.GazeCursor.SetVisibility(true);
    }

    public void DisableCursor()
    {
        var inputMessage = new InputVisualizationMessage();
        inputMessage.enableHeadCursor = false;
        NetworkServer.SendToAll<InputVisualizationMessage>(inputMessage);

        GazeProvider gazeProvider = FindObjectOfType<GazeProvider>();
        gazeProvider.GazeCursor.SetVisibility(false);
    }

    public void TogglePhantomAttached(GameObject cube = null)
    {
        if (m_bPhantomAttached)
            DetachPhantomFromTracker();
        else
            AttachPhantomToTracker();

        if (cube != null)
        {
            if (m_bPhantomAttached)
                cube.GetComponent<MeshRenderer>().material.color = new Color(0, 1, 0);
            else
                cube.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0);
        }
    }

    #endregion

    public void ResetConnections(GameObject button)
    {
        var color = button.GetComponent<MeshRenderer>().material.color;
        button.GetComponent<MeshRenderer>().material.color = new Color(color.r, color.b, color.g);


        networkManager.StopServer();
        networkManager.StartServer();
    }
	
	void setupHololens()
	{
        var cam = Camera.main.gameObject;

        if (cam.GetComponent<Vuforia.VuforiaBehaviour>() == null)
        {
            var vuf = cam.AddComponent<Vuforia.VuforiaBehaviour>();
        }

        syncMan.UnhideObjects();
        var auth = FindObjectOfType<BasicAuthenticator>();
        auth.username = SystemInfo.deviceUniqueIdentifier;
        menuMan.NetworkServerYesNo(false);
        menuMan.HideAllButServer();
        spawnedObject = syncMan.viveTracker;
        AttachPhantomToTracker();
        FindObjectOfType<NetworkDiscovery>().StartDiscovery();

    }

    void setupIOS()
    {
        var cam = Camera.main.gameObject;

        if (cam.GetComponent<Vuforia.VuforiaBehaviour>()==null)
        {
            var vuf = cam.AddComponent<Vuforia.VuforiaBehaviour>();
        }
            
        var auth = FindObjectOfType<BasicAuthenticator>();
        auth.username = SystemInfo.deviceUniqueIdentifier;
        syncMan.UnhideObjects();
        menuMan.NetworkServerYesNo(false);
        AttachPhantomToTracker();
        spawnedObject = syncMan.viveTracker;


        var networkDiscovery = FindObjectOfType<NetworkDiscovery>();
        networkDiscovery.StartDiscovery();
#if UNITY_EDITOR
        UnityEditor.Events.UnityEventTools.AddPersistentListener(networkDiscovery.OnServerFound, OnDiscoveredServer);
#endif
    }
	
	void setupZedMini()
	{
        if (zedStereoRig)
        {
            zedStereoRig.SetActive(true);
            zedStereoRig.transform.GetChild(0).GetChild(0).gameObject.tag = "MainCamera";
        }

        menuMan.NetworkServerYesNo(true);
        var cam = Camera.main.gameObject;
        cam.GetComponent<Camera>().enabled = false;

        //Start Server
        NetworkManager.singleton.StartServer();
        
        networkDiscovery.AdvertiseServer();
        var studyMan = FindObjectOfType<StudyManager>();
        NetworkServer.RegisterHandler<HoloLensPositionMessage>(studyMan.OnHoloLensPositionMessage);

        //networkManager.StartServer();

        var viveTracker = Instantiate(prefab);

        var poseDriver = viveTracker.AddComponent<TrackedPoseDriver>();
        poseDriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRController, TrackedPoseDriver.TrackedPose.LeftPose);
        //NetworkServer.Spawn(viveTracker);

        spawnedObject = viveTracker;
        
        AttachPhantomToTracker();

    }

    public void connectToServer(string ip)
    {
        networkManager.networkAddress = ip;
        networkManager.StartClient();
        NetworkClient.RegisterHandler<TrackedObjectMessage>(OnTrackerMessage);
        NetworkClient.RegisterHandler<SceneStateMessage>(OnSceneStateMessage);
        NetworkClient.RegisterHandler<InputVisualizationMessage>(OnInputVisualizationMessage);
        NetworkClient.RegisterHandler<TumorPositionMessage>(OnTumorPositionMessage);
        var studyMan = FindObjectOfType<StudyManager>();
        NetworkClient.RegisterHandler<VisualizationMethodMessage>(studyMan.ApplyVisMethodMessage);
        NetworkClient.RegisterHandler<StudyManagerMessage>(studyMan.ApplyStudyManagerMessage);
    }

    public void connectToServer(System.Uri uri)
    {
        networkManager.StartClient(uri);
        NetworkClient.RegisterHandler<TrackedObjectMessage>(OnTrackerMessage);
        NetworkClient.RegisterHandler<SceneStateMessage>(OnSceneStateMessage);
        NetworkClient.RegisterHandler<InputVisualizationMessage>(OnInputVisualizationMessage);
        NetworkClient.RegisterHandler<TumorPositionMessage>(OnTumorPositionMessage);
        var studyMan = FindObjectOfType<StudyManager>();
        NetworkClient.RegisterHandler<VisualizationMethodMessage>(studyMan.ApplyVisMethodMessage);
        NetworkClient.RegisterHandler<StudyManagerMessage>(studyMan.ApplyStudyManagerMessage);
    }

    public void OnDiscoveredServer(ServerResponse info)
    {
        Debug.Log("Found Server at " + info.uri.ToString());
        if (NetworkClient.isConnected)
            return;
        connectToServer(info.uri);
    }

    public void DisconnectFromServer()
    {
        NetworkClient.Disconnect();
    }

    private void OnTrackerMessage(NetworkConnection arg1, TrackedObjectMessage arg2)
    {
        syncMan.updateTrackedObject(arg2.id, arg2.type, arg2.position, arg2.rotation);
    }

    private void OnSceneStateMessage(NetworkConnection arg1, SceneStateMessage arg2)
    {
        phantomManager.applyUpdate(arg2);
    }

    private void OnInputVisualizationMessage(NetworkConnection arg1, InputVisualizationMessage arg2)
    {
        GazeProvider gazeProvider = FindObjectOfType<GazeProvider>();
        gazeProvider.GazeCursor.SetVisibility(arg2.enableHeadCursor);
    }

    private void OnTumorPositionMessage(NetworkConnection arg1, TumorPositionMessage arg2)
    {
        if (tumorManager != null)
        {
            tumorManager.RepositionTumor(arg2.position);
        }
        //move tumor to position
    }
   
}
