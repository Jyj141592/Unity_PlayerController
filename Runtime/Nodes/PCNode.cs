using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PlayerController{
public class PCNode : ScriptableObject, IComparable<PCNode>
{
    public enum NodeState{
        Wait, Runnning
    }
    public NodeState state {
        get; protected set;
    }
    [SerializeField]
    private string _actionName;
    public string actionName{
        get => _actionName;
        private set => _actionName = value;
    }
    [HideInInspector]
    [SerializeField]
    private int _actionID;
    public int actionID{
        get => _actionID;
        private set => _actionID = value;
    }
    [HideInInspector]
    [SerializeField]
    private Vector2 _position;
    public Vector2 position{
        get => _position;
        private set => _position = value;
    }
    [HideInInspector]
    [SerializeField]
    private string _guid;
    public string guid{
        get => _guid;
        private set => _guid = value;
    }
    [HideInInspector]
    [SerializeField]
    private List<Transition> _transitions = new List<Transition>();
    public List<Transition> transitions{
        get => _transitions;
    }

    public int CompareTo(PCNode other)
    {
        return actionID - other.actionID;
    }
    public virtual void Init(PlayerControllerAsset asset){
        for(int i = 0; i < transitions.Count; i++){
            transitions[i].Init(asset);
        }
    }
}
}