using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using System;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using Codice.Client.BaseCommands.Update;
using System.Linq;

namespace PlayerController.Editor{
public class TransitionInspector : VisualElement
{
    public new class UxmlFactory : UxmlFactory<TransitionInspector, VisualElement.UxmlTraits>{}
    private PCEdgeView edge;
    private PCNodeView nodeView;
    private ParameterList parameterList;
    private ParameterView parameterView;
    private Label transitionName;
    private Foldout foldout;
    private ListView listView;
    private Button button;
    
    public TransitionInspector(){
        Undo.undoRedoPerformed += OnUndoRedoPerformed;
    }
    public void Init(ParameterView view){
        parameterView = view;
        parameterList = view.parameterList;
        parameterView.onAddOrDeleted -= OnParameterChanged;
        parameterView.onAddOrDeleted += OnParameterChanged;
        foldout = this.Q<Foldout>();
        listView = this.Q<ListView>();
        button = this.Q<Button>();
        transitionName = this.Q<Label>("TransitionName");
        listView.reorderable = false;
        listView.selectionType = SelectionType.Single;
        listView.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
        listView.makeItem = () => {
            ConditionElement element = new ConditionElement();
            foreach(var p in parameterList.parameters){
                element.dropdown.choices.Add(p.name);
            }
            return element;
        };
        listView.bindItem = (e, i) => {
            BindItem(e, i);
        };

        button.clicked -= AddCondition;
        button.clicked += AddCondition;
    }

    private void BindItem(VisualElement e, int i){
        string name = edge.transition.conditions[i].paramName;
            int index = parameterView.ParameterIndex(name);
            ConditionElement element = e as ConditionElement;
            element.index = i;
            element.dropdown.RegisterValueChangedCallback((callback) => {
                if(Application.isPlaying){
                    element.dropdown.SetValueWithoutNotify(callback.previousValue);
                    return;
                }
                int idx = parameterView.ParameterIndex(callback.newValue);
                ParameterType t = parameterList.parameters[idx].paramType;
                SerializedObject obj = new SerializedObject(nodeView.node);
                SerializedProperty p = obj.FindProperty("_transitions").GetArrayElementAtIndex(edge.transitionIndex).FindPropertyRelative("_conditions").GetArrayElementAtIndex(i);
                p.FindPropertyRelative("_paramName").stringValue = callback.newValue;
                p.FindPropertyRelative("_paramID").intValue = Animator.StringToHash(callback.newValue);
                obj.ApplyModifiedProperties();

                switch(t){
                    case ParameterType.Bool:
                    element.ChangeToBool(nodeView.node, edge.transitionIndex);
                    break;
                    case ParameterType.Float:
                    element.ChangeToFloat(nodeView.node, edge.transitionIndex);
                    break;
                    case ParameterType.Int:
                    element.ChangeToInt(nodeView.node, edge.transitionIndex);
                    break;
                }
            });
            if(index < 0){
                element.ParameterNotExist();
            }
            else{
                element.dropdown.SetValueWithoutNotify(name);
                ParameterType t = parameterList.parameters[index].paramType;
                switch(t){
                    case ParameterType.Bool:
                    element.ChangeToBool(nodeView.node, edge.transitionIndex);
                    break;
                    case ParameterType.Float:
                    element.ChangeToFloat(nodeView.node, edge.transitionIndex);
                    break;
                    case ParameterType.Int:
                    element.ChangeToInt(nodeView.node, edge.transitionIndex);
                    break;
                }
            }
    }

    private void OnUndoRedoPerformed(){
        ClearInspector();
    }

    public void ChangeParameterView(ParameterView view){
        parameterView = view;
        parameterList = view.parameterList;
        parameterView.onAddOrDeleted -= OnParameterChanged;
        parameterView.onAddOrDeleted += OnParameterChanged;
        ClearInspector();
    }

