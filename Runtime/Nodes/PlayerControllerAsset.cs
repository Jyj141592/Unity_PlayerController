using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PlayerController{
[CreateAssetMenu(fileName = "New PlayerController", menuName = "PlayerController")]
public class PlayerControllerAsset : PCNode
{
    [HideInInspector]
    public List<PCNode> nodes;
}
}