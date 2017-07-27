using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

/**
 * author:qixiao
 * create:2017-7-24 17:04:49
 * */
namespace QX_Frame.Helper_DG.Log
{
    /// <summary>
    /// enum LogType
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// general log
        /// </summary>
        log,
        /// <summary>
        /// use log
        /// </summary>
        use,
        /// <summary>
        /// error log
        /// </summary>
        error
    }

    /// <summary>
    /// log class
    /// </summary>
    public class Log:IDisposable
    {
        public string type { get; set; } = nameof(LogType.log);
        public string date { get => DateTime.Now.ToLongDateString(); }
        public string time { get => DateTime.Now.ToLongTimeString(); }
        public string client_ip { get; set; } = "";
        public string server_name { get => Ip_Helper_DG.GetServerHostName(); }
        public string server_ip { get => Ip_Helper_DG.GetServerIpAddress(); }
        public string uri { get; set; }="";
        public string parameters { get; set; }="";
        public string message { get; set; }="";

        public Log(LogType logType) => this.type = nameof(logType);

        public Log(LogType logType, string message)
        {
            this.type = nameof(logType);
            this.message = message;
        }

        public Log(LogType logType, HttpRequestMessage request)
        {
            this.type = nameof(logType);
            this.client_ip = request.GetIpAddressFromRequest();
            this.uri = request.RequestUri.ToString();
        }
        public Log(LogType logType, HttpRequestMessage request, string message)
        {
            this.type = nameof(logType);
            this.client_ip = request.GetIpAddressFromRequest();
            this.uri = request.RequestUri.ToString();
            this.message = message;
        }

        /// <summary>
        /// Convert object to json string
        /// </summary>
        /// <returns></returns>
        public string ToJson() => Convert_Helper_DG.T_To_Json(this).ToString();

        public void Dispose()
        {
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }
    }
}
