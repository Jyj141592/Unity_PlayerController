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
    //[HideInInspector]
    [SerializeField]
    private ParameterType _paramType;
    public ParameterType paramType{
        get => _paramType;
        private set => _paramType = value;
    }
    //[HideInInspector]
    [SerializeField]
    private string _name;
    public string name{
        get => _name;
        private set => _name = value;
    }
    //[HideInInspector]
    [SerializeField]
    private int _paramID;
    public int paramID{
        get => _paramID;
        private set => _paramID = value;
    }
    [SerializeField]
    private float value = 0;

    public virtual int GetInt(){ return (int)value; }
    public virtual void SetInt(int value){ this.value = value; }
    public virtual float GetFloat(){ return value; }
    public virtual void SetFloat(float value){ this.value = value; }
    public virtual bool GetBool(){ return value > 0; }
    public virtual void SetBool(bool value){ this.value = value ? 1 : 0; }

    public int CompareTo(Parameter other)
    {
        if(paramID > other.paramID) return 1;
        else if(paramID == other.paramID) return 0;
        else return -1;
    }

    public int CompareTo(int other)
    {
        if(paramID > other) return 1;
        else if(paramID == other) return 0;
        else return -1;
    }
    public Parameter Clone(){
        Parameter clone = new Parameter();
        clone._name = _name;
        clone._paramID = _paramID;
        clone._paramType = _paramType;
        clone.value = value;
        return clone;
    }
}
}