using System;

namespace Assets.Scripts.UI.Interfaces
{
    /// <summary>
    /// Gameplay page interface that allows to subscribe to Start / Stop Button click
    /// </summary>
    internal interface IGameplayPageObservable
    {
        public IObservable<bool> OnGameStartButtonObservable { get; }

    }
}
