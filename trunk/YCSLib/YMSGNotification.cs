
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
using System.Runtime.Serialization;

namespace YCSLib
{
    [Serializable]
    public class YMSGNotification : Exception
    {
        public YMSGNotification(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            if (info != null)
                this.NotificationType = (YMSGNotificationTypes)Enum.Parse(typeof(YMSGNotificationTypes), info.GetString("_NotificationType"));
        }

        public YMSGNotification(string message) :
            base(message)
        {
        }

        public YMSGNotification(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public YMSGNotificationTypes NotificationType { get; internal set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info != null)
                info.AddValue("_NotificationType", ((int)this.NotificationType).ToString());
        }
    }
}
