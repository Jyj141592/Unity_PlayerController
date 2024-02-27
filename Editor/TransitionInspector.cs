using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PlayerController.Editor{
public class TransitionInspector : VisualElement
{
    public new class UxmlFactory : UxmlFactory<TransitionInspector, VisualElement.UxmlTraits>{}
    private PCEdgeView edge;
    private ParameterList parameterList;
    private Foldout foldout;
    private ListView listView;
    
    public TransitionInspector(){}
    public void Init(ParameterList list){
        parameterList = list;
        foldout = this.Q<Foldout>();
        listView = this.Q<ListView>();
    }

    public void UpdateInspector(PCEdgeView edge){
        this.edge = edge;
    }
    public void Update(){
        
    }
}
}