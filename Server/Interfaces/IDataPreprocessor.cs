namespace Respicere.Server.Interfaces;

public interface IDataPreprocessor : IDataProcessor
{
    /// <summary>
    /// The static preprocessor for this type
    /// </summary>
    /// <param name="data">The data to be preprocessed.</param>
    /// <returns></returns>
    static abstract Task<object> PreprocessDataAsync(object data);
}
