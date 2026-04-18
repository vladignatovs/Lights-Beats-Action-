public interface INewsCard : ICard {
    void Setup(NewsMetadata metadata, INewsCardCallbacks callbacks);
}
