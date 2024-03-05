using System.Collections;
using System.Collections.Generic;
using PlayerController.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace PlayerController{
[CreateAssetMenu(fileName = "New PlayerController", menuName = "PlayerControllerAsset")]
[DisallowCreateNode]
public class PlayerControllerAsset : PCNode
{
    [HideInInspector]
    [SerializeField]
    private List<PCNode> _nodes;
    public List<PCNode> nodes{
        get => _nodes;
    }
    [HideInInspector]
    [SerializeField]
    private AnyState _anyState = null;
    public AnyState anyState{
        get => _anyState;
    }
    //[HideInInspector]
    [SerializeField]
    private ParameterList _parameterList = null;
    public ParameterList parameterList{
        get => _parameterList;
    }
    private const int indexOfEntry = -1;
    private const int indexOfAnyState = -2;
    private const int noFound = -10;
    public PlayerControllerAsset(){
        _parameterList = new ParameterList();
    }

    public PlayerControllerAsset CloneAsset(){
        PlayerControllerAsset asset = Instantiate(this);
        asset._anyState = Instantiate(anyState);

        //asset._parameterList = _parameterList.Clone();
        asset._parameterList.parameters.Sort();

        for(int i = 0; i < asset._nodes.Count; i++){
            asset._nodes[i] = Instantiate(asset._nodes[i]);
        }
        asset._nodes.Sort();
        asset.Init(asset);
        asset._anyState.Init(asset);
        for(int i = 0; i < asset._nodes.Count; i++){
            asset._nodes[i].Init(asset);
        }
        return asset;
    }

    public int FindIndexOfNode(string name){
        if(name.Equals("Entry")) return indexOfEntry;
        else if(name.Equals("Any State")) return indexOfAnyState;
        int id = Animator.StringToHash(name);
        int left = 0, right = _nodes.Count;
        int mid;
        while(left < right){
            mid = (left + right) / 2;
            if(_nodes[mid].actionID == id) return mid;
            else if(_nodes[mid].actionID < id) left = mid + 1;
            else right = mid;
        }
        return noFound;
    }

    public PCNode FindNodeByName(string name){
        int index = FindIndexOfNode(name);
        switch(index){
            case noFound:
            return null;
            case indexOfAnyState:
            return anyState;
            case indexOfEntry:
            return this;
            default:
            return _nodes[index];
        }
    }

    public PCNode FindNodeByIndex(int index){
        if(index == indexOfAnyState) return anyState;
        else if(index == indexOfEntry) return this;
        return _nodes[index];
    }
    
}
}