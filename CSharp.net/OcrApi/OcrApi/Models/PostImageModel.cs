using System;
namespace OcrApi.Models
{
    /// <summary>
    /// Model of the request content, displayed on the form
    /// </summary>
    [Serializable]
    public class PostImageModel
    {
        // Path to image file
        public string PicturePath { get; set; }
        // Language of the image content
        public string LanguageCode { get; set; }
        // Customers API access key
        public string ApiKey { get; set; }
    }
}
