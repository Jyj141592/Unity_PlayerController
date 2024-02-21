using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace PlayerController.Editor{
public class PCGraphView : GraphView
{
    public EntryNode entryNode;

#region Initialize
    public PCGraphView(){
        CreateGridBackground();
        AddManipulators();
    }
    private void CreateGridBackground(){
        GridBackground gridBackground = new GridBackground();
        gridBackground.StretchToParentSize();
        Insert(0, gridBackground);
    }
    private void AddStyleSheet(string path){
        StyleSheet styleSheet = (StyleSheet) EditorGUIUtility.Load(path);
        styleSheets.Add(styleSheet);
    }
    private void AddManipulators(){

    }
#endregion Initialize
}
}