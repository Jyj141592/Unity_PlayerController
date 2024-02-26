using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController{
//[CreateAssetMenu(fileName = "New Parameter List", menuName = "PlayerController/Parameter List")]
[Serializable]
public class ParameterList
{
    public List<Parameter> parameters;
    public ParameterList(){
        parameters = new List<Parameter>();
    }

    public void Init(){
        parameters.Sort();
    }
    
}
}