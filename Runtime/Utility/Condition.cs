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
    private string _paramName;
    public string paramName{
        get => _paramName;
        private set => _paramName = value;
    }
    [SerializeField]
    private int _paramID;
    public int paramID{
        get => _paramID;
        private set => _paramID = value;
    }

    private int paramIndex;
    [SerializeField]
    private TransitionCondition _condition;
    public TransitionCondition condition{
        get => _condition;
        private set => _condition = value;
    }
    [SerializeField]
    private float value;
    public float GetFloat(){
        return value;
    }
    public int GetInt(){
        return (int) value;
    }
    public bool IsTrue(ParameterList list){
        switch(condition){
            case TransitionCondition.Float_Greater:
            return list.GetFloat(paramIndex) > value;
            case TransitionCondition.Float_Less:
            return list.GetFloat(paramIndex) < value;
            case TransitionCondition.Int_Greater:
            return list.GetInt(paramIndex) > value;
            case TransitionCondition.Int_Equal:
            return list.GetInt(paramIndex) == (int)value;
            case TransitionCondition.Int_Less:
            return list.GetInt(paramIndex) < value;
            case TransitionCondition.Bool_True:
            return list.GetBool(paramIndex) == true;
            case TransitionCondition.Bool_False:
            return list.GetBool(paramIndex) == false;
        }
        return false;
    }
    public void Init(PlayerControllerAsset asset){
        paramIndex = asset.parameterList.FindIndexOfParameter(paramName);
    }
}
}