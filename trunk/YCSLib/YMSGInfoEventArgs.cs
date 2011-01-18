using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCSLib
{
    public sealed class YMSGInfoEventArgs : EventArgs
    {
        private YMSGInfoEventArgs()
        {

        }
        public YMSGInfoEventArgs(YMSGInfoEventType type, object data)
        {
            this.EventType = type;
            this.Data = data;
        }
        public YMSGInfoEventType EventType { get; private set; }
        public object Data { get; private set; }
    }
}
