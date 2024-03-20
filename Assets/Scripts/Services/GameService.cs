using UniRx;
using System;
using Assets.Scripts.UI.Interfaces;
using Assets.Scripts.Services.Interfaces;

namespace Assets.Scripts.Services
{
    /// <summary>
    /// Class controld the main game logic.
    /// </summary>
    internal class GameService : IGameStateObservable, IDisposable
    {
        /// <summary>
        /// This constant allows to change the gamefield. To implement it need to adjust the way how the app creates toggles in the GamefieldController (Out of scope)
        /// </summary>
        private const int FIELD_SIZE = 5;
        // This constant adjusts the difficulty of the game
        private const int RIDDLE_DIFFICULTY = 3;

        private bool _isInitialized;
        private bool _isGameStarted;
        private GameNode[,] _nodes;

        private IGamefieldDataSource _gamefieldDataSource;
        private IWinPopupObservable _winPopupObservable;
        private ITimer _timerService;
        private ICounter _counter;
        private IPopup _winPopup;
        
        private Subject<bool> _onGameStateSubject = new();
        private Subject<Unit> _onCheckIsTheGameSolvedSubject = new();
        private CompositeDisposable _disposables = new();
        
        public IObservable<bool> OnGameStateObservable => _onGameStateSubject;
        public IObservable<Unit> OnCheckIsTheGameSolvedObservable => _onCheckIsTheGameSolvedSubject;

        /// <summary>
        /// Dependencies setup
        /// </summary>
        /// <param name="gamefieldDataSource">The gamefield data source provide visual elements for the nodes</param>
        /// <param name="gameplayPageObservable">Notify when button start/stop was clicked</param>
        /// <param name="timerService">Timer service needed to control start/stop of the timer</param>
        /// <param name="counter">Counter service needed for reseting the values</param>
        /// <param name="winPopupController"></param>
        public GameService(IGamefieldDataSource gamefieldDataSource, IGameplayPageObservable gameplayPageObservable, ITimer timerService, ICounter counter, WinPopupController winPopupController)
        {
            _gamefieldDataSource = gamefieldDataSource;
            _timerService = timerService;
            _counter = counter;
            _winPopup = winPopupController;
            _winPopupObservable = winPopupController;
            
            gameplayPageObservable.OnGameStartButtonObservable.Subscribe(ChangeGameStatus).AddTo(_disposables);
            _winPopupObservable.OnRestartGameSubject.Subscribe(OnGameRestart).AddTo(_disposables);   
            OnCheckIsTheGameSolvedObservable.Subscribe(CheckIsRiddleResolved).AddTo(_disposables);

            InitializeNodes();
        }

        /// <summary>
        /// Clean up data
        /// </summary>
        public void Dispose()
        {
            _disposables?.Dispose();
        }

        /// <summary>
        /// Creates GameNode entities in the array
        /// </summary>
        private void InitializeNodes()
        {
            _nodes = new GameNode[FIELD_SIZE, FIELD_SIZE];
            for (int row = 0; row < _nodes.GetLength(0); ++row)
            {
                for (int col = 0; col < _nodes.GetLength(1); ++col)
                {
                    _nodes[row, col] = new GameNode();
                }
            }
        }

        /// <summary>
        /// Binds node data and sets the data
        /// </summary>
        private void BuildField()
        {
            var toggles = _gamefieldDataSource.GetToggles();
            int index = 0;
            GameNode topNeighbor = null;
            GameNode bottomNeighbor = null;
            GameNode leftNeighbor = null;
            GameNode rightNeighbor = null;

            for (int row = 0; row < _nodes.GetLength(0); ++row)
            {
                for (int col = 0; col < _nodes.GetLength(1); ++col)
                {
                    //Finding an index in a list
                    index = row * FIELD_SIZE + col;

                    //Neighbour search
                    topNeighbor = row > 0 ? topNeighbor = _nodes[row - 1, col] : null;
                    bottomNeighbor = row < FIELD_SIZE - 1 ? bottomNeighbor = _nodes[row + 1, col] : null;
                    leftNeighbor = col > 0 ? leftNeighbor = _nodes[row, col - 1] : null;
                    rightNeighbor = col < FIELD_SIZE - 1 ? rightNeighbor = _nodes[row, col + 1] : null;

                    _nodes[row, col].SetupData(_onCheckIsTheGameSolvedSubject, toggles[index], rightNeighbor, leftNeighbor, topNeighbor, bottomNeighbor);
                }
            }
        }

        /// <summary>
        /// Changes the game state
        /// </summary>
        /// <param name="isStart">True if the game is started, false if not</param>
        private void ChangeGameStatus(bool isStart)
        {
            _isGameStarted = isStart;
            if (_isGameStarted)
            {
                StartGame();
            }
            else
            {
                CleanScreenData();
                StopGame();
            }
        }

        /// <summary>
        /// Starts the game and creates riddle
        /// </summary>
        private void StartGame()
        {
            if (!_isInitialized)
            {
                BuildField();
                _isInitialized = true;
            }

            CreateRiddle();
            _timerService.StartTimer();
            _onGameStateSubject?.OnNext(true);
        }

        /// <summary>
        /// Stops timer and block all nodes
        /// </summary>
        private void StopGame()
        {
            _timerService.StopTimer();
            DisableAllNodes();
            _onGameStateSubject?.OnNext(false);
        }

        /// <summary>
        /// Disable all nodes
        /// </summary>
        private void DisableAllNodes()
        {
            for (int row = 0; row < _nodes.GetLength(0); ++row)
            {
                for (int col = 0; col < _nodes.GetLength(1); ++col)
                {
                    _nodes[row, col].UpdateState(false);
                }
            }
        }

        /// <summary>
        /// Creates reddle by simulation random click and switching the neighbour state to make riddle solvable
        /// </summary>
        private void CreateRiddle()
        {
            Random random = new Random();
            int targetRow = 0;
            int targetCol = 0;

            for (int i = 0; i < RIDDLE_DIFFICULTY; ++i)
            {
                targetRow = random.Next(FIELD_SIZE);
                targetCol = random.Next(FIELD_SIZE);
                _nodes[targetRow, targetCol].OnNodeClicked(!_nodes[targetRow, targetCol].CheckState());
            }
        }

        /// <summary>
        /// Restart game handler
        /// </summary>
        /// <param name="unit"></param>
        private void OnGameRestart(Unit unit)
        {
            CleanScreenData();
            StartGame();
        }

        /// <summary>
        /// Check if any toggle is On. If all toggles are off invokes the win popup and stops the game
        /// </summary>
        /// <param name="unit"></param>
        private void CheckIsRiddleResolved(Unit unit)
        {
            if (!_isGameStarted) return;

            for (int row = 0; row < _nodes.GetLength(0); ++row)
            {
                for (int col = 0; col < _nodes.GetLength(1); ++col)
                {
                    if (_nodes[row, col].CheckState())
                    {
                        return;
                    }
                }
            }
            _winPopup.Show();
            StopGame();
        }

        /// <summary>
        /// Clears the times and counter
        /// </summary>
        private void CleanScreenData()
        {
            _timerService.ClearTimer();
            _counter.Clear();
        }
    }
}
