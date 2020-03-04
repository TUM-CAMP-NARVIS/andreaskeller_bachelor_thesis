using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudyManager : MonoBehaviour
{
    private VisualizationMethod[] methods;

    public Material bichlmeier;
    public Material normal;
    public Material normalBox;
    public Material hatching;
    public Material hatchingBox;
    public Material blueShadows;
    public Material blueShadowsBox;
    

    public State state = State.Initial;

    private List<(VisualizationMethod, bool)> order;
    private List<(VisualizationMethod, bool)> elements;

    public enum State {Initial, ResetSlider, PositionTumor, Finished }
    // Start is called before the first frame update
    void Start()
    {
        FillMethods();
        GenerateLatinSquares();
        PrintLatinSquare();
    }

    private void FillMethods()
    {
        methods = new VisualizationMethod[4];
        methods[0] = new VisualizationMethod(normal, bichlmeier, false);
        methods[1] = new VisualizationMethod(normal, normalBox);
        methods[2] = new VisualizationMethod(blueShadows, blueShadowsBox);
        methods[3] = new VisualizationMethod(hatching, hatchingBox);

    }

    private void PrintLatinSquare()
    {
        
        for (int i = 0; i<elements.Count; i++)
        {
            string matrix = "";
            for (int j = 0; j<elements.Count; j++)
            {
                matrix += order[i * elements.Count + j].ToString();
                matrix += " - ";
            }
            Debug.Log(matrix);
        }
        
    }


    private void GenerateLatinSquares()
    {
        var n = methods.Length * 2;
        elements = new List<(VisualizationMethod, bool)>();
        for (int i = 0; i < methods.Length; i++)
        {
            elements.Add((methods[i], true));
            elements.Add((methods[i], false));
        }
        order = new List<(VisualizationMethod, bool)>();

        //Generate Latin Square
        for (int i = 0; i < n; i++)
        {
            List<(VisualizationMethod, bool)> conflictsRow = new List<(VisualizationMethod, bool)>();
            List<(VisualizationMethod, bool)> currentRow = new List<(VisualizationMethod, bool)>();

            List<List<(VisualizationMethod, bool)>> failedRows = new List<List<(VisualizationMethod, bool)>>();

            for (int j = 0; j < n; j++)
            {
                Debug.Log("Position: " + i + "-" + j);
                HashSet<(VisualizationMethod, bool)> conflictsColumn = new HashSet<(VisualizationMethod, bool)>();
                conflictsColumn.Clear();

                for (int k = 0; k < i; k++)
                {
                    
                    int pos = (k * n + j);
                    var conflictElement = order[pos];
                    //Debug.Log("Checking Position: " +pos + ": "+conflictElement);
                    conflictsColumn.Add(conflictElement);
                }

                HashSet<(VisualizationMethod, bool)> conflicts = new HashSet<(VisualizationMethod, bool)>(conflictsColumn);
                conflicts.UnionWith(conflictsRow);
               // Debug.Log("Possible Collisions: " + conflicts.Count);

                HashSet<(VisualizationMethod, bool)> possibilities = new HashSet<(VisualizationMethod, bool)>(elements);
                possibilities.ExceptWith(conflicts);
                Debug.Log("Collision Free elements: " + possibilities.Count);
                foreach((VisualizationMethod, bool) item in possibilities)
                {
                    Debug.Log(item);
                }
                
                foreach(List<(VisualizationMethod, bool)> failedOne in failedRows)
                {
                    if (failedOne.Count == (j + 1))
                    {
                        possibilities.Remove(failedOne[j]);

                    }
                }

                List<(VisualizationMethod, bool)> possibilityList = new List<(VisualizationMethod, bool)>(possibilities);

                if (possibilityList.Count == 0)
                {
                    

                    failedRows.Add(currentRow);
                    currentRow.RemoveAt(j - 1);
                    conflictsRow.RemoveAt(j - 1);
                    j -= 2;

                }
                else
                {
                    var rng = Random.Range(0, possibilityList.Count);
                    var element = possibilityList[rng];
                    Debug.Log("Randomly selected element: " + element.ToString());
                    currentRow.Add(element);
                    conflictsRow.Add(element);
                }
                failedRows.RemoveAll(item => item.Count != currentRow.Count);


            }
            order.AddRange(currentRow);
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

    public VisualizationMethod(Material inside, Material skinWindow, bool hasWindow = true)
    {
        this.materialInside = inside;
        this.materialSkinWindow = skinWindow;
        this.hasWindow = hasWindow;
    }

    public override string ToString()
    {
        string ret = "";
        ret += materialSkinWindow.shader.name;

        return ret;
    }
}