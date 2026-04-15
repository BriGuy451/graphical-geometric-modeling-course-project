using System.Collections.Generic;
using UnityEngine;

public class SceneConfiguration :  MonoBehaviour {
    public SceneCurrent currentScene;
    public List<SceneCameraMap> sceneCameraMapList;
    public List<UIStateEntityToggleList> uiStateEntityToggleLists;
    public List<LoadingStateToggleList> uiLoadingStateEntityToggleLists;
}