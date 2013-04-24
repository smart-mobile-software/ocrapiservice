using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OcrApi.Models.Abstract;

namespace OcrApi.Models
{
    public abstract class BaseFormPartModel<T> : IFormPart
    {
        private const string _contentDisposition = "Content-Disposition: form-data; name=\"{0}\"";
        public BaseFormPartModel(string name, T value, IDictionary<string, string> headers = null)
        {
            Name = name;
            Value = value;
            Headers = headers;
        }

        protected string Name { get; set; }
        protected T Value { get; set; }
        protected IDictionary<string, string> Headers { get; set; }

        public void WriteFormPart(Stream writableStream)
        {
            if (!writableStream.CanWrite)
            {
                return;
            }

            byte[] contentDisposition = Encoding.UTF8.GetBytes(GetContentDispositionString());
            writableStream.Write(contentDisposition, 0, contentDisposition.Length);

            if (Headers != null && Headers.Count() > 0)
            {
                foreach (var header in Headers)
                {
                    string headerText = string.Concat(Environment.NewLine, header.Key, ":", header.Value);
                    byte[] headerArray = Encoding.UTF8.GetBytes(headerText);
                    writableStream.Write(headerArray, 0, headerArray.Length);
                }
            }

            byte[] newLines = Encoding.UTF8.GetBytes(string.Concat(Environment.NewLine, Environment.NewLine));
            writableStream.Write(newLines, 0, newLines.Length);
            WriteValue(writableStream);
        }

        protected virtual string GetContentDispositionString()
        {
            return string.Format(_contentDisposition, Name);
        }

        protected abstract void WriteValue(Stream writableStream);
    }
}
