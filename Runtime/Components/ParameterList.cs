using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController{
[Serializable]
public class ParameterList
{
    public List<Parameter> parameters;

    public void Init(){
        parameters.Sort();
    }
    
}
}