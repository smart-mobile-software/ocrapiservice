using System.IO;
using System.Text;
using System;
using System.Collections.Generic;
namespace OcrApi.Models
{
    /// <summary>
    /// Implementation of BaseFormPartModel class for string value to be added into request
    /// </summary>
    public class StringFormPartModel : BaseFormPartModel<string>
    {
        public StringFormPartModel(string name, string value, IDictionary<string, string> headers = null) : base(name, value, headers)
        { }

        protected override void WriteValue(Stream writableStream)
        {
            // Writes string value into request
            byte[] value = Encoding.UTF8.GetBytes(Value);
            writableStream.Write(value, 0, value.Length);
        }
    }
}
