using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudyManager : MonoBehaviour
{
    public VisualizationMethod[] methods;
    // Start is called before the first frame update
    void Start()
    {
        var n = methods.Length*2;
        List<(VisualizationMethod, bool)> elements = new List<(VisualizationMethod, bool)>();
        for (int i = 0; i<methods.Length; i++)
        {
            elements.Add((methods[i], true));
            elements.Add((methods[i], false));
        }
        List<List<(VisualizationMethod, bool)>> matrix = new List<List<(VisualizationMethod, bool)>>();
        //Generate Latin Square
        for (int i = 0; i < n; i++)
        {
            /*
            int temp = k;
            while (temp <= n)
            {
                printf("%d ", temp);
                temp++;
            }

            // This loop prints numbers from 1 to k-1. 
            for (int j = 1; j < k; j++)
                printf("%d ", j);

            k--;
            printf("\n");
            */
        }
    }

    public void NextTrial()
    {

    }
}

public class VisualizationMethod
{
    public Material materialInside;
    public Material materialSkinWindow;
    public bool hasWindow;

    public VisualizationMethod(Material inside, Material skinWindow, bool hasWindow)
    {
        this.materialInside = inside;
        this.materialSkinWindow = skinWindow;
        this.hasWindow = hasWindow;
    }
}