using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OcrApi.Models
{
    public class FileFormPartModel : BaseFormPartModel<string>
    {
        public FileFormPartModel(string name, string value, IDictionary<string, string> headers = null)
            : base(name, value, headers)
        {
        }

        protected override string GetContentDispositionString()
        {
            return string.Format("{0};filename={1}",base.GetContentDispositionString(), Value);
        }

        protected override void WriteValue(Stream writableStream)
        {
            byte[] buff = new byte[0x10000];
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
