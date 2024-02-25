using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController{
[Serializable]
public class FloatParameter : Parameter
{
    private float value;

    public override void SetFloat(float value)
    {
        this.value = value;
    }

    public override float GetFloat()
    {
        return value;
    }
}
}