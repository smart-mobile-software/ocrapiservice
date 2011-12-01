//
//  ocrapiViewController.m
//  ocrapi
//
//  Created by Favaro on 29/11/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "ocrapiViewController.h"
#import "OcrApiManager.h"
#import <MobileCoreServices/UTCoreTypes.h>

@implementation ocrapiViewController
@synthesize txtLanguageCode;
@synthesize txtApiKey;
@synthesize imgPic;
@synthesize activityIndicator;
@synthesize btnToText;
@synthesize btnTestWithLocalImage;


#pragma mark - Post to Http delegate

- (void) ocrApiPostStarted:(id) sender {
    // Show the acitivty indicator
    [self.activityIndicator startAnimating];
    NSLog(@"Post started");
}

- (void) ocrApiPostDidFailed:(id) sender withError:(NSError *)error {
    // Stop the activity indicator
    [self.activityIndicator stopAnimating];
    // Enable the buttons back
    self.btnTestWithLocalImage.enabled = YES;
    
    // Show a message with the error
    UIAlertView * alrt = [[UIAlertView alloc] initWithTitle:@"Error" message:@"There was an error while processing the image or posting to the service" delegate:nil cancelButtonTitle:@"OK" otherButtonTitles:nil];
    [alrt show];
    [alrt release];
    
    NSLog(@"Post failed");
}

- (void) ocrApiPostDidFinish:(id) sender withData:(NSData *)data {
    
    // Stop the acitivty indicator
    [self.activityIndicator stopAnimating];
    // Enable the buttons back
    self.btnTestWithLocalImage.enabled = YES;

    // Get the result in text format
    NSString * txt = [[NSString alloc] initWithData:data encoding:NSASCIIStringEncoding];   
    // Show a message with the text
    UIAlertView * alrt = [[UIAlertView alloc] initWithTitle:@"Success" message:txt delegate:nil cancelButtonTitle:@"OK" otherButtonTitles:nil];
    [alrt show];
    [alrt release];
    [txt release];
}

#pragma mark - GUI Interface

// Post the image and get the result
- (IBAction)btnToTextTouchUp:(id)sender {
    
    // Disable button so it's not pressed twice
    btnToText.enabled = NO;
    
    // Get the image (already loaded by the photo gallery)
    UIImage * img = self.imgPic.image;
    
    // Allocate the OCR API manager and execute the action (we need to release it later)
    OcrApiManager * mgr = [[[OcrApiManager alloc] init] autorelease];
    mgr.delegate = self;
    [mgr postImage:img withApiKey:self.txtApiKey.text andLanguageCode:self.txtLanguageCode.text];
}

// Select the image
- (IBAction)btnSelectFileTouchUp:(id)sender {
    
    // Start the media browser
    if ([self startMediaBrowserFromViewController:self usingDelegate:self] == NO) {
        // Can't open the media browser
        UIAlertView * alrt = [[UIAlertView alloc] initWithTitle:@"Error" message:@"Can't open the photo media browser in this device" delegate:nil cancelButtonTitle:@"OK" otherButtonTitles:nil];
        
        [alrt show];
        [alrt release];
    }
}

// Use the image in our bundle to post
- (IBAction)btnTestWithLocaImageTouchUp:(id)sender {
    
    // Disable button so it's not pressed twice
    btnTestWithLocalImage.enabled = NO;
    
    // Get the image
    UIImage * img = [UIImage imageNamed:@"english_text.png"];
    
    // Show the image preview
    self.imgPic.image = img;
        
    // Allocate the OCR API manager and execute the action (we need to release it later)
    OcrApiManager * mgr = [[[OcrApiManager alloc] init] autorelease];
    mgr.delegate = self;
    [mgr postImage:img withApiKey:self.txtApiKey.text andLanguageCode:self.txtLanguageCode.text];
}


#pragma mark - Gallery 

