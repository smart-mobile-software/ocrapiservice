using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OcrApi.Models;
using System.Net;
using System.IO;
using OcrApi.Models.Abstract;

namespace OcrApi.Communicators
{
    public class ServiceCommunicator
    {
        public ServiceResponse PostRequest(string url, IEnumerable<IFormPart> parts, int timeout, string authorization = null)
        {
            try
            {
                // Boundary to separate form parts in the request
                string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
                // Creates request and sets method to POST
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";

                if (authorization != null)
                {
                    request.Headers.Add("Authorization", authorization);
                }

                // Converts boundary into byte array
                byte[] boundery = Encoding.UTF8.GetBytes(string.Concat(Environment.NewLine, "--", boundary, Environment.NewLine));
                // Creates footer byte array
                byte[] footer = Encoding.UTF8.GetBytes(string.Concat(Environment.NewLine, "--", boundary, "--"));
                // Fills ContentType header for multipart form data request with specific boundary
                request.ContentType = string.Concat("multipart/form-data; boundary=", boundary);
                // Fills request stream with request data
                using (Stream dataStream = request.GetRequestStream())
                {
                    // Writes each form part separated with boundary
                    foreach (IFormPart part in parts)
                    {
                        dataStream.Write(boundery, 0, boundery.Length);
                        part.WriteFormPart(dataStream);
                    }

                    // Writes footer
                    dataStream.Write(footer, 0, footer.Length);
                    //dataStream.Close();
                }

                // Sends request
                return Send(request);
            }
            catch (WebException webException)
            {
                HttpWebResponse response = webException.Response as HttpWebResponse;
                return response != null
                        ? new ServiceResponse() { Status = response.StatusCode.ToString(), Response = response.StatusDescription }
                        : new ServiceResponse() { Status = webException.Status.ToString(), Response = webException.Message };
            }
            catch (Exception exception)
            {
                return new ServiceResponse() { Status = "Failed", Response = exception.Message };
            }
        }

        private ServiceResponse Send(HttpWebRequest request)
        {
            var responseFromServer = new ServiceResponse();
            // Requests response
            using (WebResponse response = request.GetResponse())
            {
                responseFromServer.Status = ((HttpWebResponse)response).StatusDescription;
                // Reads response
                using (Stream dataStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(dataStream))
                    {
                        responseFromServer.Response = reader.ReadToEnd();
                    }
                }
            }
            return responseFromServer;
        }
    }
}
