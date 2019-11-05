using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuComponents : MonoBehaviour
{

    public FocusRegionUpdater focusUpdateScript;
    private GameObject panel;
    public GameObject phantom;
    private List<GameObject> menuPoints;

    // Start is called before the first frame update
    void Start()
    {
        panel = transform.GetChild(0).gameObject;
        menuPoints = new List<GameObject>();
        


        BuildMenu();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BuildMenu()
    {
        Debug.Log("Building Menu");

        //Title
        GameObject title = Instantiate(Resources.Load("Prefabs/Title")) as GameObject;
        menuPoints.Add(title);
        title.transform.SetParent(panel.transform, false);
        title.transform.localPosition = new Vector3(0, 40, 0);
        title.transform.GetChild(0).GetComponent<Text>().text = "Visualization Options";

        //Parameters
        for (int i = 0; i<1; i++)
        {
            Debug.Log("Function #" + i);
            GameObject param1 = Instantiate(Resources.Load("Prefabs/Functionality")) as GameObject;
            menuPoints.Add(param1);
            param1.transform.SetParent(panel.transform, false);
            param1.transform.localPosition = new Vector3(0, 20 - (20 * i), 0);
            param1.transform.GetChild(0).GetComponent<Text>().text = "Inside Sphere";
            param1.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate {
                if (focusUpdateScript.sphereActive)
                {
                    focusUpdateScript.EnableSphere(false);
                    param1.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Disabled";
                } else
                {
                    focusUpdateScript.EnableSphere(true);
                    param1.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Enabled";
                }
                
            });
            
        }
        if (phantom)
        {
            GameObject param1 = Instantiate(Resources.Load("Prefabs/Parameter")) as GameObject;
            menuPoints.Add(param1);
            param1.transform.SetParent(panel.transform, false);
            param1.transform.localPosition = new Vector3(0, 20 - (20 ), 0);
            param1.transform.GetChild(0).GetComponent<Text>().text = "Phantom Scale";
            var defValue = phantom.transform.localScale;
            var defValueX = defValue.x;
            param1.transform.GetChild(1).GetComponent<Text>().text = defValueX.ToString();
            param1.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate {
                var scale = phantom.transform.localScale;
                scale = new Vector3(scale.x+0.1f, scale.x + 0.1f, scale.x + 0.1f);
                param1.transform.GetChild(1).GetComponent<Text>().text = scale.x.ToString();
            });
            param1.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate {
                var scale = phantom.transform.localScale;
                scale = new Vector3(scale.x - 0.1f, scale.x - 0.1f, scale.x - 0.1f);
                param1.transform.GetChild(1).GetComponent<Text>().text = scale.x.ToString();
            });
            param1.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate {
                var scale = phantom.transform.localScale;
                scale = defValue;
                param1.transform.GetChild(1).GetComponent<Text>().text = scale.x.ToString();
            });
        }

    }
}
