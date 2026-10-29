namespace LibSmsGateway
{
    /// <summary>
    /// A single recipient of a <see cref="SentSms"/> with its delivery status.
    /// </summary>
    public class SentSmsRecipient
    {
        /// <summary>
        /// The recipient phone number, e.g. "436641111111".
        /// </summary>
        public string Number { get; internal set; }

        /// <summary>
        /// The delivery status for this recipient.
        /// </summary>
        public SmsDeliveryStatus Status { get; internal set; }

        /// <summary>
        /// The raw delivery status code reported by the API, e.g. "007". Kept as a string
        /// to preserve leading zeros.
        /// </summary>
        public string DeliveryCode { get; internal set; }

        /// <summary>
        /// A human-readable delivery status text (in German).
        /// </summary>
        public string StatusText { get; internal set; }
    }
}
