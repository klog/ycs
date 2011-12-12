using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YCSLib;
using System.Threading;
using System.Net;

namespace YCSTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string userName, passwd;
            YMSGClient client = new YMSGClient();
            using (YMSGConnection yc = new YMSGConnection("scs.msg.yahoo.com", 5050))
            {
                yc.Subscribe(client);

                Console.WriteLine("Username: ");
                userName = Console.ReadLine();
                Console.WriteLine("Password: ");
                passwd = Console.ReadLine();

                yc.RetrieveCookies(userName, passwd);
                yc.Connect();
                yc.Logon();

                Console.WriteLine("You're logged in as {0}. This is a test client and supports the following commands.", yc.LoginName);
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
        }
    }
}
