using UnityEngine;

public class VolumeStartup : MonoBehaviour {
    
    [SerializeField] private int _noiseSize = 64;
    private Texture3D _noiseTex;

    void Start() 
    {
        _noiseTex = CreateRandom3DTexture(_noiseSize);
        Shader.SetGlobalTexture("_Random3D", _noiseTex);
    }

    Texture3D CreateRandom3DTexture(int size)
    {
        Texture3D randomNoise3dTex = new Texture3D(size, size, size, TextureFormat.RFloat, false);
        randomNoise3dTex.wrapMode = TextureWrapMode.Repeat;

        int count = size * size * size;
        float[] data = new float[count];
        
        System.Random rng = new System.Random();
        for (int i = 0; i < count; i++)
            data[i] = (float)rng.NextDouble();
        
        randomNoise3dTex.SetPixelData(data, 0);
        randomNoise3dTex.Apply();

        return randomNoise3dTex;
    }

}