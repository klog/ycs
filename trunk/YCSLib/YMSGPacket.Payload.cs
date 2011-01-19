
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
using System.Runtime.CompilerServices;

namespace YCSLib
{
    public partial class YMSGPacket : EventArgs
    {
        #region struct PayloadEntry
        /// <summary>
        /// Could have used KeyValuePair&lt;string, string&gt;, but I needed a apt name.
        /// </summary>
        public struct PayloadEntry
        {
            private string _key;
            private string _val;

            public PayloadEntry(string key, string value)
            {
                this._key = key;
                this._val = value;
            }
            public string Key { get{return _key;} set{_key = value;} }
            public string Value {get{return _val;} set{_val = value;} }
        }
        #endregion

        #region class Payload
        public class Payload : List<PayloadEntry>
        {
            public Payload()
            {
            }

            public Payload(IEnumerable<PayloadEntry> balls)
                : base(balls)
            {

            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public byte[] ToBytes()
            {
                List<byte> retVal = new List<byte>();

                foreach (PayloadEntry kv in this)
                {
                    byte[] k = YMSGPacket.GetEncoding().GetBytes(kv.Key);
                    byte[] j = YMSGPacket.GetEncoding().GetBytes(kv.Value);
                    retVal.AddRange(k);
                    retVal.AddRange(PayloadDelimeter);
                    retVal.AddRange(j);
                    retVal.AddRange(PayloadDelimeter);
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
                    base.Add(new PayloadEntry(key, value));
                }
            }

            private static Payload GetChunk(Payload payload, string startKey, string[] endKey = null)
            {
                Payload retVal = new Payload();

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
            public Payload GetChunk(string startKey, string[] endKey = null)
            {
                return GetChunk(this, startKey, endKey);
            }
        }
        #endregion
    }
}
