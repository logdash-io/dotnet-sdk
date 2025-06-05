namespace Logdash;

public interface ILogdash
{
    void Debug(params object[] data);
    void Error(params object[] data);
    void Info(params object[] data);
    void Verbose(params object[] data);
    void Http(params object[] data);
    void Silly(params object[] data);
    void Warn(params object[] data);
}