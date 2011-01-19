
/*
 * This source code is provided "as is" and without warranties as
 * to fitness for a particular purpose or merchantability. You may
 * use, distribute and modify this code under the terms of the
 * Microsoft Public License (Ms-PL) and you must retain all copyright,
 * patent, trademark, and attribution notices that are present in
 * the software.
 * 
 * You should have received a copy of the Microsoft Public License with
 * this file. If not, please write to: wickedcoder@hotmail.com,
 * or visit : http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
 * Copyright (C) 2010 Wickedcoder - All Rights Reserved.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCSLib
{
    internal class YMSGPacketBuilder
    {
        List<byte> __buffer = new List<byte>();
        List<byte[]> __packets = new List<byte[]>();

        public static int Overhead { get { return 20; } }

        public void AddBytes(byte[] bytes, int count)
        {
            if (GetPayloadLength(bytes) == (count - Overhead))
                __packets.Add(bytes.Take(count).ToArray()); // TODO: this may not always be true...bytes might be a part of a previous sequence
            else
            {
                // chunked packet...
                lock (__buffer)
                {
                    for (int i = 0; i < count; i++)
                    {
                        byte b = bytes[i];
                        if (__buffer.Count < (this.PacketLength + Overhead) - 1)
                            __buffer.Add(b);
                        else
                        {
                            lock (__packets)
                            {
                                __buffer.Add(b);
                                __packets.Add(__buffer.ToArray());
                                __buffer.Clear();
                            }
                        }
                    }
                }
            }
        }

        private void __ValidatePacketData(byte[] data)
        {
            //int pl = (int)BitConverter.ToInt16(
            //            new byte[] { data[9], data[8] }, 0);
            //YmsgPacket pkt = YmsgPacket.Parse(data, data.Length);
        }

        public int PacketLength
        {
            get
            {
                return (GetPayloadLength(__buffer.ToArray()));
            }
        }

        private int GetPayloadLength(byte[] bytes)
        {
            if (bytes.Length >= 10)
                return (int)BitConverter.ToInt16(
                    new byte[] { bytes[9], bytes[8] }, 0);
            return -1;
        }

        public bool Ready
        {
            get
            {
                return (__packets.Count > 0);
            }
        }

        public byte[][] GetPackets()
        {
            byte[][] retVal = null;
            lock (__packets)
            {
                retVal = __packets.ToArray();
                __packets.Clear();
            }
            return retVal;
        }
    }
}
