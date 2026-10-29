using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace LibSmsGateway
{
    /// <summary>
    /// Provides the sent messages and error/success status information for an SMS history
    /// ("SMS-Ausgang") query.
    /// </summary>
    public class SmsHistoryResult : SmsResult
    {
        /// <summary>
        /// The username of the account whose history was queried.
        /// </summary>
        public string Account { get; internal set; }

        /// <summary>
        /// The start of the period actually queried, as reported by the API.
        /// </summary>
        public DateTimeOffset? From { get; internal set; }

        /// <summary>
        /// The end of the period actually queried, as reported by the API.
        /// </summary>
        public DateTimeOffset? To { get; internal set; }

        /// <summary>
        /// The returned page, starting at 1.
        /// </summary>
        public int Page { get; internal set; }

        /// <summary>
        /// The number of messages per page.
        /// </summary>
        public int PageSize { get; internal set; }

        /// <summary>
        /// The total number of messages matching the query, across all pages.
        /// </summary>
        public int TotalCount { get; internal set; }

        /// <summary>
        /// Indicates whether more pages are available. Request the next one with
        /// <see cref="SmsHistoryQuery.Page"/>.
        /// </summary>
        public bool HasMore { get; internal set; }

        /// <summary>
        /// The sent messages on this page. Empty (never <c>null</c>) if the query failed.
        /// </summary>
        public IReadOnlyList<SentSms> Messages { get; internal set; } = new SentSms[0];

        /// <summary>
        /// Creates a new <see cref="SmsHistoryResult"/> instance.
        /// </summary>
        /// <param name="statusCode">The <see cref="LibSmsGateway.StatusCode"/> for the new object.</param>
        /// <param name="statusMessage">The status message for the new object.</param>
        /// <param name="rawResponse">The raw HTTP API response body.</param>
        public SmsHistoryResult(StatusCode statusCode, string statusMessage, string rawResponse)
            : base(statusCode, statusMessage, rawResponse)
        {
        }

        /// <summary>
        /// Parses a given <see cref="HttpResponseMessage"/> and creates a corresponding
        /// <see cref="SmsHistoryResult"/> object.
        /// </summary>
        /// <param name="httpResponse">The <see cref="HttpResponseMessage"/> object to parse.</param>
        public new static SmsHistoryResult FromHttpResponse(HttpResponseMessage httpResponse)
        {
            if (httpResponse == null)
                return new SmsHistoryResult(StatusCode.UnknownError, "unknown error", "");

            string content = httpResponse.Content.ReadAsStringAsync().Result;

            // Errors come with a non-success HTTP status (e.g. 401) but still carry a JSON
            // body with an error code, so parse the body regardless of the status
            if (string.IsNullOrWhiteSpace(content))
                return new SmsHistoryResult(StatusCode.UnknownError, "unknown error", content ?? "");

            ResponseDto response;
            try
            {
                var serializer = new DataContractJsonSerializer(typeof(ResponseDto));
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                    response = (ResponseDto)serializer.ReadObject(stream);
            }
            catch (Exception)
            {
                response = null;
            }

            if (response == null || response.Ok == null)
                return new SmsHistoryResult(StatusCode.UnknownError, "unknown error", content);

            if (response.Ok == false || !httpResponse.IsSuccessStatusCode)
            {
                int errorCode;
                if (!int.TryParse(response.Code?.Trim(), out errorCode) ||
                    !StatusCode.IsDefined(typeof(StatusCode), errorCode))
                {
                    errorCode = (int)StatusCode.UnknownError;
                }

                return new SmsHistoryResult((StatusCode)errorCode,
                    response.Error ?? "unknown error", content);
            }

            return new SmsHistoryResult(StatusCode.Success, "", content)
            {
                Account = response.Account,
                From = ParseTime(response.From),
                To = ParseTime(response.To),
                Page = response.Page ?? 0,
                PageSize = response.PageSize ?? 0,
                TotalCount = response.TotalCount ?? 0,
                HasMore = response.HasMore ?? false,
                Messages = (response.Sms ?? new SmsDto[0])
                    .Where(sms => sms != null)
                    .Select(ToSentSms)
                    .ToArray()
            };
        }

        private static SentSms ToSentSms(SmsDto sms)
        {
            return new SentSms
            {
                Id = sms.Id ?? 0,
                Time = ParseTime(sms.Time),
                Sender = sms.Sender,
                Text = sms.Text,
                Parts = sms.Parts ?? 0,
                IsFlash = sms.Flash ?? false,
                Route = sms.Route,
                RecipientCount = sms.RecipientCount ?? 0,
                Status = SmsDeliveryStatusConverter.Parse(sms.Status),
                DeliveredCount = sms.Delivered ?? 0,
                PendingCount = sms.Pending ?? 0,
                NotDeliveredCount = sms.NotDelivered ?? 0,
                Recipients = (sms.Recipients ?? new RecipientDto[0])
                    .Where(recipient => recipient != null)
                    .Select(recipient => new SentSmsRecipient
                    {
                        Number = recipient.Number,
                        Status = SmsDeliveryStatusConverter.Parse(recipient.Status),
                        DeliveryCode = recipient.Code,
                        StatusText = recipient.Text
                    })
                    .ToArray()
            };
        }

        private static DateTimeOffset? ParseTime(string time)
        {
            DateTimeOffset result;
            if (DateTimeOffset.TryParse(time, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out result))
            {
                return result;
            }
            return null;
        }

        // DTOs matching the API's JSON response. Nullable members tolerate missing or
        // null values, so a changed response degrades to defaults instead of failing.

        [DataContract]
        private class ResponseDto
        {
            [DataMember(Name = "ok")] public bool? Ok { get; set; }
            [DataMember(Name = "code")] public string Code { get; set; }
            [DataMember(Name = "fehler")] public string Error { get; set; }
            [DataMember(Name = "konto")] public string Account { get; set; }
            [DataMember(Name = "von")] public string From { get; set; }
            [DataMember(Name = "bis")] public string To { get; set; }
            [DataMember(Name = "seite")] public int? Page { get; set; }
            [DataMember(Name = "je_seite")] public int? PageSize { get; set; }
            [DataMember(Name = "gesamt")] public int? TotalCount { get; set; }
            [DataMember(Name = "weitere")] public bool? HasMore { get; set; }
            [DataMember(Name = "sms")] public SmsDto[] Sms { get; set; }
        }

        [DataContract]
        private class SmsDto
        {
            [DataMember(Name = "sms_id")] public long? Id { get; set; }
            [DataMember(Name = "zeit")] public string Time { get; set; }
            [DataMember(Name = "absender")] public string Sender { get; set; }
            [DataMember(Name = "text")] public string Text { get; set; }
            [DataMember(Name = "teile")] public int? Parts { get; set; }
            [DataMember(Name = "flash")] public bool? Flash { get; set; }
            [DataMember(Name = "weg")] public string Route { get; set; }
            [DataMember(Name = "empfaenger")] public int? RecipientCount { get; set; }
            [DataMember(Name = "status")] public string Status { get; set; }
            [DataMember(Name = "zugestellt")] public int? Delivered { get; set; }
            [DataMember(Name = "offen")] public int? Pending { get; set; }
            [DataMember(Name = "nicht_zugestellt")] public int? NotDelivered { get; set; }
            [DataMember(Name = "empfaenger_liste")] public RecipientDto[] Recipients { get; set; }
        }

        [DataContract]
        private class RecipientDto
        {
            [DataMember(Name = "nummer")] public string Number { get; set; }
            [DataMember(Name = "status")] public string Status { get; set; }
            [DataMember(Name = "code")] public string Code { get; set; }
            [DataMember(Name = "text")] public string Text { get; set; }
        }
    }
}
