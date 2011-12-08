iPhone full example
---------------

In order to use the code in other projects here is a quick guideline:

- Import OcrApiManager* files (they are the main engine)
- In the view you would like to use the code, add the delegate for the OcrApiManager and implement these methods:  
a) (void) ocrApiPostStarted:(id) sender;  
b) (void) ocrApiPostDidFailed:(id) sender withError:(NSError *)error;  
c) (void) ocrApiPostDidFinish:(id) sender withData:(NSData *)data;  

With the first method the post is started  
The second is raised when an error ocurred  
The third one returns the result in the data parameter

- Then to call the post method you should run these lines:  
    // Get the image  
    UIImage * img = [UIImage imageNamed:@"english_text.png"];  
    // Allocate the OCR API managerOcrApiManager * mgr = [[[OcrApiManager alloc] init] autorelease];  
    mgr.delegate = self;  
    [mgr postImage:img withApiKey:@"key" andLanguageCode:@"language"];  





