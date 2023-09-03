using System.Collections;
using UnityEngine;

namespace api
{
    /// <summary>
    /// The packet recieved is too big, this could be becuase it does not contain delimeters
    /// </summary>
    public class OversizedPacketException: System.Exception { }

    /// <summary>
    /// The packet recieved was not of the expected type, this indicates a larger error present in the server
    /// </summary>
    public class UnexpectedPacketException : System.Exception { }
}