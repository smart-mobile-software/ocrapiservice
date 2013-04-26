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

        /// <summary>
        /// Gets possible language codes
        /// </summary>
        /// <returns>IEnumerable with codes from Languages.xml file</returns>
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

        /// <summary>
        /// Loads last saved request settings
        /// </summary>
        /// <returns>Model, created from saved settings or null</returns>
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

        /// <summary>
        /// Gets file path from model
        /// </summary>
        /// <param name="model">Current form model</param>
        /// <returns>Path from the current model or empty string if file does not exist or is of invalid type</returns>
        public string GetFilePath(PostImageModel model)
        {
            string pathValidationResult = ValidateFilePath(model);
            return string.IsNullOrEmpty(pathValidationResult)
                    ? model.PicturePath
                    : string.Empty;

        }

        /// <summary>
        /// Sends image to ocr service
        /// </summary>
        /// <param name="model">Model that contains values if the request properties</param>
        /// <param name="callback">Action to be executed after response is received</param>
        public void SendImage(PostImageModel model, Action<string> callback)
        {
            Task task = taskFactory.StartNew(() =>
                {
                    string result = string.Empty;
                    try
                    {
                        // Trys to save current model state
                        ToXML(model);
                        // Creating parts of the request to be sent
                        IList<IFormPart> parts = new List<IFormPart>();
                        parts.Add(new FileFormPartModel("image", model.PicturePath, new Dictionary<string, string>() { { "Content-Type", GetContentType(model.PicturePath) } }));
                        parts.Add(new StringFormPartModel("language", model.LanguageCode));
                        parts.Add(new StringFormPartModel("apikey", model.ApiKey));
                        // Creats communicator and sending request
                        ServiceCommunicator communicator = new ServiceCommunicator();
                        ServiceResponse response = communicator.PostRequest(_ocrApiServiceUrl, parts, 10000);
                        result = response.ToString();
                    }
                    catch
                    { }

                    return result;
                }
            ).ContinueWith(cTask => { if (callback != null) callback(cTask.Result); }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Displays file browsing window
        /// </summary>
        /// <param name="model">Current form model</param>
        /// <returns>Path to the selected file</returns>
        public string BrowseFile(PostImageModel model)
        {
            string fileToOpen = string.Empty;
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                //Sets initial browsing folder
                if (!string.IsNullOrEmpty(model.PicturePath))
                {
                    string directoryName = Path.GetDirectoryName(model.PicturePath);
                    if (Directory.Exists(directoryName))
                    {
                        fileDialog.InitialDirectory = directoryName;
                    }
                }

                // Adds allowed file types filter
                if (_allowedFileTypes != null && _allowedFileTypes.Count() > 0)
                {
                    fileDialog.Filter = BuildFileFilter();
                }


                //Displays dialog
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    fileToOpen = fileDialog.FileName;
                }
            }

            return fileToOpen;
        }

        /// <summary>
        /// Validates file path from model
        /// </summary>
        /// <param name="model">Current form model</param>
        /// <returns> Error message if any or empty string</returns>
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

        private string GetContentType(string path)
        {
            string type = Path.GetExtension(path);
            return _imageTypes[type];
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
