using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public enum ServerSection {
    Level,
    User
}

public class ServerSectionManager : MonoBehaviour {
    [SerializeField] GameObject _levelSection;
    [SerializeField] GameObject _userSection;
    [SerializeField] Button _toggleLevelSectionButton;
    [SerializeField] Button _toggleUserSectionButton;

    public event System.Action<ServerSection> OnSectionChanged;
    public ServerSection CurrentSection { get; private set; } = ServerSection.Level;

    void Start() {
        SetSection(CurrentSection, true);
    }

    [UsedImplicitly]
    public void ToLevelSection() {
        SetSection(ServerSection.Level);
    }

    [UsedImplicitly]
    public void ToUserSection() {
        SetSection(ServerSection.User);
    }

    void SetSection(ServerSection section, bool invoke = false) {
        bool changed = CurrentSection != section;
        CurrentSection = section;

        bool isLevel = section == ServerSection.Level;
        _levelSection.SetActive(isLevel);
        _userSection.SetActive(!isLevel);
        _toggleLevelSectionButton.gameObject.SetActive(!isLevel);
        _toggleUserSectionButton.gameObject.SetActive(isLevel);

        if (changed || invoke) {
            OnSectionChanged?.Invoke(section);
        }
    }
}
