using UniRx;
using UnityEngine.UI;

/// <summary>
/// The class is an implementation of a node and encapsulates the logic of switching the state of the node itself and its neighbours.
/// </summary>
public class GameNode
{
    private Toggle _toggle;
    private GameNode _nextNode;
    private GameNode _previousNode;
    private GameNode _topNode;
    private GameNode _bottomNode;
    
    private CompositeDisposable _disposables = new();
    private Subject<Unit> _onCheckIsTheGameSolvedSubject;
    private ReactiveProperty<bool> NodeState { get; set; } = new ReactiveProperty<bool>();

    /// <summary>
    /// Node click handler
    /// </summary>
    /// <param name="isOn">The state from the toggle</param>
    public void OnNodeClicked(bool isOn)
    {
        NodeState.Value = isOn;
        _toggle.SetIsOnWithoutNotify(NodeState.Value);

        _nextNode?.SwitchState();
        _previousNode?.SwitchState();
        _topNode?.SwitchState();
        _bottomNode?.SwitchState();

        _onCheckIsTheGameSolvedSubject?.OnNext(Unit.Default);
    }

    /// <summary>
    /// This method sets the dependencies for the node
    /// </summary>
    /// <param name="onCheckIsTheGameSolvedSubject">is called after the change of the togle state to determine the winning state</param>
    /// <param name="toggle">Toggle to control the visualization</param>
    /// <param name="next">Next neighbour</param>
    /// <param name="previous">Previous neighbour</param>
    /// <param name="top">Top neighbour</param>
    /// <param name="bottom">Bottom neighbour</param>
    public void SetupData(Subject<Unit> onCheckIsTheGameSolvedSubject, Toggle toggle, GameNode next, GameNode previous, GameNode top, GameNode bottom)
    {
        _toggle = toggle;
        _onCheckIsTheGameSolvedSubject = onCheckIsTheGameSolvedSubject;

        _toggle.onValueChanged.AsObservable().Subscribe(OnNodeClicked).AddTo(_disposables);

        _nextNode = next;
        _previousNode = previous;
        _topNode = top;
        _bottomNode = bottom;
    }

    /// <summary>
    /// Swap the current toggle state. This method is called when a node is a neighbour to the node that was clicked on 
    /// </summary>
    public void SwitchState()
    {
        NodeState.SetValueAndForceNotify(!NodeState.Value);
        _toggle.SetIsOnWithoutNotify(NodeState.Value);
    }

    /// <summary>
    /// Updates the state of the node without notifying other application elements. Used to reset the values of all nodes.
    /// </summary>
    /// <param name="isOn">True if node should be enabled, false if disabled</param>
    public void UpdateState(bool isOn)
    {
        NodeState.SetValueAndForceNotify(isOn);
        _toggle.SetIsOnWithoutNotify(NodeState.Value);
    }

    /// <summary>
    /// Returns the current state of the node
    /// </summary>
    /// <returns>True if node is, false if disabled</returns>
    public bool CheckState() => NodeState.Value;
}
