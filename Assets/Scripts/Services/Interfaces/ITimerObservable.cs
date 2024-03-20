using UniRx;

namespace Assets.Scripts.Services
{
    /// <summary>
    /// Interface for outputting timer values to the UI
    /// </summary>
    internal interface ITimerObservable
    {
        public ReactiveProperty<int> GameplayTime { get; }
    }
}
