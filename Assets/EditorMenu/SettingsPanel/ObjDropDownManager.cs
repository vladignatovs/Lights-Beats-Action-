using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ObjDropDownManager : MonoBehaviour
{
    public ActionCreator actionCreator;
    public GameObject settingsPanel;
    public List<string> gObjectOptions;
    public Dropdown dropdown;
    public static UnityEvent ChangedObjectEvent = new();

    void Awake() {
        // finds the actionCreator script by using actionCreator object
        actionCreator = GameObject.FindGameObjectWithTag("ActionCreator").GetComponent<ActionCreator>();
        // sets the value of dropdown
        dropdown = GetComponent<Dropdown>();
        // clears dropdown and optionsList of the current options
        dropdown.ClearOptions();
        gObjectOptions.Clear();
        // adds a blank option, so that the user would have to select an option himself
        gObjectOptions.Add(" ");
        // a loop that goes through the list of gameObjects in the actionCreator, and adds those as options into the optionsList
        foreach(GameObject _option in actionCreator.CreatableObjects) {
            string option = _option.name.ToString(); //not sure about the usage of ToString() here
            gObjectOptions.Add(option);
        }
        // adds the optionsList as the list of options into the dropdown
        dropdown.AddOptions(gObjectOptions);
    }

    void OnEnable() {
        SetValueOfDropdown();
    }

    public void SetValueOfDropdown() {
        // sets the value of action through the current value of actionLineManager in settingsPanel
        var action = settingsPanel.GetComponent<SettingsPanelManager>().ActionLineManager.Action;
        // used to monitor whether the value of dropdown has been changed or not
        bool valueChanged = false;
        // sets the value of list of objects in actionCreator
        var gObjects = actionCreator.CreatableObjects;
        // sets length of list of objects to increase efficiency
        int gObjectsLength = gObjects.Length;
        // for loop that goes through each object and if its name matches the current gameObject in action, will set the value
        // of dropdown to its index in the list of options.
        for(int i = 0; i < gObjectsLength; i++) {
            if(gObjects[i].name.Equals(action.gObject)) { // this is weird, might change
                valueChanged = true;
                // sets the value of dropdown to the matching one
                dropdown.value = i+1;
                ToggleCertainInputFields(dropdown.value);
                break;
            }
        }
        // if value was not found in the end, sets the value to 0
        if(!valueChanged) {
            dropdown.value = 0;
        }
        ChangedObjectEvent.Invoke();
    }

    public void ToggleCertainInputFields(int value) {
        // sets the value of "helper" fields used to optimize the code
        var settingsPanelContent = settingsPanel.transform.GetChild(0);
        var amountOfChidren = settingsPanelContent.childCount;
        //for loop, which goes through each of the children
        for (int i = 0; i < amountOfChidren; i++) {
            // sets the value of inputField to the current child in the list of children
            var settingsPartPanel = settingsPanelContent.GetChild(i).gameObject;
            if (i == 2) {
                settingsPartPanel.SetActive(value != 8 && value != 9 && value != 10);
            }
            if(i == 3) {
                settingsPartPanel.SetActive(value == 8);
            }
            if(i == 4) {
                settingsPartPanel.SetActive(value == 9);
            }
            if(i == 5) {
                settingsPartPanel.SetActive(value == 10);
            }
        }
    }
}
