using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Unsupported;
using Phone.Controls;
using Microsoft.Phone.Tasks;
using System.IO;
using System.Text;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;

namespace OnlineOCRApp
{
    /// <summary>
    /// The OCR Sample App for Windows Phone 7.
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        #region Private Data.

        /// <summary>
        /// Here list country - langcode map, we'll use this to present a list to user.
        /// </summary>
        private readonly Dictionary<string, string> langCodeMap = new Dictionary<string, string>
            {
                {"Arabic",                  "ara"},
                {"Bulgarian",	            "bul"},
                {"Catalan",	                "cat"},
                {"Czech",	                "ces"},
                {"Chinese (Simplified)",	"chi_sim"},
                {"Chinese (Traditional)",	"chi_tra"},
                {"Danish",	                "dan"},
                {"Danish (Fracktur)",	    "dan-frak"},
                {"German",	                "deu"},
                {"German (Fracktur)",	    "deu-frak"},
                {"Greek",	                "ell"},
                {"English",	                "eng"},
                {"Finnish",	                "fin"},
                {"French",	                "fra"},
                {"Hebrew",	                "heb"},
                {"Hindi",	                "hin"},
                {"Croatian",	            "hrv"},
                {"Hungarian",	            "hun"},
                {"Indonesian",	            "ind"},
                {"Italian",	                "ita"},
                {"Japanese",	            "jpn"},
                {"Korean",	                "kor"},
                {"Latvian",	                "lav"},
                {"Lithuanian",	            "lit"},
                {"Dutch",	                "nld"},
                {"Norwegian",	            "nor"},
                {"Polish",	                "pol"},
                {"Portuguese",	            "por"},
                {"Romanian",	            "ron"},
                {"Russian",	                "rus"},
                {"Slovakian",	            "slk"},
                {"Slovakian (Fracktur)",	"slk-frak"},
                {"Slovenian",	            "slv"},
                {"Spanish",	                "spa"},
                {"Serbian",	                "srp"},
                {"Swedish",	                "swe"},
                {"Tagalog",	                "tgl"},
                {"Thai",	                "tha"},
                {"Turkish",	                "tur"},
                {"Ukdrainian",	            "ukr"},
                {"Vietnamese",	            "vie"}
            };

        /// <summary>
        /// The langCode selector dialog.
        /// </summary>
        private PickerBoxDialog langCodeDlg = new PickerBoxDialog();

        /// <summary>
        /// The target photo stream from PhotoChooserTask.
        /// </summary>
        private Stream photoStream = null;

        /// <summary>
        /// The filename of selected photo.
        /// </summary>
        private string photoName = "";

        /// <summary>
        /// This string is the bound of a multipart web request.
        /// </summary>
        private string multipartBound = "";

        /// <summary>
        /// This string saves the latest language code.
        /// </summary>
        private string langCode = "eng";

        /// <summary>
        /// This string saves the api key from app.
        /// </summary>
        private string apiKey = "";

        /// <summary>
        /// This variable saves the selected photo as a bitmap.
        /// </summary>
        private WriteableBitmap selectedPhoto = null;

        /// <summary>
        /// This delegate is used for cross-thread call.
        /// </summary>
        /// <param name="text"></param>
        private delegate void ShowMessageBox(string text);

        #endregion

        #region Constructor.

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            // Setup the language code dialog.
            langCodeDlg.ItemSource = this.langCodeMap.Keys;
            langCodeDlg.Title = "SELECT THE LANGUAGE OF YOUR IMAGE";
            langCodeDlg.Closed += new EventHandler(langCodeDlg_Closed);
            TiltEffect.SetIsTiltEnabled(this, true);

