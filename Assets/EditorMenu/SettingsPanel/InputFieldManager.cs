using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldManager : MonoBehaviour {
    [SerializeField] GameObject _settingsPanel;
    [SerializeField] InputField _inputField;
    Dictionary<string, Func<string>> _fields;
    
    void OnEnable() {
        AssignInputValue();
    }

    public void AssignInputValue() {
        // Sets the _action field to the action of the parent action line
        var actionLineManager = _settingsPanel.GetComponent<SettingsPanelManager>().ActionLineManager;
        var action = actionLineManager.Action;
        // Creates a dictionary with key - value pairs, where key is a string, and value is a function
        _fields = new Dictionary<string, Func<string>>() {
            { "beat", () => action.beat.ToString() },
            { "times", () => action.times.ToString() },
            { "delay", () => action.delay.ToString() },
            { "position", () => action.position.x.ToString() + "; " + action.position.y.ToString() },
            { "rotation", () => action.rotation.eulerAngles.z.ToString() },
            { "scale", () => action.scale.x.ToString() + "; " + action.scale.y.ToString() },
            { "animationDuration", () => action.animationDuration.ToString() },
            { "lifeTime", () => action.lifeTime.ToString()},
            { "layer", () => actionLineManager.Layer.ToString()},
            // CUSTOM KEY - VALUE PAIRS, USED FOR CONTROLLERS
            { "targetGroup", () => action.scale.x.ToString()},
            // MOVE CONTROLLER
            { "targetSpeedX", () => action.position.x.ToString()},
            { "targetSpeedY", () => action.position.y.ToString()},
            { "acceleration", () => action.scale.y.ToString()}, // TODO: this
            // WIGGLE CONTROLLER
            { "frequency", () => action.position.x.ToString()},
            { "amplitude", () => action.position.y.ToString()},
            // RANDPOS CONTROLLER
            { "minPos", () => action.position.x.ToString()},
            { "maxPos", () => action.position.y.ToString()}
        };
        // foreach loop that loops through each mapping and chages the input name text to a value of containing key.
        foreach (var field in _fields) {
            if (_inputField.name.Contains(field.Key)) {
                _inputField.text = field.Value();
                break;
            }
        }
    }

    public void ReAssignInputValue() {
        foreach (var field in _fields) {
            if (_inputField.name.Contains(field.Key)) {
                _inputField.text = field.Value();
                break;
            }
        }
    }
}