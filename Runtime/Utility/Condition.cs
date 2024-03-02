using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController{
public enum TransitionCondition{
    Float_Greater, Float_Less, Int_Greater, Int_Equal, Int_Less, Bool_True, Bool_False
}
[Serializable]
public class Condition
{
    //[SerializeField]
    public string paramName;
    [SerializeField]
    private int paramID;
    private int paramIndex;
    [SerializeField]
    private TransitionCondition condition;
    [SerializeField]
    private float value;
    public string GetName(){
        return paramName;
    }
    public TransitionCondition GetTransitionCondition(){
        return condition;
    }
    public float GetFloat(){
        return value;
    }
    public int GetInt(){
        return (int) value;
    }
    public bool IsTrue(){
        return false;
    }
}
}