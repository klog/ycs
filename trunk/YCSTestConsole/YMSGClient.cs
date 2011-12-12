
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
using YCSLib;

namespace YCSTestConsole
{
    internal class YMSGClient : IObserver<YMSGPacket>
    {
        public void OnCompleted()
        {
            Console.WriteLine("Done!");
        }

        public void OnError(Exception error)
        {
            YMSGNotification yn = error as YMSGNotification;
            switch (yn.NotificationType)
            {
                case YMSGNotificationTypes.BytesReceived:
                    Console.WriteLine("Bytes received: " + yn.Message);
                    break;
                case YMSGNotificationTypes.BytesSent:
                    Console.WriteLine("Bytes sent: " + yn.Message);
                    break;
                case YMSGNotificationTypes.Information:
                    Console.WriteLine("Information: " + yn.Message);
                    break;
                default:
                    Console.WriteLine(yn.InnerException.Message);
                    break;
            }
        }

        public void OnNext(YMSGPacket value)
        {
            if (value.Service == 6)
            {
                Console.WriteLine("PM received from {0}:", value["4"]);
                Console.WriteLine(YMSGText.StripTags(value["14"], YmsgStripTagOptions.StripAll));
            }
            //Console.WriteLine(e.YMSGData.ToString());
        }
    }
}
