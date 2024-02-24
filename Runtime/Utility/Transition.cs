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
    public Transition(PCNode node){
        dest = node;
    }
    public void Init(PCNode dest){
        this.dest = dest;
    }

    public int test;
    public bool canTransition(){
        return false;
    }
}
}