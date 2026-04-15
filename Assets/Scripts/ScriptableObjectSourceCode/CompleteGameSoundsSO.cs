using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CompleteGameSoundsSO", menuName = "ScriptableObjects/CompleteGameSoundsSO", order = 0)]
public class CompleteGameSoundsSO : ScriptableObject {

    public List<SoundGroup> soundEffectsAudioClips;
    public List<SoundGroup> environmentAudioClips;
    public List<SoundGroup> musicAudioClips;
    public List<SoundGroup> dialogueAudioClips;

}