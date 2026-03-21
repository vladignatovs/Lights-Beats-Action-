using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserFilterPanel : FilterPanelManager<UserMetadata> {
    [Header("Username Filter")]
    [SerializeField] TMP_InputField _usernameInput;
    [SerializeField] Toggle _usernameEqualsToggle;
    [SerializeField] Toggle _usernameStartsWithToggle;
    [SerializeField] Toggle _usernameContainsToggle;

    protected override List<IFilter> BuildFilters() {
        var filters = new List<IDataFilter<ServerUserMetadata>>();

        if (!string.IsNullOrWhiteSpace(_usernameInput.text)) {
            if (_usernameStartsWithToggle.isOn) {
                filters.Add(new UsernameStartsWithFilter(_usernameInput.text));
            } else if (_usernameContainsToggle.isOn) {
                filters.Add(new UsernameContainsFilter(_usernameInput.text));
            } else if (_usernameEqualsToggle.isOn) {
                filters.Add(new UsernameEqualsFilter(_usernameInput.text));
            }
        }

        var genericFilters = new List<IFilter>();
        foreach (var filter in filters) {
            genericFilters.Add(filter);
        }
        return genericFilters;
    }

    protected override void ClearInputs() {
        _usernameInput.text = "";
    }
}
