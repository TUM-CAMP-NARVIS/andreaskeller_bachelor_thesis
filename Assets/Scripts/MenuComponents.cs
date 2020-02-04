using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuComponents : MonoBehaviour
{

    public FocusRegionUpdater focusUpdateScript;
    private GameObject panel;
    public GameObject phantom;
    public GameObject rootNode;
    private SurfaceAlign surfaceAlign;
    private GameObject focusObjectScaler;
    private List<GameObject> menuPoints;
    private ObjectPlacer objectPlacer;

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

        int position = 80;
        //Title
        GameObject title = Instantiate(Resources.Load("Prefabs/Menu/Title")) as GameObject;
        menuPoints.Add(title);
        title.transform.SetParent(panel.transform, false);
        title.transform.localPosition = new Vector3(0, position, 0);
        title.transform.GetChild(0).GetComponent<Text>().text = "Visualization Options";
        position -= 20;
        

        
        if (true)
        {
            GameObject param1 = Instantiate(Resources.Load("Prefabs/Menu/Functionality")) as GameObject;
            menuPoints.Add(param1);
            param1.transform.SetParent(panel.transform, false);
            param1.transform.localPosition = new Vector3(0,position, 0);
            param1.transform.GetChild(0).GetComponent<Text>().text = "Render Skin";
            param1.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate {
                if (phantom)
                {
                    phantom.GetComponent<PhantomManager>().ToggleSkin();
                    param1.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Toggle";
                }

            });
            position -= 20;
        }
        

        if (true)
        {
            GameObject param1 = Instantiate(Resources.Load("Prefabs/Menu/Functionality")) as GameObject;
            menuPoints.Add(param1);
            param1.transform.SetParent(panel.transform, false);
            param1.transform.localPosition = new Vector3(0, position, 0);
            param1.transform.GetChild(0).GetComponent<Text>().text = "Phantom Placing";
            param1.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Reset";
            param1.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate {
                objectPlacer.undoPlacing();
            });
            position -= 20;
        }

        if (false)
        {
            GameObject param1 = Instantiate(Resources.Load("Prefabs/Menu/Parameter")) as GameObject;
            menuPoints.Add(param1);
            var desc = param1.transform.GetChild(0);
            var value = param1.transform.GetChild(1);
            var plus = param1.transform.GetChild(2);
            var minus = param1.transform.GetChild(3);
            var reset = param1.transform.GetChild(4);
            param1.transform.SetParent(panel.transform, false);
            param1.transform.localPosition = new Vector3(0,position, 0);
            desc.GetComponent<Text>().text = "Phantom Scale";
            var defValue = phantom.transform.localScale;
            value.GetComponent<Text>().text = defValue.x.ToString();
            value.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 20);

            plus.transform.localPosition = new Vector3(plus.transform.localPosition.x + 10, plus.transform.localPosition.y, plus.transform.localPosition.z);
            minus.transform.localPosition = new Vector3(minus.transform.localPosition.x - 10, minus.transform.localPosition.y, minus.transform.localPosition.z);
            reset.transform.localPosition = new Vector3(reset.transform.localPosition.x + 10, reset.transform.localPosition.y, reset.transform.localPosition.z);

            plus.GetComponent<Button>().onClick.AddListener(delegate {
                var scale = phantom.transform.localScale;
                phantom.transform.localScale = new Vector3(scale.x + 0.1f, scale.x + 0.1f, scale.x + 0.1f);
                value.GetComponent<Text>().text = scale.x.ToString();
            });
            minus.GetComponent<Button>().onClick.AddListener(delegate {
                var scale = phantom.transform.localScale;
                phantom.transform.localScale = new Vector3(scale.x - 0.1f, scale.x - 0.1f, scale.x - 0.1f);
                value.GetComponent<Text>().text = scale.x.ToString();
            });
            reset.GetComponent<Button>().onClick.AddListener(delegate {
                var scale = phantom.transform.localScale;
                phantom.transform.localScale = defValue;
                param1.transform.GetChild(1).GetComponent<Text>().text = scale.x.ToString();
            });
            position -= 20;
        }

        

        /*
        if (phantom)
        {
            GameObject param1 = Instantiate(Resources.Load("Prefabs/Parameter")) as GameObject;
            menuPoints.Add(param1);
            var desc = param1.transform.GetChild(0);
            var value = param1.transform.GetChild(1);
            var plus = param1.transform.GetChild(2);
            var minus = param1.transform.GetChild(3);
            var reset = param1.transform.GetChild(4);
            param1.transform.SetParent(panel.transform, false);
            param1.transform.localPosition = new Vector3(0,position, 0);
            desc.GetComponent<Text>().text = "Inside Box Scale";
            var defValue = surfaceAlign.windowMesh.transform.localScale;
            value.GetComponent<Text>().text = (defValue.x/2.0f).ToString();
            value.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 20);

            plus.transform.localPosition = new Vector3(plus.transform.localPosition.x + 10, plus.transform.localPosition.y, plus.transform.localPosition.z);
            minus.transform.localPosition = new Vector3(minus.transform.localPosition.x - 10, minus.transform.localPosition.y, minus.transform.localPosition.z);
            reset.transform.localPosition = new Vector3(reset.transform.localPosition.x + 10, reset.transform.localPosition.y, reset.transform.localPosition.z);

            plus.GetComponent<Button>().onClick.AddListener(delegate {
                var scale = surfaceAlign.windowMesh.transform.localScale;
                surfaceAlign.windowMesh.transform.localScale = new Vector3(scale.x+0.2f, scale.y + 0.2f, scale.z+0.2f);
                value.GetComponent<Text>().text = (surfaceAlign.windowMesh.transform.localScale.x /2.0f).ToString();
            });
            minus.GetComponent<Button>().onClick.AddListener(delegate {
                var scale = surfaceAlign.windowMesh.transform.localScale;
                surfaceAlign.windowMesh.transform.localScale = new Vector3(scale.x - 0.2f, scale.y - 0.2f, scale.z - 0.2f);
                value.GetComponent<Text>().text = (surfaceAlign.windowMesh.transform.localScale.x/2.0f).ToString();
            });
            reset.GetComponent<Button>().onClick.AddListener(delegate {
                surfaceAlign.windowMesh.transform.localScale = defValue;
            });
        }
        */
    }
}
