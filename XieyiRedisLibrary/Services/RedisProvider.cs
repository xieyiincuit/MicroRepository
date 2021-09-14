using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using XieyiRedisLibrary.Config;
using XieyiRedisLibrary.Interfaces;

namespace XieyiRedisLibrary.Services
{
    public class RedisProvider : IRedisProvider
    {
        private static readonly ILoggerFactory _loggerFactory = new LoggerFactory();
        private static readonly ILogger<RedisProvider> _logger = _loggerFactory.CreateLogger<RedisProvider>();

        public RedisProvider(IOptions<RedisConfig> redisOptions)
        {
            try
            {
                _logger.LogInformation("Start to Initialize RedisClient");
                //获取配置
                var clientInfo = redisOptions.Value;

                var options = ConfigurationOptions.Parse(clientInfo.ConnectionString + "," + clientInfo.OptionalSettings);
                //配置链接参数
                options.ClientName = clientInfo.ClintName;
                options.Password = clientInfo.Password;
                options.DefaultDatabase = clientInfo.DbNum;
                options.AllowAdmin = true;

                //设置监听事件
                var connect = ConnectionMultiplexer.Connect(options);
                connect.ConnectionFailed += MuxerConnectionFailed;
                connect.ConnectionRestored += MuxerConnectionRestored;
                connect.ErrorMessage += MuxerErrorMessage;
                connect.ConfigurationChanged += MuxerConfigurationChanged;
                connect.HashSlotMoved += MuxerHashSlotMoved;
                connect.InternalError += MuxerInternalError;

                RedisMultiplexer = connect;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "RedisClient Initialized failed.");
            }
        }

        #region 事件

        /// <summary>
        /// 配置更改时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConfigurationChanged(object sender, EndPointEventArgs e)
        {
            _logger.LogInformation("Configuration changed: " + e.EndPoint);
        }

        /// <summary>
        /// 发生错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerErrorMessage(object sender, RedisErrorEventArgs e)
        {
            _logger.LogError("ErrorMessage: " + e.Message);
        }

        /// <summary>
        /// 重新建立连接之前的错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            _logger.LogInformation("ConnectionRestored: " + e.EndPoint);
        }

        /// <summary>
        /// 连接失败 ， 如果重新连接成功你将不会收到这个通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            _logger.LogError("重新连接：Endpoint failed: " + e.EndPoint + ", " + e.FailureType + (e.Exception == null ? "" : (", " + e.Exception.Message)));
        }

        /// <summary>
        /// 更改集群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            _logger.LogInformation("HashSlotMoved:NewEndPoint" + e.NewEndPoint + ", OldEndPoint" + e.OldEndPoint);
        }

        /// <summary>
        /// redis类库错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerInternalError(object sender, InternalErrorEventArgs e)
        {
            _logger.LogError("InternalError:Message" + e.Exception.Message);
        }

        #endregion 事件

        public ConnectionMultiplexer RedisMultiplexer { get; set; }
    }
}