using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class SharedNetworkManager : MonoBehaviour
{

    private void Start()
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        SetupServer();
        SetupLocalClient();
#else
        SetupClient();
#endif
    }
    void Update()
    {

    }

    // Create a server and listen on a port  
    public void SetupServer()
    {
        NetworkServer.Listen(7777);
        //SpawnViveTracker();
    }

    // Create a client and connect to the server port  
    public void SetupClient()
    {
        NetworkClient.RegisterHandler<ConnectMessage>(OnConnected);
        NetworkClient.Connect("localhost");
    }

    // Create a local client and connect to the local server  
    public void SetupLocalClient()
    {
        NetworkClient.RegisterHandler<ConnectMessage>(OnConnected);
    }

    // client function
    public void OnConnected(NetworkConnection conn, ConnectMessage netMsg)
    {
        Debug.Log("Connected to server");
    }
    //server function
    public void OnConnectedServer(NetworkConnection conn, ConnectMessage netMsg)
    {
        Debug.Log("Client Connected");
    }

    public void SpawnViveTracker()
    {
        GameObject tracker = Instantiate (Resources.Load("Prefabs/NetworkedViveTracker") as GameObject);
        tracker.AddComponent<NetworkIdentity>();
        NetworkServer.Spawn(tracker);
    }
}
