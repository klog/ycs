using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Web;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace YCSLib
{
    public partial class YMSGConnection : IDisposable
    {
        public static string[] GetYahooChatServers()
        {
            List<string> retVal = new List<string>();
            IPAddress[] ips = Dns.GetHostAddresses("scs.msg.yahoo.com");
            foreach(IPAddress ip in ips)
                retVal.Add(Dns.GetHostEntry(ip).HostName);
            return retVal.ToArray();
        }

        private string __GetCookie(string userName, string passWord)
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                new RemoteCertificateValidationCallback(ValidateServerCertificate);
            string getRequestSecure = Resources._1004;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(getRequestSecure);
            request.AllowAutoRedirect = false;
            request.Method = "POST";
            request.UserAgent = "Mozilla/1.0";

            using (StreamWriter sw = new StreamWriter(request.GetRequestStream(), Encoding.ASCII))
            {
                sw.Write("login=" + userName);
                sw.Write("&passwd=" + passWord);
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            return response.Headers["Set-Cookie"];
        }

        /// <summary>
        /// Splits the cookies chunk into Y and T cookies. NOTE: Could be expanded later for the Z cookie.
        /// </summary>
        /// <param name="allCookies">The 'Set-Cookie' header value from the Yahoo login server.</param>
        /// <returns>Array of Y and T cookie</returns>
        private string[] __SplitCookie(string allCookies)
        {
            string cy = string.Empty;
            string ct = string.Empty;
            try
            {
                cy = allCookies.Substring(allCookies.IndexOf("Y=v="),
                    allCookies.IndexOf(";",
                    allCookies.IndexOf("Y=v=")) - allCookies.IndexOf("Y=v="));
                ct = allCookies.Substring(allCookies.IndexOf("T=z="),
                    allCookies.IndexOf(";",
                    allCookies.IndexOf("T=z=")) - allCookies.IndexOf("T=z="));
            }
            catch (Exception)
            {
                this.OnYMSGInformation(new YMSGInfoEventArgs(YMSGInfoEventType.Information, Resources._1001));
                throw;
            }
            return new string[] { cy, ct };
        }

        bool ValidateServerCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == 0)
                return true;
            this.OnYMSGInformation(new YMSGInfoEventArgs(YMSGInfoEventType.Information, Resources._1002));
            return false;
        }

        #region IDisposable stuff

        private bool __isDisposed = false;
        protected virtual void Dispose(bool fromDispose)
        {
            if (fromDispose)
            {
                this.isConnecting.Dispose();
                this.isSending.Dispose();
                this.socket.Close();
            }
        }

        public void Dispose()
        {
            if (!this.__isDisposed)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            this.__isDisposed = true;
        }

        ~YMSGConnection()
        {
            Dispose(false);
        }

        #endregion
    }
}
