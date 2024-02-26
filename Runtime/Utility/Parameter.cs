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
    private float value = 0;
    public void SetName(string name){
        this.name = name;
        paramID = Animator.StringToHash(name);
    }
    public string GetName() {return name; }
    public void SetParameterType(ParameterType type){ this.type = type; }
    public ParameterType GetParameterType(){ return type; }
    public int GetID(){ return paramID; }
    public virtual int GetInt(){ return (int)value; }
    public virtual void SetInt(int value){ this.value = value; }
    public virtual float GetFloat(){ return value; }
    public virtual void SetFloat(float value){ this.value = value; }
    public virtual bool GetBool(){ return value > 0; }
    public virtual void SetBool(bool value){ this.value = value ? 1 : 0; }

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