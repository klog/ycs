using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YMSGLib;
using System.Threading;
using System.Net;

namespace YCSTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string userName, passwd;
            YMSGConnection yc = new YMSGConnection("scs.msg.yahoo.com", 5050);
            yc.OnYMSGInformation += new EventHandler<YMSGInfoEventArgs>(yc_OnYMSGInformation);
            yc.OnYMSGMessage += new EventHandler<YMSGMessageEventArgs>(yc_OnYMSGMessage);
            
            Console.WriteLine("Username: ");
            userName = Console.ReadLine();
            Console.WriteLine("Password: ");
            passwd = Console.ReadLine();
            
            yc.RetrieveCookies(userName, passwd);
            yc.Connect();
            yc.Logon();

            Console.WriteLine("You're logged in as {0}. This is a test client and supports the following commands.");
            Console.WriteLine("/pm <username> <message>");
            Console.WriteLine("/quit");
            while (true)
            {
                string cmd = Console.ReadLine();
                try
                {
                    if (cmd.Substring(1, 2) == "pm")
                        yc.SendPM(cmd.Split(' ')[1], string.Join(" ", cmd.Split(' ').Skip(2).ToArray()));
                    else if (cmd.Substring(1, 4) == "quit")
                        break;
                    else
                        Console.WriteLine("Unknown Command.");
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("Unknown Command.");
                }
            }
        }

        static void yc_OnYMSGMessage(object sender, YMSGMessageEventArgs e)
        {
            if (e.YMSGData.Service == 6)
            {
                Console.WriteLine("PM received from {0}:", e.YMSGData["4"]);
                Console.WriteLine(YMSGText.StripTags(e.YMSGData["14"], YmsgStripTagOptions.StripAll));
            }
            //Console.WriteLine(e.YMSGData.ToString());
        }

        static void yc_OnYMSGInformation(object sender, YMSGInfoEventArgs e)
        {
            if(e.EventType== YMSGInfoEventType.BytesSent)
                Console.WriteLine("Bytes Sent: " + e.Data);
            else if(e.EventType == YMSGInfoEventType.BytesReceived)
                Console.WriteLine("Bytes Received: " + e.Data);
            else if(e.EventType== YMSGInfoEventType.Information)
                Console.WriteLine(e.Data);
        }
    }
}
