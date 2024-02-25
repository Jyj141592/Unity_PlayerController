using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace PlayerController.Editor{
public class ParameterView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ParameterView, VisualElement.UxmlTraits>{}
    public ParameterView(){}
    private ParameterList parameterList;
    public void Init(ParameterList parameterList){
        this.parameterList = parameterList;


        LoadParameterView();
    }

    private void LoadParameterView(){
        
    }
}
}