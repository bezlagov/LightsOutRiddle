using UniRx;
using System;

namespace Assets.Scripts.UI.Interfaces
{
    /// <summary>
    /// Win page interface that allows to subscribe to Restart button click
    /// </summary>
    internal interface IWinPopupObservable
    {
        public IObservable<Unit> OnRestartGameSubject { get; }

    }
}
