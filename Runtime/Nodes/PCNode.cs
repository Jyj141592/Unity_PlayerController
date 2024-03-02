using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PlayerController{
public class PCNode : ScriptableObject
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
    private List<Transition> _transitions;
    public List<Transition> transitions{
        get => _transitions;
    }

    public void Init(string actionName, Vector2 position){
        this.actionName = actionName;
        this.position = position;
        _transitions = new List<Transition>();
        guid = GUID.Generate().ToString();
    }
}
}