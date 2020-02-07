using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterManager : MonoBehaviour
{
    public enum ParameterType
    {
        Bichlmeier, Hatching
    }
    // Start is called before the first frame update
    public float resetValue = 1.0f;
    public float step = 0.1f;
    public string parameterName = "";
    public ParameterType type = ParameterType.Bichlmeier;
    
    private GameObject parameterValueText;
    private GameObject parameterNameText;
    private PhantomManager pMan;
    void Start()
    {
        parameterValueText = transform.GetChild(0).gameObject;
        parameterNameText = transform.GetChild(1).gameObject;
        pMan = FindObjectOfType<PhantomManager>();
        parameterValueText.GetComponent<TMPro.TextMeshPro>().text = resetValue.ToString();
        var tmp = parameterNameText.GetComponent<TMPro.TextMeshPro>();
        tmp.text = type.ToString() + "\n" + parameterName;
        tmp.fontSize = 1f;

    }

    public void IncreaseParam()
    {
        float intensity = pMan.ChangeParam(parameterName, step, type);
        parameterValueText.GetComponent<TMPro.TextMeshPro>().text = intensity.ToString();
    }
    public void DecreaseParam()
    {
        float intensity = pMan.ChangeParam(parameterName, (-1)*step, type);
        parameterValueText.GetComponent<TMPro.TextMeshPro>().text = intensity.ToString();
    }

    public void ResetParam()
    {
        float intensity = pMan.ResetParam(parameterName, resetValue, type);
        parameterValueText.GetComponent<TMPro.TextMeshPro>().text = intensity.ToString();
    }

}
