using UnityEngine;

public class FrameRateController : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 60;
    }
}
