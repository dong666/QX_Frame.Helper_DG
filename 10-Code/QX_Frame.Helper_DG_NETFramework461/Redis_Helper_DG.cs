using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Redis;
using QX_Frame.Helper_DG.Configs;

/**
 * author:qixiao
 * create:2017-6-5 10:47:01
 * */
namespace QX_Frame.Helper_DG
{
    public class Redis_Helper_DG
    {
        /// <summary>
        /// client
        /// </summary>
        private static IRedisClient client;

        /// <summary>
        /// singleton object
        /// </summary>
        private static readonly object lockHelper = new object();
        static Redis_Helper_DG()
        {
            if (client == null)
            {
                lock (lockHelper)
                {
                    if (client == null)
                        client = new RedisClient(QX_Frame_Helper_DG_Config.Cache_Redis_Host, QX_Frame_Helper_DG_Config.Cache_Redis_Port);
                }
            }
        }
        /// <summary>
        /// Get RedisClient
        /// </summary>
        public static IRedisClient Client
        {
            get => client;
        }
    }
}
