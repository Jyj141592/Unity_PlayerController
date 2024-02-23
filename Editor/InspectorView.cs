using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using Unity.VisualScripting.YamlDotNet.Core.Events;
using UnityEngine;
using UnityEditor.UIElements;
using System.Linq;
using System;
using UnityEditor.Experimental.GraphView;

namespace PlayerController.Editor{
public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits>{}
    private PCNodeView focused = null;
    public ScrollView scrollView;
    public ListView listView;

    private Func<VisualElement> makeItem;
    private Action<VisualElement, int> bindItem;
    public TransitionInspector transitionInspector;
    public InspectorView(){
        makeItem = () => new Label();
        bindItem = (e, i) => {
            Edge edge = focused?.outputPort.connections.ElementAt(i);
            e.Q<Label>().text = (edge as PCEdgeView).transition.dest.actionName;
        };
    }
    public void UpdateInspector(PCNodeView nodeView) {
        scrollView.Clear();
        listView.Clear();
        focused = nodeView;
        SerializedObject obj = new SerializedObject(nodeView.node);
        var fields = nodeView.node.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach(var field in fields){
            if(!field.IsPublic && field.GetCustomAttribute<SerializeField>(true) == null) continue;
            if(field.GetCustomAttribute<HideInInspector>(true) != null) continue;
            if(field.FieldType == typeof(int)){
                IntegerField intField = new IntegerField(){
                    label = field.Name
                };
                intField.value = (int) field.GetValue(nodeView.node);
                SetField(intField, field, obj);
            }
            else if(field.FieldType == typeof(float)){
                FloatField floatField = new FloatField(){
                    label = field.Name
                };
                floatField.value = (float) field.GetValue(nodeView.node);
                SetField(floatField, field, obj);
            }
            else if(field.FieldType == typeof(bool)){
                Toggle toggle = new Toggle(){
                    label = field.Name
                };
                toggle.value = (bool) field.GetValue(nodeView.node);
                SetField(toggle, field, obj);
            }
            else if(field.FieldType == typeof(string)){
                TextField textField = new TextField(){
                    label = field.Name
                };
                textField.value = (string) field.GetValue(nodeView.node);
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
                vector2Field.value = (Vector2) field.GetValue(nodeView.node);
                SetField(vector2Field, field, obj);
            }
            else if(field.FieldType == typeof(Vector3)){
                Vector3Field vector3Field = new Vector3Field(){
                    label = field.Name
                };
                vector3Field.value = (Vector3) field.GetValue(nodeView.node);
                SetField(vector3Field, field, obj);
            }
        }
        // Update transition list
        //listView = SetListView();
        //Add(listView);
    }
    private void SetField<T>(BaseField<T> field, FieldInfo fieldInfo, SerializedObject obj){
        if(!Application.isPlaying){
            field.BindProperty(obj.FindProperty(fieldInfo.Name));
        }
        else{
            field.RegisterValueChangedCallback(callback => {
                fieldInfo.SetValue(focused.node, callback.newValue);
            });
        }
        scrollView.Add(field);
    }
    private ListView SetListView(){

        ListView listView = new ListView(focused?.outputPort.connections.ToList(), -1,makeItem, bindItem);
        listView.selectionType = SelectionType.Single;
        listView.style.paddingLeft = listView.style.paddingRight = 5;
        listView.style.backgroundColor = new Color(0.4f,0.4f,0.4f);
        focused.updated = false;
        return listView;
    }
    public void Update(){
        // if(listView != null && focused != null && focused.updated){
        //     Remove(listView);
            
        //     listView = SetListView();
        //     Add(listView);
        // }
    }
}
}