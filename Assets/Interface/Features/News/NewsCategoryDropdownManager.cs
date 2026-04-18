using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewsCategoryDropdownManager : MonoBehaviour {
    [SerializeField] TMP_Dropdown dropdown;
    bool _isPopulated;

    void Awake() {
        EnsurePopulated();
    }

    public void EnsurePopulated() {
        if (_isPopulated) {
            return;
        }

        dropdown.ClearOptions();
        dropdown.AddOptions(new List<string> {
            "None",
            NewsCategories.Announcement,
            NewsCategories.Update,
            NewsCategories.Other,
        });
        _isPopulated = true;
    }
}
