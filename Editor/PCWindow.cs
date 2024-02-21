using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace PlayerController.Editor{
public class PCWindow : EditorWindow
{
    private PCGraphView graphView;

#region Initialize
    public static PCWindow Open(){
        PCWindow wnd = GetWindow<PCWindow>();
        wnd.titleContent = new GUIContent("PlayerController");
        return wnd;
    }
    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceID, int line){
        Object target = EditorUtility.InstanceIDToObject(instanceID);
        if(target is EntryNode node){
            PCWindow wnd = Open();
            
            return true;
        }
        else return false;
    }
    private void CreateGUI(){
        graphView = new PCGraphView();
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }
#endregion Initialize
}
}