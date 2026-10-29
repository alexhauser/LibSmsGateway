using System;
using System.Collections.Generic;

namespace LibSmsGateway
{
    /// <summary>
    /// A sent SMS message as returned by the SMS history ("SMS-Ausgang") API.
    /// </summary>
    public class SentSms
    {
        /// <summary>
        /// The unique ID of the message. IDs are increasing, see <see cref="SmsHistoryQuery.AfterId"/>.
        /// </summary>
        public long Id { get; internal set; }

        /// <summary>
        /// The time the message was sent.
        /// </summary>
        public DateTimeOffset? Time { get; internal set; }

        /// <summary>
        /// The sender name or number of the message.
        /// </summary>
        public string Sender { get; internal set; }

        /// <summary>
        /// The message text.
        /// </summary>
        public string Text { get; internal set; }

        /// <summary>
        /// The number of message parts the text was split into, each charged separately.
        /// </summary>
        public int Parts { get; internal set; }

        /// <summary>
        /// Indicates whether the message was sent as a "flash message".
        /// </summary>
        public bool IsFlash { get; internal set; }

        /// <summary>
        /// The route the message was sent by, e.g. "http2sms".
        /// </summary>
        public string Route { get; internal set; }

        /// <summary>
        /// The number of recipients of the message.
        /// </summary>
        public int RecipientCount { get; internal set; }

        /// <summary>
        /// The overall delivery status of the message.
        /// </summary>
        public SmsDeliveryStatus Status { get; internal set; }

        /// <summary>
        /// The number of recipients the message was delivered to.
        /// </summary>
        public int DeliveredCount { get; internal set; }

        /// <summary>
        /// The number of recipients for which delivery is still pending.
        /// </summary>
        public int PendingCount { get; internal set; }

        /// <summary>
        /// The number of recipients the message could not be delivered to.
        /// </summary>
        public int NotDeliveredCount { get; internal set; }

        /// <summary>
        /// The recipients with their delivery status. Only filled if
        /// <see cref="SmsHistoryQuery.IncludeRecipients"/> was set, empty otherwise.
        /// </summary>
        public IReadOnlyList<SentSmsRecipient> Recipients { get; internal set; } = new SentSmsRecipient[0];
    }
}
