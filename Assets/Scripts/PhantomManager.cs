using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomManager : MonoBehaviour
{
    // Start is called before the first frame update
    public enum Status { hatching, normal, chroma }

    public GameObject insides_Hatching;
    public GameObject insides_Chroma;
    public GameObject insides_Normal;

    public GameObject skin;
    public GameObject skin_inv;
    public GameObject skin_stencil;
    public GameObject skin_stencilwindow;

    private int materialUsed = 0;
    public bool hatchingInverted = false;
    private bool hatchingInv = false;

    public bool useTriPlanar = false;
    private bool useTriPl = false;

    public Status status;

    private FocusManager focusManager;
    private MenuManager menuMan;
    private SurfaceAlign surfAlign;


    void Start()
    {
        if (skin_inv == null)
        {
            skin_inv = transform.Find("skin_inv").gameObject;

        }
        if (skin_stencil == null)
            skin_stencil = transform.Find("skin_stencil").gameObject;

        if (skin_stencilwindow == null)
            skin_stencilwindow = transform.Find("skin_stencilwindow").gameObject;



        focusManager = FindObjectOfType<FocusManager>();
        menuMan = FindObjectOfType<MenuManager>();
        surfAlign = FindObjectOfType<SurfaceAlign>();

        if (skin == null)
        {
            var getSkin = focusManager.GetSkin();
            if (getSkin == null)
            {
                skin = transform.Find("skin").gameObject;
                focusManager.RegisterSkin(skin);
            }
            else
                skin = getSkin;
        }
        

        skin.SetActive(true);

        if (insides_Normal != null && insides_Hatching != null && insides_Chroma != null)
        {
            insides_Chroma.SetActive(false);
            insides_Hatching.SetActive(false);
            insides_Normal.SetActive(true);
            skin_inv.SetActive(false);
            skin_stencil.SetActive(true);
            skin_stencilwindow.SetActive(false);

            menuMan.BichlmeierSetActive(skin.GetComponent<MeshRenderer>().enabled);
            menuMan.HatchingSetActive(false);
            status = Status.normal;
        }        
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (hatchingInv != hatchingInverted)
            ToggleHatching();

        if (useTriPl != useTriPlanar)
            ToggleTriPlanar();

#endif
        if (insides_Chroma && insides_Chroma.activeSelf)
        {
            foreach (Transform child in insides_Chroma.transform)
            {
                var renderer = child.GetComponent<Renderer>();
                if (renderer)
                {
                    renderer.material.SetVector("_FocusPosition", focusManager.focusPosition);
                    renderer.material.SetVector("_FocusNormal", focusManager.focusNormal);
                }
            }
        }
        var skinRenderer = skin.GetComponent<MeshRenderer>();
        if (skinRenderer.enabled)
        {
            skinRenderer.material.SetVector("_FocusPosition", focusManager.focusPosition);
            if (Utils.IsVR)
            {
              
                skinRenderer.material.EnableKeyword("_ZEDMINI_BLENDING");
                
            }
        }
            

    }

    

    public void ToggleHatching()
    {
        hatchingInv = !hatchingInv;
        hatchingInverted = hatchingInv;
        
        if (hatchingInv)
        {
            foreach (Transform child in insides_Hatching.transform)
            {
                child.GetComponent<Renderer>().material.EnableKeyword("_INVERTHATCHING");
            }
        }
        else
        {
            foreach (Transform child in insides_Hatching.transform)
            {
                child.GetComponent<Renderer>().material.DisableKeyword("_INVERTHATCHING");
            }
        }
        
    }

    public void CycleInsides()
    {
        switch (status)
        {
            case Status.normal:
                insides_Normal.SetActive(false);
                insides_Hatching.SetActive(true);
                menuMan.HatchingSetActive(true);
                status = Status.hatching;
                break;
            case Status.hatching:
                insides_Hatching.SetActive(false);
                insides_Chroma.SetActive(true);
                skin_inv.SetActive(true);
                menuMan.HatchingSetActive(false);
                status = Status.chroma;
                break;
            default:
                insides_Chroma.SetActive(false);
                insides_Normal.SetActive(true);
                skin_inv.SetActive(false);
                status = Status.normal;
                break;
        }
    }

    public void ToggleTriPlanar()
    {
        useTriPl = !useTriPl;
        useTriPlanar = useTriPl;

        if (useTriPl)
        {
            foreach (Transform child in insides_Hatching.transform)
            {
                child.GetComponent<Renderer>().material.EnableKeyword("_TRIPLANAR");
            }
        }
        else
        {
            foreach (Transform child in insides_Hatching.transform)
            {
                child.GetComponent<Renderer>().material.DisableKeyword("_TRIPLANAR");
            }
        }
    }

    public void ToggleSkin()
    {
        var newstatus = !skin.GetComponent<MeshRenderer>().enabled;
        skin.GetComponent<MeshRenderer>().enabled = newstatus;
        menuMan.BichlmeierSetActive(newstatus);

    }

    public void ToggleWindow()
    {
        
        if (surfAlign != null)
        {
            surfAlign.ToggleActive();
        }
        if (surfAlign.isActive)
        {
            skin_stencilwindow.SetActive(false);
            skin_stencil.SetActive(true);
        } else
        {
            skin_stencilwindow.SetActive(true);
            skin_stencil.SetActive(false);
        }
    }

    #region ParameterAdjustment
    public float ChangeParam(string param, float step, ParameterManager.ParameterType type)
    {
        float intensity = 0;
        if (type == ParameterManager.ParameterType.Bichlmeier)
        {
            intensity = skin.GetComponent<MeshRenderer>().material.GetFloat(param) + step;
            skin.GetComponent<MeshRenderer>().material.SetFloat(param, (intensity));
        }
        else
        {
            intensity = insides_Hatching.transform.GetChild(0).GetComponent<Renderer>().material.GetFloat(param) + step;
            foreach (Transform child in insides_Hatching.transform)
            {
                child.GetComponent<Renderer>().material.SetFloat(param, (intensity ));
            }
        }
        return intensity;

    }
    public float ResetParam(string param, float orig, ParameterManager.ParameterType type)
    {
        if (type == ParameterManager.ParameterType.Bichlmeier)
        {
            skin.GetComponent<MeshRenderer>().material.SetFloat(param, orig);
        }
        else if (type == ParameterManager.ParameterType.Hatching)
        {
            foreach (Transform child in insides_Hatching.transform)
            {
                child.GetComponent<Renderer>().material.SetFloat(param, orig);
            }
        }
        
        return orig;
    }

    #endregion

    #region state-management
    public struct State
    {
        public Status status;

        public bool skinEnabled;
        public bool windowEnabled;


        public BichlmeierState bichlmeierState;
        public HatchingState hatchingState;

        public State(Status status, bool skinEnabled, bool windowEnabled, BichlmeierState bichlmeierState, HatchingState hatchingState)
        {
            this.status = status;
            this.skinEnabled = skinEnabled;
            this.windowEnabled = windowEnabled;
            this.bichlmeierState = bichlmeierState;
            this.hatchingState = hatchingState;
        }
    }

    public struct BichlmeierState
    {
        public float alpha, beta, gamma, weightCurv, weightAngle, weightDistance, focusSize;

        public BichlmeierState(float a, float b, float g, float weightCurv, float weightAngle, float weightDistance, float focusSize)
        {
            this.alpha = a;
            this.beta = b;
            this.gamma = g;
            this.weightAngle = weightAngle;
            this.weightCurv = weightCurv;
            this.weightDistance = weightDistance;
            this.focusSize = focusSize;
        }

    }

    public struct HatchingState
    {
        public float intensity, uvscale;
        public bool isInverted;
        public bool isTriPlanar;

        public HatchingState(float intensity, float uvscale, bool isInverted, bool isTriPlanar)
        {
            this.intensity = intensity;
            this.uvscale = uvscale;
            this.isInverted = isInverted;
            this.isTriPlanar = isTriPlanar;
        }

    }

    public SceneStateMessage GetFullUpdate()
    {
        BichlmeierState bState = GetBichlmeier();
        HatchingState hState = GetHatching();


        State update = new State(status, skin.GetComponent<MeshRenderer>().enabled,surfAlign.isActive, bState, hState);
        
        return new SceneStateMessage(update);
    }

    public BichlmeierState GetBichlmeier()
    {
        var mat = skin.GetComponent<MeshRenderer>().material;
        var a = mat.GetFloat("_Alpha");
        var b = mat.GetFloat("_Beta");
        var g = mat.GetFloat("_Gamma");
        var wC = mat.GetFloat("_WeightCurvature");
        var wA = mat.GetFloat("_WeightAngleofIncidence");
        var wD = mat.GetFloat("_WeightDistanceFalloff");
        var focusSize = mat.GetFloat("_FocusRadius");
        BichlmeierState bState = new BichlmeierState(a, b, g, wC, wA, wD, focusSize);
        return bState;
    }

    public void ApplyBichlmeierState(BichlmeierState update)
    {
        var mat = skin.GetComponent<MeshRenderer>().material;
        mat.SetFloat("_Alpha", update.alpha);
        mat.SetFloat("_Beta", update.beta);
        mat.SetFloat("_Gamma", update.gamma);
        mat.SetFloat("_WeightCurvature", update.weightCurv);
        mat.SetFloat("_WeightAngleofIncidence", update.weightAngle);
        mat.SetFloat("_WeightDistanceFalloff", update.weightDistance);
        mat.SetFloat("_FocusRadius", update.focusSize);
    }

    public HatchingState GetHatching()
    {
        var mat = insides_Hatching.transform.GetChild(0).GetComponent<Renderer>().material;
        HatchingState hState = new HatchingState(mat.GetFloat("_Intensity"), mat.GetFloat("_UVScale"), mat.IsKeywordEnabled("_TRIPLANAR"), mat.IsKeywordEnabled("_INVERTHATCHING"));
        return hState;
    }

    public void ApplyHatchingState(HatchingState update)
    {
        foreach (Transform child in insides_Hatching.transform)
        {
            child.GetComponent<Renderer>().material.SetFloat("_UVScale", update.uvscale);
            child.GetComponent<Renderer>().material.SetFloat("_Intensity", update.intensity);

            if (update.isTriPlanar)
            {
                child.GetComponent<Renderer>().material.EnableKeyword("_TRIPLANAR");
            }
            if (update.isInverted)
            {
                child.GetComponent<Renderer>().material.EnableKeyword("_INVERTHATCHING");
            }
        }
    }

    public void applyUpdate(SceneStateMessage update)
    {
        if (update.status != status)
        {
            switch (status)
            {
                case Status.chroma:
                    if (update.status == Status.normal)
                    {
                        CycleInsides();
                    }
                    else
                    {
                        CycleInsides();
                        CycleInsides();
                    }
                    break;
                case Status.hatching:
                    if (update.status == Status.chroma)
                    {
                        CycleInsides();
                    }
                    else
                    {
                        CycleInsides();
                        CycleInsides();
                    }
                    break;
                default:
                    if (update.status == Status.hatching)
                    {
                        CycleInsides();
                    }
                    else
                    {
                        CycleInsides();
                        CycleInsides();
                    }
                    break;
            }
        }

        BichlmeierState bState = new BichlmeierState(update.a, update.b, update.g, update.wC, update.wA, update.wD, update.focusSize);

        ApplyBichlmeierState(bState);

        HatchingState hState = new HatchingState(update.i, update.u, update.inv, update.tri);
        ApplyHatchingState(hState);

        if (surfAlign.isActive != update.windowEnabled)
        {
            ToggleWindow();
        }

        if (skin.GetComponent<MeshRenderer>().enabled != update.skinEnabled)
        {
            ToggleSkin();
        }
        
    }
    #endregion
}
