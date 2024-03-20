using UnityEngine.UI;

namespace Assets.Scripts.UI.Interfaces
{
    /// <summary>
    /// Gamefield page interfave that allows to pass toggles values to bind it to the data
    /// </summary>
    internal interface IGamefieldDataSource
    {
        public Toggle[] GetToggles();
    }
}
