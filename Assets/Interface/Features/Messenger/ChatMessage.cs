using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessage : MonoBehaviour {
    [SerializeField] TMP_Text _messageText;
    [SerializeField] TMP_Text _timestampText;
    [SerializeField] Button _editButton;
    [SerializeField] Button _deleteButton;

    public void Setup(Message message, string senderName, bool canEdit, UnityEngine.Events.UnityAction onEdit, UnityEngine.Events.UnityAction onDelete) {
        _messageText.text = $"[{senderName}]: {message.Content}";
        _timestampText.text = message.CreatedAt.ToLocalTime().ToString("HH:mm dd.MM.yyyy");

        _editButton.gameObject.SetActive(canEdit);
        _deleteButton.gameObject.SetActive(canEdit);

        _editButton.onClick.RemoveAllListeners();
        _deleteButton.onClick.RemoveAllListeners();

        if (!canEdit) {
            return;
        }

        _editButton.onClick.AddListener(onEdit);
        _deleteButton.onClick.AddListener(onDelete);
    }
}
