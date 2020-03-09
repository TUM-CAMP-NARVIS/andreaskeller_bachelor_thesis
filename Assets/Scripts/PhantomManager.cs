using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomManager : MonoBehaviour
{
    // Start is called before the first frame update
    public enum Status { hatching, normal, chroma, chromahatch, blueshadows, study, demo}

    public GameObject insides_Hatching;
    public GameObject insides_Chroma;
    public GameObject insides_Normal;
    public GameObject insides_ChromaHatch;
    public GameObject insides_BlueShadows;
    public GameObject insides_Study;

    public GameObject skin;
    public GameObject skin_stencil;
    public GameObject skin_stencilwindow;


    public Status status = Status.normal;

    public int demoVis = 0;

    //Hatching Variables
    public bool hatchInverted = false;
    public bool hatchTriPlanar = false;
    public float hatchUVScale = 5;
    public float hatchIntensity = 1;

    //Bichlmeier Variables
    public float bichlFocusSize = 0.02f;
    public float bichlWeightCurv = 1;
    public float bichlWeightAngOfInc = 1;
    public float bichlWeightDist = 1;
    public float bichlAlpha = 1;
    public float bichlBeta = 1;
    public float bichlGamma = 1;

    //ChromaDepth Variables
    public Color chromaCloseColor;
    public Color chromaFarColor;
    public float chromaLerpDist = 0.1f;

    //General Variables
    public bool skinEnabled = true;
    public bool windowEnabled = false;

    //Managers
    private FocusManager focusManager;
    private MenuManager menuMan;
    private SurfaceAlign surfAlign;
    private WindowMaterialManager windowMaterialManager;


    void Start()
    {
        if (skin_stencil == null)
            skin_stencil = transform.Find("skin_stencil").gameObject;

        if (skin_stencilwindow == null)
            skin_stencilwindow = transform.Find("skin_stencilwindow").gameObject;



        focusManager = FindObjectOfType<FocusManager>();
        menuMan = FindObjectOfType<MenuManager>();
        surfAlign = FindObjectOfType<SurfaceAlign>();
        windowMaterialManager = FindObjectOfType<WindowMaterialManager>();

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
            insides_ChromaHatch.SetActive(false);
            insides_BlueShadows.SetActive(false);
            insides_Normal.SetActive(true);
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

        if (Input.GetKeyDown("r") && Utils.IsVR)
        {
            UpdateVariables();
        }


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
        if (insides_ChromaHatch && insides_ChromaHatch.activeSelf)
        {
            foreach (Transform child in insides_ChromaHatch.transform)
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

    public void SetVisualization(VisualizationMethod method)
    {
        if (status != Status.study)
            SetStatus(Status.study);


        var skinRenderer = skin.GetComponent<MeshRenderer>();

        windowEnabled = method.hasWindow;
        if (surfAlign.isActive != windowEnabled)
            ToggleWindow();

        if (method.hasWindow)
        {
            skinRenderer.enabled = false;
            windowMaterialManager.SetMaterial(method.materialSkinWindow);
        }
        else
        {
            skinRenderer.enabled = true;
            skinRenderer.material = method.materialSkinWindow;
        }

        insides_Study.transform.GetChild(0).GetComponent<Renderer>().material = method.materialInside;
    }

    public void UpdateVariables()
    {
        insides_Normal.SetActive(true);
        insides_Chroma.SetActive(true);
        insides_Hatching.SetActive(true);
        insides_ChromaHatch.SetActive(true);
        insides_BlueShadows.SetActive(true);

        var skinRenderer = skin.GetComponent<MeshRenderer>();
        skinRenderer.enabled = true;

        //ChromaDepth
        foreach(Transform child in insides_Chroma.transform)
        {
            var renderer = child.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.SetColor("_CloseColor", chromaCloseColor);
                renderer.material.SetColor("_FarColor", chromaFarColor);
                renderer.material.SetFloat("_DepthDistance", chromaLerpDist);
            }
        }
        //ChromaDepth Hatching
        foreach (Transform child in insides_ChromaHatch.transform)
        {
            var renderer = child.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.SetColor("_CloseColor", chromaCloseColor);
                renderer.material.SetColor("_FarColor", chromaFarColor);
                renderer.material.SetFloat("_DepthDistance", chromaLerpDist);

                if (hatchInverted)
                {
                    renderer.material.EnableKeyword("_INVERTHATCHING");
                }
                else
                {
                    renderer.material.DisableKeyword("_INVERTHATCHING");
                }

                if (hatchTriPlanar)
                {
                    renderer.material.EnableKeyword("_TRIPLANAR");
                }
                else
                {
                    renderer.material.DisableKeyword("_TRIPLANAR");
                }
                renderer.material.SetFloat("_UVScale", hatchUVScale);
                renderer.material.SetFloat("_Intensity", hatchIntensity);

            }
        }

        //Hatching
        foreach (Transform child in insides_Hatching.transform)
        {
            var renderer = child.GetComponent<Renderer>();
            if (renderer != null)
            {
                if (hatchInverted)
                {
                    renderer.material.EnableKeyword("_INVERTHATCHING");
                }
                else
                {
                    renderer.material.DisableKeyword("_INVERTHATCHING");
                }

                if (hatchTriPlanar)
                {
                    renderer.material.EnableKeyword("_TRIPLANAR");
                }
                else
                {
                    renderer.material.DisableKeyword("_TRIPLANAR");
                }
                renderer.material.SetFloat("_UVScale", hatchUVScale);
                renderer.material.SetFloat("_Intensity", hatchIntensity);
            }
        }

        //Bichlmeier Skin
        var skinMaterial = skinRenderer.material;

        skinMaterial.SetFloat("_FocusRadius", bichlFocusSize);

        skinMaterial.SetFloat("_WeightCurvature", bichlWeightCurv);
        skinMaterial.SetFloat("_WeightAngleofIncidence", bichlWeightAngOfInc);
        skinMaterial.SetFloat("_WeightDistanceFalloff", bichlWeightDist);

        skinMaterial.SetFloat("_Alpha", bichlAlpha);
        skinMaterial.SetFloat("_Beta", bichlBeta);
        skinMaterial.SetFloat("_Gamma", bichlGamma);

        skinRenderer.enabled = skinEnabled;

        if (surfAlign.isActive != windowEnabled)
            ToggleWindow();

        SetStatus(status);
    }

    public void ToggleHatching()
    {
        hatchInverted = !hatchInverted;
        
        if (hatchInverted)
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

    public void SetStatus(Status s)
    {
        insides_Hatching.SetActive(false);
        insides_Normal.SetActive(false);
        insides_ChromaHatch.SetActive(false);
        insides_BlueShadows.SetActive(false);
        insides_Chroma.SetActive(false);
        insides_Study.SetActive(false);
        switch (s)
        {
            case Status.chroma:
                
                insides_Chroma.SetActive(true);
                break;
            case Status.hatching:
                insides_Hatching.SetActive(true);
                break;
            case Status.chromahatch:
                insides_ChromaHatch.SetActive(true);
                break;
            case Status.blueshadows:
                insides_BlueShadows.SetActive(true);
                break;
            case Status.study:
                insides_Study.SetActive(true);
                break;
            default:
                insides_Normal.SetActive(true);
                break;
        }
        status = s;
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
                menuMan.HatchingSetActive(false);
                status = Status.chroma;
                break;
            default:
                insides_Chroma.SetActive(false);
                insides_Normal.SetActive(true);
                status = Status.normal;
                break;
        }
    }
    public void CycleDemoVis()
    {
        demoVis++;
        if (demoVis > 7)
        {
            demoVis = 0;
        }
        ApplyDemoVis();
    }

    public void ApplyDemoVis()
    {
        switch (demoVis)
        {
            case 0:
                skinEnabled = true;
                windowEnabled = false;
                status = Status.normal;
                break;
            case 1:
                skinEnabled = true;
                windowEnabled = false;
                status = Status.blueshadows;
                break;
            case 2:
                skinEnabled = true;
                windowEnabled = false;
                status = Status.hatching;
                break;
            case 3:
                skinEnabled = true;
                windowEnabled = false;
                status = Status.chromahatch;
                break;
            case 4:
                skinEnabled = false;
                windowEnabled = true;
                status = Status.normal;
                windowMaterialManager.SetMaterial(0);
                break;
            case 5:
                skinEnabled = false;
                windowEnabled = true;
                status = Status.blueshadows;
                windowMaterialManager.SetMaterial(2);
                break;
            case 6:
                skinEnabled = false;
                windowEnabled = true;
                status = Status.hatching;
                windowMaterialManager.SetMaterial(1);
                break;
            default:
                skinEnabled = false;
                windowEnabled = true;
                status = Status.chromahatch;
                windowMaterialManager.SetMaterial(3);
                break;
        }
        UpdateVariables();
    }

    public void ToggleTriPlanar()
    {
        hatchTriPlanar = !hatchTriPlanar;

        if (hatchTriPlanar)
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

            if (surfAlign.isActive)
            {
                skin_stencilwindow.SetActive(false);
                skin_stencil.SetActive(true);
            }
            else
            {
                skin_stencilwindow.SetActive(true);
                skin_stencil.SetActive(false);
            }
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

        public Color chromaFarColor;
        public Color chromaCloseColor;

        public float chromaLerpDist;

        public int window_mat;

        public State(Status status, bool skinEnabled, bool windowEnabled, BichlmeierState bichlmeierState, HatchingState hatchingState, Color chromaFarColor, Color chromaCloseColor, float chromaLerpDist, int window_mat)
        {
            this.status = status;
            this.skinEnabled = skinEnabled;
            this.windowEnabled = windowEnabled;
            this.bichlmeierState = bichlmeierState;
            this.hatchingState = hatchingState;
            this.chromaFarColor = chromaFarColor;
            this.chromaCloseColor = chromaCloseColor;
            this.chromaLerpDist = chromaLerpDist;
            this.window_mat = window_mat;
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
        BichlmeierState bState = new BichlmeierState(bichlAlpha, bichlBeta, bichlGamma, bichlWeightCurv, bichlWeightAngOfInc, bichlWeightDist, bichlFocusSize);
        HatchingState hState = new HatchingState(hatchIntensity, hatchUVScale, hatchInverted, hatchTriPlanar);


        State update = new State(status, skinEnabled, windowEnabled, bState, hState,chromaFarColor, chromaCloseColor, chromaLerpDist, FindObjectOfType<WindowMaterialManager>().index);
        
        return new SceneStateMessage(update);
    }

    public void applyUpdate(SceneStateMessage update)
    {
        //Bichlmeier
        bichlAlpha = update.a;
        bichlBeta = update.b;
        bichlGamma = update.g;
        bichlFocusSize = update.focusSize;
        bichlWeightAngOfInc = update.wA;
        bichlWeightCurv = update.wC;
        bichlWeightDist = update.wD;

        //Hatching
        hatchIntensity = update.hatchIntensity;
        hatchUVScale = update.hatchUVScale;
        hatchTriPlanar = update.hatchTriPlanar;
        hatchInverted = update.hatchInverted;

        //ChromaDepth
        chromaLerpDist = update.chromaLerpDist;
        chromaFarColor = update.chromaFarColor;
        chromaCloseColor = update.chromaCloseColor;

        status = update.status;

        skin.GetComponent<MeshRenderer>().enabled = true;

        UpdateVariables();

        FindObjectOfType<WindowMaterialManager>().index = update.window_mat;


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
