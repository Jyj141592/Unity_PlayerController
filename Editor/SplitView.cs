using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace PlayerController.Editor{
public class SplitView : TwoPaneSplitView
{
    public new class UxmlFactory : UxmlFactory<SplitView, TwoPaneSplitView.UxmlTraits>{}
    public SplitView(){

    }
}
}