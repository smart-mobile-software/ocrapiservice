//
//  OcrApiManagerDelegate.h
//  ocrapi
//
//  Created by Favaro on 29/11/11.
//  Copyright 2011 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>


@protocol OcrApiManagerDelegate <NSObject>

- (void) ocrApiPostStarted:(id) sender;
- (void) ocrApiPostDidFailed:(id) sender withError:(NSError *)error;
- (void) ocrApiPostDidFinish:(id) sender withData:(NSData *)data;


@end
