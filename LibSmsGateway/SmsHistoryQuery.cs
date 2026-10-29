using System;

namespace LibSmsGateway
{
    /// <summary>
    /// Optional filters for querying the SMS history ("SMS-Ausgang") with
    /// <see cref="SmsClient.GetHistory(SmsHistoryQuery)"/>. Leave a property unset to
    /// use the API's default.
    /// </summary>
    public class SmsHistoryQuery
    {
        /// <summary>
        /// Start of the queried period, in Vienna local time. Only date, hours and minutes
        /// are used. Defaults to the last 7 days if neither <see cref="From"/> nor
        /// <see cref="To"/> is set. A single query may span at most 93 days.
        /// </summary>
        public DateTime? From { get; set; }

        /// <summary>
        /// End of the queried period, in Vienna local time. Only date, hours and minutes
        /// are used.
        /// </summary>
        public DateTime? To { get; set; }

        /// <summary>
        /// The page to retrieve, starting at 1.
        /// </summary>
        public int? Page { get; set; }

        /// <summary>
        /// The number of messages per page. The API defaults to 100 and allows at most 500.
        /// </summary>
        public int? PageSize { get; set; }

        /// <summary>
        /// Only return messages with this delivery status. <see cref="SmsDeliveryStatus.Unknown"/>
        /// is ignored.
        /// </summary>
        public SmsDeliveryStatus? Status { get; set; }

        /// <summary>
        /// Set to <c>true</c> to include each recipient with its delivery status in
        /// <see cref="SentSms.Recipients"/>.
        /// </summary>
        public bool IncludeRecipients { get; set; }

        /// <summary>
        /// Only return messages with an <see cref="SentSms.Id"/> greater than this value.
        /// Useful for polling new messages only.
        /// </summary>
        public long? AfterId { get; set; }

        /// <summary>
        /// The username of one of your sub-accounts, to query its history instead of your own.
        /// Only available for main accounts.
        /// </summary>
        public string Account { get; set; }
    }
}
