using System.Net.Http;
using System.Xml.Linq;

namespace LibSmsGateway
{
    /// <summary>
    /// Provides error/success status information for an SMS send operation.
    /// </summary>
    public class SmsResult
    {
        /// <summary>
        /// The response <see cref="LibSmsGateway.StatusCode"/> returned by the HTTP2SMS API.
        /// </summary>
        public StatusCode StatusCode { get; internal set; }

        /// <summary>
        /// A message string representing the result of the operation.
        /// </summary>
        public string StatusMessage { get; }

        /// <summary>
        /// The raw HTTP XML response body received from the HTTP2SMS API.
        /// </summary>
        public string RawResponse { get; }

        /// <summary>
        /// Indicates whether the SMS message was successfully submitted.
        /// </summary>
        public bool IsSuccess { get => StatusCode == StatusCode.Success; }

        /// <summary>
        /// Creates a new <see cref="SmsResult"/> instance.
        /// </summary>
        /// <param name="statusCode">The <see cref="LibSmsGateway.StatusCode"/> for the new object.</param>
        /// <param name="statusMessage">The status message for the new object.</param>
        /// <param name="rawResponse">The raw HTTP API response body.</param>
        public SmsResult(StatusCode statusCode, string statusMessage, string rawResponse)
        {
            StatusCode = statusCode;
            StatusMessage = statusMessage;
            RawResponse = rawResponse;
        }

        /// <summary>
        /// Parses a given <see cref="HttpResponseMessage"/> and creates a corresponding 
        /// <see cref="SmsResult"/> object.
        /// </summary>
        /// <param name="httpResponse">The <see cref="HttpResponseMessage"/> object to parse.</param>
        public static SmsResult FromHttpResponse(HttpResponseMessage httpResponse)
        {
            string content = httpResponse.Content.ReadAsStringAsync().Result;

            if ( httpResponse == null ||
                 httpResponse.IsSuccessStatusCode == false || 
                 string.IsNullOrWhiteSpace(content))
            {
                goto ReturnUnknownError;
            }

            XElement xml = XElement.Parse(content);
            var result = xml?.Element("result")?.Value?.Trim();
            var errormessage = xml?.Element("errormessage")?.Value?.Trim();

            if (string.IsNullOrWhiteSpace(result))
                goto ReturnUnknownError;

            if (errormessage == null)
                errormessage = "";

            if (result == "OK:") return new SmsResult(StatusCode.Success, errormessage, content);
            if (result == "ERROR:")
            {
                var errorcodeStr = xml?.Element("errorcode")?.Value?.Trim();
                if (errorcodeStr == null) errorcodeStr = "999";

                int errorCode;
                bool success = int.TryParse(errorcodeStr, out errorCode);
                if (!success) errorCode = 999;

                if (StatusCode.IsDefined(typeof(StatusCode), errorCode))
                    return new SmsResult((StatusCode)errorCode, errormessage, content);
                else goto ReturnUnknownError;
            }

            ReturnUnknownError:
            return new SmsResult(StatusCode.UnknownError, "unknown error", content);
        }
    }
}
