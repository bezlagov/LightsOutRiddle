using UniRx;

namespace Assets.Scripts.Services
{
    /// <summary>
    /// A class that encapsulates the logic of the counter, notify services about changed value with ReactiveProperty
    /// </summary>
    internal class CounterService: ICounterObservable, ICounter
    {
        public ReactiveProperty<int> Counter { get; set; } = new ReactiveProperty<int>();

        /// <summary>
        /// Adds a value each time the toggle is pressed after the game starts
        /// </summary>
        public void AddValue()
        {
            ++Counter.Value;
        }

        /// <summary>
        /// Clears the counter value
        /// </summary>
        public void Clear()
        { 
            Counter.Value = 0;
        }
    }
}
