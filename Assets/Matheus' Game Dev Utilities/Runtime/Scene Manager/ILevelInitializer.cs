namespace com.matheusbosc.utilities
{
    public interface ILevelInitializer
    {
        bool isDone { get; }
        float progress { get; }
    }
}