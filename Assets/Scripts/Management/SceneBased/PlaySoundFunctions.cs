using System;
using MoreMountains.Tools;
using UnityEngine;

public class PlaySoundFunctions : MonoBehaviour {
    
    public void PlayClickSound()
    {
        Tuple<int, AudioClip> idWithAudioClip = MainManager.GameSoundStore.GetAudioClipAndAudioId("UI:MENU_BUTTONS_3");

        MMSoundManagerPlayOptions options = MMSoundManagerPlayOptions.Default;
        options.ID = idWithAudioClip.Item1;
        options.MmSoundManagerTrack = MMSoundManager.MMSoundManagerTracks.UI;

        MMSoundManagerSoundPlayEvent.Trigger(idWithAudioClip.Item2, options);
    }

}