            // Set the default language to 'English'.
            this.cmbLangCode.DataContext = "English";
            langCodeDlg.SelectedIndex = 11;
        }

        #endregion

        #region Private Methods.

        /// <summary>
        /// User click on this langcode button, show the selector.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbLangCode_Click(object sender, RoutedEventArgs e)
        {
            langCodeDlg.Show();
        }

        /// <summary>
        /// Update the langcode button while selector is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void langCodeDlg_Closed(object sender, EventArgs e)
        {
            string langKey = langCodeDlg.SelectedItem as string;
            this.langCode = this.langCodeMap[langKey];
            this.cmbLangCode.DataContext = langKey;
        }

        /// <summary>
        /// User want to select an image here, use PhotoChooserTask.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            PhotoChooserTask fileChooser = new PhotoChooserTask();
            fileChooser.Completed += new EventHandler<PhotoResult>(fileChooser_Completed);
            fileChooser.Show();
        }

        /// <summary>
        /// User choose an image from Image Gallery.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileChooser_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                this.photoStream = e.ChosenPhoto;

                //Save the stream into a bitmap, and each time we copy the
                //bitmap data into a new stream
                
                BitmapImage bmpImage = new BitmapImage();
                bmpImage.SetSource(e.ChosenPhoto);
                this.selectedPhoto = new WriteableBitmap(bmpImage);

                ///////////////////////////

                this.photoName = System.IO.Path.GetFileName(e.OriginalFileName);
                this.txtFileName.Text = "Selected: " + this.photoName;
            
            }
        }

        /// <summary>
        /// User selected an image then we do OCR with our web service.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConvert_Click(object sender, RoutedEventArgs e)
        {
            // No image selected, prompt message.
            if (this.photoName.Trim().Length <= 0)
            {
                MessageBox.Show("Please select an image first, to do this, click 'Select a file' button!",
                    "FORM SUBMIT ERROR", MessageBoxButton.OK
                    );
                return;
            }

            // No API KEY input, prompt message.
            this.apiKey = this.txtApiKey.Text.Trim();
            if (this.apiKey.Length <= 0)
            {
                MessageBox.Show("Please input an valid API KEY, if you don't have a KEY, please register to http://ocrapiservice.com/ first!",
                    "FORM SUBMIT ERROR", MessageBoxButton.OK
                    );
                return;
            }

            //We do not save the stream from PhotoChooserTask, we save
            //the bitmap. Then each time we read the stream from the bitmap.

            if (this.photoStream != null)
            {
                this.photoStream.Close();
                this.photoStream = new MemoryStream();
            }

            // write an image into the stream
            Extensions.SaveJpeg(this.selectedPhoto, photoStream,
                selectedPhoto.PixelWidth, selectedPhoto.PixelHeight, 0, 100);

            // reset the stream pointer to the beginning
            photoStream.Seek(0, 0);

            this.btnConvert.IsEnabled = false;
            this.multipartBound = Guid.NewGuid().ToString("N");

            HttpWebRequest httpWebRequest = HttpWebRequest.CreateHttp("http://api.ocrapiservice.com/1.0/rest/ocr");
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "multipart/form-data; boundary=" + this.multipartBound;

            IAsyncResult result = httpWebRequest.BeginGetRequestStream(
                new AsyncCallback(OnBeginGetRequestStream), httpWebRequest);
        }

        /// <summary>
        /// This allows us to copy content between two streams.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        private void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }

        /// <summary>
        /// This creates a web request stream, and we write POST data here.
        /// </summary>
        /// <param name="asyncResult"></param>
        private void OnBeginGetRequestStream(IAsyncResult asyncResult)
        {
            HttpWebRequest httpWebRequest = asyncResult.AsyncState as HttpWebRequest;
            using (Stream stream = httpWebRequest.EndGetRequestStream(asyncResult))
            {
                UTF8Encoding encoding = new UTF8Encoding();
                string formdata = "--" + multipartBound + "\r\nContent-Disposition: form-data; name=\"language\"\r\n\r\n" + this.langCode +
                    "\r\n--" + multipartBound + "\r\nContent-Disposition: form-data; name=\"image\"; filename=\"" + this.photoName + "\"" + "\r\n" + "Content-Type: image/"
                    + System.IO.Path.GetExtension(this.photoName).Replace(".", "") + "\r\n\r\n";
                byte[] bytes = encoding.GetBytes(formdata);
                stream.Write(bytes, 0, bytes.Length);

                this.CopyStream(this.photoStream, stream);

                formdata = "\r\n--" + multipartBound + "\r\nContent-Disposition: form-data; name=\"apikey\"\r\n\r\n" + this.apiKey + "\r\n" + multipartBound + "--\r\n";
                bytes = encoding.GetBytes(formdata);
                stream.Write(bytes, 0, bytes.Length);
            }

            httpWebRequest.BeginGetResponse(new AsyncCallback(OnEndGetRequestStream), httpWebRequest);
        }

        /// <summary>
        /// This will read the response text from server side.
        /// </summary>
        /// <param name="asyncResult"></param>
        private void OnEndGetRequestStream(IAsyncResult asyncResult)
        {
            HttpWebRequest httpWebRequest = asyncResult.AsyncState as HttpWebRequest;
            ShowMessageBox invoker = new ShowMessageBox(ShowServerResponse);

            try
            {
                HttpWebResponse webResponse = httpWebRequest.EndGetResponse(asyncResult) as HttpWebResponse;
                using (Stream stream = webResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream);
                    this.Dispatcher.BeginInvoke(invoker, reader.ReadToEnd());
                }
            }
            // Catch any server side error
            catch (WebException ex)
            {
                HttpWebResponse webResponse = ex.Response as HttpWebResponse;
                int statusCode = (int)webResponse.StatusCode;
                
                // If the status code is not OK
                if (statusCode != 200)
                {
                    // Display the server response error text here.
                    using (Stream stream = webResponse.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        this.Dispatcher.BeginInvoke(invoker, reader.ReadToEnd());
                    }
                }
            }
        }

        /// <summary>
        /// This will show the OCR result string.
        /// </summary>
        /// <param name="text"></param>
        private void ShowServerResponse(string text)
        {
            this.btnConvert.IsEnabled = true;
            MessageBox.Show(text.Trim(), "THE OCR RESULT", MessageBoxButton.OK);
        }

        #endregion
    }
}