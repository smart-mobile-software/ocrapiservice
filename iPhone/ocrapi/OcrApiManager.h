//
//  OcrApiManager.h
//  ocrapi
//
//  Created by Favaro on 29/11/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "OcrApiManagerDelegate.h"

#define kPostHttpStartStarted 1
#define kPostHttpStartFailedCreatingConnection -1
#define kPostHttpStartFailedConnectionInUse -2

@interface OcrApiManager : NSObject {
    
    // Store delegate object
    id<OcrApiManagerDelegate> delegate;
    
    // For HTTP connection
    NSURLConnection * theConnection;
    NSMutableData * receivedData;
    BOOL working;
    
    // Properties
    NSInteger tag; 
}
// Properties
@property (nonatomic, retain) id<OcrApiManagerDelegate> delegate;
@property NSInteger tag;

// Property for HTTP connection
@property (nonatomic, retain) NSMutableData * receivedData;

// Methods
-(BOOL) isBusy;
-(NSInteger) postImage:(UIImage *)image withApiKey:(NSString *)apiKey andLanguageCode:(NSString *)lang;

@end
