
public class AudioPathEqualsFilter : WhereFilter<ServerLevelMetadata> {
    public AudioPathEqualsFilter(string audioPath) 
        : base(x => x.AudioPath == audioPath) {
    }
}
