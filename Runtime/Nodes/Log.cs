using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController{
public class Log : PCNode
{
    [SerializeField]
    public string log;
    [field: SerializeField]
    public int testProperty{
        get; set;
    }
}
}