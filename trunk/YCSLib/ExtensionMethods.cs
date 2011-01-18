using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace YCSLib
{
    public static class ExtensionMethods
    {
        public static YMSGPacketPayload Slice(this YMSGPacketPayload payload, string key, int index)
        {
            YMSGPacketPayload retVal = null;
            int x = 0;
            for (int i = 0; i < payload.Count; i++)
                if (payload[index].Key == key)
                    if (x == index)
                        retVal = new YMSGPacketPayload(payload.GetRange(x, payload.FindIndex(x, p =>
                        { if (p.Key == key) return true; return false; }
                        )));
                    else
                        x++;
            return retVal;
        }

        [DebuggerStepThrough]
        internal static unsafe int FindIndex(this byte[] bytes, byte delimeter, int startIndex = 0)
        {
            fixed (byte* pBytes = bytes)
            {
                int i = startIndex;
                while (i < bytes.Length)
                {
                    if (*(pBytes + i) == delimeter)
                        return (i - startIndex);
                    i++;
                }
            }

            return -1;
        }

        internal static byte[] Slice(this byte[] bytes, int length, int startIndex = 0)
        {
            return bytes.Skip(startIndex).Take(length).ToArray();
        }
    }
}
