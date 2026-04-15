using System.Collections.Generic;
using UnityEngine;

public static class LayerUtils
{
    public static List<string> GetAllLayers()
    {
        List<string> layers = new List<string>();
        for (int i = 0; i < 32; i++)
        {
            string layerName = LayerMask.LayerToName(i);
            if (!string.IsNullOrEmpty(layerName))
            {
                layers.Add(layerName);
            }
        }
        return layers;
    }
}