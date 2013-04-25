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
        private ServiceResponce Send(HttpWebRequest request)
        {
            var responseFromServer = new ServiceResponce();
            using (WebResponse response = request.GetResponse())
            {
                responseFromServer.Status = ((HttpWebResponse)response).StatusDescription;
                using (Stream dataStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(dataStream))
                    {
                        responseFromServer.Response = reader.ReadToEnd();
                        reader.Close();
                    }
                    dataStream.Close();
                }
                response.Close();
            }
            return responseFromServer;
        }

        public ServiceResponce PostRequest(string url, IEnumerable<IFormPart> parts, int timeout, string authorization = null)
        {
            try
            {
                string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";

                if (authorization != null)
                {
                    request.Headers.Add("Authorization", authorization);
                }

                byte[] boundery = Encoding.UTF8.GetBytes(string.Concat(Environment.NewLine, "--", boundary, Environment.NewLine));
                byte[] footer = Encoding.UTF8.GetBytes(string.Concat(Environment.NewLine, "--", boundary, "--"));
                request.ContentType = string.Concat("multipart/form-data; boundary=", boundary);
                using (Stream dataStream = request.GetRequestStream())
                {
                    foreach (IFormPart part in parts)
                    {
                        dataStream.Write(boundery, 0, boundery.Length);
                        part.WriteFormPart(dataStream);
                    }

                    dataStream.Write(footer, 0, footer.Length);
                    dataStream.Close();
                }

                return Send(request);
            }
            catch (WebException webException)
            {
                HttpWebResponse response = webException.Response as HttpWebResponse;
                return response != null
                        ? new ServiceResponce() { Status = response.StatusCode.ToString(), Response = response.StatusDescription }
                        : new ServiceResponce() { Status = webException.Status.ToString(), Response = webException.Message };
            }
            catch (Exception exception)
            {
                return new ServiceResponce() { Status = "Failed", Response = exception.Message };
            }
        }
    }
}
