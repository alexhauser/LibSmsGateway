using System;
using System.Collections.Generic;
using System.Text;

namespace LibSmsGateway
{
    /// <summary>
    /// Represents status codes returned by the HTTP2SMS API.
    /// </summary>
    public enum StatusCode
    {
        /// <summary>The message was successfully submitted.</summary>
        Success = 0,
        /// <summary>The message could not be sent.</summary>
        MessageCannotBeSent = 100,
        /// <summary>Username missing or wrong (username/password authentication only).</summary>
        WrongUsername = 108,
        /// <summary>API key or password invalid, or account locked.</summary>
        WrongPassword = 109,
        /// <summary>Sender ID not allowed.</summary>
        NoSourceNumber = 110,
        /// <summary>No valid recipient number.</summary>
        UnsupportedDestinationNumber = 111,
        /// <summary>Destination country is blocked for this account.</summary>
        CountryBlocked = 112,
        /// <summary>Message is empty or shorter than 3 characters.</summary>
        MessageIsEmpty = 113,
        /// <summary>Message is longer than 459 characters.</summary>
        MessageLengthInvalid = 114,
        /// <summary>Sender ID is invalid or blocked.</summary>
        SenderInvalidOrBlocked = 115,
        /// <summary>Insufficient credit.</summary>
        CreditConsumed = 116,
        /// <summary>No price set for the destination country.</summary>
        NoPriceForCountry = 117,
        /// <summary>Send time is invalid, in the past, or more than 366 days ahead.</summary>
        InvalidSendTime = 118,
        /// <summary>Scheduled job could not be saved.</summary>
        ScheduleFailed = 119,
        /// <summary>Cancellation not possible: job unknown or already completed.</summary>
        CancelNotPossible = 120,
        /// <summary>A link in the message is not yet approved; nothing was sent or billed.</summary>
        LinkNotApproved = 130,
        /// <summary>A link in the message is blocked; nothing was sent or billed.</summary>
        LinkBlocked = 131,
        /// <summary>Unsupported destination address.</summary>
        UnsupportedDestinationAddress = 200,
        /// <summary>Invalid request parameters, e.g. a history query period longer than 93 days.</summary>
        InvalidRequest = 400,
        /// <summary>Too many failed logins; wait 5 minutes before retrying.</summary>
        TooManyFailedLogins = 429,
        /// <summary>Internal server error; retry later.</summary>
        InternalError = 500,
        /// <summary>Automated or unauthorized access detected.</summary>
        UnauthorizedAccess = 666,
        /// <summary>IP address blocked or not authorized.</summary>
        IpBlocked = 667,
        /// <summary>An unknown error occurred, or the API response could not be parsed.</summary>
        UnknownError = 999
    }
}
