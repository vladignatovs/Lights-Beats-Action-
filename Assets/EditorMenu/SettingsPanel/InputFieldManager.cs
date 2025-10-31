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
        // Creates a dictionary with key - value pairs, where key is an input fields name 
        // and value is a function which puts the value into the field
        // TODO: fix the values not being correctly rounded
        _fields = new Dictionary<string, Func<string>>() {
            { "beat", () => action.Beat.ToString() },
            { "times", () => action.Times.ToString() },
            { "delay", () => action.Delay.ToString() },
            { "positionX", () => action.PositionX.ToString() },
            { "positionY", () => action.PositionY.ToString() },
            { "rotation", () => action.Rotation.ToString() },
            { "scaleX", () => action.ScaleX.ToString() },
            { "scaleY", () => action.ScaleY.ToString() },
            { "animationDuration", () => action.AnimationDuration.ToString() },
            { "lifeTime", () => action.LifeTime.ToString()},
            { "layer", () => actionLineManager.Layer.ToString()},
            // CUSTOM KEY - VALUE PAIRS, USED FOR CONTROLLERS
            { "targetGroup", () => action.ScaleX.ToString()},
            // MOVE CONTROLLER
            { "targetSpeedX", () => action.PositionX.ToString()},
            { "targetSpeedY", () => action.PositionY.ToString()},
            { "acceleration", () => action.ScaleY.ToString()}, // TODO: this
            // WIGGLE CONTROLLER
            { "frequency", () => action.PositionX.ToString()},
            { "amplitude", () => action.PositionY.ToString()},
            // RANDPOS CONTROLLER
            { "minPos", () => action.PositionX.ToString()},
            { "maxPos", () => action.PositionY.ToString()}
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