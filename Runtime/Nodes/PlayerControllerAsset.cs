using System.Collections;
using System.Collections.Generic;
using PlayerController.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace PlayerController{
[CreateAssetMenu(fileName = "New PlayerController", menuName = "PlayerControllerAsset")]
[DisallowCreateNode]
public class PlayerControllerAsset : PCNode
{
    [HideInInspector]
    [SerializeField]
    private List<PCNode> _nodes;
    public List<PCNode> nodes{
        get => _nodes;
    }
    [HideInInspector]
    [SerializeField]
    private AnyState _anyState = null;
    public AnyState anyState{
        get => _anyState;
    }
    //[HideInInspector]
    [SerializeField]
    private ParameterList _parameterList = null;
    public ParameterList parameterList{
        get => _parameterList;
    }
    public PlayerControllerAsset(){
        _parameterList = new ParameterList();
    }
}
}