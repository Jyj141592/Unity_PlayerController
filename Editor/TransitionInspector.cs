using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PlayerController.Editor{
public class TransitionInspector : VisualElement
{
    public new class UxmlFactory : UxmlFactory<TransitionInspector, VisualElement.UxmlTraits>{}
    public TransitionInspector(){}

    public void UpdateInspector(PCEdgeView edge){
        Debug.Log(edge);
    }
    public void Update(){
        
    }
}
}