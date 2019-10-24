using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    public GameObject holoLens;
    public GameObject model;
    private GameObject panel;
    private Queue<(Shader, List<(string, string)>)> shaders;
    private List<GameObject> menuPoints;

    // Start is called before the first frame update
    void Start()
    {
        panel = transform.GetChild(0).gameObject;
        shaders = new Queue<(Shader, List<(string, string)>)>();
        menuPoints = new List<GameObject>();
        Shader sh = Shader.Find("Unlit/S_Bichlmeier2007");
        if (sh)
        {
            var paramList = new List<(string, string)>{
            ("_FocusRadius", "Focus Radius"),
            ("_WeightCurvature", "Curvature Weight"),
            ("_WeightAngleofIncidence", "Angle of Incidence Weight"),
            ("_WeightDistanceFalloff", "Distance Falloff Weight")
        };
            shaders.Enqueue((sh, paramList));
        }
        sh = Shader.Find("Shader Graphs/SG_FocusContextEx");
        if (sh)
        {
            var paramList = new List<(string, string)>{
            ("_FocusRadius", "Focus Radius"),
            ("_FresnelPower", "Fresnel Power")
        };
            shaders.Enqueue((sh, paramList));
        }
        sh = Shader.Find("Lit/S_OnlyRenderFocus");
        if (sh)
        {
            var paramList = new List<(string, string)>{
            ("_FocusRadius", "Focus Radius"),
        };
            shaders.Enqueue((sh, paramList));
        }

        var current = shaders.Dequeue();
        Debug.Log(shaders.Peek().Item1.name);
        
        BuildMenu(current.Item1, current.Item2);
        
        
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

    void BuildMenu(Shader s, List<(string,string)> paramList)
    {
        Debug.Log("Building Menu");
        
        //Shader Selection
        GameObject title = Instantiate(Resources.Load("Prefabs/Title")) as GameObject;
        menuPoints.Add(title);
        title.transform.SetParent(panel.transform, false);
        title.transform.localPosition = new Vector3(0, 40, 0);
        title.transform.GetChild(0).GetComponent<Text>().text = s.name;
        title.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate {
            shaders.Enqueue((s, paramList));
            var newShader = shaders.Dequeue();
            DestroyMenu();
            model.GetComponent<Renderer>().material.shader = newShader.Item1;
            BuildMenu(newShader.Item1, newShader.Item2);
        });
        title.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = "X";
        title.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate {
            shaders.Enqueue((s, paramList));
            DestroyMenu();
        });

        //Parameters
        int i = 0;
        foreach ((string name, string desc) in paramList)
        {
            GameObject param1 = Instantiate(Resources.Load("Prefabs/Parameter")) as GameObject;
            menuPoints.Add(param1);
            param1.transform.SetParent(panel.transform, false);
            param1.transform.localPosition = new Vector3(0, 20-(20*i), 0);
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
        foreach (GameObject gameObj in menuPoints)
        {
            Destroy(gameObj);
        }
    }
}
