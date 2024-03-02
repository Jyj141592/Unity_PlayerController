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
    public List<PCNode> nodes;
    [HideInInspector]
    public AnyState anyState = null;
    public ParameterList parameterList = null;
    public PlayerControllerAsset(){
        parameterList = new ParameterList();
    }
}
}