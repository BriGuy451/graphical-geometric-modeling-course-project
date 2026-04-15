using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneConfigurationSO", menuName = "ScriptableObjects/SceneConfigurationSO", order = 0)]
public class SceneConfigurationSO : ScriptableObject {
    
    /*
    Needs:
        - Camera Setup / ID Matching
        - UI Manager State Entities for Toggling On / Off
        
        - Does FunctionsContainer exist in all scenes or should it be a singleton and passed between scenes?
            - If it exist all scenes, my UI's will be connected fine? But what about UI's that are instantiated at runtime?

        How does UI that is instantiated at runtime deal with connected functionality to the functions container? If there is a button that exists, how does it get hooked up to the correct functions container method? It would require some type of SetupScript for that UI which could references something like the SceneConfigurationSO
    */

    public List<SceneCameraMap> sceneCameraMapList;
    public List<UIStateEntityToggleList> uIStateEntityToggleLists;

}