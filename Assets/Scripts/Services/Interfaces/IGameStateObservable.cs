using System;

namespace Assets.Scripts.Services.Interfaces
{
    /// <summary>
    /// Interface notifying services whether the game has started or not
    /// </summary>
    internal interface IGameStateObservable
    {
        public IObservable<bool> OnGameStateObservable { get; }
    }
}
