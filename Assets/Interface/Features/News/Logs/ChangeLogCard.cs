using System;
using TMPro;
using UnityEngine;

public class ChangeLogCard : MonoBehaviour {
    [SerializeField] TMP_Text _newsIdText;
    [SerializeField] TMP_Text _adminText;
    [SerializeField] TMP_Text _actionText;
    [SerializeField] TMP_Text _createdAtText;

    public void Setup(ChangeLogMetadata metadata) {
        _newsIdText.text = $"News #{metadata.newsId}";
        _adminText.text = metadata.adminName;
        _actionText.text = FormatAction(metadata.action);
        _createdAtText.text = metadata.createdAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
    }

    static string FormatAction(string action) {
        if (string.IsNullOrWhiteSpace(action)) {
            return "Unknown";
        }

        action = action.ToLowerInvariant();
        return char.ToUpperInvariant(action[0]) + action[1..];
    }
}
