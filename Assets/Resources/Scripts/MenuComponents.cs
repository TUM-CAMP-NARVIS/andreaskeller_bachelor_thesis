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
    private GameObject windowObject;
    private List<GameObject> menuPoints;

    // Start is called before the first frame update
    void Start()
    {
        panel = transform.GetChild(0).gameObject;
        menuPoints = new List<GameObject>();
        surfaceAlign = FindObjectOfType<SurfaceAlign>();
        windowObject = surfaceAlign.windowMesh;


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
        if (true)
        {
            Debug.Log("Function #" + 1);
            GameObject param1 = Instantiate(Resources.Load("Prefabs/Functionality")) as GameObject;
            menuPoints.Add(param1);
            param1.transform.SetParent(panel.transform, false);
            param1.transform.localPosition = new Vector3(0, 20 - (20 * 2), 0);
            param1.transform.GetChild(0).GetComponent<Text>().text = "Render Skin";
            param1.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate {
                if (focusUpdateScript.skinActive)
                {
                    focusUpdateScript.EnableSkin(false);
                    param1.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Disabled";
                }
                else
                {
                    focusUpdateScript.EnableSkin(true);
                    param1.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Enabled";
                }

            });
        }

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
            param1.transform.localPosition = new Vector3(0, 20 - (20), 0);
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
        }

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
            param1.transform.localPosition = new Vector3(0, 20 - (60), 0);
            desc.GetComponent<Text>().text = "rootNode Rotation XZ";
            var defValue = rootNode.transform.rotation;
            value.GetComponent<Text>().text = defValue.x.ToString();
            value.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 20);

            plus.transform.localPosition = new Vector3(plus.transform.localPosition.x + 10, plus.transform.localPosition.y, plus.transform.localPosition.z);
            minus.transform.localPosition = new Vector3(minus.transform.localPosition.x - 10, minus.transform.localPosition.y, minus.transform.localPosition.z);
            reset.transform.localPosition = new Vector3(reset.transform.localPosition.x + 10, reset.transform.localPosition.y, reset.transform.localPosition.z);

            plus.GetComponent<Button>().onClick.AddListener(delegate {
                var pos = rootNode.transform.rotation;
                rootNode.transform.Rotate(45, 0, 0);
                value.GetComponent<Text>().text = pos.x.ToString();
            });
            minus.GetComponent<Button>().onClick.AddListener(delegate {
                var pos = rootNode.transform.rotation;
                rootNode.transform.Rotate(0,45,0);
                value.GetComponent<Text>().text = pos.x.ToString();
            });
            reset.GetComponent<Button>().onClick.AddListener(delegate {
                var pos = rootNode.transform.rotation;
                rootNode.transform.Rotate(0, 0, 45);
                param1.transform.GetChild(1).GetComponent<Text>().text = pos.x.ToString();
            });
        }

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
            param1.transform.localPosition = new Vector3(0, 20 - (80), 0);
            desc.GetComponent<Text>().text = "rootNode Pos y";
            var defValue = rootNode.transform.rotation;
            value.GetComponent<Text>().text = defValue.x.ToString();
            value.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 20);

            plus.transform.localPosition = new Vector3(plus.transform.localPosition.x + 10, plus.transform.localPosition.y, plus.transform.localPosition.z);
            minus.transform.localPosition = new Vector3(minus.transform.localPosition.x - 10, minus.transform.localPosition.y, minus.transform.localPosition.z);
            reset.transform.localPosition = new Vector3(reset.transform.localPosition.x + 10, reset.transform.localPosition.y, reset.transform.localPosition.z);

            plus.GetComponent<Button>().onClick.AddListener(delegate {
                var pos = rootNode.transform.position;
                rootNode.transform.position = new Vector3(pos.x, pos.y+0.1f, pos.z);
                value.GetComponent<Text>().text = pos.y.ToString();
            });
            minus.GetComponent<Button>().onClick.AddListener(delegate {
                var pos = rootNode.transform.position;
                rootNode.transform.position = new Vector3(pos.x, pos.y - 0.1f, pos.z);
                value.GetComponent<Text>().text = pos.y.ToString();
            });
            reset.GetComponent<Button>().onClick.AddListener(delegate {
               
            });
        }

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
            param1.transform.localPosition = new Vector3(0, 20 - (100), 0);
            desc.GetComponent<Text>().text = "Inside Box Scale";
            var defValue = windowObject.transform.localScale;
            value.GetComponent<Text>().text = defValue.x.ToString();
            value.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 20);

            plus.transform.localPosition = new Vector3(plus.transform.localPosition.x + 10, plus.transform.localPosition.y, plus.transform.localPosition.z);
            minus.transform.localPosition = new Vector3(minus.transform.localPosition.x - 10, minus.transform.localPosition.y, minus.transform.localPosition.z);
            reset.transform.localPosition = new Vector3(reset.transform.localPosition.x + 10, reset.transform.localPosition.y, reset.transform.localPosition.z);

            plus.GetComponent<Button>().onClick.AddListener(delegate {
                var scale = windowObject.transform.localScale;
                windowObject.transform.localScale = new Vector3(scale.x+0.1f, scale.y + 0.1f, scale.z+0.1f);
                value.GetComponent<Text>().text = windowObject.transform.localScale.x.ToString();
            });
            minus.GetComponent<Button>().onClick.AddListener(delegate {
                var scale = windowObject.transform.localScale;
                windowObject.transform.localScale = new Vector3(scale.x - 0.1f, scale.y - 0.1f, scale.z - 0.1f);
                value.GetComponent<Text>().text = windowObject.transform.localScale.x.ToString();
            });
            reset.GetComponent<Button>().onClick.AddListener(delegate {
                windowObject.transform.localScale = defValue;
            });
        }


        /*s
            param1 = Instantiate(Resources.Load("Prefabs/Parameter")) as GameObject;
            menuPoints.Add(param1);
            desc = param1.transform.GetChild(0);
            value = param1.transform.GetChild(1);
            plus = param1.transform.GetChild(2);
            minus = param1.transform.GetChild(3);
            reset = param1.transform.GetChild(4);
            param1.transform.SetParent(panel.transform, false);
            param1.transform.localPosition = new Vector3(0, 20 - (60), 0);
            desc.GetComponent<Text>().text = "Phantom Position Y";
            defValue = phantom.transform.localPosition;
            value.GetComponent<Text>().text = defValue.x.ToString();
            value.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 20);

            plus.transform.localPosition = new Vector3(plus.transform.localPosition.x + 10, plus.transform.localPosition.y, plus.transform.localPosition.z);
            minus.transform.localPosition = new Vector3(minus.transform.localPosition.x - 10, minus.transform.localPosition.y, minus.transform.localPosition.z);
            reset.transform.localPosition = new Vector3(reset.transform.localPosition.x + 10, reset.transform.localPosition.y, reset.transform.localPosition.z);

            plus.GetComponent<Button>().onClick.AddListener(delegate {
                var pos = phantom.transform.localPosition;
                phantom.transform.localPosition = new Vector3(pos.x, pos.y + 0.1f, pos.z);
                value.GetComponent<Text>().text = pos.x.ToString();
            });
            minus.GetComponent<Button>().onClick.AddListener(delegate {
                var pos = phantom.transform.localPosition;
                phantom.transform.localPosition = new Vector3(pos.x, pos.y - 0.1f, pos.z);
                value.GetComponent<Text>().text = pos.x.ToString();
            });
            reset.GetComponent<Button>().onClick.AddListener(delegate {
                var pos = phantom.transform.localPosition;
                phantom.transform.localPosition = new Vector3(pos.x, defValue.y, pos.z);
                param1.transform.GetChild(1).GetComponent<Text>().text = pos.y.ToString();
            });


            param1 = Instantiate(Resources.Load("Prefabs/Parameter")) as GameObject;
            menuPoints.Add(param1);
            desc = param1.transform.GetChild(0);
            value = param1.transform.GetChild(1);
            plus = param1.transform.GetChild(2);
            minus = param1.transform.GetChild(3);
            reset = param1.transform.GetChild(4);
            param1.transform.SetParent(panel.transform, false);
            param1.transform.localPosition = new Vector3(0, 20 - (80), 0);
            desc.GetComponent<Text>().text = "Phantom Position Z";
            defValue = phantom.transform.localPosition;
            value.GetComponent<Text>().text = defValue.x.ToString();
            value.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 20);

            plus.transform.localPosition = new Vector3(plus.transform.localPosition.x + 10, plus.transform.localPosition.y, plus.transform.localPosition.z);
            minus.transform.localPosition = new Vector3(minus.transform.localPosition.x - 10, minus.transform.localPosition.y, minus.transform.localPosition.z);
            reset.transform.localPosition = new Vector3(reset.transform.localPosition.x + 10, reset.transform.localPosition.y, reset.transform.localPosition.z);

            plus.GetComponent<Button>().onClick.AddListener(delegate {
                var pos = phantom.transform.localPosition;
                phantom.transform.localPosition = new Vector3(pos.x , pos.y, pos.z + 0.1f);
                value.GetComponent<Text>().text = pos.x.ToString();
            });
            minus.GetComponent<Button>().onClick.AddListener(delegate {
                var pos = phantom.transform.localPosition;
                phantom.transform.localPosition = new Vector3(pos.x, pos.y, pos.z - 0.1f);
                value.GetComponent<Text>().text = pos.x.ToString();
            });
            reset.GetComponent<Button>().onClick.AddListener(delegate {
                var pos = phantom.transform.localPosition;
                phantom.transform.localPosition = new Vector3(pos.x,pos.y,defValue.z);
                param1.transform.GetChild(1).GetComponent<Text>().text = pos.z.ToString();
            });
            
        }
        */
    }
}
