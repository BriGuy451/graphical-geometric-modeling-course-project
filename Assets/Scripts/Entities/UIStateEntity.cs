using System;
using System.Collections.Generic;
using MoreMountains.Tools;

[Serializable]
public struct UIStateEntity
{
    public UIComplexStates uIComplexState;
    public MMToggleActive m_toggleActive;
}

[Serializable]
public struct UIStateEntityToggleList
{
    public UIComplexStates uIComplexState;
    public List<MMToggleActive> m_toggleActives;
}

[Serializable]
public struct LoadingStateToggleList
{
    public LoadingStates loadingState;
    public List<MMToggleActive> toggleActives;
}