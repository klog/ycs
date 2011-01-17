using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCSLib
{
    public enum YMSGInfoEventType
    {
        BytesSent, // Data is of type System.Int32
        BytesReceived, // Data is of type System.Int32
        Information // Data is of type System.String
    }
}
