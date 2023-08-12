namespace Respicere.Server.Interfaces;

public interface IDataPreprocessor : IDataProcessor
{
    static abstract byte[] PreprocessData(byte[] data);
}
