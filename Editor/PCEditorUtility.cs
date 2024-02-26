using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PlayerController.Editor{
public static class PCEditorUtility
{
    public static string NamespaceToClassName(string path){
        var name = path.Split('.');
        return name[^1];
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