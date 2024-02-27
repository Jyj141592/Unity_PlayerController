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
    private string paramName;
    private int paramID;
    private TransitionCondition condition;
    public bool IsTrue(){
        return false;
    }
}
}