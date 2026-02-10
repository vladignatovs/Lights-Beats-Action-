using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControllerGroupManager : MonoBehaviour {

    public static List<ControllerGroupManager> ControllerGroupManagersSingleton = new (); // holds all of the active controller instances.
    public Dictionary<GameObject, List<float>> ObjectsAndSettings = new();
    public int targetGroup;

    void OnEnable() {
        ControllerGroupManagersSingleton.Add(this);
    }
    void OnDisable() {
        ControllerGroupManagersSingleton.Remove(this);
    }

    /// <summary>
    /// Goes through all active gameObjects with groupManager instance, and checks whether it holds the target group.
    /// If it does, adds it to the list. DOES check if it already is in the list or not.
    /// </summary>
    void Start() {
        // gets the list of active groupManagers.
        var _groupManagers = GroupManager.GroupManagersSingleton;
        // loops through all active groupManagers
        foreach(var _groupManager in _groupManagers) {
            // checks if any group in the groupManager equals to the target group of THIS controller
            if(_groupManager.groups.Any(group => group == targetGroup) && !ObjectsAndSettings.ContainsKey(_groupManager.gameObject)) {
                // adds the gameObject that holds that gameObject
                // groupSharingObjects.Add(_groupManager.gameObject);
                ObjectsAndSettings.Add(_groupManager.gameObject, new ());
            }
        }
    }
}
