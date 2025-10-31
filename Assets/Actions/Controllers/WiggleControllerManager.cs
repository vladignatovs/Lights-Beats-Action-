using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ControllerGroupManager))]
public class WiggleControllerManager : ControllerManager {

    [SerializeField] float _frequency;
    [SerializeField] float _amplitude;
    int _directionState; // represents 4 options: 1 = none, 2 = vertical, 3 = horizontal, 4 = both
    void Start() {
        _lifeTime = _durationsManager.getLifeTimeInSeconds();
    }

    void Update() {
        if(!LogicManager.isPaused) {
            // Gets all the keys from the dictionary. Keys are GameObjects.
            var keys = new List<GameObject>(_controllerGroupManager.ObjectsAndSettings.Keys);
            // Loops through all the gameObjects, with the goal of updating the values of the key-value pairs, 
            // as well as changing the position of the objects
            foreach (var key in keys) {
                
                if (key == null) {
                    _controllerGroupManager.ObjectsAndSettings.Remove(key);
                    continue;
                }

                var objectTimer = key.GetComponent<DurationsManager>().Timer;
                var offset = Mathf.Sin(objectTimer * _frequency * Mathf.PI * 2) * _amplitude;
                var objectTransform = key.transform;
                var gObjectPosition = objectTransform.position;
                objectTransform.position = new Vector3(
                    gObjectPosition.x + (_directionState > 2 ? offset : 0) * Time.deltaTime, // if state is 3 or 4 (HORIZONTAL/x)
                    gObjectPosition.y + (_directionState % 2 == 0 ? offset : 0) * Time.deltaTime, // if state is 2 or 4 (VERTICAL/y)
                    gObjectPosition.z
                );
            }
        }
    }
    public override void SetAllUniqueValues(Action action) {
        _frequency = action.PositionX;
        _amplitude = action.PositionY;
        _directionState = (int)action.ScaleY;
    }
}