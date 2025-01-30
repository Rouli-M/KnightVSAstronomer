using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HandyBehaviour : StateMachineBehaviour
{
    private List<(string, Action)> onStateEnterEvents = new List<(string, Action)>();
    private List<(string, Action)> onStateExitEvents = new List<(string, Action)>();

    public void ClearEvents()
    {
        onStateEnterEvents.Clear();
        onStateExitEvents.Clear();
    }

    public void onStateEnterEvent(string eventName, Action eventAction)
    {
        onStateEnterEvents.Add((eventName, eventAction));
    }

    public void onStateExitEvent(string eventName, Action eventAction)
    {
        onStateExitEvents.Add((eventName, eventAction));
    }

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach((string, Action) e in onStateEnterEvents)
        {
            if (stateInfo.IsName(e.Item1))
                e.Item2.Invoke();
        }
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach ((string, Action) e in onStateExitEvents)
        {
            if (stateInfo.IsName(e.Item1))
                e.Item2.Invoke();
        }
    }


    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    //override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    //override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}
}
