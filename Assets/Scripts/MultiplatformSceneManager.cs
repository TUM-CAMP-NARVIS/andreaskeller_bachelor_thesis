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


public class MultiplatformSceneManager : MonoBehaviour
{

    public GameObject zedStereoRig;
    public GameObject phantomAnchor;

    public GameObject prefab;

    //Manager objects in scene
    private SynchronizationManager syncMan;
    private MenuManager menuMan;
    private NetworkManager networkManager;
    private PhantomManager phantomManager;
    private NetworkDiscovery networkDiscovery;

    private GameObject spawnedObject;

    private bool m_bPhantomAttached = false;
    private bool m_bNetworkingEnabled = false;
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

		

    }

    // Update is called once per frame
    void Update()
    {
        counter++;
        if (!Utils.IsVR)
        {
            return;
        }
        

        if (spawnedObject!=null && m_bNetworkingEnabled)
        {
            var tracker = new TrackedObjectMessage();
            tracker.id = 0;
            tracker.type = TrackedObjectMessage.Type.ViveTracker;
            tracker.position = spawnedObject.transform.position;
            tracker.rotation = spawnedObject.transform.rotation;
            NetworkServer.SendToAll<TrackedObjectMessage>(tracker);
        }

        if (Input.GetKeyDown("n"))
        {
            m_bNetworkingEnabled = true;
        }
        if (Input.GetKeyDown("m"))
        {
            m_bNetworkingEnabled = false;
        }

        if (m_bNetworkingEnabled)
        {
            if (spawnedObject != null)
            {
                spawnedObject.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(0.5f, 1, 0.5f);
            }
        }
        else
        {
            if (spawnedObject != null)
            {
                spawnedObject.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(1f, 0.5f, 0.5f);
            }
        }

        if (Input.GetKeyDown("r"))
        {
            var update = phantomManager.GetFullUpdate();
            NetworkServer.SendToAll<SceneStateMessage>(update);
        }

        if (counter >= framesBetweenUpdates)
        {
            var update = phantomManager.GetFullUpdate();
            NetworkServer.SendToAll<SceneStateMessage>(update);
            counter = 0;
        }
        
    }

    #region PhantomViveTracker
    public void AttachPhantomToTracker()
    {
        phantomAnchor.transform.rotation = spawnedObject.transform.rotation;
        phantomAnchor.transform.position = spawnedObject.transform.position + (spawnedObject.transform.rotation * new Vector3(0.1f, -0.0074f, 0.12297f));
        phantomAnchor.transform.parent = spawnedObject.transform;
        
        m_bPhantomAttached = true;

    }

    public void DetachPhantomFromTracker()
    {
        phantomAnchor.transform.parent = phantomAnchor.transform.parent.parent;
        m_bPhantomAttached = false;
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

    public void ToggleNetworking()
    {
        m_bNetworkingEnabled = !m_bNetworkingEnabled;
    }

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
        
        

        if (phantomAnchor&&false)
        {
            var posedriver = phantomAnchor.AddComponent<TrackedPoseDriver>();
            posedriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRController, TrackedPoseDriver.TrackedPose.LeftPose);

        }

        //Start Server
        NetworkManager.singleton.StartServer();
        
        networkDiscovery.AdvertiseServer();

        //networkManager.StartServer();

        var viveTracker = Instantiate(prefab);

        var poseDriver = viveTracker.AddComponent<TrackedPoseDriver>();
        poseDriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRController, TrackedPoseDriver.TrackedPose.LeftPose);
        //NetworkServer.Spawn(viveTracker);

        spawnedObject = viveTracker;

    }

    public void connectToServer(string ip)
    {
        networkManager.networkAddress = ip;
        networkManager.StartClient();
        NetworkClient.RegisterHandler<TrackedObjectMessage>(OnTrackerMessage);
        NetworkClient.RegisterHandler<SceneStateMessage>(OnSceneStateMessage);
    }

    public void connectToServer(System.Uri uri)
    {
        networkManager.StartClient(uri);
        NetworkClient.RegisterHandler<TrackedObjectMessage>(OnTrackerMessage);
        NetworkClient.RegisterHandler<SceneStateMessage>(OnSceneStateMessage);
    }

    public void OnDiscoveredServer(ServerResponse info)
    {
        Debug.Log("Found Server at " + info.uri.ToString());
        if (NetworkClient.isConnected)
            return;
        connectToServer(info.uri);
    }

    /*
    public void connectToServerIP()
    {
        TMPro.TMP_InputField input;
        if (ipaddress == null)
        {
            ipaddress = GameObject.Find("ipaddress");
            if (ipaddress == null)
                return;
        }

        input = ipaddress.GetComponent<TMPro.TMP_InputField>();
        if (input == null)
            return;

        networkManager.networkAddress = input.text;
        networkManager.StartClient();
        NetworkClient.RegisterHandler<TrackedObjectMessage>(OnTrackerMessage);
    }
    */

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
   
}
