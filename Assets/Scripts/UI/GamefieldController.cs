using UniRx;
using Zenject;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Services;
using Assets.Scripts.Services.Interfaces;
using Assets.Scripts.UI.Interfaces;

/// <summary>
/// The class is responsible for the visual part of the playing field
/// </summary>
public class GamefieldController : MonoBehaviour, IGamefieldDataSource
{
    [SerializeField] private Transform _toggleContainer;
   
    private Toggle[] _toggles;
    private ICounter _counterService;
    private bool _isGameStarted;

    /// <summary>
    /// Injecting dependencies
    /// </summary>
    /// <param name="counterService">Counter service. Injected to control the increment logic after toggle clicking</param>
    /// <param name="gameStateObservable">Observable allowing you to subscribe to the start or stop state of the game</param>
    [Inject]
    private void Constructor(ICounter counterService, IGameStateObservable gameStateObservable)
    {
        _counterService = counterService;
        gameStateObservable.OnGameStateObservable.Subscribe(OnSetGameState).AddTo(this);
    }

    /// <summary>
    /// Awake monobehaviour. Setup UI elements and subscriptions
    /// </summary>
    private void Awake()
    {
        SetupToggles();
        SetupSubscriptions();
    }

    /// <summary>
    /// Returns toggles ftom the gamefield
    /// </summary>
    /// <returns>Array of toggles</returns>
    public Toggle[] GetToggles() => _toggles;

    /// <summary>
    /// Change the game state if the game is started or finished
    /// </summary>
    /// <param name="isGameStarted">True if the game is started, False - if the game is finished</param>
    private void OnSetGameState(bool isGameStarted)
    {
        _isGameStarted = isGameStarted;
        OnSetInteractableState(isGameStarted);
    }

    /// <summary>
    /// Changes the toggles interactability depending to the game state
    /// </summary>
    /// <param name="isInteractable">True if toggles can be interacted, false if not</param>
    private void OnSetInteractableState(bool isInteractable) 
    {
        for (int i = 0; i < _toggles.Length; i++)
        {
            _toggles[i].interactable = isInteractable;
        }
    }
   
    /// <summary>
    /// Subscribe to events
    /// </summary>
    private void SetupSubscriptions()
    {
        for (int i = 0; i < _toggles.Length; ++i)
        {
            _toggles[i].onValueChanged.AsObservable().Subscribe(OnToggleValueChanged).AddTo(this);
        }
    }

    /// <summary>
    /// Handler for the toggle value changed. Uses for counter updating
    /// </summary>
    /// <param name="isOn">Changed toggle state</param>
    private void OnToggleValueChanged(bool isOn)
    {
        if (!_isGameStarted) return;

        _counterService.AddValue();
    }

    /// <summary>
    /// Get toggles from children
    /// </summary>
    private void SetupToggles()
    {
        _toggles = _toggleContainer.GetComponentsInChildren<Toggle>();
    }
}
