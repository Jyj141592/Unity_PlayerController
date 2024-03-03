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

    public void Init(){
        parameters.Sort();
    }
    
}
}