using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController{
[Serializable]
public class IntParameter : Parameter
{
    private int value;

    public override int GetInt()
    {
        return value;
    }

    public override void SetInt(int value)
    {
        this.value = value;
    }
}
}