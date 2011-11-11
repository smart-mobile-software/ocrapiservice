//author : Altaf A. M.
//Code Snippet to convert the captured/picker'd (sic) image via OCRApiService 
//Input parameter: UIImage
//output parameter: NSString
//updates possible(rather necessary): integrate it asynchronously

- (NSString *) upload:(UIImage *)myImage
{
	NSString *urlString = @"http://api.ocrapiservice.com/1.0/rest/ocr";
	NSMutableURLRequest *request = [[[NSMutableURLRequest alloc] init] autorelease];
	[request setURL:[NSURL URLWithString:urlString]];
	[request setHTTPMethod:@"POST"];

	NSMutableData *body = [NSMutableData data];

	NSString *boundary = [NSString stringWithString:@"---------------------------14737809831466499882746641449"];
	NSString *contentType = [NSString stringWithFormat:@"multipart/form-data; boundary=%@", boundary];
	[request addValue:contentType forHTTPHeaderField:@"Content-Type"];

	// file
	//    NSData *imageData = UIImagePNGRepresentation(myImage);
	NSData *imageData = UIImageJPEGRepresentation(myImage, 1.0);
	[body appendData:[[NSString stringWithFormat:@"--%@", boundary] dataUsingEncoding:NSUTF8StringEncoding]];
	[body appendData:[[NSString stringWithString:@"Content-Disposition: attachment; name=\"image\"; filename=\".jpg\""] dataUsingEncoding:NSUTF8StringEncoding]];
	[body appendData:[[NSString stringWithString:@"Content-Type: application/octet-stream"] dataUsingEncoding:NSUTF8StringEncoding]];
	[body appendData:[NSData dataWithData:imageData]];
	[body appendData:[[NSString stringWithString:@""] dataUsingEncoding:NSUTF8StringEncoding]];

	// Text parameter1

	NSString *param1 = @"en";
	[body appendData:[[NSString stringWithFormat:@"--%@", boundary] dataUsingEncoding:NSUTF8StringEncoding]];
	[body appendData:[[NSString stringWithFormat:@"Content-Disposition: form-data; name=\"language\""] dataUsingEncoding:NSUTF8StringEncoding]];
	[body appendData:[[NSString stringWithString:param1] dataUsingEncoding:NSUTF8StringEncoding]];
	[body appendData:[[NSString stringWithString:@""] dataUsingEncoding:NSUTF8StringEncoding]];

	// Another text parameter

	NSString *param2 = @"YOUR_API_KEY";
	[body appendData:[[NSString stringWithFormat:@"--%@", boundary] dataUsingEncoding:NSUTF8StringEncoding]];
	[body appendData:[[NSString stringWithFormat:@"Content-Disposition: form-data; name=\"apikey\""] dataUsingEncoding:NSUTF8StringEncoding]];
	[body appendData:[[NSString stringWithString:param2] dataUsingEncoding:NSUTF8StringEncoding]];
	[body appendData:[[NSString stringWithString:@""] dataUsingEncoding:NSUTF8StringEncoding]];

	// close form

	[body appendData:[[NSString stringWithFormat:@"--%@--", boundary] dataUsingEncoding:NSUTF8StringEncoding]];

	// set request body

	[request setHTTPBody:body];

	//return and test

	NSData *returnData = [NSURLConnection sendSynchronousRequest:request returningResponse:nil error:nil];
	NSString *returnedString = [[[NSString alloc] initWithData:returnData encoding:NSUTF8StringEncoding] autorelease];


	//  NSLog(@"received string = %@", returnString);

	//	UIAlertView *returnedText = [[UIAlertView alloc] initWithTitle:@"Returned Text" message:[NSString stringWithFormat:@"%@", returnString] delegate:self cancelButtonTitle:@"Ok" otherButtonTitles:nil];

	//	[returnedText show ];

	//	[returnedText release];

	return returnedString;

}

