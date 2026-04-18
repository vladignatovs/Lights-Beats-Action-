using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class CreateNewsManager : MonoBehaviour {
    [SerializeField] NewsSectionManager _sectionManager;
    [SerializeField] TMP_InputField _titleInput;
    [SerializeField] TMP_InputField _contentInput;
    [SerializeField] TMP_InputField _thumbnailUrlInput;
    [SerializeField] NewsCategoryDropdownManager _categoryDropdown;
    [SerializeField] NewsPaginationManager _paginationManager;

    News _editingNews;

    void Start() {
        _categoryDropdown.EnsurePopulated();
    }

    void OnEnable() {
        _categoryDropdown.EnsurePopulated();

        if (_editingNews == null) {
            ClearInputs();
        }
    }

    void OnDisable() {
        _editingNews = null;
    }

    [UsedImplicitly]
    public async void CreateNews() {
        var dropdown = _categoryDropdown.GetComponent<TMP_Dropdown>();
        if (dropdown.value == 0) {
            return;
        }

        if (_editingNews == null) {
            await SupabaseManager.Instance.News.CreateNews(
                _titleInput.text,
                _contentInput.text,
                dropdown.options[dropdown.value].text,
                _thumbnailUrlInput.text
            );
        } else {
            await SupabaseManager.Instance.News.UpdateNews(new News {
                id = _editingNews.id,
                createdAt = _editingNews.createdAt,
                title = _titleInput.text,
                content = _contentInput.text,
                category = dropdown.options[dropdown.value].text,
                thumbnailUrl = _thumbnailUrlInput.text,
            });
        }

        ClearInputs();
        _sectionManager.ToListSection();
        await _paginationManager.ReloadPage();
    }

    public void BeginEdit(News news) {
        if (news == null) {
            return;
        }

        _editingNews = news;
        _categoryDropdown.EnsurePopulated();

        _titleInput.text = news.title;
        _contentInput.text = news.content;
        _thumbnailUrlInput.text = news.thumbnailUrl ?? string.Empty;

        var dropdown = _categoryDropdown.GetComponent<TMP_Dropdown>();
        dropdown.value = GetCategoryIndex(dropdown, news.category);

        _sectionManager.ToCreateSection();
    }

    void ClearInputs() {
        _editingNews = null;
        _titleInput.text = "";
        _contentInput.text = "";
        _thumbnailUrlInput.text = "";

        var dropdown = _categoryDropdown.GetComponent<TMP_Dropdown>();
        dropdown.value = 0;
    }

    static int GetCategoryIndex(TMP_Dropdown dropdown, string category) {
        for (int i = 0; i < dropdown.options.Count; i++) {
            if (dropdown.options[i].text == category) {
                return i;
            }
        }

        return 0;
    }
}
