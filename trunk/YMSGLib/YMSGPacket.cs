using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace YMSGLib
{
    public static class ExtensionMethods
    {
        public static YMSGLib.YMSGPacket.YMSGPacketPayload Slice(this YMSGLib.YMSGPacket.YMSGPacketPayload payload, string key, int index)
        {
            YMSGLib.YMSGPacket.YMSGPacketPayload retVal = null;
            int x = 0;
            for (int i = 0; i < payload.Count; i++)
                if (payload[index].Key == key)
                    if (x == index)
                        retVal = new YMSGLib.YMSGPacket.YMSGPacketPayload(payload.GetRange(x, payload.FindIndex(x, p =>
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


    public class YMSGPacket
    {
        public class YMSGPacketPayload : List<KeyValuePair<string, string>>
        {
            public static readonly byte[] YMSGDelimeter = new byte[] { (byte)0xc0, (byte)0x80 };

            public YMSGPacketPayload()
            {

            }

            public YMSGPacketPayload(IEnumerable<KeyValuePair<string, string>> balls)
                : base(balls)
            {

            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public byte[] ToBytes()
            {
                List<byte> retVal = new List<byte>();

                foreach (KeyValuePair<string, string> kv in this)
                {
                    byte[] k = GetEncoding().GetBytes(kv.Key);
                    byte[] j = GetEncoding().GetBytes(kv.Value);
                    retVal.AddRange(k);
                    retVal.AddRange(YMSGDelimeter);
                    retVal.AddRange(j);
                    retVal.AddRange(YMSGDelimeter);
                }

                return retVal.ToArray();
            }
            /// <summary>
            /// indexer...
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public string this[string key]
            {
                get
                {
                    return base.Find(k => { if (k.Key == key) return true; return false; }).Value;
                }
                set
                {
                    base.Add(new KeyValuePair<string, string>(key, value));
                }
            }

            /// <summary>
            /// returns a set of K,V pairs for the specified index of its occurance...UNPOSSIBLE!
            /// </summary>
            /// <param name="key"></param>
            /// <param name="index"></param>
            /// <returns></returns>
            public YMSGPacketPayload this[string key, int index]
            {
                get
                {
                    var all = YMSGPacketPayload.GetChunk(this, key, null);
                    return all.Slice(key, index);
                }
                set
                {
                    this.AddRange(value);
                }
            }

            private static YMSGPacketPayload GetChunk(YMSGPacketPayload payload, string startKey, string[] endKey = null)
            {
                YMSGPacketPayload retVal = new YMSGPacketPayload();

                for (int i = 0; i < payload.Count; i++)
                    if (endKey == null)
                        if (payload[i].Key == startKey)
                        {
                            retVal.AddRange(payload.GetRange(i, payload.Count - i));
                            break;
                        }
                        else
                        {
                            retVal.AddRange(payload.GetRange(i, payload.FindIndex(i, payload.Count - i,
                                x => { if (endKey.Contains<string>(x.Key)) return true; return false; })));
                            break;
                        }
                return retVal;
            }

            /// <summary>
            /// returns the sequence of k,v pairs with the specified start and end keys
            /// </summary>
            /// <param name="startKey"></param>
            /// <param name="endKey"></param>
            /// <returns></returns>
            public YMSGPacketPayload GetChunk(string startKey, string[] endKey = null)
            {
                return GetChunk(this, startKey, endKey);
            }

        }

        public static bool QuirksMode = false;

        public YMSGPacket()
        {
            this.Payload = new YMSGPacketPayload();
            this.Version = 102;
            this.VendorID = 0x402;
            this.Status = 0x0;
        }

        #region header
        public short Version { get; set; }
        public short VendorID { get; set; }
        public short Service { get; set; }
        public short Size { get; set; }

        public int Status { get; set; }
        public int SessionID { get; set; }
        #endregion

        #region payload
        public YMSGPacketPayload Payload
        {
            get;
            protected internal set;
        }
        #endregion

        #region operations

        internal static YMSGPacket FromBytes(byte[] data)
        {
            YMSGPacket retVal = new YMSGPacket();
            retVal.Version = (short)(data[4] << 8 | (data[5]));
            retVal.Size = (short)(data[8] << 8 | data[9]);
            retVal.Service = (short)(data[10] << 8 | data[11]);
            retVal.Status = (int)(data[12] << 24 | data[13] << 16 | data[14] << 8 | data[15]);
            retVal.SessionID = (int)(data[16] << 24 | data[17] << 16 | data[18] << 8 | data[19]);

            byte[] payload = new byte[retVal.Size];
            Buffer.BlockCopy(data, 20, payload, 0, payload.Length);

            int i = 0;
            while(payload.FindIndex(YMSGPacketPayload.YMSGDelimeter[0], i) > -1)
            {
                int length = payload.FindIndex(YMSGPacketPayload.YMSGDelimeter[0], i);
                string s1 = GetEncoding().GetString(payload.Slice(length, i));
                i += length + 2;
                length = payload.FindIndex(YMSGPacketPayload.YMSGDelimeter[0], i);
                string s2 = GetEncoding().GetString(payload.Slice(length, i));
                i += length + 2;
                retVal.Payload.Add(new KeyValuePair<string, string>(s1, s2));
            }

            return retVal;
        }

        public static explicit operator byte[](YMSGPacket packet)
        {
            return packet.ToBytes();
        }

        public static implicit operator YMSGPacket(byte[] bytes)
        {
            return YMSGPacket.FromBytes(bytes);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected internal byte[] ToBytes()
        {
            byte[] payloadData = this.Payload.ToBytes();
            int payloadSize = payloadData.Length;
            byte[] retVal = new byte[payloadSize + 20];

            retVal[0] = (byte)'Y';
            retVal[1] = (byte)'M';
            retVal[2] = (byte)'S';
            retVal[3] = (byte)'G';

            retVal[4] = BitConverter.GetBytes(Version)[1];
            retVal[5] = BitConverter.GetBytes(Version)[0];

            retVal[6] = BitConverter.GetBytes(VendorID)[1];
            retVal[7] = BitConverter.GetBytes(VendorID)[0];

            retVal[8] = BitConverter.GetBytes(payloadSize)[1];
            retVal[9] = BitConverter.GetBytes(payloadSize)[0];

            retVal[10] = BitConverter.GetBytes(Service)[1];
            retVal[11] = BitConverter.GetBytes(Service)[0];

            retVal[12] = BitConverter.GetBytes(Status)[3];
            retVal[13] = BitConverter.GetBytes(Status)[2];
            retVal[14] = BitConverter.GetBytes(Status)[1];
            retVal[15] = BitConverter.GetBytes(Status)[0];

            retVal[16] = BitConverter.GetBytes(SessionID)[3];
            retVal[17] = BitConverter.GetBytes(SessionID)[2];
            retVal[18] = BitConverter.GetBytes(SessionID)[1];
            retVal[19] = BitConverter.GetBytes(SessionID)[0];

            Buffer.BlockCopy(payloadData, 0, retVal, 20, payloadData.Length);

            return retVal;
        }

        public string this[string key]
        {
            get
            {
                return this.Payload[key];
            }
            set
            {
                this.Payload[key] = value;
            }
        }

        private static Encoding GetEncoding()
        {
            return (QuirksMode ? Encoding.GetEncoding("ISO-8859-1") : Encoding.UTF8);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Version: ").Append(this.Version.ToString())
            .Append(", VendorID: ").Append(this.VendorID.ToString())
            .Append(", Service: ").Append(this.Service.ToString())
            .Append(", SessionID: ").Append(this.SessionID.ToString())
            .Append(", Status: ").Append(this.Status.ToString())
            .Append(Environment.NewLine);

            foreach (KeyValuePair<string, string> kv in this.Payload)
                sb.Append(kv.Key + ":" + kv.Value + Environment.NewLine);

            return sb.ToString();
        }
        #endregion
    }
}
