using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class SharedNetworkManager : MonoBehaviour
{
    public bool isAtStartup = true;

    void Update()
    {
        if (isAtStartup)
        {
            if (Input.GetKeyDown(KeyCode.S))
                SetupServer();

            if (Input.GetKeyDown(KeyCode.C))
                SetupClient();

            if (Input.GetKeyDown(KeyCode.B))
            {
                SetupServer();
                SetupLocalClient();
            }
        }

    }

    // Create a server and listen on a port  
    public void SetupServer()
    {
        NetworkServer.Listen(7777);
        isAtStartup = false;
        SpawnViveTracker();
    }

    // Create a client and connect to the server port  
    public void SetupClient()
    {
        NetworkClient.RegisterHandler<ConnectMessage>(OnConnected);
        NetworkClient.Connect("localhost");
        isAtStartup = false;
    }

    // Create a local client and connect to the local server  
    public void SetupLocalClient()
    {
        NetworkClient.RegisterHandler<ConnectMessage>(OnConnected);
        isAtStartup = false;
    }

    // client function
    public void OnConnected(NetworkConnection conn, ConnectMessage netMsg)
    {
        Debug.Log("Connected to server");
    }

    public void SpawnViveTracker()
    {
        GameObject tracker = Instantiate (Resources.Load("Prefabs/NetworkedViveTracker") as GameObject);
        tracker.AddComponent<NetworkIdentity>();
        NetworkServer.Spawn(tracker);
    }
}
