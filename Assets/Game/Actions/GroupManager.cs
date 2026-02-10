using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroupManager : MonoBehaviour {

    public static List<GroupManager> GroupManagersSingleton = new(); // holds all of the groupManager active instances
    public List<int> groups;
    void OnEnable() {
        GroupManagersSingleton.Add(this);
    }
    void OnDisable() {
        GroupManagersSingleton.Remove(this);
    }

    /// <summary>
    /// This method goes through all of the active controllers with controllerGroupManager instance, and checks if THIS objects groups
    /// equal to any of the targetGroups. If they do, adds them to the list. DOES check if the list already contains them.
    /// </summary>
    void Start() {
        // gets all the controllerGroupManagers of the active controllers
        var _controllerGroupManagers =  ControllerGroupManager.ControllerGroupManagersSingleton;
        // loops through each active controllerGroupManager
        foreach(var _controllerGroupManager in _controllerGroupManagers) {
            // checks if any of the groups of THIS groupManager equals to the targetGroup.
            if(groups.Any(group => group == _controllerGroupManager.targetGroup) && !_controllerGroupManager.ObjectsAndSettings.ContainsKey(gameObject)) {
                // adds THIS object to the groupSharingObjects list.
                // _controllerGroupManager.groupSharingObjects.Add(gameObject);
                _controllerGroupManager.ObjectsAndSettings.Add(gameObject, new ());
            }
        }
    }
}