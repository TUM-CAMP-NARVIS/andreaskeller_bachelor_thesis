using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkStatusUI : MonoBehaviour
{
    public GameObject ipAddressText;
    public GameObject connectionStatusLight;
    private NetworkManager netMan;
    // Start is called before the first frame update
    void Start()
    {
        if (Utils.IsVR)
            return;

        netMan = FindObjectOfType<NetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Utils.IsVR)
            return;

        if (NetworkClient.isConnected)
        {
            connectionStatusLight.SetActive(false);
            ipAddressText.SetActive(false);
        }
        else
        {
            connectionStatusLight.SetActive(true);
            ipAddressText.SetActive(true);

            if (connectionStatusLight != null)
            {
                if (NetworkClient.isConnected)
                    connectionStatusLight.GetComponent<MeshRenderer>().material.color = new Color(0, 1, 0);
                else
                    connectionStatusLight.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0);
            }
            if (ipAddressText != null)
            {
                var tmpro = ipAddressText.GetComponent<TMPro.TextMeshPro>();
                if (tmpro != null)
                    tmpro.text = netMan.networkAddress;

            }



        }

        

    }
}
