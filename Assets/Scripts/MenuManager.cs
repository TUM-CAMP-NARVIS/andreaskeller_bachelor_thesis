using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    public GameObject networkingServer;
    public GameObject hatching;
    public GameObject bichlmeier;
    public GameObject networkingClient;
    public GameObject general;

    private PhantomManager pMan;
    // Start is called before the first frame update
    void Start()
    {
        pMan = FindObjectOfType<PhantomManager>();
    }

    public void HatchingSetActive(bool b)
    {
        if (!Utils.IsVR)
        {
            return;
        }
        hatching.SetActive(false);
    }
    public void BichlmeierSetActive(bool b)
    {
        if (!Utils.IsVR)
        {
            return;
        }
        bichlmeier.SetActive(false);
    }

    public void NetworkServerYesNo(bool b)
    {
        networkingServer.SetActive(false);
        networkingClient.SetActive(!b);
        var ipField = networkingClient.transform.parent.Find("ipaddress");
        if (ipField == null)
            return;
        if (b)
            ipField.GetComponent<TMPro.TMP_InputField>().text = "localhost";
        else
            ipField.GetComponent<TMPro.TMP_InputField>().text = "192.168.1.116";
    }

    public void HideAllButServer()
    {
        hatching.SetActive(false);
        bichlmeier.SetActive(false);
        general.SetActive(false);
    }

    public void ToggleSkin()
    {
        pMan.ToggleSkin();
    }
    public void ToggleWindow()
    {
        pMan.ToggleWindow();
    }
    public void CycleInsides()
    {
        pMan.CycleInsides();
    }

    public void InvertHatching()
    {
        pMan.ToggleHatching();
    }
    public void TriplanarHatching()
    {
        pMan.ToggleTriPlanar();
    }

    public void HideAll()
    {
        if (networkingClient != null)
        {
            networkingClient.SetActive(false);
        }
        if (hatching != null)
        {
            hatching.SetActive(false);
        }
        if (bichlmeier != null)
        {
            bichlmeier.SetActive(false);
        }
        if (networkingServer != null)
        {
            networkingServer.SetActive(false);
        }
        if (general != null)
        {
            general.SetActive(false);
        }
    }

}
