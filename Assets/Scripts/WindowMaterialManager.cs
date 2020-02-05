using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowMaterialManager : MonoBehaviour
{
    public List<Material> materials;
    private FocusManager focusManager;

    private int index = 0;
    private Material material;
    // Start is called before the first frame update
    public void CycleMaterials()
    {
        index++;
        if (index >= materials.Count)
            index = 0;
        var mat = materials[index];
        GetComponent<MeshRenderer>().material = mat;
        material = GetComponent<MeshRenderer>().material;
    }

    public void Start()
    {
        focusManager = FindObjectOfType<FocusManager>();
        material = GetComponent<MeshRenderer>().material;
    }

    public void Update()
    {
        if (index == 2)
        {
            material.SetVector("_FocusNormal", focusManager.focusNormal);
            material.SetVector("_FocusPosition", focusManager.focusPosition);
        }
        
    }
}
