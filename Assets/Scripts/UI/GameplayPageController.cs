using UniRx;
using TMPro;
using System;
using Zenject;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Services;
using Assets.Scripts.UI.Interfaces;
using Assets.Scripts.Services.Interfaces;

/// <summary>
/// Ñlass encapsulates the main page logic, visualise the timer, counter view. Allows to start and stop the game
/// </summary>
public class GameplayPageController : MonoBehaviour, IGameplayPageObservable
{
    private const string START_GAME_LABEL = "Start Game";
    private const string STOP_GAME_LABEL = "Stop Game";
    
    private bool _isGameStarted;

    [SerializeField] private Button _startGameButton;
    [SerializeField] private TMP_Text _counterLabel;
    [SerializeField] private TMP_Text _timerLabel;
    [SerializeField] private TMP_Text _buttonLabel;

    private ICounterObservable _counterService;
    private ITimerObservable _timerService;
    private Subject<bool> _onGameStartButtonSubject = new();
    public IObservable<bool> OnGameStartButtonObservable => _onGameStartButtonSubject;

    /// <summary>
    /// Injecting 
    /// </summary>
    /// <param name="counterService">Counter for displaying data on UI</param>
    /// <param name="timerService">Timer for displaying data on UI</param>
    /// <param name="gameStateObservable">Observable allowing you to subscribe to the start or stop state of the game</param>
    [Inject]
    private void Constructor(ICounterObservable counterService, ITimerObservable timerService, IGameStateObservable gameStateObservable)
    {
        _counterService = counterService;
        _timerService = timerService;
        gameStateObservable.OnGameStateObservable.Subscribe(OnSetGameState).AddTo(this);
    }

    /// <summary>
    /// Awake monobehaviour. Setup subscriptions
    /// </summary>
    private void Awake()
    {
        SetupSubscriptions();
    }

    /// <summary>
    /// Subscribe to events
    /// </summary>
    private void SetupSubscriptions()
    {
        _startGameButton.onClick.AsObservable().Subscribe(HandleButtonClick).AddTo(this);
        _counterService.Counter.Subscribe(UpdateCounter).AddTo(this);
        _timerService.GameplayTime.Subscribe(UpdateTimer).AddTo(this);
    }

    /// <summary>
    /// Sets the counter value
    /// </summary>
    /// <param name="count">Amount of clicks</param>
    private void UpdateCounter(int count) => _counterLabel.text = count.ToString();

    /// <summary>
    /// Sets the timer value
    /// </summary>
    /// <param name="seconds">Timer value</param>
    private void UpdateTimer(int seconds) => _timerLabel.text = ConvertValue(seconds);

    /// <summary>
    /// Formats the time output in the required view
    /// </summary>
    /// <param name="seconds">Amount of seconds</param>
    /// <returns>Returns string in MM:SS format</returns>
    private string ConvertValue(int seconds) => $"{(seconds / 60).ToString("D2")}:{(seconds %= 60).ToString("D2")}";

    /// <summary>
    /// Handler for the start/stop game button click
    /// </summary>
    /// <param name="unit"></param>
    private void HandleButtonClick(Unit unit)
    {
        if (!_isGameStarted)
        {
            OnGameStart();
        }
        else
        {
            StopGameHandler();
        }
    }

    /// <summary>
    /// Invokes start game logic
    /// </summary>
    private void OnGameStart()
    {
        _buttonLabel.text = STOP_GAME_LABEL;
        _onGameStartButtonSubject?.OnNext(true);
    }

    /// <summary>
    /// Invokes stop game logic
    /// </summary>
    private void StopGameHandler()
    {
        _buttonLabel.text = START_GAME_LABEL;
        _onGameStartButtonSubject?.OnNext(false);
    }
    
    /// <summary>
    /// Sets the game state
    /// </summary>
    /// <param name="isGameStarted">True if the game started, false if not</param>
    private void OnSetGameState(bool isGameStarted) => _isGameStarted = isGameStarted;
}
