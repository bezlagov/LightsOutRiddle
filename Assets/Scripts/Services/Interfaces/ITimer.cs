namespace Assets.Scripts.Services
{
    /// <summary>
    /// Interface for timer control
    /// </summary>
    internal interface ITimer
    {
        public void StartTimer();
        public void StopTimer();
        public void ClearTimer();
    }
}
