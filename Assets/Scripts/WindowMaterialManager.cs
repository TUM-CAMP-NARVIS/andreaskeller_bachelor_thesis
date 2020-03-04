using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowMaterialManager : MonoBehaviour
{
    public List<Material> materials;
    private FocusManager focusManager;

    public int index = 0;
    private int curIndex = 0;
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
        if (curIndex != index)
        {
            GetComponent<MeshRenderer>().material = materials[index];
            material = GetComponent<MeshRenderer>().material;
            curIndex = index;
        }
        if (index == 2 || index == 3)
        {
            material.SetVector("_FocusNormal", focusManager.focusNormal);
            material.SetVector("_FocusPosition", focusManager.focusPosition);
        }
        
    }

    public void SetMaterial(Material m)
    {
        index = 0;
        curIndex = 0;
        GetComponent<MeshRenderer>().material = m;
    }
}
