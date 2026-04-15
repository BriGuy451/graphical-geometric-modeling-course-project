using System;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

// Fueled By SceneConfig GameObject
public class UIManager : MonoBehaviour, IGameManager {    
    public ManagerIdentifier managerIdentifier {get; private set;} = ManagerIdentifier.UIManager;

    private List<UIStateEntityToggleList> m_allUIStateEntities;
    private List<LoadingStateToggleList> m_allLoadingStateEntities;
    
    private MMStateMachine<UIViewShowingStates> m_UIViewShowingStateMachine;
    private MMStateMachine<UIComplexStates> m_UIComplexStatesMachine;
    private MMStateMachine<LoadingStates> m_loadingStatesStateMachine;

    private Dictionary<UIComplexStates, UIStateEntityToggleList> m_uiStateToToggleActivesList = new Dictionary<UIComplexStates, UIStateEntityToggleList>();

    private Dictionary<LoadingStates, LoadingStateToggleList> m_loadingStateToToggleActiveList = new Dictionary<LoadingStates, LoadingStateToggleList>(); 

    public UIComplexStates CurrentState => m_UIComplexStatesMachine != null ? m_UIComplexStatesMachine.CurrentState : UIComplexStates.None;
    public UIViewShowingStates CurrentStateView => m_UIViewShowingStateMachine != null ? m_UIViewShowingStateMachine.CurrentState : UIViewShowingStates.OFF;

    public ManagerStatus status { get; private set; } = ManagerStatus.Shutdown;

    public void StartUp()
    {
        LogFromMethod("StartUp");

        status = ManagerStatus.Initializing;
        m_UIViewShowingStateMachine = new MMStateMachine<UIViewShowingStates>(gameObject, true);
        m_UIComplexStatesMachine = new MMStateMachine<UIComplexStates>(gameObject, true);
        m_loadingStatesStateMachine = new MMStateMachine<LoadingStates>(gameObject, true);

        Messenger.AddListener(UIEvent.ON_FOCUS_ON, () =>
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        });
        
        Messenger.AddListener(UIEvent.ON_NONE, () =>
        {
            ToggleAllBesidesSpecified(new List<UIComplexStates> { UIComplexStates.None });

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

        });

        SceneSetup(GeomManager.Instance.SceneConfiguration);

