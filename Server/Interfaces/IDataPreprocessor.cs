namespace Respicere.Server.Interfaces;

public interface IDataPreprocessor : IDataProcessor
{
    static abstract object PreprocessData(object data);
}
