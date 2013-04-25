using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using OcrApi.Models;
using OcrApi.Communicators;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;
using OcrApi.Models.Abstract;

namespace OcrApi
{
    public class OcrApiServiceManager
    {
        private const string _lastPostedRequest = @"State\Request.xml";
        private const string _languagesXMLPath = @"App_Data\Languages.xml";
        private const string _ocrApiServiceUrl = "http://api.ocrapiservice.com/1.0/rest/ocr";
        private const string _filterText = "Image Files ({0})|{1}";
        private const string _requestParamTemplate = "apikey={0}&language={1}&image=";
        private readonly IDictionary<string, string> _imageTypes = new Dictionary<string, string>()
        {
            {".png", "image/png"},
            {".jpg", "image/jpg"},
            {".jpeg", "image/jpg"}
        };
        private readonly IEnumerable<string> _allowedFileTypes = new[] { "*.png", "*.jpg", "*.jpeg" };
        private readonly XmlSerializer _uiModelSerializer;
        private readonly TaskFactory taskFactory;
        public OcrApiServiceManager()
        {
            _uiModelSerializer = new XmlSerializer(typeof(PostImageModel));
            taskFactory = new TaskFactory();
        }

        public IEnumerable<string> GetCodes()
        {
            if (!File.Exists(_languagesXMLPath))
            {
                return new string[0];
            }

            try
            {
                XDocument languagesDocument = XDocument.Load(_languagesXMLPath);
                IEnumerable<XElement> codesElements = languagesDocument.XPathSelectElements("//code");
                return codesElements.Select(element => element.Value);
            }
            catch
            {
                return new string[0];
            }
        }

        public PostImageModel LoadLastRequest()
        {
            PostImageModel model = null;
            try
            {
                using (StreamReader streamReader = new StreamReader(_lastPostedRequest))
                {
                    model = _uiModelSerializer.Deserialize(streamReader) as PostImageModel;
                }
            }
            catch
            { }

            return model ?? new PostImageModel();
        }

        public string GetFilePath(PostImageModel model)
        {
            return File.Exists(model.PicturePath)
                    && GetAllowedFileExtentions().Contains(Path.GetExtension(model.PicturePath))
                    ? model.PicturePath
                    : string.Empty;

        }

        public void SendImage(PostImageModel model, Action<string> callback)
        {
            Task task = taskFactory.StartNew(() =>
                {
                    string result = string.Empty;
                    try
                    {
                        ToXML(model);
                        ServiceCommunicator communicator = new ServiceCommunicator();
                        IList<IFormPart> parts = new List<IFormPart>();
                        parts.Add(new FileFormPartModel("image", model.PicturePath, new Dictionary<string, string>() { { "Content-Type", GetImageType(model.PicturePath) } }));
                        parts.Add(new StringFormPartModel("language", model.LanguageCode));
                        parts.Add(new StringFormPartModel("apikey", model.ApiKey));
                        ServiceResponce response = communicator.PostRequest(_ocrApiServiceUrl, parts, 10000);
                        result = response.ToString();
                    }
                    catch
                    { }

                    return result;
                }
            ).ContinueWith(cTask => callback(cTask.Result), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private string GetImageType(string path)
        {
            string type = Path.GetExtension(path);
            return _imageTypes[type];
        }

        public string BrowseFile(PostImageModel model)
        {
            string fileToOpen = string.Empty;
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                if (!string.IsNullOrEmpty(model.PicturePath))
                {
                    string directoryName = Path.GetDirectoryName(model.PicturePath);
                    if (Directory.Exists(directoryName))
                    {
                        fileDialog.InitialDirectory = directoryName;
                    }
                }

                if (_allowedFileTypes != null && _allowedFileTypes.Count() > 0)
                {
                    fileDialog.Filter = BuildFileFilter();
                }

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    fileToOpen = fileDialog.FileName;
                }
            }

            return fileToOpen;
        }

        public string ValidateFilePath(PostImageModel model)
        {
            if (string.IsNullOrEmpty(model.PicturePath) || !File.Exists(model.PicturePath))
            {
                return string.Format(ControlsResource.FILE_NOT_FOUND, model.PicturePath);
            }

            if (!GetAllowedFileExtentions().Contains(Path.GetExtension(model.PicturePath)))
            {
                return ControlsResource.INVALID_FILE;
            }

            return string.Empty;
        }

        private IEnumerable<string> GetAllowedFileExtentions()
        {
            return _allowedFileTypes.Select(fileType => Path.GetExtension(fileType));
        }

        private string BuildFileFilter()
        {
            return string.Format(_filterText, string.Join(", ", _allowedFileTypes), string.Join(";", _allowedFileTypes));
        }

        private void ToXML(PostImageModel model)
        {
            try
            {
                string directory = Path.GetDirectoryName(_lastPostedRequest);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.Delete(_lastPostedRequest);
                using (StreamWriter streamWriter = new StreamWriter(_lastPostedRequest))
                {
                    _uiModelSerializer.Serialize(streamWriter, model);
                }
            }
            catch
            { }
        }
    }
}
