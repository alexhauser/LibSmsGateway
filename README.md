# LibSmsGateway
A .NET library for sending SMS messages via the www.sms-gateway.at `HTTP2SMS` API. Supports .NET Core 3.1+ and .NET Framework 4.8.

## Usage

### 1. Create and configure account
Please note that you will need an account at www.sms-gateway.at in order to make use of this library.

Create an API key in the customer area under _Schnittstellen_. The key starts with `sk_live_` and is only shown once, so store it safely.

### 2. Install NuGet package

Open the project/solution in Visual Studio, and open the console using the Tools > NuGet Package Manager > Package Manager Console command. 

Now run:
```Powershell 
Install-Package LibSmsGateway -ProjectName MyProject
```
...where _"MyProject"_ is the name of the project that needs SMS support.

### 3. Start using the library

```C#
// Import namespace
using LibSmsGateway;

// Create an SMS client.
// Get the API key from your sms-gateway.at account.
var smsClient = new SmsClient("sk_live_...");

// Send a message
var result = await smsClient.Send(
    message: "This is a test message",  // message text
    recipient: "999999999999",          // destination number
    sender: "SenderName",               // sender name, optional
    group: null,                        // recipient group, optional
    receipt: false,                     // receive confirmation, optional
    flash: false);                      // flash sms, optional

// Evaluate result
if (result.IsSuccess)
{
    // SMS sucesssfully delivered
}
else
{
    // There was an error, find out what happened
    Console.WriteLine(
    "SatusCode: {0}, StatusMessage: \"{1}\", RawResult:\r\n{2}\r\n",
    result.StatusCode.ToString(),
    result.StatusMessage,
    result.RawResponse);
}
```

> **Note:** The `SmsClient(username, password)` constructor still works but is deprecated, because sms-gateway.at is phasing out username/password authentication. Please switch to an API key.
## License

This project is published under the MIT license.
See https://raw.githubusercontent.com/alexhauser/LibSmsGateway/master/LICENSE for more information.
