using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine {
    private State _currentState;

    private Dictionary<string, State> _allStates = new Dictionary<string, State>();
    
    public void ChangeState(string targetId) {
        if(_currentState != null)
        {
            // Run on Exit Actions
            _currentState.Exit();
        }
        State targetState = GetState(targetId);

        // Run on Enter Actions
        _currentState = targetState;
        if(_currentState != null)
        {
            _currentState.Enter();
        }
    }

    public void RegisterState(State state) {
        _allStates.Add(state.stateID, state);
    }

    public State GetState(string targetId) {
        _allStates.TryGetValue(targetId, out State targetState);
        return targetState;
    }

    public void FixedTick() {
        if(_currentState == null)
        {
            return;
        }

        _currentState.FixedTick();
    }

    public void Tick() {
        if(_currentState == null)
        {
            return;
        }

        _currentState.Tick();
    }

    public void LateTick() {
        if(_currentState == null)
        {
            return;
        }

        _currentState.LateTick();
    }
}
