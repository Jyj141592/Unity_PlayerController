using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController{
[Serializable]
public abstract class Parameter : IComparable<Parameter>, IComparable<int>
{
    public string name;
    public int paramID;
    public void SetName(string name){
        this.name = name;
        paramID = Animator.StringToHash(name);
    }
    public virtual int GetInt(){ return 0; }
    public virtual void SetInt(int value){}
    public virtual float GetFloat(){ return 0; }
    public virtual void SetFloat(float value){}
    public virtual bool GetBool(){ return false; }
    public virtual void SetBool(bool value){}

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