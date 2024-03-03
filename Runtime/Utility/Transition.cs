using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController{
[Serializable]
public class Transition// : ScriptableObject
{
    [SerializeField]
    private PCNode _dest;
    public PCNode dest{
        get => _dest;
        private set => _dest = value;
    }
    [SerializeField]
    private bool _mute = false;
    public bool mute{
        get => _mute;
        set => _mute = value;
    }
    [field: SerializeField]
    public int testValue{
        get; set;
    }
    [HideInInspector]
    [SerializeField]
    private List<Condition> _conditions;
    public List<Condition> conditions{
        get => _conditions;
        private set => _conditions = value;
    }
    public Transition(PCNode node){
        dest = node;
        conditions = new List<Condition>();
    }
    public Transition(){
        dest = null;
        conditions = new List<Condition>();
    }
    public void Init(PCNode dest){
        this.dest = dest;
    }
    public bool canTransition(){
        return false;
    }
}
}