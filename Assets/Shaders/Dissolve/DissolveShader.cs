using NaughtyAttributes;
using UnityEngine;

public class DissolveShader : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material dissolveMaterial;

    private float startValue = 0f;
    private float endValue = 1f;

    private float interpolationStartValue = 0.15f;
    [SerializeField][Range(0f, 1f)] private float interpolationValue = 0.02f;
    [SerializeField][Range(0f, .5f)] private float interpolationStepValue = 0.1f;

    private bool runForward = false;
    private bool runBackward = false;
    private bool isLooping = false;
    private bool isPaused = false;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        dissolveMaterial = GetComponent<MeshRenderer>().materials[0];

        Bounds bounds = mesh.bounds;

        LogFromMethod($"_MinY:{bounds.min.y}");
        LogFromMethod($"_MaxY:{bounds.max.y}");

        LogFromMethod($"{dissolveMaterial.HasProperty("_MinY")}");
        LogFromMethod($"{dissolveMaterial.HasProperty("_MaxY")}");
        
        dissolveMaterial.SetFloat("_MinY", bounds.max.y);
        dissolveMaterial.SetFloat("_MaxY", bounds.min.y - .5f);

        LogFromMethod($"{dissolveMaterial.GetFloat("_MinY")}");
        LogFromMethod($"{dissolveMaterial.GetFloat("_MaxY")}");

        interpolationStartValue = interpolationValue;
    }

    private void Update() {
        if (isPaused)
            return;

        if (runForward)
        {
            float interimValue = Mathf.Lerp(startValue, endValue, interpolationValue);
            dissolveMaterial.SetFloat("_Threshold" ,interimValue);

            if (dissolveMaterial.GetFloat("_Threshold") == 1.0f)
            {
                interpolationValue = interpolationStartValue;
                runForward = false;
                if (isLooping)
                {
                    SetRunBackward();
                }
                return;
            } else
            {
                interpolationValue += interpolationStepValue * Time.deltaTime;
            }
        } else if (runBackward)
        {
            float interimValue = Mathf.Lerp(endValue, startValue, interpolationValue);
            dissolveMaterial.SetFloat("_Threshold", interimValue);

            if (dissolveMaterial.GetFloat("_Threshold") == 0f)
            {
                interpolationValue = interpolationStartValue;
                runBackward = false;
                if (isLooping)
                {
                    SetRunForward();
                }
                return;
            }
            else
            {
                interpolationValue += interpolationStepValue * Time.deltaTime;
            }
        }
    }

    [Button("RunForward", EButtonEnableMode.Playmode)]
    private void SetRunForward()
    {
        runBackward = false;
        runForward = true;
    }
    [Button("RunBackward", EButtonEnableMode.Playmode)]
    private void SetRunBackward()
    {
        runForward = false;
        runBackward = true;
    }
    [Button("Loop", EButtonEnableMode.Playmode)]
    private void RunLooping()
    {
        isLooping = !isLooping;
    }
    
    [Button("Play", EButtonEnableMode.Playmode)]
    private void Play()
    {
        isPaused = false;
    }
    private void Pause()
    {
        isPaused = true;
    }

    private void LogFromMethod(string message)
    {
        Debug.Log($"DissolveShader:{message}");
    }
}