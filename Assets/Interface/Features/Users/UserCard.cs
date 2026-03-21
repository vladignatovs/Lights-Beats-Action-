using TMPro;
using UnityEngine;

public class UserCard : MonoBehaviour, IUserCard {
    [SerializeField] TMP_Text _usernameText;

    public void Setup(UserMetadata metadata, IUserCardCallbacks callbacks) {
        _usernameText.text = metadata.username;
        // user callbacks go here!
    }
}
