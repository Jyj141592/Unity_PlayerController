using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PlayerController{
public class PlayerControl : MonoBehaviour
{
    public PlayerControllerAsset playerControllerAsset;

    private void Awake() {
        playerControllerAsset = playerControllerAsset?.CloneAsset();
        playerControllerAsset?.Init(gameObject);
    }

    private void Update() {
        playerControllerAsset?.Run();
    }

    public int GetIndexOfParameter(string paramName){
        return playerControllerAsset.parameterList.FindIndexOfParameter(paramName);
    }
    public void SetInt(string paramName, int value){
        playerControllerAsset.parameterList.SetInt(paramName, value);
    }
    public void SetInt(int index, int value){
        playerControllerAsset.parameterList.SetInt(index, value);
    }
    public int GetInt(string paramName){
        return playerControllerAsset.parameterList.GetInt(paramName);
    }
    public int GetInt(int index){
        return playerControllerAsset.parameterList.GetInt(index);
    }
    public void SetFloat(string paramName, float value){
        playerControllerAsset.parameterList.SetFloat(paramName, value);
    }
    public void SetFloat(int index, float value){
        playerControllerAsset.parameterList.SetFloat(index, value);
    }
    public float GetFloat(string paramName){
        return playerControllerAsset.parameterList.GetFloat(paramName);
    }
    public float GetFloat(int index){
        return playerControllerAsset.parameterList.GetFloat(index);
    }
    public void SetBool(string paramName, bool value){
        playerControllerAsset.parameterList.SetBool(paramName, value);
    }
    public void SetBool(int index, bool value){
        playerControllerAsset.parameterList.SetBool(index, value);
    }
    public bool GetBool(string paramName){
        return playerControllerAsset.parameterList.GetBool(paramName);
    }
    public bool GetBool(int index){
        return playerControllerAsset.parameterList.GetBool(index);
    }
    public bool SetTransitionMute(string nodeName, int transitionIndex, bool value){
        return playerControllerAsset.SetTransitionMute(nodeName, transitionIndex, value);
    }
}
}