        status = ManagerStatus.Started;
    }

    private void InitializeToggleDictionary()
    {
        foreach (UIStateEntityToggleList uiStateEntityToggleList in m_allUIStateEntities)
        {
            m_uiStateToToggleActivesList.Add(uiStateEntityToggleList.uIComplexState, uiStateEntityToggleList);
        }
    }
    
    private void InitializeLoadingToggleDictionary()
    {
        foreach (LoadingStateToggleList loadingStateToggleList in m_allLoadingStateEntities)
        {
            m_loadingStateToToggleActiveList.Add(loadingStateToggleList.loadingState, loadingStateToggleList);
        }
    }
    
    private void ClearToggleDictionary()
    {
        m_uiStateToToggleActivesList.Clear();
    }
    private void ClearLoadingToggleDictionary()
    {
        m_loadingStateToToggleActiveList.Clear();
    }

    private void ToggleAllBesidesSpecified(List<UIComplexStates> uiToNotToggle)
    {
        foreach (UIStateEntityToggleList uiStateEntity in m_allUIStateEntities)
        {
            bool shouldToggle = true;
            foreach (UIComplexStates uiComplexStateToNotToggle in uiToNotToggle)
            {
                if (uiComplexStateToNotToggle == uiStateEntity.uIComplexState)
                {
                    shouldToggle = false;
                    break;
                }
            }

            if (shouldToggle)
            {
                if (uiStateEntity.m_toggleActives != null && uiStateEntity.m_toggleActives.Count > 0)
                {
                    foreach (MMToggleActive toggleActive in uiStateEntity.m_toggleActives)
                    {
                        if (toggleActive == null)
                            continue;
                        if (toggleActive.TargetGameObject.activeInHierarchy)
                            toggleActive.ToggleActive();
                    }
                }
            }
        }
    }
    
    private void ToggleAllBesidesSpecified(List<LoadingStates> uiToNotToggle)
    {
        foreach (LoadingStateToggleList loadingStateToggleList in m_allLoadingStateEntities)
        {
            bool shouldToggle = true;
            foreach (LoadingStates loadingState in uiToNotToggle)
            {
                if (loadingState == loadingStateToggleList.loadingState)
                {
                    shouldToggle = false;
                    break;
                }
            }

            if (shouldToggle)
            {
                if (loadingStateToggleList.toggleActives != null && loadingStateToggleList.toggleActives.Count > 0)
                {
                    foreach (MMToggleActive toggleActive in loadingStateToggleList.toggleActives)
                    {
                        if (toggleActive == null)
                            continue;
                        if (toggleActive.TargetGameObject.activeInHierarchy)
                            toggleActive.ToggleActive();
                    }
                }
            }
        }
    }

    private void ToggleActiveList(UIStateEntityToggleList uiStateEntityToggleList)
    {
        if (uiStateEntityToggleList.m_toggleActives.Count > 0)
        {
            foreach (MMToggleActive toggleActive in uiStateEntityToggleList.m_toggleActives)
            {
                if (!toggleActive.TargetGameObject.activeInHierarchy)
                    toggleActive.ToggleActive();
            }
        }
    }
    
    private void ToggleActiveList(LoadingStateToggleList loadingStateToggleList)
    {
        if (loadingStateToggleList.toggleActives.Count > 0)
        {
            foreach (MMToggleActive toggleActive in loadingStateToggleList.toggleActives)
            {
                if (!toggleActive.TargetGameObject.activeInHierarchy)
                    toggleActive.ToggleActive();
            }
        }
    }

    public UIViewShowingStates GetCurrentStateViewShowing()
    {
        return m_UIViewShowingStateMachine.CurrentState;
    }
    public void ChangeStateViewShowing(UIViewShowingStates uiViewShowingState, bool bypassGuard = false)
    {
        if (m_UIViewShowingStateMachine.CurrentState == uiViewShowingState && !bypassGuard) return;

        m_UIViewShowingStateMachine.ChangeState(uiViewShowingState);

        switch (uiViewShowingState)
        {
            case UIViewShowingStates.FocusOn:
                Messenger.Broadcast(UIEvent.ON_FOCUS_ON, MessengerMode.DONT_REQUIRE_LISTENER);
                break;
            case UIViewShowingStates.NonFocusOn:
                Messenger.Broadcast(UIEvent.ON_NON_FOCUS_ON, MessengerMode.DONT_REQUIRE_LISTENER);
                break;
        }
    }
    
    public UIComplexStates GetCurrentStateComplexStates()
    {
        return m_UIComplexStatesMachine.CurrentState;
    }
    public void ChangeStateComplexView(UIComplexStates uiComplexStates)
    {
        if (m_UIComplexStatesMachine.CurrentState == uiComplexStates) return;

        m_UIComplexStatesMachine.ChangeState(uiComplexStates);

        switch (uiComplexStates)
        {
            case UIComplexStates.None:
                ChangeStateViewShowing(UIViewShowingStates.NonFocusOn);
                Messenger.Broadcast(UIEvent.ON_NONE, MessengerMode.DONT_REQUIRE_LISTENER);
                break;

            case UIComplexStates.MainMenuScreen:
                ChangeStateViewShowing(UIViewShowingStates.FocusOn);
                Messenger.Broadcast(UIEvent.ON_MAIN_MENU, MessengerMode.DONT_REQUIRE_LISTENER);
                break;
            case UIComplexStates.PlayerCreateHostRoomFormScreen:
                ChangeStateViewShowing(UIViewShowingStates.FocusOn);
                Messenger.Broadcast(UIEvent.ON_PLAYER_CREATE_HOST_ROOM_FORM, MessengerMode.DONT_REQUIRE_LISTENER);
                break;
            case UIComplexStates.PlayerClientRoomListScreen:
                ChangeStateViewShowing(UIViewShowingStates.FocusOn);
                Messenger.Broadcast(UIEvent.ON_PLAYER_CLIENT_ROOM_LIST_SCREEN, MessengerMode.DONT_REQUIRE_LISTENER);
                break;
            case UIComplexStates.PlayerLobbyHostRoomScreen:
                ChangeStateViewShowing(UIViewShowingStates.FocusOn);
                Messenger.Broadcast(UIEvent.ON_PLAYER_LOBBY_HOST_ROOM, MessengerMode.DONT_REQUIRE_LISTENER);
                break;
            case UIComplexStates.PlayerLobbyClientRoomScreen:
                ChangeStateViewShowing(UIViewShowingStates.FocusOn);
                Messenger.Broadcast(UIEvent.ON_PLAYER_LOBBY_CLIENT_ROOM, MessengerMode.DONT_REQUIRE_LISTENER);
                break;

            case UIComplexStates.MainMenuScreenGeom:
                ChangeStateViewShowing(UIViewShowingStates.FocusOn);
                Messenger.Broadcast(UIEvent.ON_MAIN_MENU_SCREEN_GEOM, MessengerMode.DONT_REQUIRE_LISTENER);
                break;
            case UIComplexStates.TitleOnGeom:
                ChangeStateViewShowing(UIViewShowingStates.FocusOn);
                Messenger.Broadcast(UIEvent.ON_TITLE_ON_GEOM, MessengerMode.DONT_REQUIRE_LISTENER);
                break;
            case UIComplexStates.TunerVisibleOnGeom:
                ChangeStateViewShowing(UIViewShowingStates.FocusOn);
                Messenger.Broadcast(UIEvent.ON_TUNER_VISIBLE_ON_GEOM, MessengerMode.DONT_REQUIRE_LISTENER);
                break;
            case UIComplexStates.TunerVisibleOffGeom:
                ChangeStateViewShowing(UIViewShowingStates.FocusOn);
                Messenger.Broadcast(UIEvent.ON_TUNER_VISIBLE_OFF_GEOM, MessengerMode.DONT_REQUIRE_LISTENER);
                break;
        }
    }
    public void ChangeStateLoadingState(LoadingStates loadingState, bool bypassGuard = false)
    {
        if (!bypassGuard && m_loadingStatesStateMachine.CurrentState == loadingState) return;

        m_loadingStatesStateMachine.ChangeState(loadingState);

        switch (loadingState)
        {
            case LoadingStates.None:
                Messenger.Broadcast(UIEvent.ON_LOADING_STATE_NONE, MessengerMode.DONT_REQUIRE_LISTENER);
                break;

            case LoadingStates.BlockInputOverlay:
                Messenger.Broadcast(UIEvent.ON_BLOCK_INPUT_OVERLAY, MessengerMode.DONT_REQUIRE_LISTENER);
                break;
        }
    }

    public void SubscribeEvent(LoadingStates loadingState)
    {
        switch (loadingState)
        {
            case LoadingStates.BlockInputOverlay:
                LogFromMethod("Subscribing BlockInputOverlay");
                Messenger.AddListener(UIEvent.ON_BLOCK_INPUT_OVERLAY, GetToggleActiveAction(LoadingStates.BlockInputOverlay));
                break;
            case LoadingStates.None:
                LogFromMethod("Subscribing LoadingStateNone");
                Messenger.AddListener(UIEvent.ON_LOADING_STATE_NONE, GetToggleClearAction(LoadingStates.None));
                break;
        }
    }

    public void SubscribeEvent(UIComplexStates complexState)
    {
        switch (complexState)
        {
            case UIComplexStates.MainMenuScreen:
                LogFromMethod("Subscription Internal MainMenuScreen");
                Messenger.AddListener(UIEvent.ON_MAIN_MENU, GetToggleActiveAction(UIComplexStates.MainMenuScreen));
                break;
            case UIComplexStates.PlayerCreateHostRoomFormScreen:
                LogFromMethod("Subscription Internal PlayerCreateHostRoomFormScreen");
                Messenger.AddListener(UIEvent.ON_PLAYER_CREATE_HOST_ROOM_FORM, GetToggleActiveAction(UIComplexStates.PlayerCreateHostRoomFormScreen));
                break;
            case UIComplexStates.PlayerClientRoomListScreen:
                LogFromMethod("Subscription Internal PlayerClientRoomListScreen");
                Messenger.AddListener(UIEvent.ON_PLAYER_CLIENT_ROOM_LIST_SCREEN, GetToggleActiveAction(UIComplexStates.PlayerClientRoomListScreen));
                break;
            case UIComplexStates.PlayerLobbyHostRoomScreen:
                LogFromMethod("Subscription Internal PlayerLobbyHostRoomScreen");
                Messenger.AddListener(UIEvent.ON_PLAYER_LOBBY_HOST_ROOM, GetToggleActiveAction(UIComplexStates.PlayerLobbyHostRoomScreen));
                break;
            case UIComplexStates.PlayerLobbyClientRoomScreen:
                LogFromMethod("Subscription Internal PlayerLobbyClientRoomScreen");
                Messenger.AddListener(UIEvent.ON_PLAYER_LOBBY_CLIENT_ROOM, GetToggleActiveAction(UIComplexStates.PlayerLobbyClientRoomScreen));
                break;

            case UIComplexStates.MainMenuScreenGeom:
                LogFromMethod("Subscription Internal ON_MAIN_MENU_SCREEN_GEOM");
                Messenger.AddListener(UIEvent.ON_MAIN_MENU_SCREEN_GEOM, GetToggleActiveAction(UIComplexStates.MainMenuScreenGeom));
                break;
            case UIComplexStates.TitleOnGeom:
                LogFromMethod("Subscription Internal ON_TITLE_ON_GEOM");
                Messenger.AddListener(UIEvent.ON_TITLE_ON_GEOM, GetToggleActiveAction(UIComplexStates.TitleOnGeom));
                break;
            case UIComplexStates.TunerVisibleOnGeom:
                LogFromMethod("Subscription Internal ON_TUNER_VISIBLE_ON_GEOM");
                Messenger.AddListener(UIEvent.ON_TUNER_VISIBLE_ON_GEOM, GetToggleActiveAction(UIComplexStates.TunerVisibleOnGeom));
                break;
            case UIComplexStates.TunerVisibleOffGeom:
                LogFromMethod("Subscription Internal ON_TUNER_VISIBLE_OFF_GEOM");
                Messenger.AddListener(UIEvent.ON_TUNER_VISIBLE_OFF_GEOM, GetToggleActiveAction(UIComplexStates.TunerVisibleOffGeom));
                break;
        }
    }

    public void UnsubscribeEvent(UIComplexStates complexState)
    {
        // This needs to be tested
        switch (complexState)
        {
            case UIComplexStates.MainMenuScreen:
                LogFromMethod("Unsubscribing All Subscribers MainMenuScreen");
                Messenger.RemoveListener(UIEvent.ON_MAIN_MENU, GetToggleActiveAction(UIComplexStates.MainMenuScreen));
                break;
            case UIComplexStates.PlayerCreateHostRoomFormScreen:
                LogFromMethod("Unsubscribing All Subscribers PlayerCreateHostRoomFormScreen");
                Messenger.RemoveListener(UIEvent.ON_PLAYER_CREATE_HOST_ROOM_FORM, GetToggleActiveAction(UIComplexStates.PlayerCreateHostRoomFormScreen));
                break;
            case UIComplexStates.PlayerClientRoomListScreen:
                LogFromMethod("Unsubscribing All Subscribers PlayerClientRoomListScreen");
                Messenger.RemoveListener(UIEvent.ON_PLAYER_CLIENT_ROOM_LIST_SCREEN, GetToggleActiveAction(UIComplexStates.PlayerClientRoomListScreen));
                break;
            case UIComplexStates.PlayerLobbyHostRoomScreen:
                LogFromMethod("Unsubscribing All Subscribers PlayerLobbyHostRoomScreen");
                Messenger.RemoveListener(UIEvent.ON_PLAYER_LOBBY_HOST_ROOM, GetToggleActiveAction(UIComplexStates.PlayerLobbyHostRoomScreen));
                break;
            case UIComplexStates.PlayerLobbyClientRoomScreen:
                LogFromMethod("Unsubscribing All Subscribers PlayerLobbyClientRoomScreen");
                Messenger.RemoveListener(UIEvent.ON_PLAYER_LOBBY_CLIENT_ROOM, GetToggleActiveAction(UIComplexStates.PlayerLobbyClientRoomScreen));
                break;

            case UIComplexStates.MainMenuScreenGeom:
                LogFromMethod("Subscription Internal ON_MAIN_MENU_SCREEN_GEOM");
                Messenger.RemoveListener(UIEvent.ON_MAIN_MENU_SCREEN_GEOM, GetToggleActiveAction(UIComplexStates.MainMenuScreenGeom));
                break;
            case UIComplexStates.TitleOnGeom:
                LogFromMethod("Subscription Internal ON_TITLE_ON_GEOM");
                Messenger.RemoveListener(UIEvent.ON_TITLE_ON_GEOM, GetToggleActiveAction(UIComplexStates.TitleOnGeom));
                break;
            case UIComplexStates.TunerVisibleOnGeom:
                LogFromMethod("Subscription Internal ON_TUNER_VISIBLE_ON_GEOM");
                Messenger.RemoveListener(UIEvent.ON_TUNER_VISIBLE_ON_GEOM, GetToggleActiveAction(UIComplexStates.TunerVisibleOnGeom));
                break;
            case UIComplexStates.TunerVisibleOffGeom:
                LogFromMethod("Subscription Internal ON_TUNER_VISIBLE_OFF_GEOM");
                Messenger.RemoveListener(UIEvent.ON_TUNER_VISIBLE_OFF_GEOM, GetToggleActiveAction(UIComplexStates.TunerVisibleOffGeom));
                break;
        }
    }

    public void UnsubscribeEvent(LoadingStates loadingState)
    {
        switch (loadingState)
        {
            case LoadingStates.BlockInputOverlay:
                LogFromMethod("Unsubscribing All Subscribers BlockInputOverlay");
                Messenger.RemoveListener(UIEvent.ON_BLOCK_INPUT_OVERLAY, GetToggleActiveAction(LoadingStates.BlockInputOverlay));
                break;
            case LoadingStates.None:
                LogFromMethod("Unsubscribing All Subscribers LoadingStateNone");
                Messenger.RemoveListener(UIEvent.ON_LOADING_STATE_NONE, GetToggleClearAction(LoadingStates.None));
                break;
        }
    }

    public Action GetToggleActiveAction(UIComplexStates complexState)
    {
        return () =>
        {
            ToggleAllBesidesSpecified(new List<UIComplexStates> { complexState });

            ToggleActiveList(m_uiStateToToggleActivesList[complexState]);
        };
    }
    public Action GetToggleActiveAction(LoadingStates loadingState)
    {
        return () =>
        {
            ToggleAllBesidesSpecified(new List<LoadingStates> { loadingState });

            ToggleActiveList(m_loadingStateToToggleActiveList[loadingState]);
        };
    }
    public Action GetToggleClearAction(LoadingStates loadingState)
    {
        return () =>
        {
            ToggleAllBesidesSpecified(new List<LoadingStates> { loadingState });
        };
    }

    public void Enable()
    {
        enabled = true;
    }
    public void Disable()
    {
        enabled = false;
    }

    public void SceneSetup(SceneConfiguration sceneConfiguration)
    {
        LogFromMethod("SceneSetup");

        m_allUIStateEntities = sceneConfiguration.uiStateEntityToggleLists;

        m_allLoadingStateEntities = sceneConfiguration.uiLoadingStateEntityToggleLists;
        LogFromMethod($"{m_allUIStateEntities.ToString()}");

        ClearToggleDictionary();
        ClearLoadingToggleDictionary();


        if (m_allUIStateEntities.Count > 0)
            InitializeToggleDictionary();
        if (m_allLoadingStateEntities.Count > 0)
            InitializeLoadingToggleDictionary();

        foreach (UIComplexStates complexState in Enum.GetValues(typeof(UIComplexStates)))
        {
            if (m_uiStateToToggleActivesList.ContainsKey(complexState))
            {
                // subscribe
                SubscribeEvent(complexState);
            }
            else
            {
                // unsubscribe
                try {
                    UnsubscribeEvent(complexState);
                } catch (Exception ex){ LogFromMethod(ex.Message); }
            }
        }

        foreach (LoadingStates loadingState in Enum.GetValues(typeof(LoadingStates)))
        {
            if (m_loadingStateToToggleActiveList.ContainsKey(loadingState))
            {
                // subscribe
                SubscribeEvent(loadingState);
            }
            else
            {
                // unsubscribe
                try {
                    UnsubscribeEvent(loadingState);
                } catch (Exception ex){ LogFromMethod(ex.Message); }
            }
        }
        
    }

    public void SceneSetupAdditive(SceneConfiguration sceneConfiguration)
    {
        LogFromMethod("SceneSetupAdditive");
    }

    private void LogFromMethod(string message)
    {
        print($"UIManager:{message}");
    }
}