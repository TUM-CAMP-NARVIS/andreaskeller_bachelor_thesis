using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    public GameObject networkingServer;
    public GameObject hatching;
    public GameObject bichlmeier;
    public GameObject networkingClient;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void HatchingSetActive(bool b)
    {
        hatching.SetActive(b);
    }
    public void BichlmeierSetActive(bool b)
    {
        bichlmeier.SetActive(b);
    }

    public void NetworkServerYesNo(bool b)
    {
        networkingServer.SetActive(b);
        networkingClient.SetActive(!b);
    }
}
