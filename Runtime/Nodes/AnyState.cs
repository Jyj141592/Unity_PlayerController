using System.Collections;
using System.Collections.Generic;
using PlayerController.Editor;
using UnityEngine;

namespace PlayerController{
[DisallowCreateNode]
public class AnyState : PCNode
{
    private PCNode currentNode = null;
    public override float runningTime => currentNode.runningTime;
    public void ChangeNode(PCNode node){
        currentNode = node;
    }
}
}