using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController{
[Serializable]
public class BoolParameter : Parameter
{
    private bool value;

    public override bool GetBool()
    {
        return value;
    }

    public override void SetBool(bool value)
    {
        this.value = value;
    }
}
}