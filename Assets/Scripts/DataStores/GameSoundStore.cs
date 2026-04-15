
using UnityEngine;
using MoreMountains.Tools;
using System.Collections.Generic;
using System;

public class GameSoundStore : MonoBehaviour, IGameManager
{
    [SerializeField] private CompleteGameSoundsSO completeGameSoundsSO;
    public ManagerIdentifier managerIdentifier { get; private set; } = ManagerIdentifier.GameSoundStore;

    private Dictionary<string, AudioClip> m_labelNameToAudioClip;
    private Dictionary<string, int> m_labelNameToAudioClipId;
    private int m_startId = 1;

    public ManagerStatus status { get; private set; } = ManagerStatus.Shutdown;

    public void StartUp()
    {
        status = ManagerStatus.Initializing;
        LogFromMethod("StartUp");
        
        if (completeGameSoundsSO == null)
        {
            LogFromMethod("CompleteGameSoundsSO is null");
            status = ManagerStatus.Started;
            return;
        }

        m_labelNameToAudioClip = new Dictionary<string, AudioClip>();
        m_labelNameToAudioClipId = new Dictionary<string, int>();

        if (completeGameSoundsSO.soundEffectsAudioClips.Count > 0)
            AddLabelToAudioClipsToDictionary(completeGameSoundsSO.soundEffectsAudioClips);

        if (completeGameSoundsSO.environmentAudioClips.Count > 0)
            AddLabelToAudioClipsToDictionary(completeGameSoundsSO.environmentAudioClips);

        if (completeGameSoundsSO.musicAudioClips.Count > 0)
            AddLabelToAudioClipsToDictionary(completeGameSoundsSO.musicAudioClips);

        if (completeGameSoundsSO.dialogueAudioClips.Count > 0)
            AddLabelToAudioClipsToDictionary(completeGameSoundsSO.dialogueAudioClips);
        
        status = ManagerStatus.Started;
    }

    private void AddLabelToAudioClipsToDictionary(List<SoundGroup> soundGroups)
    {
        foreach (SoundGroup soundGroup in soundGroups)
        {
            if (soundGroup.labelToAudioClips.Count > 0)
            {
                string audioClipPrefix = soundGroup.categoryLabel;

                foreach (LabelToAudioClip labelToAudioClip in soundGroup.labelToAudioClips)
                {
                    string keyName = $"{audioClipPrefix}:{labelToAudioClip.label}";
                    m_labelNameToAudioClip[keyName] = labelToAudioClip.audioClip; 

                    m_labelNameToAudioClipId[keyName] = m_startId;
                }
            }
        }
    }

    public Tuple<int,AudioClip> GetAudioClipAndAudioId(string key)
    {   
        
        AudioClip clipToReturn = null;
        int audioId = -1;

        print($"audioclip key: {key}");

        if (m_labelNameToAudioClip.ContainsKey(key))
        {
            clipToReturn = m_labelNameToAudioClip[key];
            audioId = m_labelNameToAudioClipId[key];
        }

        
        print($"audioclip: {clipToReturn}");

        Tuple<int, AudioClip> idWithAudioClipTuple = new Tuple<int, AudioClip>(audioId, clipToReturn);

        return idWithAudioClipTuple;
        
    }

    public void SceneSetup(SceneConfiguration sceneConfiguration)
    {
        LogFromMethod("SceneSetup");
    }
    public void SceneSetupAdditive(SceneConfiguration sceneConfiguration)
    {
        LogFromMethod("SceneSetupAdditive");
    }

    public void Enable()
    {
        enabled = true;
    }
    public void Disable()
    {
        enabled = false;
    }

    private void LogFromMethod(string message)
    {
        print($"GameSoundStore:{message}");
    }

}