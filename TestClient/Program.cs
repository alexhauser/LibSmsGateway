using Microsoft.Extensions.Configuration;
using LibSmsGateway;
using System;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get API key (or legacy username and password) from secret app settings
            var config = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsecrets.json", false, true)
                .Build();

            // Create an SMS client
            SmsClient smsClient;
            if (!string.IsNullOrEmpty(config["apikey"]))
            {
                smsClient = new SmsClient(config["apikey"]);
            }
            else
            {
#pragma warning disable CS0618 // Username/password authentication is deprecated
                smsClient = new SmsClient(
                    config["user"], config["password"]);
#pragma warning restore CS0618
            }

            // Send a message
            var result = smsClient.Send(
                message: "This is a test message",  // message text
                recipient: "999999999999",          // destination number
                sender: "SenderName",               // sender name, optional
                group: null,                        // recipient group, optional
                receipt: false,                     // receive confirmation, optional
                flash: false                        // flash sms, optional
                ).Result;

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
        }
    }
}
