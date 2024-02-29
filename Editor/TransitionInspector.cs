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
    private Foldout foldout;
    private ListView listView;
    private Button button;
    
    public TransitionInspector(){
        Undo.undoRedoPerformed += OnUndoRedoPerformed;
    }
    public void Init(ParameterList list){
        parameterList = list;
        foldout = this.Q<Foldout>();
        listView = this.Q<ListView>();
        button = this.Q<Button>();
        listView.reorderable = false;
        listView.selectionType = SelectionType.Single;
        listView.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
        listView.makeItem = () => {
            VisualElement root = new VisualElement();
            root.style.flexDirection = FlexDirection.Row;
            DropdownField dropdownField = new DropdownField();
            foreach(var p in parameterList.parameters){
                dropdownField.choices.Add(p.GetName());
            }
            root.Add(dropdownField);
            return root;
        };
        listView.bindItem = (e, i) => {

        };

        button.clicked -= AddCondition;
        button.clicked += AddCondition;
    }

    private void OnUndoRedoPerformed(){
        ClearInspector();
    }

    public void UpdateInspector(PCEdgeView edge){
        Debug.Log(edge.transitionIndex);

        ClearInspector();
        edge.onDeleted += OnEdgeDeleted;
        edge.onUpdated += OnEdgeUpdated;
        this.edge = edge;
        nodeView = edge.output.node as PCNodeView;
        SerializedObject obj = new SerializedObject(nodeView.node);
        var fields = typeof(Transition).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach(var field in fields){
            if(!field.IsPublic && field.GetCustomAttribute<SerializeField>(true) == null) continue;
            if(field.GetCustomAttribute<HideInInspector>(true) != null) continue;
            if(field.Name.Equals("conditions")){
                LoadListView();
                return;
            }
            if(field.FieldType == typeof(int)){
                IntegerField intField = new IntegerField(){
                    label = field.Name
                };
                SetField(intField, field, obj);
            }
            else if(field.FieldType == typeof(float)){
                FloatField floatField = new FloatField(){
                    label = field.Name
                };
                SetField(floatField, field, obj);
            }
            else if(field.FieldType == typeof(bool)){
                Toggle toggle = new Toggle(){
                    label = field.Name
                };
                SetField(toggle, field, obj);
            }
            else if(field.FieldType == typeof(string)){
                TextField textField = new TextField(){
                    label = field.Name
                };
                textField.isDelayed = true;
                SetField(textField, field, obj);
            }
            else if(field.FieldType.IsEnum){
                EnumField enumField = new EnumField((System.Enum) field.GetValue(nodeView.node)){
                    label = field.Name
                };               
                SetField(enumField, field, obj);
            }
            else if(field.FieldType == typeof(Vector2)){
                Vector2Field vector2Field = new Vector2Field(){
                    label = field.Name
                };
                SetField(vector2Field, field, obj);
            }
            else if(field.FieldType == typeof(Vector3)){
                Vector3Field vector3Field = new Vector3Field(){
                    label = field.Name
                };
                SetField(vector3Field, field, obj);
            }
        }
    }

    private void SetField<T>(BaseField<T> field, FieldInfo fieldInfo, SerializedObject obj){
        SerializedProperty property = obj.FindProperty("transition").GetArrayElementAtIndex(edge.transitionIndex);
        if(!Application.isPlaying){
            field.BindProperty(property.FindPropertyRelative(fieldInfo.Name));
        }
        else{
            field.RegisterValueChangedCallback(callback => {
                fieldInfo.SetValue(edge.transition, callback.newValue);
            });
        }
        foldout.Add(field);
    }

    public void ClearInspector(){
        if(edge != null){
            edge.onDeleted -= OnEdgeDeleted;
            edge.onUpdated -= OnEdgeUpdated;
            edge = null;
        }
        foldout.Clear();
        listView.Clear();
    }
    public void LoadListView(){
        listView.Clear();
        listView.itemsSource = edge.transition.conditions.ToList();
        listView.Rebuild();
    }
    private void AddCondition(){
        if(parameterList.parameters.Count <= 0) return;
        Parameter param = parameterList.parameters[0];
        SerializedObject obj = new SerializedObject(nodeView.node);
        SerializedProperty property = obj.FindProperty("transition").GetArrayElementAtIndex(edge.transitionIndex).FindPropertyRelative("conditions");
        int index = edge.transition.conditions.Count;
        property.InsertArrayElementAtIndex(index);
        property = property.GetArrayElementAtIndex(index);
        property.FindPropertyRelative("paramName").stringValue = param.GetName();
        property.FindPropertyRelative("paramID").intValue = param.GetID();
        property.FindPropertyRelative("value").floatValue = 0;
        property = property.FindPropertyRelative("condition");
        switch(param.GetParameterType()){
            case ParameterType.Bool:
            property.SetEnumValue(TransitionCondition.Bool_False);
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

    }
    private void OnEdgeUpdated(){
        UpdateInspector(edge);
    }
    private void OnEdgeDeleted(){
        ClearInspector();
    }
}
}