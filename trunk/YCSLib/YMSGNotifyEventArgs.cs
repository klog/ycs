using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCSLib
{
    public sealed class YMSGNotifyEventArgs : EventArgs
    {
        private YMSGNotifyEventArgs()
        {

        }
        public YMSGNotifyEventArgs(YMSGNotifyEventTypes type, object data)
        {
            this.EventType = type;
            this.Data = data;
        }
        public YMSGNotifyEventTypes EventType { get; private set; }
        public object Data { get; private set; }
    }
}
