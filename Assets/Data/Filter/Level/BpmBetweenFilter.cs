
public class BpmBetweenFilter : WhereFilter<ServerLevelMetadata> {
    public BpmBetweenFilter(float min, float max) 
        : base(x => x.Bpm >= min && x.Bpm <= max) {
    }
}
