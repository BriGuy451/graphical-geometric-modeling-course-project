using System;
using System.Collections.Generic;
using MoreMountains.Tools;
using Cinemachine;
using UnityEngine;


[Serializable]
public struct SceneCameraMap
{
    public SceneCurrent currentScene;
    public Camera MainCamera;
    public List<CameraIdentifierToCamera> CameraIdentifierToCamera;
}

[Serializable]
public struct CameraIdentifierToCamera
{
    public CameraIdentifier cameraId;
    public CinemachineVirtualCamera cinemachineCamera;
    public List<MMToggleActive> toggleActives;
}