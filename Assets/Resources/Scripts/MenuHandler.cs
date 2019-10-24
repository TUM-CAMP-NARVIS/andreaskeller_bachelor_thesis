using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    public GameObject holoLens;
    public GameObject model;
    private GameObject panel;

    // Start is called before the first frame update
    void Start()
    {
        panel = transform.GetChild(0).gameObject;
        BuildMenu(model.GetComponent<Renderer>().material.shader);
        
        
    }

    // Update is called once per frame
    void Update()
    {
        //Rotate towards camera
        if (holoLens)
        {
            Vector3 angle = Vector3.Normalize(this.transform.position - holoLens.transform.position);
            this.transform.rotation = Quaternion.LookRotation(angle);
        }
    }

    void BuildMenu(Shader s)
    {
        Debug.Log("Building Menu");
        switch (s.name)
        {
            case "UnLit/S_Bichlmeier2007":
                break;
            default:
                break;
        }
        var paramList = new List<(string, string)>{
            ("_FocusRadius", "Focus Radius"),
            ("_WeightCurvature", "Curvature Weight"),
            ("_WeightAngleofIncidence", "Angle of Incidence Weight"),
            ("_WeightDistanceFalloff", "Distance Falloff Weight") 
        };
        int i = 0;
        foreach ((string name, string desc) in paramList)
        {
            GameObject param1 = Instantiate(Resources.Load("Prefabs/Parameter")) as GameObject;
            param1.transform.SetParent(panel.transform, false);
            param1.transform.localPosition = new Vector3(0, 40-(20*i), 0);
            param1.transform.GetChild(0).GetComponent<Text>().text = desc;
            var defValue = model.GetComponent<Renderer>().material.GetFloat(name);
            param1.transform.GetChild(1).GetComponent<Text>().text = defValue.ToString();
            param1.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate {
                var mat = model.GetComponent<Renderer>().material;
                mat.SetFloat(name, mat.GetFloat(name)+0.1f);
                param1.transform.GetChild(1).GetComponent<Text>().text = mat.GetFloat(name).ToString();
            });
            param1.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate {
                var mat = model.GetComponent<Renderer>().material;
                mat.SetFloat(name, mat.GetFloat(name) - 0.1f);
                param1.transform.GetChild(1).GetComponent<Text>().text = mat.GetFloat(name).ToString();
            });
            param1.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate {
                var mat = model.GetComponent<Renderer>().material;
                mat.SetFloat(name,defValue);
                param1.transform.GetChild(1).GetComponent<Text>().text = mat.GetFloat(name).ToString();
            });
            i++;
        }

    }
    void DestroyMenu()
    {
        foreach (Transform child in panel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
