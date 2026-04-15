using MoreMountains.Tools;
using UnityEngine;

public class CameraFollow : MMSingleton<CameraFollow>
{
    private Transform target;
    private GameObject owningGameObject;

    private void LateUpdate()
    {
        print($"CameraFollow:Target {target}");
        if (target != null)
        {
            transform.SetPositionAndRotation(target.position, target.rotation);
        }
    }

    public void SetTarget(Transform newTarget, GameObject owner)
    {
        target = newTarget;
        this.owningGameObject = owner;
    }
}
