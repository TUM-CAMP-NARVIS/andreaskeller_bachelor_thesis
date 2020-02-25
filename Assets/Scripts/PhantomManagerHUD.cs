using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(PhantomManager))]
public class PhantomManagerHUD : MonoBehaviour
{
    PhantomManager manager;
    MultiplatformSceneManager multiplatformSceneManager;
    WindowMaterialManager windowMaterialManager;

    private string bichlFocusSize;
    public string bichlWeightCurv;
    public string bichlWeightAngOfInc;
    public string bichlWeightDist;
    public string bichlAlpha;
    public string bichlBeta;
    public string bichlGamma;

    public string chromaLerpDist;

    public string hatchUVScale;
    public string hatchIntensity;

    public int offsetX, offsetY;

#if UNITY_WSA
    public bool showGUI = false;
#else
    public bool showGUI = true;
#endif

    private void Awake()
    {
        manager = GetComponent<PhantomManager>();
        multiplatformSceneManager = FindObjectOfType<MultiplatformSceneManager>();
        windowMaterialManager = FindObjectOfType<WindowMaterialManager>();

        bichlFocusSize = manager.bichlFocusSize.ToString();
        bichlWeightAngOfInc = manager.bichlWeightAngOfInc.ToString();
        bichlWeightCurv = manager.bichlWeightCurv.ToString();
        bichlWeightDist = manager.bichlWeightDist.ToString();
        bichlAlpha = manager.bichlAlpha.ToString();
        bichlBeta = manager.bichlBeta.ToString();
        bichlGamma = manager.bichlGamma.ToString();

        chromaLerpDist = manager.chromaLerpDist.ToString();

        hatchUVScale = manager.hatchUVScale.ToString();
        hatchIntensity = manager.hatchIntensity.ToString();

    }

    private void OnGUI()
    {
        if (!showGUI)
            return;

        GUILayout.BeginArea(new Rect(10 + offsetX, 40 + offsetY, 300, 9999));

        if (multiplatformSceneManager != null)
        {
            if (GUILayout.Button("Toggle Cursor"))
            {
                multiplatformSceneManager.ToggleCursor();
            }
        }

        if (GUILayout.Button("Skin: "+manager.skinEnabled.ToString()))
        {
            manager.skinEnabled = !manager.skinEnabled;
        }

        if (manager.skinEnabled)
        {
            GUILayout.Label("Bichlmeier Settings:");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Focus Size");
            bichlFocusSize = GUILayout.TextField(bichlFocusSize);
            float.TryParse(bichlFocusSize, out manager.bichlFocusSize);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Weight Curvature");
            bichlWeightCurv = GUILayout.TextField(bichlWeightCurv);
            float.TryParse(bichlWeightCurv, out manager.bichlWeightCurv);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Weight Angle of Incidence");
            bichlWeightAngOfInc = GUILayout.TextField(bichlWeightAngOfInc);
            float.TryParse(bichlWeightAngOfInc, out manager.bichlWeightAngOfInc);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Weight Distance Falloff");
            bichlWeightDist = GUILayout.TextField(bichlWeightDist);
            float.TryParse(bichlWeightDist, out manager.bichlWeightDist);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Alpha");
            bichlAlpha = GUILayout.TextField(bichlAlpha);
            float.TryParse(bichlAlpha, out manager.bichlAlpha);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Beta");
            bichlBeta = GUILayout.TextField(bichlBeta);
            float.TryParse(bichlBeta, out manager.bichlBeta);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Gamma");
            bichlGamma = GUILayout.TextField(bichlGamma);
            float.TryParse(bichlGamma, out manager.bichlGamma);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
        }

        GUILayout.Label("Current mode: " + manager.status.ToString());
        if (manager.status != PhantomManager.Status.normal)
        {
            if (GUILayout.Button("Switch to normal"))
            {
                manager.status = PhantomManager.Status.normal;
            }
        }
        
        if (manager.status != PhantomManager.Status.hatching)
        {
            if (GUILayout.Button("Switch to hatching"))
            {
                manager.status = PhantomManager.Status.hatching;
            }
        }
        
        if (manager.status != PhantomManager.Status.chroma)
        {
            if (GUILayout.Button("Switch to chromadepth"))
            {
                manager.status = PhantomManager.Status.chroma;
            }
        }
        

        if (manager.status == PhantomManager.Status.hatching)
        {
            GUILayout.Label("Hatch Settings");
            GUILayout.BeginHorizontal();
            GUILayout.Label("UV Scale");
            hatchUVScale = GUILayout.TextField(hatchUVScale);
            float.TryParse(hatchUVScale, out manager.hatchUVScale);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Intensity");
            hatchIntensity = GUILayout.TextField(hatchIntensity);
            float.TryParse(hatchIntensity, out manager.hatchIntensity);
            GUILayout.EndHorizontal();
            if (manager.hatchInverted)
            {
                if (GUILayout.Button("Un-Invert Hatching"))
                {
                    manager.hatchInverted = false;
                }
            }
            else
            {
                if (GUILayout.Button("Invert Hatching"))
                {
                    manager.hatchInverted = true;
                }
            }

            if (manager.hatchTriPlanar)
            {
                if (GUILayout.Button("Don't Use Triplanar"))
                {
                    manager.hatchTriPlanar = false;
                }
            }
            else
            {
                if (GUILayout.Button("Use Triplanar"))
                {
                    manager.hatchTriPlanar = true;
                }
            }
            GUILayout.Space(10);
        }
        else if (manager.status == PhantomManager.Status.chroma)
        {
            GUILayout.Label("ChromaDepth Settings");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Lerp Distance");
            chromaLerpDist = GUILayout.TextField(chromaLerpDist);
            float.TryParse(chromaLerpDist, out manager.chromaLerpDist);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        if (GUILayout.Button("Window: "+manager.windowEnabled.ToString()))
        {
            manager.windowEnabled = !manager.windowEnabled;
        }

        if (manager.windowEnabled && windowMaterialManager != null)
        {
            if (GUILayout.Button("Cycle window materials"))
            {
                windowMaterialManager.CycleMaterials();
            }
        }


        if (GUILayout.Button("Apply Variables"))
        {
            manager.UpdateVariables();
        }



        GUILayout.EndArea();
    }
}
