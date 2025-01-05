using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleManager : MonoBehaviour {
    [SerializeField] GameObject _settingsPanel;
    [SerializeField] Toggle _toggle;
    Dictionary<string, Func<bool>> _toggles;
    
    void OnEnable() {
        ObjDropDownManager.ChangedObjectEvent.AddListener(AssignToggleState);
    }
    
    void OnDisable() {
        ObjDropDownManager.ChangedObjectEvent.RemoveListener(AssignToggleState);
    }
    
    void AssignToggleState() {
        var actionLineManager = _settingsPanel.GetComponent<SettingsPanelManager>().ActionLineManager;
        Action action = actionLineManager.Action;
        
        _toggles = new Dictionary<string, Func<bool>>() {
            { "ShowTimes", () => actionLineManager.ShowTimes },
            { "ShowLifeTime", () => actionLineManager.ShowLifeTime },
            // CUSTOM KEY - VALUE PAIRS, USED FOR CONTROLLERS
            { "vertical", () => action.scale.y % 2 == 0 && action.scale.y >= 1 && action.scale.y <= 4}, // returns true if scale is 2,4
            { "horizontal", () => action.scale.y > 2 && action.scale.y >= 1 && action.scale.y <= 4} // returns true if scale is 3,4
        };

        foreach(var toggle in _toggles) {
            if(_toggle.name.Contains(toggle.Key)) {
                _toggle.isOn = toggle.Value();
                break;
            }
        }
    }
}