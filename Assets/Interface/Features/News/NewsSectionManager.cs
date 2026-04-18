using JetBrains.Annotations;
using UnityEngine;

public enum NewsSection {
    List,
    View,
    Create
}

public class NewsSectionManager : MonoBehaviour {
    [SerializeField] GameObject _listSection;
    [SerializeField] GameObject _viewSection;
    [SerializeField] GameObject _createSection;

    public NewsSection CurrentSection { get; private set; }

    void Start() {
        ToListSection();
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
    public void ToggleCreateSection() {
        if (CurrentSection == NewsSection.Create) {
            ToListSection();
            return;
        }

        ToCreateSection();
    }

    void SetSection(NewsSection section) {
        CurrentSection = section;

        _listSection.SetActive(section == NewsSection.List);
        _viewSection.SetActive(section == NewsSection.View);
        _createSection.SetActive(section == NewsSection.Create);
    }
}
