using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OcrApi.Models.Abstract;

namespace OcrApi.Models
{
    /// <summary>
    /// Abstract form part model class, implements methods for form part creation and writing into request stream
    /// </summary>
    /// <typeparam name="T"> Type of the form part value</typeparam>
    public abstract class BaseFormPartModel<T> : IFormPart
    {
        // Content disposition header template
        private const string _contentDisposition = "Content-Disposition: form-data; name=\"{0}\"";

        public BaseFormPartModel(string name, T value, IDictionary<string, string> headers = null)
        {
            Name = name;
            Value = value;
            Headers = headers;
        }

        // Name of the form part
        protected string Name { get; set; }
        // Value of the form part
        protected T Value { get; set; }
        // Additional form part headers
        protected IDictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Writes form part into request stream
        /// </summary>
        /// <param name="writableStream"> Writable request stream</param>
        public void WriteFormPart(Stream writableStream)
        {
            if (!writableStream.CanWrite)
            {
                return;
            }

            // Writes Content disposition header
            byte[] contentDisposition = Encoding.UTF8.GetBytes(GetContentDispositionString());
            writableStream.Write(contentDisposition, 0, contentDisposition.Length);

            // Writes additional headers
            if (Headers != null && Headers.Count() > 0)
            {
                foreach (var header in Headers)
                {
                    string headerText = string.Concat(Environment.NewLine, header.Key, ":", header.Value);
                    byte[] headerArray = Encoding.UTF8.GetBytes(headerText);
                    writableStream.Write(headerArray, 0, headerArray.Length);
                }
            }

            // Writes form part value
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
