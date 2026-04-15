using MoreMountains.Tools;
using UnityEngine;

public class SoundOptions
{
    public static MMSoundManagerPlayOptions PlayerDefault
    {
        get
        {
            MMSoundManagerPlayOptions defaultOptions = new MMSoundManagerPlayOptions();
            defaultOptions.Initialized = true;
            defaultOptions.MmSoundManagerTrack = MMSoundManager.MMSoundManagerTracks.Sfx;
            defaultOptions.Location = Vector3.zero;
            defaultOptions.Loop = false;
            defaultOptions.Volume = .05f;
            defaultOptions.ID = 0;
            defaultOptions.Fade = false;
            defaultOptions.FadeInitialVolume = 0f;
            defaultOptions.FadeDuration = 1f;
            defaultOptions.FadeTween = MMTweenType.DefaultEaseInCubic;
            defaultOptions.Persistent = false;
            defaultOptions.RecycleAudioSource = null;
            defaultOptions.AudioGroup = null;
            defaultOptions.Pitch = 1f;
            defaultOptions.PanStereo = 0f;
            defaultOptions.SpatialBlend = 1.0f;
            defaultOptions.SoloSingleTrack = false;
            defaultOptions.SoloAllTracks = false;
            defaultOptions.AutoUnSoloOnEnd = false;
            defaultOptions.BypassEffects = false;
            defaultOptions.BypassListenerEffects = false;
            defaultOptions.BypassReverbZones = false;
            defaultOptions.Priority = 128;
            defaultOptions.ReverbZoneMix = 1f;
            defaultOptions.DopplerLevel = 1f;
            defaultOptions.Spread = 0;
            defaultOptions.RolloffMode = AudioRolloffMode.Logarithmic;
            defaultOptions.MinDistance = 1f;
            defaultOptions.MaxDistance = 25f;
            defaultOptions.DoNotAutoRecycleIfNotDonePlaying = true;
            return defaultOptions;
        }
    }
    
    public static MMSoundManagerPlayOptions EnemyDefault
    {
        get
        {
            MMSoundManagerPlayOptions defaultOptions = new MMSoundManagerPlayOptions();
            defaultOptions.Initialized = true;
            defaultOptions.MmSoundManagerTrack = MMSoundManager.MMSoundManagerTracks.Sfx;
            defaultOptions.Location = Vector3.zero;
            defaultOptions.Loop = false;
            defaultOptions.Volume = .5f;
            defaultOptions.ID = 5;
            defaultOptions.Fade = false;
            defaultOptions.FadeInitialVolume = 0f;
            defaultOptions.FadeDuration = 1f;
            defaultOptions.FadeTween = MMTweenType.DefaultEaseInCubic;
            defaultOptions.Persistent = false;
            defaultOptions.RecycleAudioSource = null;
            defaultOptions.AudioGroup = null;
            defaultOptions.Pitch = 1f;
            defaultOptions.PanStereo = 0f;
            defaultOptions.SpatialBlend = 1.0f;
            defaultOptions.SoloSingleTrack = false;
            defaultOptions.SoloAllTracks = false;
            defaultOptions.AutoUnSoloOnEnd = false;
            defaultOptions.BypassEffects = false;
            defaultOptions.BypassListenerEffects = false;
            defaultOptions.BypassReverbZones = false;
            defaultOptions.Priority = 128;
            defaultOptions.ReverbZoneMix = 1f;
            defaultOptions.DopplerLevel = 1f;
            defaultOptions.Spread = 0;
            defaultOptions.RolloffMode = AudioRolloffMode.Logarithmic;
            defaultOptions.MinDistance = 1f;
            defaultOptions.MaxDistance = 50f;
            defaultOptions.DoNotAutoRecycleIfNotDonePlaying = true;
            return defaultOptions;
        }
    }
}