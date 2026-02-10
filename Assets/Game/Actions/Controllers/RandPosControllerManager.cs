using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ControllerGroupManager))]
public class RandPosControllerManager : ControllerManager {
    List<float> _randPositions = new();
    float _minPos;
    float _maxPos;
    int _directionState; // represents 4 options: 1 = none, 2 = vertical, 3 = horizontal, 4 = both
    void Start() {
        _lifeTime = _durationsManager.getLifeTimeInSeconds();
    }
    // for some reason LateUpdate doesn't create the visual bugs as Update() does, so will be used further on.
    new void LateUpdate() {
        base.LateUpdate();
        var keys = new List<GameObject>(_controllerGroupManager.ObjectsAndSettings.Keys);

        foreach (var key in keys) {
            if (key == null) {
                _controllerGroupManager.ObjectsAndSettings.Remove(key);
                continue;
            }

            var objectTransform = key.transform;
            var gObjectPosition = objectTransform.position;

            var randPosition = Random.Range(_minPos, _maxPos);
            
            if(objectTransform.CompareTag("Telegraph")) {
                _randPositions.Add(randPosition);
            } else if (_randPositions.Count > 0 && objectTransform.CompareTag("Attack")) {
                randPosition = _randPositions[0];
                _randPositions.RemoveAt(0);
            }

            objectTransform.position = new Vector3(
                _directionState > 2 ? randPosition : gObjectPosition.x, // if state is 3 or 4 (HORIZONTAL/x)
                _directionState % 2 == 0 ? randPosition : gObjectPosition.y, // if state is 2 or 4 (VERTICAL/y)
                gObjectPosition.z
            );
            _controllerGroupManager.ObjectsAndSettings.Remove(key);
        }
    }
    public override void SetAllUniqueValues(Action action) {
        _minPos = action.PositionX;
        _maxPos = action.PositionY;
        _directionState = (int)action.ScaleY;
    }
}