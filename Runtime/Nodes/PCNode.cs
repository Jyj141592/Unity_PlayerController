using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController{
public class PCNode : ScriptableObject
{
    public enum NodeState{
        Wait, Runnning
    }
    [NonSerialized]
    public NodeState state = NodeState.Wait;
    public string actionName;
    [HideInInspector]
    public Vector2 position;
    [HideInInspector]
    public string guid;
    //[HideInInspector]
    public List<Transition> transition;
}
}