    public void UpdateInspector(PCEdgeView edge){
        ClearInspector();
        transitionName.text = edge.ToString();
        edge.onDeleted += OnEdgeDeleted;
        this.edge = edge;
        nodeView = edge.output.node as PCNodeView;
        SerializedObject obj = new SerializedObject(nodeView.node);
        var fields = typeof(Transition).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach(var field in fields){
            if(!field.IsPublic && field.GetCustomAttribute<SerializeField>(true) == null) continue;
            if(field.Name.Equals("_conditions")){
                LoadListView();
                return;
            }
            if(field.GetCustomAttribute<HideInInspector>(true) != null) continue;
            string name = PCEditorUtility.ToInspectorName(field.Name);
            if(field.FieldType == typeof(int)){
                IntegerField intField = new IntegerField(){
                    label = name
                };
                SetField(intField, field, obj);
            }
            else if(field.FieldType == typeof(float)){
                FloatField floatField = new FloatField(){
                    label = name
                };
                SetField(floatField, field, obj);
            }
            else if(field.FieldType == typeof(bool)){
                Toggle toggle = new Toggle(){
                    label = name
                };
                SetField(toggle, field, obj);
            }
            else if(field.FieldType == typeof(string)){
                TextField textField = new TextField(){
                    label = name
                };
                textField.isDelayed = true;
                SetField(textField, field, obj);
            }
            else if(field.FieldType.IsEnum){
                EnumField enumField = new EnumField((System.Enum) field.GetValue(nodeView.node)){
                    label = name
                };               
                SetField(enumField, field, obj);
            }
            else if(field.FieldType == typeof(Vector2)){
                Vector2Field vector2Field = new Vector2Field(){
                    label = name
                };
                SetField(vector2Field, field, obj);
            }
            else if(field.FieldType == typeof(Vector3)){
                Vector3Field vector3Field = new Vector3Field(){
                    label = name
                };
                SetField(vector3Field, field, obj);
            }
        }
    }

    private void SetField<T>(BaseField<T> field, FieldInfo fieldInfo, SerializedObject obj){
        SerializedProperty property = obj.FindProperty("_transitions").GetArrayElementAtIndex(edge.transitionIndex);
        //if(!Application.isPlaying){
            field.BindProperty(property.FindPropertyRelative(fieldInfo.Name));
        //}
        foldout.Add(field);
    }

    public void ClearInspector(){
        transitionName.text = null;
        if(edge != null){
            edge.onDeleted -= OnEdgeDeleted;
            edge = null;
        }
        foldout.Clear();
        listView.Clear();
        listView.itemsSource = null;
        listView.Rebuild();
    }
    public void LoadListView(){
        listView.itemsSource = null;
        listView.Clear();
        listView.itemsSource = edge?.transition.conditions.ToList();
        listView.Rebuild();
    }
    private void AddCondition(){
        if(Application.isPlaying) return;
        if(parameterList.parameters.Count <= 0) return;
        if(nodeView == null) return;
        Parameter param = parameterList.parameters[0];
        SerializedObject obj = new SerializedObject(nodeView.node);
        SerializedProperty property = obj.FindProperty("_transitions").GetArrayElementAtIndex(edge.transitionIndex).FindPropertyRelative("_conditions");
        int index = edge.transition.conditions.Count;
        property.InsertArrayElementAtIndex(index);
        property = property.GetArrayElementAtIndex(index);
        property.boxedValue = new Condition();
        property.FindPropertyRelative("_paramName").stringValue = param.name;
        property.FindPropertyRelative("_paramID").intValue = param.paramID;
        property.FindPropertyRelative("value").floatValue = 0;
        property = property.FindPropertyRelative("_condition");
        switch(param.paramType){
            case ParameterType.Bool:
            property.SetEnumValue(TransitionCondition.Bool_True);
            break;
            case ParameterType.Float:
            property.SetEnumValue(TransitionCondition.Float_Greater);
            break;
            case ParameterType.Int:
            property.SetEnumValue(TransitionCondition.Int_Greater);
            break;
        }
        obj.ApplyModifiedProperties();
        LoadListView();
    }
    public void OnDestroy(){
        Undo.undoRedoPerformed -= OnUndoRedoPerformed;
    }

