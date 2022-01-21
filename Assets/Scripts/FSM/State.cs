using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State {
    public string stateID;

    protected Entity _entity;

    protected float _startTime;

    public State(Entity entity, string name) {
        _entity = entity;
        stateID = name;
    }

    public virtual void Enter() {
        _startTime = Time.time;

        DoChecks();

    }

    public virtual void Exit() {
    }

    public virtual void Tick() {

    }

    public virtual void FixedTick() {
        DoChecks();
    }

    public virtual void LateTick() {

    }

    public virtual void DoChecks() {

    }
}
