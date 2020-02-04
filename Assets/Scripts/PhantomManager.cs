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
    private GameObject skin;
    private int materialUsed = 0;
    public bool hatchingInverted = false;
    private bool hatchingInv = false;

    public bool useTriPlanar = false;
    private bool useTriPl = false;

    private GameObject skin_inv;
    private GameObject skin_stencil;
    private GameObject skin_stencilwindow;

    public Status status;

    private FocusManager focusManager;
    private MenuManager menuMan;


    void Start()
    {
        skin_inv = transform.Find("skin_inv").gameObject;
        skin_stencil = transform.Find("skin_stencil").gameObject;
        skin_stencilwindow = transform.Find("skin_stencilwindow").gameObject;

        focusManager = FindObjectOfType<FocusManager>();
        menuMan = FindObjectOfType<MenuManager>();

        var getSkin = focusManager.GetSkin();
        if (getSkin == null)
        {
            skin = transform.Find("skin").gameObject;
            focusManager.RegisterSkin(skin);
        }
        else
            skin = getSkin;

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
        var surfAlign = FindObjectOfType<SurfaceAlign>();
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

    #region HatchingParams
    public void HatchIntensityIncrease()
    {
        float intensity = insides_Hatching.transform.GetChild(0).GetComponent<Renderer>().material.GetFloat("_Intensity");
        foreach (Transform child in insides_Hatching.transform)
        {
            child.GetComponent<Renderer>().material.SetFloat("_Intensity", (intensity+0.1f));
        }
    }
    public void HatchIntensityDecrease()
    {
        float intensity = insides_Hatching.transform.GetChild(0).GetComponent<Renderer>().material.GetFloat("_Intensity");
        foreach (Transform child in insides_Hatching.transform)
        {
            child.GetComponent<Renderer>().material.SetFloat("_Intensity", (intensity - 0.1f));
        }
    }
    public void HatchIntensityReset()
    {
        foreach (Transform child in insides_Hatching.transform)
        {
            child.GetComponent<Renderer>().material.SetFloat("_Intensity", 1);
        }
    }
    #endregion

    #region BichlmeierParam
    public void SkinIncrParam(string param)
    {
        float intensity = skin.GetComponent<MeshRenderer>().material.GetFloat(param);
        skin.GetComponent<MeshRenderer>().material.SetFloat(param, (intensity + 0.1f));
    }
    public void SkinDecrParam(string param)
    {
        float intensity =skin.GetComponent<MeshRenderer>().material.GetFloat(param);
        skin.GetComponent<MeshRenderer>().material.SetFloat(param, (intensity - 0.1f));
    }
    public void SkinResetParam(string param)
    {
        skin.GetComponent<MeshRenderer>().material.SetFloat(param, 1);
    }

    public void SkinIncrParamSmall(string param)
    {
        float intensity = skin.GetComponent<MeshRenderer>().material.GetFloat(param);
        skin.GetComponent<MeshRenderer>().material.SetFloat(param, (intensity + 0.01f));
    }

    public void SkinDecrParamSmall(string param)
    {
        float intensity = skin.GetComponent<MeshRenderer>().material.GetFloat(param);
        skin.GetComponent<MeshRenderer>().material.SetFloat(param, (intensity - 0.01f));
    }

    #endregion
}
