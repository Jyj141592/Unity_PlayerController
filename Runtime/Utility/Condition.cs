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
    [SerializeField]
    private string paramName;
    [SerializeField]
    private int paramID;
    private int paramIndex;
    [SerializeField]
    private TransitionCondition condition;
    [SerializeField]
    private float value;
    public bool IsTrue(){
        return false;
    }
}
}