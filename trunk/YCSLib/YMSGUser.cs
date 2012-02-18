
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
    public class YMSGUser
    {
        private YMSGPacket.Payload _payload;
        public YMSGUser(YMSGPacket.Payload payload, YMSGUserType type)
        {
            _payload = payload;
            Type = type;
            //TODO: build object
        }

        public enum YMSGUserType { Buddy, Roomie }

        public string ID { get; private set; }
        public string Nickname { get; private set; }
        public string Age { get; private set; }
        public string Gender { get; private set; }
        public string City { get; private set; }
        public string INFData { get; private set; }
        public string Avatar { get; private set; }
        public string ClientGlyph { get; private set; }
        public YMSGUserType Type { get; private set; }
    }
}
