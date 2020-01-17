using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{

    private bool isAtStartup = true;

    private List<GameObject> networkedObjects = new List<GameObject>();

    NetworkClient myClient;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        if (isAtStartup)
        {
#if !UNITY_WSA
            SetupServer();
            SetupLocalClient();
#else
            SetupClient();
#endif
        }
        else
        {
#if !UNITY_WSA
            sendNetworkedObjects();
#endif
        }

#if UNITY_WSA
        
#endif
    }

    public void SetupServer()
    {
        NetworkServer.Listen(4444);
        NetworkServer.RegisterHandler(MsgType.Connect, OnConnectedServer);
        isAtStartup = false;
    }

    public void OnConnectedServer(NetworkMessage netMsg)
    {
        Debug.Log("Someone has connected to the server");
    }

    public void SetupLocalClient()
    {
        myClient = ClientScene.ConnectLocalServer();
        myClient.RegisterHandler(MsgType.Connect, OnConnected);
        isAtStartup = false;
    }

    public void OnConnected(NetworkMessage netMsg)
    {
        Debug.Log("Connected to server");
#if UNITY_WSA
        Debug.LogError("Connected to SERVER!!!!!!");
        myClient.RegisterHandler(MsgType.SyncEvent, OnRecSyncEvent);
#endif
    }

    public void SetupClient()
    {
        myClient = new NetworkClient();
        myClient.RegisterHandler(MsgType.Connect, OnConnected);
        Debug.Log("Connecting to server");
        myClient.Connect("192.168.1.116", 4444);
        isAtStartup = false;
    }

    public void OnRecSyncEvent(NetworkMessage netMsg)
    {
        TrackingMessage m = netMsg.ReadMessage<TrackingMessage>();
        positionNetworkedObjects(m.position, m.rotation);

    }

    public bool RegisterNetworkedObject(GameObject obj)
    {
        if (obj == null)
            return false;
        networkedObjects.Add(obj);
        return true;
    }

    public void positionNetworkedObjects(Vector3 position, Quaternion rotation)
    {
        GameObject g = networkedObjects[0];
        g.transform.position = position;
        g.transform.rotation = rotation;
    }

    public void sendNetworkedObjects()
    {
        GameObject g = networkedObjects[0];
        if (!g)
            return;

        TrackingMessage m = new TrackingMessage();
        //NetworkIdentity gId = g.GetComponent<NetworkIdentity>();
        m.netId = 1;
        m.assetID = new NetworkHash128();
        m.position = g.transform.position;
        m.rotation = g.transform.rotation;
        


        NetworkServer.SendToAll(MsgType.SyncEvent, m);
    }



}
