<?php

// Setup parameters
$post = array(
'image'        =>    '@img.jpg',
'apikey'    =>    "mgFru85dqW",
'language'    =>    "en"
);

// Initiate the request
$ch = curl_init();
curl_setopt($ch, CURLOPT_URL, "http://api.ocrapiservice.com/1.0/rest/ocr");
curl_setopt($ch, CURLOPT_TIMEOUT, 10);
curl_setopt($ch, CURLOPT_HEADER, 0);
curl_setopt($ch, CURLOPT_VERBOSE, 0);
curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
curl_setopt($ch, CURLOPT_HTTPHEADER, array("Content-type: multipart/form-data"));
curl_setopt($ch, CURLOPT_USERAGENT, "Mozilla/5.0 (X11; Linux i686; rv:6.0) Gecko/20100101 Firefox/6.0Mozilla/4.0 (compatible;)");
curl_setopt($ch, CURLOPT_POST, true);
curl_setopt($ch, CURLOPT_POSTFIELDS, $post);
// Execture the request
$response = curl_exec($ch);

if (curl_errno($ch)) echo curl_error($ch);
curl_close($ch);

// Print response
echo $response;

?>