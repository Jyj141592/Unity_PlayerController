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

namespace PlayerController.Editor{
public class TransitionInspector : VisualElement
{
    public new class UxmlFactory : UxmlFactory<TransitionInspector, VisualElement.UxmlTraits>{}
    private PCEdgeView edge;
    private ParameterList parameterList;
    private Foldout foldout;
    private ListView listView;
    
    public TransitionInspector(){
        Undo.undoRedoPerformed += OnUndoRedoPerformed;
    }
    public void Init(ParameterList list){
        parameterList = list;
        foldout = this.Q<Foldout>();
        listView = this.Q<ListView>();
    }

    private void OnUndoRedoPerformed(){
        ClearInspector();
    }

    public void UpdateInspector(PCEdgeView edge){
        ClearInspector();
        this.edge = edge;
        PCNodeView nodeView = edge.output.node as PCNodeView;
        SerializedObject obj = new SerializedObject(nodeView.node);
        var fields = typeof(Transition).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach(var field in fields){
            if(!field.IsPublic && field.GetCustomAttribute<SerializeField>(true) == null) continue;
            if(field.GetCustomAttribute<HideInInspector>(true) != null) continue;
            if(field.Name.Equals("conditions")){
                
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
        foldout.Clear();
        listView.Clear();
    }
    public void Update(){
        if(edge != null){
            if(edge.updated){
                edge.updated = false;
                UpdateInspector(edge);
            }
            if(edge.deleted){
                edge = null;
                ClearInspector();
            }
        }

    }
    public void OnDestroy(){
        Undo.undoRedoPerformed -= OnUndoRedoPerformed;
    }
}
}