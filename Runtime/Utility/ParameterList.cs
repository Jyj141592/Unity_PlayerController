using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace PlayerController{
[Serializable]
public class ParameterList
{
    //[HideInInspector]
    [SerializeField]
    private List<Parameter> _parameters;
    public List<Parameter> parameters{
        get => _parameters;
    }
    public ParameterList(){
        _parameters = new List<Parameter>();
    }
    public int FindIndexOfParameter(string paramName){
        int id = Animator.StringToHash(paramName);
        int left = 0, right = _parameters.Count;
        int mid;
        while(left < right){
            mid = (left + right) / 2;
            if(_parameters[mid].paramID == id) return mid;
            else if(_parameters[mid].paramID < id) left = mid + 1;
            else right = mid;
        }
        return -1;
    }
    public void SetInt(string paramName, int value){
        int index = FindIndexOfParameter(paramName);
        SetInt(index, value);
    }
    public void SetInt(int index, int value){
        if(index < 0 || index >= parameters.Count) return;
        parameters[index].SetInt(value);
    }
    public int GetInt(string paramName){
        int index = FindIndexOfParameter(paramName);
        return GetInt(index);
    }
    public int GetInt(int index){
        if(index < 0 || index >= parameters.Count) return 0;
        return parameters[index].GetInt();
    }
    public void SetBool(string paramName, bool value){
        int index = FindIndexOfParameter(paramName);
        SetBool(index, value);
    }
    public void SetBool(int index, bool value){
        if(index < 0 || index >= parameters.Count) return;
        parameters[index].SetBool(value);
    }
    public bool GetBool(string paramName){
        int index = FindIndexOfParameter(paramName);
        return GetBool(index);
    }
    public bool GetBool(int index){
        if(index < 0 || index >= parameters.Count) return false;
        return parameters[index].GetBool();
    }
    public void SetFloat(string paramName, float value){
        int index = FindIndexOfParameter(paramName);
        SetFloat(index, value);
    }
    public void SetFloat(int index, float value){
        if(index < 0 || index >= parameters.Count) return;
        parameters[index].SetFloat(value);
    }
    public float GetFloat(string paramName){
        int index = FindIndexOfParameter(paramName);
        return GetFloat(index);
    }
    public float GetFloat(int index){
        if(index < 0 || index >= parameters.Count) return 0;
        return parameters[index].GetFloat();
    }
    public ParameterList Clone(){
        ParameterList clone = new ParameterList();
        for(int i = 0; i < _parameters.Count; i++){
            clone._parameters.Add(_parameters[i].Clone());
        }
        clone._parameters.Sort();
        return clone;
    }
    
}
}