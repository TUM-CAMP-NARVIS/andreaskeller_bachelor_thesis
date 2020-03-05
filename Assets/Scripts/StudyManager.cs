using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.IO;

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

    public GameObject buttonIndicator;
    public TMPro.TextMeshPro status;
    public TMPro.TextMeshPro elementCounter;

    public GameObject experiment;
    private Camera cam;

    public string subjectID = "testSubject";
    private string data = "";
    

    public State state = State.NotReady;

    private PhantomManager phantomManager;
    private TumorManager tumorManager;

    private List<(VisualizationMethod, bool)> order;
    private List<(VisualizationMethod, bool)> elements;

    private bool registeredHandlers = false;

    public int currentTrial = 0;

    public int totalTrials = 0;

    public enum State {Ready, NotReady, MoveSliderBack, MoveSliderFront, PositionTumor, Finished }
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        phantomManager = FindObjectOfType<PhantomManager>();
        tumorManager = FindObjectOfType<TumorManager>();
        state = State.NotReady;
        FillMethods();
#if UNITY_WSA

#else
        GenerateLatinSquare();
        totalTrials = order.Count;
#endif


    }

    public void Update()
    {
        elementCounter.text = (currentTrial + 1).ToString() + " of " + totalTrials.ToString();
        status.text = state.ToString();

        if (!registeredHandlers)
        {
#if UNITY_WSA
            if (NetworkClient.isConnected)
            {
                NetworkClient.RegisterHandler<VisualizationMethodMessage>(ApplyVisMethodMessage);
                NetworkClient.RegisterHandler<StudyManagerMessage>(ApplyStudyManagerMessage);
                registeredHandlers = true;
            }
#else

            if (NetworkServer.active)
            {
                NetworkServer.RegisterHandler<HoloLensPositionMessage>(OnHoloLensPositionMessage);
                registeredHandlers = true;
            }
        
        SendStudyManagerMessage();
#endif
        }

    }

    public void SendStudyManagerMessage()
    {
#if !UNITY_WSA
        if (NetworkServer.active)
        {
            NetworkServer.SendToAll<StudyManagerMessage>(CreateStudyManagerMessage());
        }
#endif
    }

    private void FillMethods()
    {
        methods = new VisualizationMethod[4];
        methods[0] = new VisualizationMethod(normal, bichlmeier, false);
        methods[1] = new VisualizationMethod(normal, normalBox);
        methods[2] = new VisualizationMethod(blueShadows, blueShadowsBox);
        methods[3] = new VisualizationMethod(hatching, hatchingBox);

    }

#region LatinSquare
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


    private void GenerateLatinSquare()
    {
        var n = methods.Length * 2;
        elements = new List<(VisualizationMethod, bool)>();
        for (int i = 0; i < methods.Length; i++)
        {
            elements.Add((methods[i], true));
            elements.Add((methods[i], false));
        }
        order = new List<(VisualizationMethod, bool)>();

        int counter = 0;

        //Generate Latin Square
        for (int i = 0; i < n; i++)
        {
            List<(VisualizationMethod, bool)> currentRow = new List<(VisualizationMethod, bool)>();

            HashSet<List<(VisualizationMethod, bool)>> failedRows = new HashSet<List<(VisualizationMethod, bool)>>();

            for (int j = 0; j < n; j++)
            {
                //Debug.Log("Position: " + i + "-" + j);
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
                conflicts.UnionWith(currentRow);
               // Debug.Log("Possible Collisions: " + conflicts.Count);

                HashSet<(VisualizationMethod, bool)> possibilities = new HashSet<(VisualizationMethod, bool)>(elements);
                possibilities.ExceptWith(conflicts);

                foreach (List<(VisualizationMethod, bool)> failedOne in failedRows)
                {
                    if (failedOne.Count == (j + 1))
                    {
                        possibilities.Remove(failedOne[j]);
                    }
                }
                
                

                List<(VisualizationMethod, bool)> possibilityList = new List<(VisualizationMethod, bool)>(possibilities);

                if (possibilityList.Count == 0)
                {
                    failedRows.Add(new List<(VisualizationMethod, bool)>(currentRow));
                    currentRow.RemoveAt(j - 1);

                    //Go back one item
                    j -= 2;
                }
                else
                {
                    var rng = Random.Range(0, possibilityList.Count);
                    var element = possibilityList[rng];
                    currentRow.Add(element);
                    failedRows.RemoveWhere(item => item.Count == currentRow.Count+1);
                }
            }
            order.AddRange(currentRow);
        }
    }
