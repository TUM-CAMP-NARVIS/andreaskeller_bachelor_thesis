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
public struct TrackedObjectMessage : IMessageBase
{
    public enum Type {ViveTracker, ViveController, HMD };
    public short id;
    public Type type;
    public Vector3 position;
    public Quaternion rotation;


    public TrackedObjectMessage(short id, Type type, Vector3 position, Quaternion rotation)
    {
        this.id = id;
        this.type = type;
        this.position = position;
        this.rotation = rotation;
    }

    public void Deserialize(NetworkReader reader)
    {
        id = reader.ReadInt16();
        type = (Type) reader.ReadInt16();
        position = reader.ReadVector3();
        rotation = reader.ReadQuaternion();
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.WriteInt16(id);
        writer.WriteInt16((short)type);
        writer.WriteVector3(position);
        writer.WriteQuaternion(rotation);
    }
}

public class MultiplatformSceneManager : MonoBehaviour
{

    public GameObject zedStereoRig;
    public GameObject phantomAnchor;

    public GameObject prefab;

    private SynchronizationManager syncMan;

    private NetworkManager networkManager;

    private GameObject spawnedObject;

    // Start is called before the first frame update
    void Start()
    {
        syncMan = FindObjectOfType<SynchronizationManager>();

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
            //NetworkServer.Spawn(viveTracker);

            spawnedObject = viveTracker;
        }

        if (spawnedObject!=null)
        {
            var tracker = new TrackedObjectMessage();
            tracker.id = 0;
            tracker.type = TrackedObjectMessage.Type.ViveTracker;
            tracker.position = spawnedObject.transform.position;
            tracker.rotation = spawnedObject.transform.rotation;
            NetworkServer.SendToAll<TrackedObjectMessage>(tracker);
        }
        
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
        auth.username = "testHolo";
        connectToServer("192.168.1.116");
        
    }

    void setupIOS()
    {
        var cam = Camera.main.gameObject;

        if (cam.GetComponent<Vuforia.VuforiaBehaviour>()==null)
        {
            var vuf = cam.AddComponent<Vuforia.VuforiaBehaviour>();
        }
            
        var auth = FindObjectOfType<BasicAuthenticator>();
        auth.username = "testIOS";
        syncMan.UnhideObjects();
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
        NetworkClient.RegisterHandler<TrackedObjectMessage>(OnTrackerMessage);
    }

    private void OnTrackerMessage(NetworkConnection arg1, TrackedObjectMessage arg2)
    {
        syncMan.updateTrackedObject(arg2.id, arg2.type, arg2.position, arg2.rotation);
    }
   
}