    private void OnKeyDown(KeyDownEvent ev){
        if(ev.keyCode == KeyCode.Delete){
            if(Application.isPlaying) return;
            int selected = listView.selectedIndex;
            if(selected < 0) return;
            SerializedObject obj = new SerializedObject(nodeView.node);
            SerializedProperty property = obj.FindProperty("_transitions").GetArrayElementAtIndex(edge.transitionIndex).FindPropertyRelative("_conditions");
            property.DeleteArrayElementAtIndex(selected);
            obj.ApplyModifiedProperties();
            LoadListView();
        }
    }
    private void OnEdgeDeleted(){
        ClearInspector();
    }
    private void OnParameterChanged(){
        LoadListView();
    }

    private class ConditionElement : VisualElement{
        public DropdownField dropdown;
        public VisualElement control;
        public int index;
        public ConditionElement(){
            VisualElement drop = new VisualElement();
            drop.style.flexGrow = 1;
            dropdown = new DropdownField();
            control = new VisualElement();
            control.style.flexDirection = FlexDirection.Row;
            dropdown.style.flexGrow = control.style.flexGrow = 1;
            this.style.flexDirection = FlexDirection.Row;
            drop.Add(dropdown);
            this.Add(drop);
            this.Add(control);
        }
        public void ParameterNotExist(){
            control.Clear();
            control.Add(new Label("Parameter does not exist"));
        }
        public void ChangeToBool(PCNode node, int edgeIdx){
            control.Clear();
            DropdownField d = new DropdownField();
            d.style.flexGrow = 1;
            d.choices.Add("true");
            d.choices.Add("false");
            SerializedObject obj = new SerializedObject(node);
            TransitionCondition c = node.transitions[edgeIdx].conditions[index].condition;
            if(c == TransitionCondition.Bool_False){
                d.SetValueWithoutNotify("false");
            }
            else if(c == TransitionCondition.Bool_True){
                d.SetValueWithoutNotify("true");
            }
            else{
                SerializedProperty p = obj.FindProperty("_transitions").GetArrayElementAtIndex(edgeIdx).FindPropertyRelative("_conditions").GetArrayElementAtIndex(index);
                p.FindPropertyRelative("_condition").SetEnumValue(TransitionCondition.Bool_True);
                p.FindPropertyRelative("value").floatValue = 0;
                obj.ApplyModifiedProperties();
                d.SetValueWithoutNotify("true");
            }
            d.RegisterValueChangedCallback((callback) => {
                TransitionCondition t = callback.newValue.Equals("false") ? TransitionCondition.Bool_False : TransitionCondition.Bool_True;
                SerializedObject obj = new SerializedObject(node);
                SerializedProperty p = obj.FindProperty("_transitions").GetArrayElementAtIndex(edgeIdx).FindPropertyRelative("_conditions").GetArrayElementAtIndex(index);
                p.FindPropertyRelative("_condition").SetEnumValue(t);
                obj.ApplyModifiedProperties();
            });
            control.Add(d);
        }
        public void ChangeToFloat(PCNode node, int edgeIdx){
            control.Clear();
            DropdownField d = new DropdownField();
            d.style.flexGrow = 1;

            d.choices.Add("Greater");
            d.choices.Add("Less");
            TransitionCondition c = node.transitions[edgeIdx].conditions[index].condition;
            if(c == TransitionCondition.Float_Greater){
                d.SetValueWithoutNotify("Greater");
            }
            else if(c == TransitionCondition.Float_Less){
                d.SetValueWithoutNotify("Less");
            }
            else{
                SerializedObject obj = new SerializedObject(node);
                SerializedProperty p = obj.FindProperty("_transitions").GetArrayElementAtIndex(edgeIdx).FindPropertyRelative("_conditions").GetArrayElementAtIndex(index);
                p.FindPropertyRelative("_condition").SetEnumValue(TransitionCondition.Float_Greater);
                p.FindPropertyRelative("value").floatValue = 0;
                obj.ApplyModifiedProperties();
                d.SetValueWithoutNotify("Greater");
            }
            d.RegisterValueChangedCallback((callback) => {
                TransitionCondition t = callback.newValue.Equals("Greater") ? TransitionCondition.Float_Greater : TransitionCondition.Float_Less;
                SerializedObject obj = new SerializedObject(node);
                SerializedProperty p = obj.FindProperty("_transitions").GetArrayElementAtIndex(edgeIdx).FindPropertyRelative("_conditions").GetArrayElementAtIndex(index);
                p.FindPropertyRelative("_condition").SetEnumValue(t);
                obj.ApplyModifiedProperties();
            });
            control.Add(d);

            FloatField f = new FloatField();
            f.style.flexGrow = 1;
            f.isDelayed = true;
            f.style.minWidth = 50;

            f.SetValueWithoutNotify(node.transitions[edgeIdx].conditions[index].GetFloat());
            f.RegisterValueChangedCallback((callback) => {
                SerializedObject obj = new SerializedObject(node);
                SerializedProperty p = obj.FindProperty("_transitions").GetArrayElementAtIndex(edgeIdx).FindPropertyRelative("_conditions").GetArrayElementAtIndex(index);
                p.FindPropertyRelative("value").floatValue = callback.newValue;
                obj.ApplyModifiedProperties();
            });
            control.Add(f);
        }
        public void ChangeToInt(PCNode node, int edgeIdx){
            control.Clear();
            DropdownField d = new DropdownField();
            d.style.flexGrow = 1;

            d.choices.Add("Greater");
            d.choices.Add("Equal");
            d.choices.Add("Less");
            TransitionCondition c = node.transitions[edgeIdx].conditions[index].condition;
            if(c == TransitionCondition.Int_Equal){
                d.SetValueWithoutNotify("Equal");
            }
            else if(c == TransitionCondition.Int_Greater){
                d.SetValueWithoutNotify("Greater");
            }
            else if(c == TransitionCondition.Int_Less){
                d.SetValueWithoutNotify("Less");
            }
            else{
                SerializedObject obj = new SerializedObject(node);
                SerializedProperty p = obj.FindProperty("_transitions").GetArrayElementAtIndex(edgeIdx).FindPropertyRelative("_conditions").GetArrayElementAtIndex(index);
                p.FindPropertyRelative("_condition").SetEnumValue(TransitionCondition.Int_Greater);
                p.FindPropertyRelative("value").floatValue = 0;
                obj.ApplyModifiedProperties();
                d.SetValueWithoutNotify("Greater");
            }
            d.RegisterValueChangedCallback((callback) => {
                TransitionCondition t;
                if(callback.newValue.Equals("Greater")){
                    t = TransitionCondition.Int_Greater;
                }
                else if(callback.newValue.Equals("Equal")){
                    t = TransitionCondition.Int_Equal;
                }
                else{
                    t = TransitionCondition.Int_Less;
                }
                SerializedObject obj = new SerializedObject(node);
                SerializedProperty p = obj.FindProperty("_transitions").GetArrayElementAtIndex(edgeIdx).FindPropertyRelative("_conditions").GetArrayElementAtIndex(index);
                p.FindPropertyRelative("_condition").SetEnumValue(t);
                obj.ApplyModifiedProperties();
            });
            control.Add(d);

            IntegerField i = new IntegerField();
            i.style.flexGrow = 1;
            i.isDelayed = true;

            i.SetValueWithoutNotify(node.transitions[edgeIdx].conditions[index].GetInt());
            i.RegisterValueChangedCallback((callback) => {
                SerializedObject obj = new SerializedObject(node);
                SerializedProperty p = obj.FindProperty("_transitions").GetArrayElementAtIndex(edgeIdx).FindPropertyRelative("_conditions").GetArrayElementAtIndex(index);
                p.FindPropertyRelative("value").floatValue = callback.newValue;
                obj.ApplyModifiedProperties();
            });
            control.Add(i);
        }
    }
    public void Update(){
        if(edge != null && edge.updated){
            edge.updated = false;
            UpdateInspector(edge);
        }
    }
}
}