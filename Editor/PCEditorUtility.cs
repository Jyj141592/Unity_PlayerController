using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController.Editor{
public static class PCEditorUtility
{
    public static string NamespaceToClassName(string path){
        var name = path.Split('.');
        return name[^1];
    }
}
}