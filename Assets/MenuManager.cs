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
        var ipField = networkingClient.transform.parent.Find("ipaddress");
        if (ipField == null)
            return;
        if (b)
            ipField.GetComponent<TMPro.TMP_InputField>().text = "localhost";
        else
            ipField.GetComponent<TMPro.TMP_InputField>().text = "192.168.1.116";
    }
}
