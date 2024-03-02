using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController{
[Serializable]
public class Transition// : ScriptableObject
{
    public PCNode dest;
    public bool mute = false;
    [field: SerializeField]
    public int testValue{
        get; set;
    }
    [HideInInspector]
    public List<Condition> conditions;
    public Transition(PCNode node){
        dest = node;
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