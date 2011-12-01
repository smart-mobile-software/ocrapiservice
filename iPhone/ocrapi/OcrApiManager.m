//
//  OcrApiManager.m
//  ocrapi
//
//  Created by Favaro on 29/11/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import "OcrApiManager.h"

// Define the service URL
#define kApiURL @"http://api.ocrapiservice.com/1.0/rest/ocr"

@implementation OcrApiManager

@synthesize delegate, receivedData, tag;


#pragma mark - Methods

// Start the proccess of post data from a server and receive the answer
// Return 1 if started to download information, 
// Return -1 if fails
// Return -2 if already working
-(NSInteger) postImage:(UIImage *)image withApiKey:(NSString *)apiKey andLanguageCode:(NSString *)lang { 
    
    NSInteger result = -1;
    
    // Check if we can send data (if service is not running)
    if (theConnection == nil || working == NO) {
        
        // Create the request
        NSURL * postUrl = [NSURL URLWithString:kApiURL];
        NSMutableURLRequest * serviceRequest = [NSMutableURLRequest requestWithURL:postUrl cachePolicy:NSURLRequestReloadIgnoringCacheData timeoutInterval:60.0];
        
        
        // Generate message body
        NSMutableData *postData = [NSMutableData data];
        
        NSString *shortBoundary = [NSString stringWithString:@"---------------------------14737809831466499882746641349"];
        NSString *boundary = [NSString stringWithFormat:@"--%@", shortBoundary];
        NSString *contentType = [NSString stringWithFormat:@"multipart/form-data; boundary=%@", shortBoundary];
        
        // file
        //    NSData *imageData = UIImagePNGRepresentation(myImage);
        NSData *imageData = UIImageJPEGRepresentation(image, 1.0);
        [postData appendData:[boundary dataUsingEncoding:NSUTF8StringEncoding]];
        [postData appendData:[[NSString stringWithString:@"\r\nContent-Disposition: form-data; name=\"image\"; filename=\"mytest.jpg\""] dataUsingEncoding:NSUTF8StringEncoding]];
        [postData appendData:[[NSString stringWithString:@"\r\nContent-Type: image/jpg\r\n\r\n"] dataUsingEncoding:NSUTF8StringEncoding]];
        [postData appendData:imageData];

        // Language parameter
        [postData appendData:[[NSString stringWithFormat:@"\r\n%@", boundary] dataUsingEncoding:NSUTF8StringEncoding]];
        [postData appendData:[[NSString stringWithFormat:@"\r\nContent-Disposition: form-data; name=\"language\"\r\n\r\n"] dataUsingEncoding:NSUTF8StringEncoding]];
        [postData appendData:[lang dataUsingEncoding:NSUTF8StringEncoding]];
        
        // APIKey parameter
        [postData appendData:[[NSString stringWithFormat:@"\r\n%@", boundary] dataUsingEncoding:NSUTF8StringEncoding]];
        [postData appendData:[[NSString stringWithFormat:@"\r\nContent-Disposition: form-data; name=\"apikey\"\r\n\r\n"] dataUsingEncoding:NSUTF8StringEncoding]];
        [postData appendData:[[NSString stringWithString:apiKey] dataUsingEncoding:NSUTF8StringEncoding]];
        
        // Close post data
        [postData appendData:[[NSString stringWithFormat:@"\r\n%@--", boundary] dataUsingEncoding:NSUTF8StringEncoding]];
        
        // Post data information
        [serviceRequest setValue:contentType forHTTPHeaderField:@"Content-Type"];
        [serviceRequest setHTTPMethod:@"POST"];
        [serviceRequest setHTTPBody:postData];
        
        // Create the connection with the request
        theConnection = [[NSURLConnection alloc] initWithRequest:serviceRequest delegate:self];
        
        // Check if we did it
        if (theConnection) {
            
            // Report information
            NSLog(@"Connection started");
            working = YES;
            
            // Create the NSMutableData to hold the received data.
            // receivedData is an instance variable declared elsewhere.
            receivedData = [[NSMutableData data] retain];
            
            // Raise the event if any subcriptor 
            if (self.delegate != nil) {
                // Notify
                [self.delegate ocrApiPostStarted:self];
            }
            
            // It started to work
            result = kPostHttpStartStarted;
        } else {
            // It failed
            result = kPostHttpStartFailedCreatingConnection;
        }
        
    }
    else {
        // Report information
        NSLog(@"Connection already in use");
        // Return result
        result = kPostHttpStartFailedConnectionInUse;
    }
    
    // Return final result
    return result;
}


// Check if busy
-(BOOL) isBusy {
    // Check status
    if (theConnection == nil) 
        return NO;
    else
        return working;
}


#pragma mark - Download events handling

// All related to the connection object
// Happens when we receive the response
- (void) connection:(NSURLConnection *)connection didReceiveResponse:(NSURLResponse *)response {
    
    // This method is called when the server has determined that it
    // has enough information to create the NSURLResponse.
    
    // It can be called multiple times, for example in the case of a
    // redirect, so each time we reset the data.
    
    // receivedData is an instance variable declared elsewhere.
    [self.receivedData setLength:0]; 
}

// All related to the connection object
// Happens when we receive data
- (void) connection:(NSURLConnection *)connection didReceiveData:(NSData *)data {
    
    // Append the new data to receivedData.
    // receivedData is an instance variable declared elsewhere.
    // You can also use the connection:didReceiveData: method to provide an indication of the connection’s progress to the user.
    [receivedData appendData:data];
}


// All related to the connection object
// Happens when an error ocurred
- (void) connection:(NSURLConnection *)connection didFailWithError:(NSError *)error {
    
    // release the connection, and the data object
    [connection release];
    // receivedData is declared as a method instance elsewhere
    [receivedData release];
    // We're not working anymore
    working = NO;
    
    // inform the user
    NSLog(@"Connection failed! Error - %@ %@",
          [error localizedDescription],
          [[error userInfo] objectForKey:NSURLErrorFailingURLStringErrorKey]);
    
    // Raise the event if any subcriptor 
    if (self.delegate != nil) {
        // Notify
        [self.delegate ocrApiPostDidFailed:self withError:error];
    }
}


// All related to the connection object
// Happens with the connection ends
- (void) connectionDidFinishLoading:(NSURLConnection *)connection {
    
    // receivedData is declared as a method instance elsewhere
    NSLog(@"Succeeded! Received %d bytes of data",[receivedData length]);
    
    // Copy the result to throw it out
    NSData * resultData = [[NSData alloc] initWithData:receivedData];
    [resultData autorelease];
        
    // Release the connection, and the data object
    [connection release];
    [receivedData release]; // Be aware that this will release resultData reference
    // We're not working anymore
    working = NO;
    
    
    // Raise the event if any subcriptor 
    if (self.delegate != nil) {
        // Notify
        [self.delegate ocrApiPostDidFinish:self withData:resultData];
    }
}



#pragma mark - Memory handling

// Class constructor
- (id) init {
    if ((self = [super init])) {
        // Initialization stuff
    }
    return self;
}

// Release objects in memory
- (void) dealloc {
    [delegate release];	
    [super dealloc];
}




@end
