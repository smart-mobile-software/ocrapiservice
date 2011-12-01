//
//  ocrapiViewController.h
//  ocrapi
//
//  Created by Favaro on 29/11/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "OcrApiManagerDelegate.h"

// Implement PostHttpDataManager delegate to process the post events to the OCR API
@interface ocrapiViewController : UIViewController <UITextFieldDelegate, UIImagePickerControllerDelegate, UINavigationControllerDelegate,
    OcrApiManagerDelegate> {
    
    UILabel *lblImgSelected;
    UITextField *txtLanguageCode;
    UITextField *txtApiKey;
    UIImageView *imgPic;
        UIActivityIndicatorView *activityIndicator;
        UIButton *btnToText;
        UIButton *btnTestWithLocalImage;
}
// GUI Controls
@property (nonatomic, retain) IBOutlet UITextField *txtLanguageCode;
@property (nonatomic, retain) IBOutlet UITextField *txtApiKey;
@property (nonatomic, retain) IBOutlet UIImageView *imgPic;
@property (nonatomic, retain) IBOutlet UIActivityIndicatorView *activityIndicator;
@property (nonatomic, retain) IBOutlet UIButton *btnToText;
@property (nonatomic, retain) IBOutlet UIButton *btnTestWithLocalImage;

// Events
- (IBAction)btnToTextTouchUp:(id)sender;
- (IBAction)btnSelectFileTouchUp:(id)sender;
- (IBAction)btnTestWithLocaImageTouchUp:(id)sender;

// Methods
- (BOOL) startMediaBrowserFromViewController: (UIViewController*) controller
                               usingDelegate: (id <UIImagePickerControllerDelegate,
                                               UINavigationControllerDelegate>) delegate;

@end
