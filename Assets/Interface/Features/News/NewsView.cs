using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewsView : MonoBehaviour {
    static readonly Regex BoldRegex = new(@"\*\*(.+?)\*\*");
    static readonly Regex UnderlineRegex = new(@"__(.+?)__");
    static readonly Regex ItalicRegex = new(@"(?<!\*)\*(?!\*)(.+?)(?<!\*)\*(?!\*)");

    [SerializeField] TMP_Text _titleText;
    [SerializeField] TMP_Text _categoryText;
    [SerializeField] TMP_Text _createdAtText;
    [SerializeField] TMP_Text _contentText;
    [SerializeField] Button _backButton;
    [SerializeField] NewsSectionManager _sectionManager;

    void Awake() {
        _backButton.onClick.AddListener(ToListSection);
    }

    public async Task Show(long newsId) {
        var news = await SupabaseManager.Instance.News.LoadNewsById(newsId);
        if (news == null) {
            return;
        }

        _titleText.text = news.title;
        _categoryText.text = news.category;
        _createdAtText.text = news.createdAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
        _contentText.text = ConvertMarkdownToTmp(news.content);
        _sectionManager.ToViewSection();
    }

    public void ToListSection() {
        _sectionManager.ToListSection();
    }

    static string ConvertMarkdownToTmp(string markdown) {
        if (string.IsNullOrEmpty(markdown)) {
            return string.Empty;
        }

        var builder = new StringBuilder();
        string[] lines = markdown.Replace("\r\n", "\n").Split('\n');

        for (int i = 0; i < lines.Length; i++) {
            if (i > 0) {
                builder.Append('\n');
            }

            builder.Append(ConvertMarkdownLine(lines[i]));
        }

        return builder.ToString();
    }

    static string ConvertMarkdownLine(string line) {
        if (line.StartsWith("### ")) {
            return $"<size=110%>{ConvertInlineMarkdown(line[4..])}</size>";
        }

        if (line.StartsWith("## ")) {
            return $"<size=125%>{ConvertInlineMarkdown(line[3..])}</size>";
        }

        if (line.StartsWith("# ")) {
            return $"<size=150%>{ConvertInlineMarkdown(line[2..])}</size>";
        }

        return ConvertInlineMarkdown(line);
    }

    static string ConvertInlineMarkdown(string text) {
        text = BoldRegex.Replace(text, "<b>$1</b>");
        text = UnderlineRegex.Replace(text, "<u>$1</u>");
        text = ItalicRegex.Replace(text, "<i>$1</i>");
        return text;
    }
}
