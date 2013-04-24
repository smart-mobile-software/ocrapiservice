using System;
namespace OcrApi.Models
{
    [Serializable]
    public class PostImageModel
    {
        public string PicturePath { get; set; }
        public string LanguageCode { get; set; }
        public string ApiKey { get; set; }
    }
}
