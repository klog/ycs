using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YMSGLib
{
    public sealed class YMSGInfoEventArgs : EventArgs
    {
        private YMSGInfoEventArgs()
        {

        }
        public YMSGInfoEventArgs(YMSGInfoEventType type, string data)
        {
            this.EventType = type;
            this.Data = data;
        }
        public YMSGInfoEventType EventType { get; private set; }
        public string Data { get; private set; }
    }
}
