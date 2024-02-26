using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codice.CM.Client.Differences;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PlayerController.Editor{
public class ParameterView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ParameterView, VisualElement.UxmlTraits>{}
    private ToolbarSearchField searchField;
    private ToolbarMenu menu;
    private ListView listView;
    private int clickedIndex = -1;
    private double clickedTime = 0;

    //private List<string> test = new List<string>{"one", "two", "three", "four", "five"};
    
    private PlayerControllerAsset asset;
    private ParameterList parameterList;
#region Initialize
    public ParameterView(){
        Undo.undoRedoPerformed += UndoRedoPerformed;
    }
    private void UndoRedoPerformed(){
        // PCEditorUtility.InvokeFunctionWithDelay(() => {
        //     if(!Application.isPlaying) AssetDatabase.SaveAssets();
        //     LoadParameterView(); 
        // }, 0.01);       
        if(!Application.isPlaying) AssetDatabase.SaveAssets();
            LoadParameterView(); 
    }
    public void Init(PlayerControllerAsset asset){
        this.asset = asset;
        parameterList = asset.parameterList;
        // Initialize listview
        listView = this.Q<ListView>("ParameterList");
        listView.selectionType = SelectionType.Single;
        listView.reorderable = true;
        listView.reorderMode = ListViewReorderMode.Animated;
        listView.itemIndexChanged -= OnChangeListOrder;
        listView.itemIndexChanged += OnChangeListOrder;
        listView.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
        listView.makeItem = () => {
            VisualElement container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            Label label = new Label();
            label.style.flexGrow = label.style.flexShrink = 1;
            label.RegisterCallback<MouseUpEvent>((ev) => {
                if(IsDoubleClicked(ev)){
                    ChangeParamName(label);
                }
            });
            container.Add(label);
            return container;
        };
        listView.bindItem = (e, i) => {
            Label label = e.Q<Label>();
            //label.text = parameterList.parameters[i].GetName();
            SerializedObject obj = new SerializedObject(asset);
            SerializedProperty property = obj.FindProperty("parameterList").FindPropertyRelative("parameters").GetArrayElementAtIndex(i);
            label.BindProperty(property.FindPropertyRelative("name"));
                Toggle toggle = e.Q<Toggle>();
                if(toggle != null) e.Remove(toggle);
                IntegerField intField = e.Q<IntegerField>();
                if(intField != null) e.Remove(intField);
                FloatField floatField = e.Q<FloatField>();
                if(floatField != null) e.Remove(floatField);

            
            if(parameterList.parameters[i].GetParameterType() == ParameterType.Bool){
                toggle = new Toggle();
                toggle.SetValueWithoutNotify(parameterList.parameters[i].GetBool());
                toggle.RegisterValueChangedCallback((callback) => {
                    float val = callback.newValue ? 1 : 0;
                    SerializedObject obj = new SerializedObject(asset);
                    SerializedProperty property = obj.FindProperty("parameterList").FindPropertyRelative("parameters").GetArrayElementAtIndex(i);
                    property.FindPropertyRelative("value").floatValue = val;
                    obj.ApplyModifiedProperties();
                });
                e.Add(toggle);
            }
            else if(parameterList.parameters[i].GetParameterType() == ParameterType.Int){
                intField = new IntegerField();
                intField.isDelayed = true;
                intField.SetValueWithoutNotify(parameterList.parameters[i].GetInt());
                intField.RegisterValueChangedCallback((callback) => {
                    float val = callback.newValue;
                    SerializedObject obj = new SerializedObject(asset);
                    SerializedProperty property = obj.FindProperty("parameterList").FindPropertyRelative("parameters").GetArrayElementAtIndex(i);
                    property.FindPropertyRelative("value").floatValue = val;
                    obj.ApplyModifiedProperties();
                });
                intField.style.minWidth = 35;
                intField.style.maxWidth = 45;
                e.Add(intField);
            }
            else if(parameterList.parameters[i].GetParameterType() == ParameterType.Float){
                floatField = new FloatField();
                floatField.isDelayed = true;
                floatField.SetValueWithoutNotify(parameterList.parameters[i].GetFloat());
                floatField.RegisterValueChangedCallback((callback) => {
                    SerializedObject obj = new SerializedObject(asset);
                    SerializedProperty property = obj.FindProperty("parameterList").FindPropertyRelative("parameters").GetArrayElementAtIndex(i);

                    property.FindPropertyRelative("value").floatValue = callback.newValue;
                    obj.ApplyModifiedProperties();
                });
                floatField.style.minWidth = 35;
                floatField.style.maxWidth = 45;
                e.Add(floatField);
            }       
        };     
                
        // Initialize toolbar searchfield
        searchField = this.Q<ToolbarSearchField>();

        // Initialize toolbar menu
        menu = this.Q<ToolbarMenu>();
        menu.menu.AppendAction("int", (action) => {
            AddParameter(ParameterType.Int);
        });
        menu.menu.AppendAction("float", (action) => {
            AddParameter(ParameterType.Float);
        });
        menu.menu.AppendAction("bool", (action) => {
            AddParameter(ParameterType.Bool);
        });

        LoadParameterView();
    }
    
    private void LoadParameterView(){
        clickedIndex = -1;
        clickedTime = 0;
        listView.Clear();
        listView.itemsSource = parameterList.parameters.ToList();

        listView.Rebuild();
    }
#endregion Initialize

#region Modify List
    private void AddParameter(ParameterType type){
        string name = "New";
        switch(type){
            case ParameterType.Bool:
            name = "New Bool";
            break;
            case ParameterType.Float:
            name = "New Float";
            break;
            case ParameterType.Int:
            name = "New Int";
            break;
        }
        SerializedObject obj = new SerializedObject(asset);
        SerializedProperty p1 = obj.FindProperty("parameterList");
        SerializedProperty p2 = p1.FindPropertyRelative("parameters");
        int index = parameterList.parameters.Count;
        string uName = GetUniqueName(name);
        p2.InsertArrayElementAtIndex(index);
        p2.GetArrayElementAtIndex(index).FindPropertyRelative("name").stringValue = uName;
        p2.GetArrayElementAtIndex(index).FindPropertyRelative("paramID").intValue = Animator.StringToHash(uName);
        p2.GetArrayElementAtIndex(index).FindPropertyRelative("value").floatValue = 0;
        p2.GetArrayElementAtIndex(index).FindPropertyRelative("type").SetEnumValue(type);
        obj.ApplyModifiedProperties();

        AssetDatabase.SaveAssets();
            
        LoadParameterView();
        
        listView.SetSelection(index);
        listView.ScrollToItem(index);
        Label label = listView.GetRootElementForId(index).Q<Label>();
        ChangeParamName(label);
    }

    private void ChangeParamName(Label label){
        TextField textField = new TextField();
        label.Add(textField);
        textField.SetValueWithoutNotify(label.text);
        textField.selectAllOnFocus = true;
        textField.style.paddingLeft = textField.style.marginLeft = 0;
        textField.isDelayed = true;
        textField.RegisterCallback<FocusOutEvent>((ev) => {
            string newName = textField.value;
            if(!newName.Equals(label.text)){
                newName = GetUniqueName(newName);
                SerializedObject obj = new SerializedObject(asset);
                SerializedProperty property = obj.FindProperty("parameterList").FindPropertyRelative("parameters").GetArrayElementAtIndex(clickedIndex);
                property.FindPropertyRelative("name").stringValue = newName;
                property.FindPropertyRelative("paramID").intValue = Animator.StringToHash(newName);
                obj.ApplyModifiedProperties();
            }
            label.Remove(textField);
        });
        textField.Focus();
    }

    public void OnChangeListOrder(int oldPos, int newPos){
        SerializedObject obj = new SerializedObject(asset);
        SerializedProperty property = obj.FindProperty("parameterList").FindPropertyRelative("parameters");
        if(oldPos > newPos){
            for(int i = oldPos; i > newPos; i--){
                property.MoveArrayElement(i, i - 1);
            }
        }
        else{
            for(int i = oldPos; i < newPos; i++){
                property.MoveArrayElement(i, i + 1);
            }
        }
        obj.ApplyModifiedProperties();
    }


#endregion Modify List

#region Utility
    private bool IsDoubleClicked(MouseUpEvent ev){
        if(ev.button != 0){
            clickedTime = 0;
            clickedIndex = -1;
            return false;
        }
        double prevTime = clickedTime;
        int prevIdx = clickedIndex;
        clickedTime = EditorApplication.timeSinceStartup;
        clickedIndex = listView.selectedIndex;
        if(prevIdx == clickedIndex && clickedTime - prevTime < 0.3){
            //Debug.Log("double clicked");
            return true;
        }
        return false;
    }

    private void OnKeyDown(KeyDownEvent ev){
        if(ev.keyCode == KeyCode.Delete){
            int selected = listView.selectedIndex;
            if(selected < 0) return;
            SerializedObject obj = new SerializedObject(asset);
            SerializedProperty property = obj.FindProperty("parameterList").FindPropertyRelative("parameters");
            property.DeleteArrayElementAtIndex(selected);
            obj.ApplyModifiedProperties();
            LoadParameterView();
        }
    }

    private string GetUniqueName(string name){
        
        return name;
    }

    public void OnDestroy(){
        Undo.undoRedoPerformed -= UndoRedoPerformed;
        listView.UnregisterCallback<KeyDownEvent>(OnKeyDown);
    }
#endregion Utility
}
}