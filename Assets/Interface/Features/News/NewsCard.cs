using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NewsCard : MonoBehaviour, INewsCard {
    [SerializeField] Button _openButton;
    [SerializeField] Button _editButton;
    [SerializeField] Button _deleteButton;
    [SerializeField] TMP_Text _titleText;
    [SerializeField] TMP_Text _categoryText;
    [SerializeField] TMP_Text _createdAtText;
    [SerializeField] Image _thumbnailImage;
    [SerializeField] GameObject _emptyText;

    int _thumbnailLoadVersion;

    void OnDisable() {
        _thumbnailLoadVersion++;
    }

    public void Setup(NewsMetadata metadata, INewsCardCallbacks callbacks) {
        bool canEdit = SupabaseManager.Instance.User.IsAdmin;

        _titleText.text = metadata.title;
        _categoryText.text = metadata.category.ToString();
        _createdAtText.text = metadata.createdAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
        _editButton.gameObject.SetActive(canEdit);
        _deleteButton.gameObject.SetActive(canEdit);

        _openButton.onClick.RemoveAllListeners();
        _editButton.onClick.RemoveAllListeners();
        _deleteButton.onClick.RemoveAllListeners();

        _openButton.onClick.AddListener(async () => await callbacks.OnOpenNews(metadata.id));

        if (canEdit) {
            _editButton.onClick.AddListener(async () => await callbacks.OnEditNews(metadata.id));
            _deleteButton.onClick.AddListener(async () => await callbacks.OnDeleteNews(metadata.id));
        }

        _thumbnailLoadVersion++;
        int thumbnailLoadVersion = _thumbnailLoadVersion;

        if (string.IsNullOrWhiteSpace(metadata.thumbnailUrl)) {
            SetEmptyThumbnailState();
            return;
        }

        _emptyText.SetActive(false);
        _ = LoadThumbnailAsync(metadata.thumbnailUrl, thumbnailLoadVersion);
    }

    async Task LoadThumbnailAsync(string url, int thumbnailLoadVersion) {
        _thumbnailImage.gameObject.SetActive(false);
        _emptyText.SetActive(false);

        using var request = UnityWebRequestTexture.GetTexture(url);
        var operation = request.SendWebRequest();

        while (!operation.isDone) {
            if (thumbnailLoadVersion != _thumbnailLoadVersion || this == null) {
                request.Abort();
                return;
            }

            await Task.Yield();
        }

        if (thumbnailLoadVersion != _thumbnailLoadVersion || this == null) {
            return;
        }

        if (request.result != UnityWebRequest.Result.Success) {
            SetEmptyThumbnailState();
            return;
        }

        var texture = DownloadHandlerTexture.GetContent(request);
        var sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );

        _thumbnailImage.sprite = sprite;
        _thumbnailImage.color = Color.white;
        _thumbnailImage.preserveAspect = true;
        _thumbnailImage.gameObject.SetActive(true);
        _emptyText.SetActive(false);
    }

    void SetEmptyThumbnailState() {
        _thumbnailImage.sprite = null;
        _thumbnailImage.color = new Color(0f, 0f, 0f, 0.5f);
        _thumbnailImage.preserveAspect = false;
        _thumbnailImage.gameObject.SetActive(true);
        _emptyText.SetActive(true);
    }
}
