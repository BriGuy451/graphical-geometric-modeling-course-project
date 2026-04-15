using NaughtyAttributes;
using UnityEngine;

public class ChangeSmokeColor : MonoBehaviour
{
    private Renderer[] _renderers;
    private Material[] _smokeMaterials;
    [SerializeField] private int CurrentColor = 0;

    void Start()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        _smokeMaterials = new Material[_renderers.Length];

        for (int i = 0; i < _renderers.Length; i++)
        {
            _smokeMaterials[i] = _renderers[i].material;
        }

        CurrentColor = _smokeMaterials[0].GetInteger("_LUTColorChoice");
    }

    [Button("Change Color", EButtonEnableMode.Playmode)]
    public void ChangeColor()
    {
        CurrentColor++;
        if (CurrentColor > 15)
            CurrentColor = 0;
        foreach (Material material in _smokeMaterials)
        {
            material.SetInteger("_LUTColorChoice", CurrentColor);
        }
    }

}
