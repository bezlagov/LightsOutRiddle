using UniRx;

namespace Assets.Scripts.Services
{
    /// <summary>
    /// Interface for outputting counter values to the UI
    /// </summary>
    internal interface ICounterObservable
    {
        public ReactiveProperty<int> Counter { get; }
    }
}
