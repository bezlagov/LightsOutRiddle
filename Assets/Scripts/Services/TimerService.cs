using UniRx;
using System;

namespace Assets.Scripts.Services
{
    /// <summary>
    /// A class that encapsulates the logic of the timer, notify services about changed value with ReactiveProperty
    /// </summary>
    internal class TimerService : ITimerObservable, ITimer, IDisposable
    {
        private const float TIME_STEP = 1;
        
        private IDisposable _disposables;

        public ReactiveProperty<int> GameplayTime { get; set; } = new ReactiveProperty<int>();
        
        /// <summary>
        /// Starts the timer using observables
        /// </summary>
        public void StartTimer()
        {
            var observable = Observable.Interval(TimeSpan.FromSeconds(TIME_STEP));

            _disposables = observable.Subscribe(timer =>
                {
                    ++GameplayTime.Value;
                });
        }

        /// <summary>
        /// Stop timer without resetting the value
        /// </summary>
        public void StopTimer()
        {
            _disposables?.Dispose();
        }

        /// <summary>
        /// Resets the timer value
        /// </summary>
        public void ClearTimer()
        {
            GameplayTime.Value = 0;
        }

        /// <summary>
        /// Clean up data
        /// </summary>
        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}
