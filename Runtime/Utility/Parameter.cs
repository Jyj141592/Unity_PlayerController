using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController{
public enum ParameterType{
    Int, Float, Bool
}
[Serializable]
public class Parameter : IComparable<Parameter>, IComparable<int>
{
    [SerializeField]
    private ParameterType type;
    [SerializeField]
    private string name;
    [SerializeField]
    private int paramID;
    [SerializeField]
    private int intValue = 0;
    [SerializeField]
    private float floatValue = 0;
    [SerializeField]
    private bool boolValue = false;
    public void SetName(string name){
        this.name = name;
        paramID = Animator.StringToHash(name);
    }
    public string GetName() {return name; }
    public void SetParameterType(ParameterType type){ this.type = type; }
    public ParameterType GetParameterType(){ return type; }
    public int GetID(){ return paramID; }
    public virtual int GetInt(){ return intValue; }
    public virtual void SetInt(int value){ intValue = value; }
    public virtual float GetFloat(){ return floatValue; }
    public virtual void SetFloat(float value){ floatValue = value; }
    public virtual bool GetBool(){ return boolValue; }
    public virtual void SetBool(bool value){ boolValue = value; }

    public int CompareTo(Parameter other)
    {
        return paramID - other.paramID;
    }

    public int CompareTo(int other)
    {
        return paramID - other;
    }
}
}