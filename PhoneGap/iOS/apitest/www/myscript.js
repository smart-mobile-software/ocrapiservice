$(document).ready(function() {
                  
    // Set the language and YOUR api key here
    var languageFrom = "en";
    var apiKey = "Abcdefgh";
	



	// Album image picker
	$("#selectImageAlbum").click(function(){
		navigator.camera.getPicture(function(imageData) {

			window.resolveLocalFileSystemURI(imageData, pictureSuccess, null);

		}, null, { 
			// Image Picker parameters
			sourceType: Camera.PictureSourceType.PHOTOLIBRARY,
			destinationType: Camera.DestinationType.FILE_URI,
			MediaType: Camera.MediaType.PICTURE
		}); 
	});
	
	// Picture successfully taken              
	var pictureSuccess = function(fileEntry){

	   
	    // Get file info : name, uri, type
        fileType = null;
	    fileEntry.file(function(file) { fileType = file.type; }, null);
	    fileName = fileEntry.name;
	    fileURI = fileEntry.fullPath;
		alert('file :' + fileURI + ' selected. Now proceed to conversion');
                  
        // File upload options
        var options = new FileUploadOptions();

        options.fileKey = "image";
		options.fileName = fileName;
		options.mimeType = fileType;
		options.chunkedMode = false;    
        
		// Adding language and apikey parameters in the request
		var params = new Object();
		params.language = languageFrom;
		params.apikey = apiKey;
		options.params = params;
		// Doing request
		var fileTransfer = new FileTransfer();
		fileTransfer.upload(fileURI, "http://api.ocrapiservice.com/1.0/rest/ocr", function(response) {
					
			// Showing response data
			if (response.responseCode == 200) {
				//$("#resultText").html("Success");
			} else {
				$("#resultText").html("Failed");
			}

			// Request successfully completed 
			$("#extractedText").html(decodeURIComponent(response.response));
					
		}, function(error) {
			// Request unsuccessful


			// Showing error dialog
			switch (error.code) {
				case FileTransferError.FILE_NOT_FOUND_ERR: navigator.notification.alert("File not found.", null, "Failed"); break;
				case FileTransferError.INVALID_URL_ERR: navigator.notification.alert("Invalid URL.", null, "Failed"); break;
				case FileTransferError.CONNECTION_ERR: navigator.notification.alert("Connection error.", null, "Failed"); break;
				}
			}, options);
	       
	}
	
	
	
});