using UniRx;
using System;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.UI.Interfaces;

/// <summary>
/// The class contains the logic for the win popup
/// </summary>
public class WinPopupController : MonoBehaviour, IPopup, IWinPopupObservable
{
    [SerializeField] private Button _restartGameButton;
    
    private Subject<Unit> _onRestartGameSubject = new();
    public IObservable<Unit> OnRestartGameSubject => _onRestartGameSubject;

    /// <summary>
    /// Awake monobehaviour. Setup subscriptions
    /// </summary>
    private void Awake()
    {
        SetupSubscriptions();
    }

    /// <summary>
    /// Shows popup
    /// </summary>
    public void Show() => gameObject.SetActive(true);

    /// <summary>
    /// Hides popup
    /// </summary>
    public void Hide() => gameObject.SetActive(false);

    /// <summary>
    /// Subscribe to events
    /// </summary>
    private void SetupSubscriptions()
    {
        _restartGameButton.onClick.AsObservable().Subscribe(OnRestartGameHandler).AddTo(this);
    }

    /// <summary>
    /// Restarts game handler
    /// </summary>
    private void OnRestartGameHandler(Unit unit)
    {
        Hide();
        _onRestartGameSubject?.OnNext(unit);
    }
}
