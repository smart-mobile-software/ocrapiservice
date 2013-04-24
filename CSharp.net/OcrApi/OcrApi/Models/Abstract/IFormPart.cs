using System.IO;

namespace OcrApi.Models.Abstract
{
    public interface IFormPart
    {
        void WriteFormPart(Stream writableStream);
    }
}
