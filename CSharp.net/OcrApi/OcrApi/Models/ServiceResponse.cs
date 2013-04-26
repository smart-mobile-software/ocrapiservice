using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OcrApi.Models
{
    public class ServiceResponse
    {
        public string Response { get; set; }
        public string Status { get; set; }

        public override string ToString()
        {
            return string.Concat(ControlsResource.STATUS, " ", Status, Environment.NewLine, Environment.NewLine, ControlsResource.RESPONSE, Environment.NewLine, Response);
        }
    }
}
