using JetBrains.Annotations;
using UnityEngine;

public enum NewsSection {
    List,
    View,
    Create,
    Logs
}

public class NewsSectionManager : MonoBehaviour {
    [SerializeField] GameObject _listSection;
    [SerializeField] GameObject _viewSection;
    [SerializeField] GameObject _createSection;
    [SerializeField] GameObject _logsSection;

    public event System.Action<NewsSection> OnSectionChanged;
    public NewsSection CurrentSection { get; private set; } = NewsSection.List;

    void Start() {
        SetSection(CurrentSection, true);
    }

    [UsedImplicitly]
    public void ToListSection() {
        SetSection(NewsSection.List);
    }

    [UsedImplicitly]
    public void ToViewSection() {
        SetSection(NewsSection.View);
    }

    [UsedImplicitly]
    public void ToCreateSection() {
        SetSection(NewsSection.Create);
    }

    [UsedImplicitly]
    public void ToLogsSection() {
        SetSection(NewsSection.Logs);
    }

    [UsedImplicitly]
    public void ToggleCreateSection() {
        if (CurrentSection == NewsSection.Create) {
            ToListSection();
            return;
        }

        ToCreateSection();
    }

    void SetSection(NewsSection section, bool invoke = false) {
        bool changed = CurrentSection != section;
        CurrentSection = section;

        _listSection.SetActive(section == NewsSection.List);
        _viewSection.SetActive(section == NewsSection.View);
        _createSection.SetActive(section == NewsSection.Create);
        _logsSection.SetActive(section == NewsSection.Logs);

        if (changed || invoke) {
            OnSectionChanged?.Invoke(section);
        }
    }
}
