using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController{
[Serializable]
public class Transition
{
    [SerializeField]
    private PCNode _dest;
    public PCNode dest{
        get => _dest;
        private set => _dest = value;
    }
    [SerializeField]
    private bool _mute = false;
    public bool mute{
        get => _mute;
        set => _mute = value;
    }
    [SerializeField]
    private float _exitTime;
    public float exitTime{
        get => _exitTime;
        private set => _exitTime = value;
    }
    [SerializeField]
    private List<Condition> _conditions;
    public List<Condition> conditions{
        get => _conditions;
        private set => _conditions = value;
    }
    public Transition(){
        dest = null;
        conditions = new List<Condition>();
    }
    public bool CanTransition(ParameterList list, float runningTime){
        if(mute) return false;
        if(runningTime < exitTime) return false;
        for(int i = 0; i < conditions.Count; i++){
            if(!conditions[i].IsTrue(list)) return false;
        }
        return true;
    } 
    public void Init(PlayerControllerAsset asset){
        dest = asset.FindNodeByName(dest.actionName);
        for(int i = 0; i < conditions.Count; i++){
            conditions[i].Init(asset);
        }
    }
}
}