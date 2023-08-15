namespace Respicere.Server.Interfaces;

public interface IDataPreprocessor : IDataProcessor
{
    static abstract Task<object> PreprocessDataAsync(object data);
}
