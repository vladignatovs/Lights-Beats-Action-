using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControllerPanelManager : MonoBehaviour {

    Dictionary<Action, int>  _controllerVisualizersAndIndexes = new();
    List<VisualizerManager> _childrenVMs = new();
    int _oldChildCount;

    void Update () {
        if(transform.childCount != _oldChildCount)
            UpdateDictionary();
        else
            UpdateChildsSiblingIndex();
    }
    /// <summary>
    /// Used to loop through the children, and add new actions into dictionary if needed.
    /// </summary>
    void UpdateDictionary() {
        _oldChildCount = transform.childCount;

        _childrenVMs.Clear();
        _childrenVMs.AddRange(transform.GetComponentsInChildren<VisualizerManager>().ToList());
        foreach(var vm in _childrenVMs) {
            var vmAction = vm.actionLineManager.Action;
            if(!_controllerVisualizersAndIndexes.ContainsKey(vmAction)) {
                _controllerVisualizersAndIndexes.Add(vmAction, vm.transform.GetSiblingIndex());
            }
        }
    }
    /// <summary>
    /// Used to set childs sibling index to the one saved in the dictionary in case it changed.
    /// </summary>
    void UpdateChildsSiblingIndex() {
        foreach(var vm in _childrenVMs) {
            var childTransform = vm.transform;
            var childAction = vm.actionLineManager.Action;
            if(_controllerVisualizersAndIndexes[childAction] != childTransform.GetSiblingIndex()) {
                childTransform.transform.SetSiblingIndex(_controllerVisualizersAndIndexes[childAction]);
            }
        }
    }
}