#endregion

    public void ButtonInput()
    {
        switch (state)
        {
            case State.PositionTumor:
                ConfirmTumorPosition();
                break;
            default:
                break;
        }
    }

    public void ConfirmTumorPosition()
    {

        currentTrial++;

        var pos = tumorManager.transform.localPosition.y;
        data += Time.time.ToString() + ": position: " + pos.ToString() + " delta: " + tumorManager.GetDeltaPosition()+"\n";
        if (currentTrial >= totalTrials)
        {
            state = State.Finished;
            WriteDataToFile();
            NetworkServer.SendToAll<StudyManagerMessage>(CreateStudyManagerMessage());
        }
        else
        {
            state = State.Ready;
            NetworkServer.SendToAll<StudyManagerMessage>(CreateStudyManagerMessage());
            NextTrial();
        }


    }

    public void WriteDataToFile()
    {
        var dateTime = System.DateTime.Now;
        string filename = dateTime.Year + "_" + dateTime.Month + "_" + dateTime.Day + "_" + dateTime.Hour + "-" + dateTime.Hour + "-" + dateTime.Minute+"-"+dateTime.Second+"_" +subjectID+".txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(filename, true);
        writer.WriteLine(data);
        writer.Close();
        data = "";
    }

    public void SliderMoved(float position)
    {
        if (state == State.PositionTumor)
        {

#if UNITY_WSA
            var msg = new HoloLensPositionMessage(experiment.transform.InverseTransformPoint(cam.transform.position), (Quaternion.Inverse(experiment.transform.rotation) * cam.transform.rotation));
            NetworkClient.Send<HoloLensPositionMessage>(msg);
#else

            tumorManager.MoveTumor(position);
#endif
        }

#if !UNITY_WSA
        else if (state == State.MoveSliderFront)
        {
            if (position < 0.05f)
            {
                state = State.PositionTumor;
                NetworkServer.SendToAll<StudyManagerMessage>(CreateStudyManagerMessage());
            }
        }
        else if (state == State.MoveSliderBack)
        {
            if (position > 0.95f)
            {
                state = State.PositionTumor;
                NetworkServer.SendToAll<StudyManagerMessage>(CreateStudyManagerMessage());
            }
                
        }
#endif

    }

    public void StartTrial()
    {
        //TODO: grab user name
        if (state != State.Ready)
            return;
        currentTrial = 0;
        phantomManager.SetStatus(PhantomManager.Status.study);
        NextTrial();
    }

    public void NextTrial()
    {
        if (state != State.Ready)
            return;
        

#if !UNITY_WSA
        var message = CreateVisMethodMessage(order[currentTrial].Item1, order[currentTrial].Item2);
        if (NetworkServer.active)
        {
            NetworkServer.SendToAll<VisualizationMethodMessage>(message);
        }
#endif
        phantomManager.SetVisualization(order[currentTrial].Item1);
        if (order[currentTrial].Item2)
        {
            tumorManager.SetTumorFront();
            state = State.MoveSliderFront;
            NetworkServer.SendToAll<StudyManagerMessage>(CreateStudyManagerMessage());
        }
        else
        {
            tumorManager.SetTumorBack();
            state = State.MoveSliderBack;
            NetworkServer.SendToAll<StudyManagerMessage>(CreateStudyManagerMessage());
        }

        data += Time.time.ToString() + ": Trial " + currentTrial + " of " + totalTrials + " - " + order[currentTrial].Item1.ToString()+" ";
        if (order[currentTrial].Item2)
        {
            data += "front\n";
        }
        else
            data += "back\n";
    }

    public void SetComPort(string name)
    {
        SerialController sctrl = FindObjectOfType<SerialController>();
        if (sctrl.portName.Equals(name))
            return;
        sctrl.portName = name;
        sctrl.enabled = false;
        sctrl.enabled = true;
    }

#region Networking
    public StudyManagerMessage CreateStudyManagerMessage()
    {
        float buttonAmount = FindObjectOfType<StudyDemoMovement>().CurrentConfirmationProgress;
        
        return new StudyManagerMessage(state, currentTrial, totalTrials, buttonAmount);
    }

    public void ApplyStudyManagerMessage(NetworkConnection conn, StudyManagerMessage msg)
    {
        state = msg.state;
        currentTrial = msg.currentTrial;
        totalTrials = msg.totalTrials;
        buttonIndicator.GetComponent<Renderer>().material.color = new Color(1-msg.buttonPressAmount, 1, 1-msg.buttonPressAmount);
    }

    public VisualizationMethodMessage CreateVisMethodMessage(VisualizationMethod method, bool front)
    {
        int inside = 0;
        int window = 0;

        if (method.materialInside == hatching)
        {
            inside = 1;
        }
        else if (method.materialInside == blueShadows)
        {
            inside = 2;
        }
        if (method.materialSkinWindow == hatchingBox)
        {
            window = 1;
        }
        else if (method.materialSkinWindow == blueShadowsBox)
        {
            window = 2;
        }

        VisualizationMethodMessage v = new VisualizationMethodMessage(inside, window, method.hasWindow, front);
        return v;
    }

    public void ApplyVisMethodMessage(NetworkConnection conn,VisualizationMethodMessage msg)
    {

        Material inside;
        Material skinWindow;
        if (!msg.hasWindow)
        {
            skinWindow = bichlmeier;
        }
        else
        {
            switch (msg.matSkinWindow)
            {
                case 0:
                    skinWindow = normalBox;
                    break;
                case 1:
                    skinWindow = hatchingBox;
                    break;
                case 2:
                    skinWindow = blueShadowsBox;
                    break;
                default:
                    skinWindow = normalBox;
                    break;
            }
        }
        switch (msg.matInside)
        {
            case 1:
                inside = hatching;
                break;
            case 2:
                inside = blueShadows;
                break;
            default:
                inside = normal;
                break;
        }

        VisualizationMethod v = new VisualizationMethod(inside, skinWindow, msg.hasWindow);
        phantomManager.SetVisualization(v);
    }

    public void OnHoloLensPositionMessage(NetworkConnection conn, HoloLensPositionMessage msg)
    {
        if (state == State.PositionTumor)
            data += Time.time.ToString() + ": HoloLens position: " + msg.position.ToString() + " rotation: " + msg.rotation.ToString()+ "\n";
    }
#endregion
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