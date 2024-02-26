using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace PlayerController{
[CreateAssetMenu(fileName = "New PlayerController", menuName = "PlayerControllerAsset")]
public class PlayerControllerAsset : PCNode
{
    [HideInInspector]
    public List<PCNode> nodes;

    public ParameterList parameterList = null;
    public PlayerControllerAsset(){
        actionName = "Entry";
        parameterList = new ParameterList();
    }
}
}