using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Web;
using System.Collections.Specialized;

namespace LibSmsGateway
{
    /// <summary>
    /// A client for sending SMS messages over the <see href="www.sms-gateway.at">
    /// sms-gateway.at</see> HTTP2SMS API. Please register and configure an account 
    /// before using this.
    /// </summary>
    public class SmsClient
    {
        private const string _apiUrl = @"https://www.sms-gateway.at/sms/sendsms.php";
        private string _username;
        private string _password;
        private HttpClient _client;

        /// <summary>
        /// Creates a new <see cref="SmsClient"/> instance.
        /// </summary>
        /// <param name="username">The username of your sms-gateway.at account.</param>
        /// <param name="password">The HTTP2SMS API password generated within your sms-gateway.at account.</param>
        public SmsClient(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("Invalid username specified!", nameof(username));

            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Invalid password specified!", nameof(password));

            _username = username;
            _password = password;

            _client = new HttpClient();
        }

        /// <summary>
        /// Sends an SMS message.
        /// </summary>
        /// <param name="message">The message to be sent. Up to 459 chararcters are supported,
        /// and messages longer than 160 characters will be split into multiple messages, which
        /// will be charged separately.</param>
        /// <param name="recipient">The recipient phone number in E.164 format, e.g. "436641111111".
        /// Can be <c>null</c> if a valid <paramref name="group"/> is provided instead.</param>
        /// <param name="group">Name of a valid user group created in the user account panel.
        /// Can be <c>null</c> if a valid <paramref name="recipient"/> is provided.</param>
        /// <param name="sender">The sender name for the SMS message. Please note that 
        /// setting a sender name may cause extra charges. The sender name may consist of up to
        /// 10 characters, but may not include spaces or other special characters.</param>
        /// <param name="receipt">Set to <c>true</c> to get an acknowledgement of receipt.
        /// This will cause additional charges.</param>
        /// <param name="flash">Set to <c>true</c> to send a "flash message", which will be
        /// displayed directly on the recipient's start screen and cannot be saved.</param>
        /// <returns></returns>
        public async Task<SmsResult> Send(string message, string recipient, string sender = "",
            string group = "", bool receipt = false, bool flash = false)
        {
            if (message == null)
                throw new ArgumentException("Invalid message!", nameof(message));

            if (string.IsNullOrEmpty(recipient) && string.IsNullOrEmpty(group))
                throw new ArgumentException("Both recipient and recipient group are invalid!");

            NameValueCollection queryParams = new NameValueCollection();
            queryParams.Add("username", _username);
            queryParams.Add("validpass", _password);
            queryParams.Add("message", message);
            if (!string.IsNullOrWhiteSpace(recipient)) queryParams.Add("number[]", recipient);
            if (!string.IsNullOrWhiteSpace(group)) queryParams.Add("group[]", group);
            if (!string.IsNullOrWhiteSpace(sender)) queryParams.Add("absender", SanitizeSender(sender));
            queryParams.Add("receipt", receipt ? "1" : "0");
            queryParams.Add("flash", flash ? "1" : "0");
            queryParams.Add("encoding", "utf8");

            try
            {
                var fullQueryUrl = _apiUrl + ToQueryString(queryParams);
                var httpResponse = await _client.GetAsync(fullQueryUrl);

                return SmsResult.FromHttpResponse(httpResponse);
            }
            catch (Exception ex)
            {
                return new SmsResult(StatusCode.UnknownError, ex.Message, "");
            }
        }

        /// <summary>
        /// Helper function to turn a <see cref="NameValueCollection"/> into a query string.
        /// </summary>
        /// <param name="queryParams">The <see cref="NameValueCollection"/> containing the query parameters.</param>
        private string ToQueryString(NameValueCollection queryParams)
        {
            var array = (
                from key in queryParams.AllKeys
                from value in queryParams.GetValues(key)
                select string.Format(
                    "{0}={1}",
                    HttpUtility.UrlEncode(key),
                    HttpUtility.UrlEncode(value))
                ).ToArray();
            return "?" + string.Join("&", array);
        }

        /// <summary>
        /// Strips a list of invalid characters from the given <paramref name="sender"/> string 
        /// and truncates it to a length of 10 characters max.
        /// </summary>
        /// <param name="sender">The sender name to sanitize.</param>
        private string SanitizeSender(string sender)
        {
            sender = sender
                .Replace(" ", "")
                .Replace(",", "")
                .Replace(";", "")
                .Replace("-", "")
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace(":", "");

            if (sender.Length > 10)
                sender = sender.Substring(0, 10);

            return sender;
        }
    }
}
