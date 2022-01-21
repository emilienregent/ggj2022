using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {
    public FiniteStateMachine stateMachine { get; private set; }

    public Animator entityAnimator { get; private set; }
    
    private Vector2 _velocity;
    

    public virtual void Awake() {
        entityAnimator = GetComponentInChildren<Animator>();

        stateMachine = new FiniteStateMachine();
    }

    public virtual void Update() {

        stateMachine.Tick();
    }

    public virtual void FixedUpdate() {
        stateMachine.FixedTick();
    }

    public virtual void LateUpdate() {
        stateMachine.LateTick();
    }
}
