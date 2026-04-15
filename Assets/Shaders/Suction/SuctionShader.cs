using NaughtyAttributes;
using UnityEngine;

public class SuctionShader : MonoBehaviour {

    [SerializeField] private Mesh mesh;
    [SerializeField] private Material suctionMaterial;

    private float startValue = 0f;
    private float endValue = 1f;

    private float interpolationStartValue = 0.15f;
    [SerializeField][Range(0f, 1f)] private float interpolationValue = 0.02f;
    [SerializeField][Range(0f, .5f)] private float interpolationStepValue = 0.1f;

    private bool runForward = false;
    private bool runBackward = false;


    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        suctionMaterial = GetComponent<MeshRenderer>().materials[0];
        interpolationStartValue = interpolationValue;
    }

    private void Update()
    {
        if (runForward)
        {
            float interimValue = Mathf.Lerp(startValue, endValue, interpolationValue);
            suctionMaterial.SetFloat("_Threshold", interimValue);

            if (suctionMaterial.GetFloat("_Threshold") == 1.0f)
            {
                interpolationValue = interpolationStartValue;
                runForward = false;
                return;
            }
            else
            {
                interpolationValue += interpolationStepValue * Time.deltaTime;
            }
        }
        else if (runBackward)
        {
            float interimValue = Mathf.Lerp(endValue, startValue, interpolationValue);
            suctionMaterial.SetFloat("_Threshold", interimValue);

            if (suctionMaterial.GetFloat("_Threshold") == 0f)
            {
                interpolationValue = interpolationStartValue;
                runBackward = false;
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

    private void LogFromMethod(string message)
    {
        Debug.Log($"DissolveShader:{message}");
    }
}