using System;
using CustomTween;
using UnityEngine;

/// <summary>
/// Owns application-level flow only. Concrete gameplay lives outside the Starter Kit
/// and can subscribe to the lifecycle callbacks below.
/// </summary>
public class GameManager : SingletonDontDestroy<GameManager>
{
    public GameState gameState;

    public static event Action GameStarted;
    public static event Action GameReturnedHome;
    public static event Action GameReplayRequested;

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
        Input.multiTouchEnabled = true;
        CustomTweenConfig.warnZeroDuration = false;
    }

    private void Start()
    {
        ReturnHome();
    }

    public void ReturnHome()
    {
        gameState = GameState.Home;

        SoundController.Instance.PlayBackground(SoundName.HomeBackgroundMusic);
        PopupController.Instance.HideAll();
        PopupController.Instance.Show<PopupBackground>();
        PopupController.Instance.Show<PopupHome>();

        GameReturnedHome?.Invoke();
    }

    public void StartGame()
    {
        gameState = GameState.PlayingGame;

        SoundController.Instance.PlayBackground(SoundName.InGameBackgroundMusic);
        PopupController.Instance.HideAll();
        PopupController.Instance.Show<PopupInGame>();

        GameStarted?.Invoke();
    }

    public void ReplayGame()
    {
        GameReplayRequested?.Invoke();
        StartGame();
    }

    public void OnWinGame(float delayPopupShowTime = 2.5f)
    {
        if (IsShowingResult()) return;
        gameState = GameState.WinGame;

        Sequence.Create().ChainDelay(delayPopupShowTime).ChainCallback(() =>
        {
            PopupController.Instance.HideAll();
            if (PopupController.Instance.Get<PopupWin>() is PopupWin popupWin)
            {
                popupWin.Show();
            }
        });
    }

    public void OnLoseGame(float delayPopupShowTime = 2.5f)
    {
        if (IsShowingResult()) return;
        gameState = GameState.LoseGame;

        Sequence.Create().ChainDelay(delayPopupShowTime).ChainCallback(() =>
        {
            PopupController.Instance.Hide<PopupInGame>();
            PopupController.Instance.Show<PopupLose>();
        });
    }

    private bool IsShowingResult()
    {
        return gameState == GameState.WaitingResult ||
               gameState == GameState.LoseGame ||
               gameState == GameState.WinGame;
    }
}

public enum GameState
{
    Home,
    PlayingGame,
    WaitingResult,
    LoseGame,
    WinGame,
}
