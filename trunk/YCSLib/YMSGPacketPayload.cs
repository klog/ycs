using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace YCSLib
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
                byte[] k = YMSGPacket.GetEncoding().GetBytes(kv.Key);
                byte[] j = YMSGPacket.GetEncoding().GetBytes(kv.Value);
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
}
