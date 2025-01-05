using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ControllerGroupManager))]
public class MoveControllerManager : ControllerManager {

    float _targetSpeedX;
    float _targetSpeedY;

    float _acceleration; // might change all of this due to the fact that is replacable with movement easing and stuff

    void Update() {
        // Gets all the keys from the dictionary. The keys are GameObjects.
        var keys = new List<GameObject>(_controllerGroupManager.ObjectsAndSettings.Keys);
        // Loops through all the gameObjects, with the goal of updating the values of the key-value pairs, 
        // as well as changing the position of the objects
        foreach (var key in keys) {
            // If the gameObject is null, remove it from the dictionary and continue to the next gameObject
            if (key == null) {
                _controllerGroupManager.ObjectsAndSettings.Remove(key);
                continue;
            }
            /* Cool example of logic usage. If List is successfully recieved, will return false, and then check if it is too small. If can't be
            recieved, returns true right away and proceeds. */
            if (!_controllerGroupManager.ObjectsAndSettings.TryGetValue(key, out List<float> objectSpeeds) || objectSpeeds.Count < 2) {
                // Reinitializes the list, and sets its value to the new one in the dictionary.
                objectSpeeds = new List<float>() { 0f, 0f };
                _controllerGroupManager.ObjectsAndSettings[key] = objectSpeeds;
            }

            // Simple position calculation, moving the value to the targetSpeed by acceleration (surprisingly works with negative numbers.)
            objectSpeeds[0] = Mathf.MoveTowards(objectSpeeds[0], _targetSpeedX, _acceleration);
            objectSpeeds[1] = Mathf.MoveTowards(objectSpeeds[1], _targetSpeedY, _acceleration);

            var objectTransform = key.transform;
            var gObjectPosition = objectTransform.position;

            // changes the position to the newly calculated one.
            objectTransform.position = new Vector3(
                gObjectPosition.x + objectSpeeds[0] * Time.deltaTime,
                gObjectPosition.y + objectSpeeds[1] * Time.deltaTime,
                gObjectPosition.z
            );
        }
    }

    /// <summary>
    /// Sets the values of the following properties: 
    /// <br/> targetSpeedX = action.position.x
    /// <br/> targetSpeedY = action.position.y
    /// <br/> acceleration = action.scale.y
    /// </summary>
    /// <param name="action"></param>
    public override void SetAllUniqueValues(Action action) {
        _targetSpeedX = action.position.x;
        _targetSpeedY = action.position.y;
        _acceleration = action.scale.y;
    }
}
