using System.Collections;
using UnityEngine;

namespace api
{
    /// <summary>
    /// The packet recieved is too big, this could be becuase it does not contain delimeters
    /// </summary>
    public class OversizedPacketException: System.Exception { }
}