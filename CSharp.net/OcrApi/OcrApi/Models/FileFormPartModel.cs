using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OcrApi.Models
{
    /// <summary>
    /// Implementation of BaseFormPartModel class for file to be added into request
    /// </summary>
    public class FileFormPartModel : BaseFormPartModel<string>
    {
        public FileFormPartModel(string name, string value, IDictionary<string, string> headers = null)
            : base(name, value, headers)
        {
        }

        protected override string GetContentDispositionString()
        {
            return string.Format("{0};filename='{1}'",base.GetContentDispositionString(), Value);
        }

        protected override void WriteValue(Stream writableStream)
        {
            // Creates buffer to read file parts
            byte[] buff = new byte[0x10000];
            // Reads file content into stream
            using (FileStream fs = new FileStream(Value, FileMode.Open))
            {
                for (int bytesRead = fs.Read(buff, 0, buff.Length); bytesRead > 0; bytesRead = fs.Read(buff, 0, buff.Length))
                {
                    writableStream.Write(buff, 0, bytesRead);
                }
            }
        }
    }
}