// Show the media browser with our settings, then the browser will call our delegate if needed
- (BOOL) startMediaBrowserFromViewController: (UIViewController*) controller
                               usingDelegate: (id <UIImagePickerControllerDelegate,
                                               UINavigationControllerDelegate>) delegate {
    
    if (([UIImagePickerController isSourceTypeAvailable:
          UIImagePickerControllerSourceTypeSavedPhotosAlbum] == NO)
        || (delegate == nil)
        || (controller == nil))
        return NO;
    
    UIImagePickerController *mediaUI = [[UIImagePickerController alloc] init];
    mediaUI.sourceType = UIImagePickerControllerSourceTypeSavedPhotosAlbum;
    
    // Check for images type
    NSArray * availableTypes = [UIImagePickerController availableMediaTypesForSourceType:UIImagePickerControllerSourceTypeSavedPhotosAlbum];
    BOOL imgsOnlyAvailable = NO;
    // Check if we have only images 
    for (int i = 0; i < [availableTypes count]; i++) {
        // Convert the type
        CFStringRef type = (CFStringRef) [availableTypes objectAtIndex:i];
        if (CFStringCompare ((CFStringRef) type, kUTTypeImage, 0) == kCFCompareEqualTo) {
            // We have images
            imgsOnlyAvailable = YES;
            break;
        }
    }
    
    // Check if they are available
    if (imgsOnlyAvailable == NO) 
        return NO;
    
    // Displays only saved pictures from the Camera Roll album.
    mediaUI.mediaTypes = [NSArray arrayWithObject:(id) kUTTypeImage];
    
    // Hides the controls for moving & scaling pictures, or for
    // trimming movies. To instead show the controls, use YES.
    mediaUI.allowsEditing = NO;
    
    mediaUI.delegate = delegate;
    
    [controller presentModalViewController: mediaUI animated: YES];
    return YES;
}

// Picker delegate
- (void)imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary *)info {
    
    NSString *mediaType = [info objectForKey: UIImagePickerControllerMediaType];
    UIImage *originalImage;
    
    // Handle a still image picked from a photo album
    if (CFStringCompare ((CFStringRef) mediaType, kUTTypeImage, 0)
        == kCFCompareEqualTo) {
        
        originalImage = (UIImage *) [info objectForKey:
                                     UIImagePickerControllerOriginalImage];    
        // Set image
        self.imgPic.image = originalImage;
        
        // Now set the button to enabled
        self.btnToText.enabled = YES;
    }
    
    // Hide picker selector
    [[picker parentViewController] dismissModalViewControllerAnimated: YES];
    [picker release];
}



// Picker has cancelled
-(void) imagePickerControllerDidCancel:(UIImagePickerController *)picker {
    // Hide picker selector
    [[picker parentViewController] dismissModalViewControllerAnimated: YES];
    [picker release];
}


#pragma mark - Keyboard delegate

// Hide keyboard
-(BOOL) textFieldShouldReturn:(UITextField *)textField {
    [textField resignFirstResponder];
    return YES;
}




#pragma mark - Memory management

- (void)dealloc
{
    [txtLanguageCode release];
    [txtApiKey release];
    [imgPic release];
    [activityIndicator release];
    [btnToText release];
    [btnTestWithLocalImage release];
    [super dealloc];
}

- (void)didReceiveMemoryWarning
{
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc that aren't in use.
}

#pragma mark - View lifecycle

/*
// Implement viewDidLoad to do additional setup after loading the view, typically from a nib.
- (void)viewDidLoad
{
    [super viewDidLoad];
}
*/

- (void)viewDidUnload
{
    [self setTxtLanguageCode:nil];
    [self setTxtApiKey:nil];
    [self setImgPic:nil];
    [self setActivityIndicator:nil];
    [self setBtnToText:nil];
    [self setBtnTestWithLocalImage:nil];
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    // Return YES for supported orientations
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}


@end
