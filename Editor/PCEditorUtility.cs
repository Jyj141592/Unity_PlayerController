using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PlayerController.Editor{
public static class PCEditorUtility
{
    public static string NamespaceToClassName(string path){
        var name = path.Split('.');
        return name[^1];
    }
    public static string ToUpperFirstLetter(string str){
        if(str[0] == '_') str = str.Substring(1);
        if(str.Length == 0){
            return "";
        }
        else if(str.Length == 1){
            return char.ToUpper(str[0]).ToString();
        }
        else{
            return char.ToUpper(str[0]) + str.Substring(1);
        }        
    }
    public static string ToPropertyName(string str){
        string name = "";
        int i = 1;
        while(str[i] != '>'){
            name += str[i];
            i++;
        }
        return ToUpperFirstLetter(name);        
    }

    public static string ToInspectorName(string str){
        if(str[0] != '<')
            return ToUpperFirstLetter(str);
        else 
            return ToPropertyName(str);
    }

    public static void InvokeFunctionWithDelay(Action action, double delayTime){
        EditorCoroutine coroutine = new EditorCoroutine(action, delayTime);
        coroutine.Start();
    }

    private class EditorCoroutine{
        private Action func;
        private double delayTime;
        private double startTime;
        public EditorCoroutine(Action action, double delayTime){ 
            func = action;
            this.delayTime = delayTime;
        }
        public void Start(){
            EditorApplication.update += Update;
            startTime = EditorApplication.timeSinceStartup;
        }
        private void Update(){
            if(EditorApplication.timeSinceStartup > startTime + delayTime){
                Stop();
            }
        }
        private void Stop(){
            EditorApplication.update -= Update;
            func.Invoke();
        }
    }
}
}