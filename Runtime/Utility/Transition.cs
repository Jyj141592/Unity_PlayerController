using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController{
[Serializable]
public class Transition
{
    public PCNode dest;
    public bool canTransition(){
        return false;
    }
}
}