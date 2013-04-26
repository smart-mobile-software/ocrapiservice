using System.IO;

namespace OcrApi.Models.Abstract
{
    /// <summary>
    /// IFormPart declares methods to be implemented by concrete form part classes
    /// </summary>
    public interface IFormPart
    {
        /// <summary>
        /// Writes form part into request stream
        /// </summary>
        /// <param name="writableStream"> Writable request stream</param>
        void WriteFormPart(Stream writableStream);
    }
}
