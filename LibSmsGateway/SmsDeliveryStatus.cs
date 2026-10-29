namespace LibSmsGateway
{
    /// <summary>
    /// Represents the delivery status of a sent SMS message or one of its recipients,
    /// as reported by the SMS history ("SMS-Ausgang") API.
    /// </summary>
    public enum SmsDeliveryStatus
    {
        /// <summary>The status reported by the API is unknown or could not be parsed.</summary>
        Unknown = 0,
        /// <summary>The message was delivered ("zugestellt").</summary>
        Delivered,
        /// <summary>Delivery is still pending ("offen").</summary>
        Pending,
        /// <summary>The message could not be delivered ("nicht_zugestellt").</summary>
        NotDelivered
    }

    /// <summary>
    /// Converts between <see cref="SmsDeliveryStatus"/> values and the API's status strings.
    /// </summary>
    internal static class SmsDeliveryStatusConverter
    {
        public static SmsDeliveryStatus Parse(string status)
        {
            switch (status?.Trim())
            {
                case "zugestellt": return SmsDeliveryStatus.Delivered;
                case "offen": return SmsDeliveryStatus.Pending;
                case "nicht_zugestellt": return SmsDeliveryStatus.NotDelivered;
                default: return SmsDeliveryStatus.Unknown;
            }
        }

        public static string ToApiString(SmsDeliveryStatus status)
        {
            switch (status)
            {
                case SmsDeliveryStatus.Delivered: return "zugestellt";
                case SmsDeliveryStatus.Pending: return "offen";
                case SmsDeliveryStatus.NotDelivered: return "nicht_zugestellt";
                default: return null;
            }
        }
    }
}
