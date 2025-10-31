using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GroupSettingsManager : MonoBehaviour {
    [SerializeField] Text _toggleGroupsToggleText;
    [SerializeField] Transform _groupListTransform;
    [SerializeField] InputField _addGroupInput;
    [SerializeField] GameObject _groupPanel;

    void OnEnable() {
        ObjDropDownManager.ChangedObjectEvent.AddListener(GenerateGroupPanels);
    }
    
    void OnDisable() {
        ObjDropDownManager.ChangedObjectEvent.RemoveListener(GenerateGroupPanels);
    }

    public void GenerateGroupPanels() {
        var settingsPanelManager = GetComponentInParent<SettingsPanelManager>();
        var groups = settingsPanelManager.ActionLineManager.Action.Groups;
        ClearGroupPanels();
        foreach(var group in groups) {
            // skipping the group "0", as it is default and should not be shown/deleted
            if(group == 0) {
                continue;
            }
            // Instantiating a groupPanel, settings its parent and text value.
            var groupPanel = Instantiate(_groupPanel, _groupListTransform);
            groupPanel.GetComponentInChildren<Text>().text = group.ToString();
            // Gettings the deleteGroupButton from a child of groupPanel, removing the listeners from it and adding a new listener
            // with the current group.
            var deleteGroupButton = groupPanel.GetComponentInChildren<Button>();
            deleteGroupButton.onClick.RemoveAllListeners();

            deleteGroupButton.onClick.AddListener(() => {
                settingsPanelManager.DeleteGroup(group);
                GenerateGroupPanels();
            });
        }
    }

    void ClearGroupPanels() {
        for(int i = 0; i < _groupListTransform.childCount; i++) {
            var _child = _groupListTransform.GetChild(i);
            if(_child.CompareTag("GroupPanel")) {
                Destroy(_child.gameObject);
            }
        }
    }

    public void ChangeToggleTitle(bool value) {;
        if(value) {
            _toggleGroupsToggleText.text = "Hide Groups";
        } else {
            _toggleGroupsToggleText.text = "Show Groups";
        }
    }

    public void ToggleGroupList(bool value) {
        _groupListTransform.gameObject.SetActive(value);
    }

    public void Minus() {

        int.TryParse(_addGroupInput.text, out int _textValue);
        // will always return _textValue - 1 unless it is negative, in which case will return 0
        _addGroupInput.text = Mathf.Max(_textValue - 1, 0).ToString();
    }

    public void Plus() {

        int.TryParse(_addGroupInput.text, out int _textValue);
        // Groups have a limit of 99999
        _addGroupInput.text = Mathf.Min(_textValue + 1, 99999).ToString();
    }

    // could be replaced by HashSet search method instead, but should only consider doing so when ALMSingleton contains more than a 
    // thousand of elements.
    public void FindNextFreeGroup() { 
        var nextFreeGroup = 1;
        const int maxGroup = 99999;

        while(ActionLineManager.ActionLineManagersSingleton.Any(actionLineManager => actionLineManager.Action.Groups.Any(group => group == nextFreeGroup)) && nextFreeGroup < maxGroup) {
            nextFreeGroup++;
        }
        _addGroupInput.text = nextFreeGroup.ToString();
    }
}
