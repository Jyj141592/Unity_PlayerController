using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PlayerController.Editor{
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class CreateNodeMenuAttribute : Attribute{
    public string path;
    public CreateNodeMenuAttribute(string path){
        this.path = path;
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class DisallowCreateNodeAttribute : Attribute{
    
}
}