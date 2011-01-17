using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCSLib
{
    public class YMSGMessageEventArgs : EventArgs
    {
        private YMSGMessageEventArgs()
        {

        }

        public YMSGMessageEventArgs(YMSGPacket packet, DateTime timeStamp)
        {
            this.YMSGData = packet;
            this.TimeStamp = timeStamp;
        }
        public DateTime TimeStamp { get; private set; }
        public YMSGPacket YMSGData { get; private set; }
    }
}
