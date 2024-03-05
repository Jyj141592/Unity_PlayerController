using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController{
//[CreateAssetMenu(fileName = "New Parameter List", menuName = "PlayerController/Parameter List")]
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
    public int FindIndexOfParameter(string name){
        int id = Animator.StringToHash(name);
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