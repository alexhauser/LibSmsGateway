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
        Success = 0,
        MessageCannotBeSent = 100,
        WrongUsername = 108,
        WrongPassword = 109,
        NoSourceNumber = 110,
        UnsupportedDestinationNumber = 111,
        MessageIsEmpty = 113,
        MessageLengthInvalid = 114,
        CreditConsumed = 116,
        UnsupportedDestinationAddress = 200,
        UnknownError = 999
    }
}
