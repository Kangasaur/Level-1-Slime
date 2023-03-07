 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMaterials : MonoBehaviour
{
    Renderer r;
    [SerializeField] Material mat;
    [SerializeField] Material targetMat;
    [SerializeField] Material selectedMat;

    bool target;
    bool select;

    private void Awake()
    {
        r = GetComponent<Renderer>();
    }

    public void Select(bool isSelected)
    {
        select = isSelected;
        UpdateMaterial();
    }

    public void Target(bool isTargeted)
    {
        target = isTargeted;
        UpdateMaterial();
    }

    private void UpdateMaterial()
    {
        if (select) r.material = selectedMat;
        else r.material = target ? targetMat : mat;
    }
}
